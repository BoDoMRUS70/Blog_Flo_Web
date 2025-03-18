using Blog_Flo_Web.Services_model.ViewModels.Users;
using Microsoft.AspNetCore.Identity;
using Blog_Flo_Web.Business_model.Models;

namespace Blog_Flo_Web.Services_model.Services.IServices
{
    public interface IUserService
    {
        Task<IdentityResult> Register(UserRegisterViewModel model);

        Task<IdentityResult> CreateUser(UserCreateViewModel model);

        Task<SignInResult> Login(UserLoginViewModel model);

        Task<IdentityResult> EditAccount(UserEditViewModel model);

        Task<UserEditViewModel> EditAccount(Guid id);

        Task RemoveAccount(Guid id);

        Task<List<User>> GetAccounts();

        Task<User> GetAccount(Guid id);

        Task LogoutAccount();
    }
}
