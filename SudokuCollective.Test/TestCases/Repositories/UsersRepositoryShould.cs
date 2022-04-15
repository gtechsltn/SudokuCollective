using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Extensions;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models;
using SudokuCollective.Repos;
using SudokuCollective.Test.Services;
using SudokuCollective.Test.TestData;

namespace SudokuCollective.Test.TestCases.Repositories
{
    public class UsersRepositoryShould
    {
        private DatabaseContext context;
        private MockedRequestService mockedRequestService;
        private Mock<ILogger<UsersRepository<User>>> mockedLogger;
        private IUsersRepository<User> sut;

        [SetUp]
        public async Task Setup()
        {
            context = await TestDatabase.GetDatabaseContext();
            mockedRequestService = new MockedRequestService();
            mockedLogger = new Mock<ILogger<UsersRepository<User>>>();

            sut = new UsersRepository<User>(
                context,
                mockedRequestService.SuccessfulRequest.Object,
                mockedLogger.Object);
        }

        [Test, Category("Repository")]
        public async Task CreateUsers()
        {
            // Arrange
            var newUser = TestObjects.GetNewUser();
            var app = context.Apps.FirstOrDefault(a => a.Id == 1);
            var userApp = new UserApp
            {
                UserId = newUser.Id,
                User = newUser,
                AppId = app.Id,
                App = app
            };

            newUser.Apps.Add(userApp);

            // Act
            var result = await sut.Add(newUser);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That((User)result.Object, Is.InstanceOf<User>());
        }

        [Test, Category("Repository")]
        public async Task ThrowExceptionIfCreateUsersFails()
        {
            // Arrange
            var newUser = TestObjects.GetNewUser();
            newUser.Id = 1;

            // Act
            var result = await sut.Add(newUser);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
        }

