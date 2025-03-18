using NUnit.Framework;
using Moq;
using AutoMapper;
using Blog_Flo_Web.Business_model.Models;
using Blog_Flo_Web.Services_model.Services;
using Blog_Flo_Web.Services_model.ViewModels.Roles;
using Microsoft.AspNetCore.Identity;

namespace Blog_Flo_Web.Test
{
    [TestFixture]
    public class RoleServiceTests
    {
        private Mock<RoleManager<Role>> _roleManagerMock;
        private RoleService _roleService;

        [SetUp]
        public void SetUp()
        {
            _roleManagerMock = GetRoleManagerMock();
            _roleService = new RoleService(_roleManagerMock.Object);
        }

        [Test]
        public async Task CreateRole_Should_Create_Role()
        {
            // Arrange
            var model = new RoleCreateViewModel { Name = "TestRole", Description = "Test Description" };
            var role = new Role { Id = Guid.NewGuid().ToString(), Name = model.Name, Description = model.Description };
            _roleManagerMock.Setup(manager => manager.CreateAsync(It.IsAny<Role>())).ReturnsAsync(IdentityResult.Success).Callback<Role>(r => role = r);

            // Act
            var roleId = await _roleService.CreateRole(model);

            // Assert
            _roleManagerMock.Verify(manager => manager.CreateAsync(It.IsAny<Role>()), Times.Once);
            Assert.AreEqual(Guid.Parse(role.Id), roleId);
        }

        [Test]
        public async Task EditRole_Should_Edit_Role()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var model = new RoleEditViewModel { Id = roleId, Name = "UpdatedRole", Description = "Updated Description" };
            var existingRole = new Role { Id = roleId.ToString(), Name = "OldRole", Description = "Old Description" };
            _roleManagerMock.Setup(manager => manager.FindByIdAsync(roleId.ToString())).ReturnsAsync(existingRole);
            _roleManagerMock.Setup(manager => manager.UpdateAsync(It.IsAny<Role>())).ReturnsAsync(IdentityResult.Success);

            // Act
            await _roleService.EditRole(model);

            // Assert
            _roleManagerMock.Verify(manager => manager.FindByIdAsync(roleId.ToString()), Times.Once);
            _roleManagerMock.Verify(manager => manager.UpdateAsync(It.IsAny<Role>()), Times.Once);
            Assert.AreEqual(model.Name, existingRole.Name);
            Assert.AreEqual(model.Description, existingRole.Description);
        }

        [Test]
        public async Task RemoveRole_Should_Remove_Role()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var existingRole = new Role { Id = roleId.ToString(), Name = "TestRole", Description = "Test Description" };
            _roleManagerMock.Setup(manager => manager.FindByIdAsync(roleId.ToString())).ReturnsAsync(existingRole);
            _roleManagerMock.Setup(manager => manager.DeleteAsync(It.IsAny<Role>())).ReturnsAsync(IdentityResult.Success);

            // Act
            await _roleService.RemoveRole(roleId);

            // Assert
            _roleManagerMock.Verify(manager => manager.FindByIdAsync(roleId.ToString()), Times.Once);
            _roleManagerMock.Verify(manager => manager.DeleteAsync(It.IsAny<Role>()), Times.Once);
        }

        [Test]
        public async Task GetRoles_Should_Return_List_Of_Roles()
        {
            // Arrange
            var roles = new List<Role>
        {
            new Role { Id = Guid.NewGuid().ToString(), Name = "Role1", Description = "Description1" },
            new Role { Id = Guid.NewGuid().ToString(), Name = "Role2", Description = "Description2" }
        };
            _roleManagerMock.Setup(manager => manager.Roles).Returns(roles.AsQueryable());

            // Act
            var resultRoles = await _roleService.GetRoles();

            // Assert
            Assert.AreEqual(roles.Count, resultRoles.Count);
            foreach (var role in roles)
            {
                Assert.IsTrue(resultRoles.Any(r => r.Id == role.Id && r.Name == role.Name && r.Description == role.Description));
            }
        }

        [Test]
        public async Task GetRole_Should_Return_Single_Role()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var role = new Role { Id = roleId.ToString(), Name = "Role1", Description = "Description1" };
            _roleManagerMock.Setup(manager => manager.FindByIdAsync(roleId.ToString())).ReturnsAsync(role);

            // Act
            var resultRole = await _roleService.GetRole(roleId);

            // Assert
            Assert.NotNull(resultRole);
            Assert.AreEqual(role.Id, resultRole.Id.ToString());
            Assert.AreEqual(role.Name, resultRole.Name);
            Assert.AreEqual(role.Description, resultRole.Description);
        }
        //This part of the code is a helper method that creates a mock instance of RoleManager<Role> for testing purposes.
        private Mock<RoleManager<Role>> GetRoleManagerMock()
        {
            // Set up a mock RoleManager
            var storeMock = new Mock<IRoleStore<Role>>();
            return new Mock<RoleManager<Role>>(storeMock.Object, null, null, null, null);
        }
    }
}
