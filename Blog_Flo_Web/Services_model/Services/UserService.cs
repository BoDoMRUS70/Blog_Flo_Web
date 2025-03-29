using AutoMapper;
using Blog_Flo_Web.Services_model.Services.IServices;
using Blog_Flo_Web.Services_model.ViewModels.Users;
using Blog_Flo_Web.Services_model.ViewModels.Roles; // Добавьте эту директиву
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Blog_Flo_Web.Business_model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_Flo_Web.Services_model.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;

        public UserService(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<Role> roleManager, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<IdentityResult> Register(UserRegisterViewModel model)
        {
            var user = _mapper.Map<User>(model);
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);

                // Создаем роль "Пользователь", если она не существует
                var userRole = await _roleManager.FindByNameAsync("Пользователь");
                if (userRole == null)
                {
                    userRole = new Role { Name = "Пользователь", Description = "Имеет ограниченные права" };
                    await _roleManager.CreateAsync(userRole);
                }

                // Добавляем пользователя в роль
                await _userManager.AddToRoleAsync(user, userRole.Name);
            }
            return result;
        }

        public async Task<SignInResult> Login(UserLoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return SignInResult.Failed;

            return await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: true, lockoutOnFailure: false);
        }

        public async Task<UserEditViewModel> EditAccount(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                throw new ArgumentException("Пользователь не найден", nameof(id));

            var allRoles = await _roleManager.Roles.ToListAsync();
            var model = new UserEditViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                NewPassword = string.Empty,
                Id = id,
                Roles = (await Task.WhenAll(allRoles.Select(async r => new RoleViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    IsSelected = await _userManager.IsInRoleAsync(user, r.Name)
                }))).ToList(),
            };
            return model;
        }

        public async Task<IdentityResult> EditAccount(UserEditViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
                throw new ArgumentException("Пользователь не найден", nameof(model.Id));

            if (model.FirstName != null)
                user.FirstName = model.FirstName;

            if (model.LastName != null)
                user.LastName = model.LastName;

            if (model.Email != null)
                user.Email = model.Email;

            if (model.NewPassword != null)
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.NewPassword);

            if (model.UserName != null)
                user.UserName = model.UserName;

            // Обновляем роли пользователя
            foreach (var role in model.Roles)
            {
                var roleName = (await _roleManager.FindByIdAsync(role.Id))?.Name;
                if (roleName == null)
                    continue;

                if (role.IsSelected && !await _userManager.IsInRoleAsync(user, roleName))
                {
                    await _userManager.AddToRoleAsync(user, roleName);
                }
                else if (!role.IsSelected && await _userManager.IsInRoleAsync(user, roleName))
                {
                    await _userManager.RemoveFromRoleAsync(user, roleName);
                }
            }

            return await _userManager.UpdateAsync(user);
        }

        public async Task RemoveAccount(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                throw new ArgumentException("Пользователь не найден", nameof(id));

            await _userManager.DeleteAsync(user);
        }

        public async Task<List<User>> GetAccounts()
        {
            var users = await _userManager.Users.Include(u => u.Posts).ToListAsync();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.Roles = roles.Select(r => new Role { Name = r }).ToList();
            }
            return users;
        }

        public async Task<User> GetAccount(Guid id)
        {
            return await _userManager.FindByIdAsync(id.ToString());
        }

        public async Task LogoutAccount()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> CreateUser(UserCreateViewModel model)
        {
            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.UserName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var adminRole = await _roleManager.FindByNameAsync("Администратор");
                if (adminRole == null)
                {
                    adminRole = new Role { Name = "Администратор", Description = "Не имеет ограничений" };
                    await _roleManager.CreateAsync(adminRole);
                }

                await _userManager.AddToRoleAsync(user, adminRole.Name);
            }
            return result;
        }
    }
}
