using Moq;
using AutoMapper;
using Blog_Flo_Web.Business_model.Models;
using Blog_Flo_Web.Services_model.Services;
using Blog_Flo_Web.Services_model.ViewModels.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;


namespace Blog_Flo_Web.Test
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<UserManager<User>> _userManagerMock;
        private Mock<SignInManager<User>> _signInManagerMock;
        private Mock<RoleManager<Role>> _roleManagerMock;
        private Mock<IMapper> _mapperMock;
        private UserService _userService;

        public UserServiceTests()
        {
            _userManagerMock = GetMockUserManager();
            _signInManagerMock = GetMockSignInManager();
            _roleManagerMock = new Mock<RoleManager<Role>>(
                Mock.Of<IRoleStore<Role>>(),
                Mock.Of<IEnumerable<IRoleValidator<Role>>>(),
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<ILogger<RoleManager<Role>>>()
            );
            _mapperMock = new Mock<IMapper>();
            _userService = new UserService(_userManagerMock.Object, _signInManagerMock.Object, _roleManagerMock.Object, _mapperMock.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _userManagerMock = GetMockUserManager();
            _signInManagerMock = GetMockSignInManager();
            _roleManagerMock = new Mock<RoleManager<Role>>(
                Mock.Of<IRoleStore<Role>>(),
                Mock.Of<IEnumerable<IRoleValidator<Role>>>(),
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<ILogger<RoleManager<Role>>>()
            );
            _mapperMock = new Mock<IMapper>();
            _userService = new UserService(_userManagerMock.Object, _signInManagerMock.Object, _roleManagerMock.Object, _mapperMock.Object);
        }

        private Mock<SignInManager<User>> GetMockSignInManager()
        {
            return new Mock<SignInManager<User>>(
                _userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<User>>(),
                Mock.Of<IOptions<IdentityOptions>>(), // Используем макет вместо null
                Mock.Of<ILogger<SignInManager<User>>>(),
                Mock.Of<IAuthenticationSchemeProvider>(),
                Mock.Of<IUserConfirmation<User>>()
            );
        }

        private Mock<UserManager<User>> GetMockUserManager()
        {
            var storeMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
                storeMock.Object,
                Mock.Of<IOptions<IdentityOptions>>(), // Используем макет вместо null
                Mock.Of<IPasswordHasher<User>>(),
                new List<IUserValidator<User>>(),
                new List<IPasswordValidator<User>>(),
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<User>>>()
            );
        }

        [Test]
        public async Task Register_Should_Create_User_And_Assign_Role()
        {
            // Arrange
            var model = new UserRegisterViewModel
            {
                FirstName = "John",
                LastName = "Doe",
                UserName = "john.doe",
                Email = "john@example.com",
                Password = "P@ssw0rd"
            };
            var user = new User { Id = Guid.NewGuid().ToString() };
            _mapperMock.Setup(mapper => mapper.Map<User>(model)).Returns(user);
            _userManagerMock.Setup(manager => manager.CreateAsync(user, model.Password)).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(manager => manager.FindByIdAsync(user.Id)).ReturnsAsync(user);
            _roleManagerMock.Setup(manager => manager.CreateAsync(It.IsAny<Role>())).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.Register(model);

            // Assert
            Assert.AreEqual(IdentityResult.Success, result);
            _mapperMock.Verify(mapper => mapper.Map<User>(model), Times.Once);
            _userManagerMock.Verify(manager => manager.CreateAsync(user, model.Password), Times.Once);
            _signInManagerMock.Verify(manager => manager.SignInAsync(user, It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
            _roleManagerMock.Verify(manager => manager.CreateAsync(It.IsAny<Role>()), Times.Once);
            _userManagerMock.Verify(manager => manager.AddToRoleAsync(user, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task Login_Should_Return_SignInResult()
        {
            // Arrange
            var model = new UserLoginViewModel { Email = "john@example.com", Password = "P@ssw0rd" };
            var user = new User();
            _userManagerMock.Setup(manager => manager.FindByEmailAsync(model.Email)).ReturnsAsync(user);
            _signInManagerMock.Setup(manager => manager.PasswordSignInAsync(user, model.Password, true, false))
                             .ReturnsAsync(SignInResult.Success);

            // Act
            var result = await _userService.Login(model);

            // Assert
            Assert.AreEqual(SignInResult.Success, result);
            _userManagerMock.Verify(manager => manager.FindByEmailAsync(model.Email), Times.Once);
            _signInManagerMock.Verify(manager => manager.PasswordSignInAsync(user, model.Password, true, false), Times.Once);
        }

        [Test]
        public async Task RemoveAccount_Should_RemoveUserSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId.ToString() };
            _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            await _userService.RemoveAccount(userId);

            // Assert
            _userManagerMock.Verify(um => um.FindByIdAsync(userId.ToString()), Times.Once);
            _userManagerMock.Verify(um => um.DeleteAsync(user), Times.Once);
        }

        [Test]
        public async Task GetAccounts_Should_ReturnListOfUsersWithRoles()
        {
            // Arrange
            var user1 = new User { Id = "1" };
            var user2 = new User { Id = "2" };
            var role1 = "Role1";
            var role2 = "Role2";
            var users = new List<User> { user1, user2 }.AsQueryable();
            var usersDbSetMock = new Mock<DbSet<User>>();
            usersDbSetMock.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            usersDbSetMock.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            usersDbSetMock.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            usersDbSetMock.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            _userManagerMock.Setup(um => um.Users).Returns(usersDbSetMock.Object);
            _userManagerMock.Setup(um => um.GetRolesAsync(user1)).ReturnsAsync(new List<string> { role1 });
            _userManagerMock.Setup(um => um.GetRolesAsync(user2)).ReturnsAsync(new List<string> { role2 });

            // Act
            var result = await _userService.GetAccounts();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("1", result[0].Id);
            Assert.AreEqual(1, result[0].Roles.Count);
            Assert.AreEqual(role1, result[0].Roles[0].Name);
            Assert.AreEqual("2", result[1].Id);
            Assert.AreEqual(1, result[1].Roles.Count);
            Assert.AreEqual(role2, result[1].Roles[0].Name);
        }

        [Test]
        public async Task GetAccount_Should_CallFindByIdAsync()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userMock = new Mock<User>();
            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                           .ReturnsAsync(userMock.Object);

            // Act
            var user = await _userService.GetAccount(userId);

            // Assert
            Assert.AreEqual(userMock.Object, user);
            _userManagerMock.Verify(um => um.FindByIdAsync(userId.ToString()), Times.Once);
        }

        [Test]
        public async Task LogoutAccount_Should_CallSignOutAsync()
        {
            // Act
            await _userService.LogoutAccount();

            // Assert
            _signInManagerMock.Verify(manager => manager.SignOutAsync(), Times.Once);
        }

        [Test]
        public async Task CreateUser_Should_CallCreateAsyncAndAddToRoleAsync()
        {
            // Arrange
            var model = new UserCreateViewModel
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                UserName = "johndoe",
                Password = "P@ssw0rd"
            };

            var userMock = new Mock<User>();
            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                           .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                           .ReturnsAsync(userMock.Object);
            _roleManagerMock.Setup(rm => rm.FindByNameAsync(It.IsAny<string>()))
                            .ReturnsAsync(new Role());

            // Act
            var result = await _userService.CreateUser(model);

            // Assert
            Assert.IsTrue(result.Succeeded);
            _userManagerMock.Verify(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
            _userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        }
    }
}