        [Test, Category("Repository")]
        public async Task RequireEmailConfirmationForNewUsers()
        {
            // Arrange
            var newUser = TestObjects.GetNewUser();
            var app = context.Apps.FirstOrDefault(a => a.Id == 1);
            var userApp = new UserApp
            {
                UserId = newUser.Id,
                User = newUser,
                AppId = app.Id,
                App = app
            };

            newUser.Apps.Add(userApp);

            // Act
            var result = await sut.Add(newUser);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(((User)result.Object).IsEmailConfirmed, Is.False);
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfConfirmUserEmailsFails()
        {
            // Arrange and Act
            var result = await sut.ConfirmEmail(TestObjects.GetNewEmailConfirmation());

            Assert.That(result.IsSuccess, Is.False);
        }

        [Test, Category("Repository")]
        public async Task GetUsersById()
        {
            // Arrange and Act
            var result = await sut.Get(1);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That((User)result.Object, Is.InstanceOf<User>());
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfGetByIdFails()
        {
            // Arrange and Act
            var result = await sut.Get(5);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Object, Is.Null);
        }

        [Test, Category("Repository")]
        public async Task GetUsersByUserName()
        {
            // Arrange and Act
            var result = await sut.GetByUserName("TestSuperUser");

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That((User)result.Object, Is.InstanceOf<User>());
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfGetUsersByUserNameFails()
        {
            // Arrange and Act
            var result = await sut.GetByUserName("SuperUser");

            // Assert
            Assert.That(result.IsSuccess, Is.False);
        }

        [Test, Category("Repository")]
        public async Task GetUsersByEmail()
        {
            // Arrange and Act
            var result = await sut.GetByEmail("TestSuperUser@example.com");

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That((User)result.Object, Is.InstanceOf<User>());
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfGetUsersByEmailFails()
        {
            // Arrange and Act
            var result = await sut.GetByEmail("SuperUser@example.com");

            // Assert
            Assert.That(result.IsSuccess, Is.False);
        }

        [Test, Category("Repository")]
        public async Task GetAllUsers()
        {
            // Arrange and Act
            var result = await sut.GetAll();

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Objects.ConvertAll(a => (User)a), Is.InstanceOf<List<User>>());
        }

        [Test, Category("Repository")]
        public async Task UpdateUsers()
        {
            // Arrange
            var user = context
                .Users
                .FirstOrDefault(u => u.Id == 1);

            user.UserName = string.Format("{0} UPDATED!", user.UserName);

            // Act
            var result = await sut.Update(user);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Object, Is.InstanceOf<User>());
            Assert.That(((User)result.Object).UserName, Is.EqualTo(user.UserName));
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfUpdateUsersFails()
        {
            // Arrange
            var newUser = TestObjects.GetNewUser();

            // Act
            var result = await sut.Update(newUser);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Object, Is.Null);
        }

        [Test, Category("Repository")]
        public async Task UpdateRangeOfUsers()
        {
            // Arrange
            var users = context
                .Users
                .ToList();

            foreach (var user in users)
            {
                user.UserName = string.Format("{0} UPDATED!", user.UserName);
            }

            // Act
            var result = await sut.UpdateRange(users);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfUpdateRangeOfUsersFails()
        {
            // Arrange
            var users = context
                .Users
                .ToList();

            users.Add(TestObjects.GetNewUser());

            foreach (var user in users)
            {
                user.UserName = string.Format("{0} UPDATED!", user.UserName);
            }

            // Act
            var result = await sut.UpdateRange(users);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
        }

        [Test, Category("Repository")]
        public async Task DeleteUsers()
        {
            // Arrange
            var user = context
                .Users
                .FirstOrDefault(u => u.Id == 2);

            // Act
            var result = await sut.Delete(user);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfDeleteUsersFails()
        {
            // Arrange
            var newUser = TestObjects.GetNewUser();

            // Act
            var result = await sut.Delete(newUser);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
        }

        [Test, Category("Repository")]
        public async Task DeleteRangeOfUsers()
        {
            // Arrange
            var users = context
                .Users
                .ToList();

            // Act
            var result = await sut.DeleteRange(users);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfDeleteRangeOfUsersFails()
        {
            // Arrange
            var users = context
                .Users
                .ToList();

            users.Add(TestObjects.GetNewUser());

            // Act
            var result = await sut.DeleteRange(users);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
        }

        [Test, Category("Repository")]
        public async Task ConfirmItHasAnUser()
        {
            // Arrange and Act
            var result = await sut.HasEntity(1);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfConfirmItHasAnUserFails()
        {
            // Arrange
            var id = context
                .Users
                .ToList()
                .OrderBy(u => u.Id)
                .Last().Id + 1;

            // Act
            var result = await sut.HasEntity(id);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test, Category("Repository")]
        public async Task AddRoleToUsers()
        {
            // Arrange
            var userId = 3;

            var roleId = context
                .Roles
                .Where(r => r.RoleLevel == RoleLevel.ADMIN)
                .Select(r => r.Id)
                .FirstOrDefault();

            // Act
            var result = await sut.AddRole(userId, roleId);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfAddRoleToUsersFails()
        {
            // Arrange
            var userId = 3;

            var roleId = context
                .Roles
                .Where(r => r.RoleLevel == RoleLevel.USER)
                .Select(r => r.Id)
                .FirstOrDefault();

            // Act
            var result = await sut.AddRole(userId, roleId);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
        }

        [Test, Category("Repository")]
        public async Task AddRolesToUsers()
        {
            // Arrange
            var userId = 3;

            var ids = context
                .Roles
                .Where(r => r.RoleLevel != RoleLevel.NULL)
                .ToList()
                .Select(r => r.Id)
                .ToList();

            var userRoleIds = context
                .Roles
                .Where(r => r.Users.Any(ur => ur.UserId == userId))
                .ToList()
                .Select(r => r.Id)
                .ToList();

            ids = ids.RemoveSubList(userRoleIds).ToList();

            // Act
            var result = await sut.AddRoles(userId, ids);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfAddRolesToUsersFails()
        {
            // Arrange
            var userId = context
                .Users
                .ToList()
                .OrderBy(u => u.Id)
                .Last().Id + 1;

            var ids = context
                .Roles
                .Where(r => r.RoleLevel != RoleLevel.NULL)
                .ToList()
                .Select(r => r.Id)
                .ToList();

            // Act
            var result = await sut.AddRoles(userId, ids);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
        }

        [Test, Category("Repository")]
        public async Task RemoveRoleFromUsers()
        {
            // Arrange
            var userId = 1;

            var roleId = context
                .Roles
                .Where(r => r.RoleLevel == RoleLevel.USER)
                .Select(r => r.Id)
                .FirstOrDefault();

            // Act
            var result = await sut.RemoveRole(userId, roleId);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfRemoveRoleFromUsersFails()
        {
            // Arrange
            var userId = 1;

            var roleId = context
                .Roles
                .Where(r => r.RoleLevel == RoleLevel.NULL)
                .Select(r => r.Id)
                .FirstOrDefault();

            // Act
            var result = await sut.RemoveRole(userId, roleId);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
        }

        [Test, Category("Repository")]
        public async Task RemoveRolesFromUsers()
        {
            // Arrange
            var userId = 1;

            var ids = context
                .Roles
                .Where(r => r.Users
                    .Any(ur => ur.UserId == userId
                        && ur.Role.RoleLevel == RoleLevel.USER))
                .ToList()
                .Select(r => r.Id)
                .ToList();

            // Act
            var result = await sut.RemoveRoles(userId, ids);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfRemoveRolesToUsersFails()
        {
            // Arrange
            var userId = context
                .Users
                .ToList()
                .OrderBy(u => u.Id)
                .Last().Id + 1;

            var ids = context
                .Roles
                .Where(r => r.RoleLevel == RoleLevel.USER)
                .ToList()
                .Select(r => r.Id)
                .ToList();

            // Act
            var result = await sut.RemoveRoles(userId, ids);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
        }

        [Test, Category("Repository")]
        public async Task ActivateUsers()
        {
            // Arrange
            var user = context.Users.FirstOrDefault(u => u.Id == 1);

            // Act
            var result = await sut.Activate(user.Id);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test, Category("Repository")]
        public async Task ReturnFalsefsActivateUsersFails()
        {
            // Arrange
            var userid = context
                .Users
                .ToList()
                .OrderBy(u => u.Id)
                .Last().Id + 1;

            // Act
            var result = await sut.Activate(userid);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test, Category("Repository")]
        public async Task DeactivateUsers()
        {
            // Arrange
            var user = context.Users.FirstOrDefault(u => u.Id == 1);

            // Act
            var result = await sut.Deactivate(user.Id);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfDeactivateUsersFails()
        {
            // Arrange
            var userid = context
                .Users
                .ToList()
                .OrderBy(u => u.Id)
                .Last().Id + 1;

            // Act
            var result = await sut.Deactivate(userid);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test, Category("Repository")]
        public async Task PromoteUserToAdmin()
        {
            // Arrange
            var user = context.Users.FirstOrDefault(u => u.Id == 3);

            // Act
            var result = await sut.PromoteToAdmin(user.Id);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfPromoteUserToAdminFails()
        {
            // Arrange
            var userid = context
                .Users
                .ToList()
                .OrderBy(u => u.Id)
                .Last().Id + 1;

            // Act
            var result = await sut.PromoteToAdmin(userid);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test, Category("Repository")]
        public async Task ConfirmUserIsRegistered()
        {
            // Arrange
            var user = context.Users.FirstOrDefault(u => u.Id == 3);

            // Act
            var result = await sut.IsUserRegistered(user.Id);

            Assert.That(result, Is.True);
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfConfirmUserIsRegisteredFails()
        {
            // Arrange
            var userid = context
                .Users
                .ToList()
                .OrderBy(u => u.Id)
                .Last().Id + 1;

            // Act
            var result = await sut.IsUserRegistered(userid);

            Assert.That(result, Is.False);
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfUserHasNotRegistered()
        {
            // Arrange
            var user = new User();

            // Act
            var result = await sut.IsUserRegistered(user.Id);

            Assert.That(result, Is.False);
        }

        [Test, Category("Repository")]
        public async Task ConfirmIfUserNameUnique()
        {
            // Arrange
            var userName = context.Users.FirstOrDefault(u => u.Id == 1).UserName;
            userName = string.Format("{0}UPDATED!", userName);

            // Act
            var result = await sut.IsUserNameUnique(userName);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfConfirmIfUserNameUniqueFails()
        {
            // Arrange
            var userName = context.Users.FirstOrDefault(u => u.Id == 1).UserName;

            // Act
            var result = await sut.IsUserNameUnique(userName);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test, Category("Repository")]
        public async Task ConfirmIfEmailUnique()
        {
            // Arrange
            var email = context.Users.FirstOrDefault(u => u.Id == 1).Email;
            email = string.Format("UPDATED{0}", email);

            // Act
            var result = await sut.IsEmailUnique(email);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfConfirmIfEmailUniqueFails()
        {
            // Arrange
            var email = context.Users.FirstOrDefault(u => u.Id == 1).Email;

            // Act
            var result = await sut.IsEmailUnique(email);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test, Category("Repository")]
        public async Task ConfirmUpdateIfUserNameUnique()
        {
            // Arrange
            var userId = 1;
            var userName = context.Users.FirstOrDefault(u => u.Id == userId).UserName;

            // Act
            var result = await sut.IsUpdatedUserNameUnique(userId, userName);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfConfirmIfUpdateUserNameUniqueFails()
        {
            // Arrange
            var userIdOne = 1;
            var userIdTwo = 2;
            var userName = context.Users.FirstOrDefault(u => u.Id == userIdTwo).UserName;

            // Act
            var result = await sut.IsUpdatedUserNameUnique(userIdOne, userName);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test, Category("Repository")]
        public async Task GetUsersApps()
        {
            // Arrange and Act
            var result = await sut.GetMyApps(1);

            // Assert
            Assert.That(result, Is.TypeOf<RepositoryResponse>());
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Objects.Count, Is.EqualTo(2));
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfGetUsersAppsFails()
        {
            // Arrange and Act
            var result = await sut.GetMyApps(5);

            // Assert
            Assert.That(result, Is.TypeOf<RepositoryResponse>());
            Assert.That(result.IsSuccess, Is.False);
        }

        [Test, Category("Repository")]
        public async Task GetUsersAppLicense()
        {
            // Arrange and Act
            var result = await sut.GetAppLicense(1);

            // Assert
            Assert.That(result, Is.TypeOf<string>());
        }

        [Test, Category("Repository")]
        public async Task ReturnNullIfGetUsersAppLicenseFails()
        {
            // Arrange and Act
            var result = await sut.GetMyApps(5);

            // Assert
            Assert.That(result, Is.TypeOf<RepositoryResponse>());
            Assert.That(result.IsSuccess, Is.False);
        }
    }
}
