using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using SudokuCollective.Cache;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Payloads;
using SudokuCollective.Data.Models.Results;
using SudokuCollective.Data.Services;
using SudokuCollective.Test.Cache;
using SudokuCollective.Test.Repositories;
using SudokuCollective.Test.Services;
using SudokuCollective.Test.TestData;

namespace SudokuCollective.Test.TestCases.Services
{
    internal class UsersServiceShould
    {
        private DatabaseContext context;
        private MockedEmailService mockedEmailService;
        private MockedUsersRepository mockedUsersRepository;
        private MockedAppsRepository mockedAppsRepository;
        private MockedRolesRepository mockedRolesRepository;
        private MockedAppAdminsRepository mockedAppAdminsRepository;
        private MockedEmailConfirmationsRepository mockedEmailConfirmationsRepository;
        private MockedPasswordResetsRepository mockPasswordResetRepository;
        private MockedCacheService mockedCacheService;
        private MemoryDistributedCache memoryCache;
        private IUsersService sut;
        private IUsersService sutFailure;
        private IUsersService sutEmailFailure;
        private IUsersService sutResetPassword;
        private IUsersService sutResendEmailConfirmation;
        private IUsersService sutRequestPasswordReset;
        private Request request;

        [SetUp]
        public async Task Setup()
        {
            context = await TestDatabase.GetDatabaseContext();

            mockedEmailService = new MockedEmailService();
            mockedUsersRepository = new MockedUsersRepository(context);
            mockedAppsRepository = new MockedAppsRepository(context);
            mockedRolesRepository = new MockedRolesRepository(context);
            mockedAppAdminsRepository = new MockedAppAdminsRepository(context);
            mockedEmailConfirmationsRepository = new MockedEmailConfirmationsRepository(context);
            mockPasswordResetRepository = new MockedPasswordResetsRepository(context);
            mockedCacheService = new MockedCacheService(context);
            memoryCache = new MemoryDistributedCache(
                Options.Create(new MemoryDistributedCacheOptions()));

            sut = new UsersService(
                mockedUsersRepository.SuccessfulRequest.Object,
                mockedAppsRepository.SuccessfulRequest.Object,
                mockedRolesRepository.SuccessfulRequest.Object,
                mockedAppAdminsRepository.SuccessfulRequest.Object,
                mockedEmailConfirmationsRepository.SuccessfulRequest.Object,
                mockPasswordResetRepository.SuccessfulRequest.Object,
                mockedEmailService.SuccessfulRequest.Object,
                memoryCache,
                mockedCacheService.SuccessfulRequest.Object,
                new CacheKeys(),
                new CachingStrategy());

            sutFailure = new UsersService(
                mockedUsersRepository.FailedRequest.Object,
                mockedAppsRepository.SuccessfulRequest.Object,
                mockedRolesRepository.SuccessfulRequest.Object,
                mockedAppAdminsRepository.FailedRequest.Object,
                mockedEmailConfirmationsRepository.FailedRequest.Object,
                mockPasswordResetRepository.FailedRequest.Object,
                mockedEmailService.SuccessfulRequest.Object,
                memoryCache,
                mockedCacheService.FailedRequest.Object,
                new CacheKeys(),
                new CachingStrategy());

            sutEmailFailure = new UsersService(
                mockedUsersRepository.EmailFailedRequest.Object,
                mockedAppsRepository.SuccessfulRequest.Object,
                mockedRolesRepository.SuccessfulRequest.Object,
                mockedAppAdminsRepository.FailedRequest.Object,
                mockedEmailConfirmationsRepository.FailedRequest.Object,
                mockPasswordResetRepository.SuccessfulRequest.Object,
                mockedEmailService.SuccessfulRequest.Object,
                memoryCache,
                mockedCacheService.FailedRequest.Object,
                new CacheKeys(),
                new CachingStrategy());

            sutResetPassword = new UsersService(
                mockedUsersRepository.InitiatePasswordSuccessfulRequest.Object,
                mockedAppsRepository.InitiatePasswordSuccessfulRequest.Object,
                mockedRolesRepository.SuccessfulRequest.Object,
                mockedAppAdminsRepository.SuccessfulRequest.Object,
                mockedEmailConfirmationsRepository.SuccessfulRequest.Object,
                mockPasswordResetRepository.SuccessfulRequest.Object,
                mockedEmailService.SuccessfulRequest.Object,
                memoryCache,
                mockedCacheService.SuccessfulRequest.Object,
                new CacheKeys(),
                new CachingStrategy());

            sutResendEmailConfirmation = new UsersService(
                mockedUsersRepository.ResendEmailConfirmationSuccessfulRequest.Object,
                mockedAppsRepository.SuccessfulRequest.Object,
                mockedRolesRepository.SuccessfulRequest.Object,
                mockedAppAdminsRepository.SuccessfulRequest.Object,
                mockedEmailConfirmationsRepository.SuccessfulRequest.Object,
                mockPasswordResetRepository.SuccessfulRequest.Object,
                mockedEmailService.SuccessfulRequest.Object,
                memoryCache,
                mockedCacheService.SuccessfulRequest.Object,
                new CacheKeys(),
                new CachingStrategy());

            sutRequestPasswordReset = new UsersService(
                mockedUsersRepository.SuccessfulRequest.Object,
                mockedAppsRepository.SuccessfulRequest.Object,
                mockedRolesRepository.SuccessfulRequest.Object,
                mockedAppAdminsRepository.SuccessfulRequest.Object,
                mockedEmailConfirmationsRepository.SuccessfulRequest.Object,
                mockPasswordResetRepository.SuccessfullyCreatedRequest.Object,
                mockedEmailService.SuccessfulRequest.Object,
                memoryCache,
                mockedCacheService.SuccessfulRequest.Object,
                new CacheKeys(),
                new CachingStrategy());

            request = TestObjects.GetRequest();
        }

        [Test, Category("Services")]
        public async Task GetUser()
        {
            // Arrange
            var userId = 1;
            var license = TestObjects.GetLicense();

            // Act
            var result = await sut.Get(userId, license);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("User Found"));
            Assert.That((User)result.Payload[0], Is.TypeOf<User>());
        }

        [Test, Category("Services")]
        public async Task ReturnMessageIfUserNotFound()
        {
            // Arrange
            var userId = 5;
            var license = TestObjects.GetLicense();

            // Act
            var result = await sutFailure.Get(userId, license);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("User not Found"));
        }

        [Test, Category("Services")]
        public async Task GetUsers()
        {
            // Arrange
            var license = TestObjects.GetLicense();

            // Act
            var result = await sut.GetUsers(
                request.RequestorId,
                license,
                request.Paginator);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Users Found"));
            Assert.That(result.Payload.ConvertAll(u => (IUser)u), Is.TypeOf<List<IUser>>());
        }

        [Test, Category("Services")]
        public async Task CreateUser()
        {
            // Arrange
            var payload = new RegisterPayload()
            {
                UserName = "NewUser",
                FirstName = "New",
                LastName = "User",
                NickName = "New Guy",
                Email = "newuser@example.com",
                Password = "T3stP@ssw0rd"
            };

            request.Payload = payload;

            var baseUrl = "https://example.com";

            var html = "../../../../SudokuCollective.Api/Content/EmailTemplates/create-email-inlined.html";

            // Act
            var result = await sut.Create(request, baseUrl, html);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("User Created"));
            Assert.That(((UserResult)result.Payload[0]).User, Is.TypeOf<User>());
        }

        [Test, Category("Services")]
        public async Task ConfirmUserEmail()
        {
            // Arrange
            var emailConfirmation = context.EmailConfirmations.FirstOrDefault();

            var baseUrl = "https://example.com";

            var html = "../../../../SudokuCollective.Api/Content/EmailTemplates/confirm-old-email-inlined.html";

            // Act
            var result = await sut.ConfirmEmail(emailConfirmation.Token, baseUrl, html);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Old Email Confirmed"));
        }

        [Test, Category("Services")]
        public async Task NotifyIfConfirmUserEmailFails()
        {
            // Arrange
            var emailConfirmation = TestObjects.GetNewEmailConfirmation();

            var baseUrl = "https://example.com";

            var html = "../../../../SudokuCollective.Api/Content/EmailTemplates/confirm-old-email-inlined.html";

            // Act
            var result = await sutEmailFailure.ConfirmEmail(emailConfirmation.Token, baseUrl, html);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Email not Confirmed"));
        }

        [Test, Category("Services")]
        public async Task RequireUserNameUnique()
        {
            // Arrange
            var payload = new RegisterPayload()
            {
                UserName = "TestUser",
                FirstName = "New",
                LastName = "User",
                NickName = "New Guy",
                Email = "newuser@example.com",
                Password = "T3stP@ssw0rd"
            };

            request.Payload = payload;

            var baseUrl = "https://example.com";

            var html = "c:/path/to/html";

            // Act
            var result = await sutFailure.Create(request, baseUrl, html);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("User Name not Unique"));
        }

        [Test, Category("Services")]
        public async Task RequireUserName()
        {
            // Arrange
            var payload = new RegisterPayload()
            {
                FirstName = "New",
                LastName = "User",
                NickName = "New Guy",
                Email = "newuser@example.com",
                Password = "T3stP@ssw0rd"
            };

            request.Payload = payload;

            var baseUrl = "https://example.com";

            var html = "c:/path/to/html";

            // Act
            var result = await sutEmailFailure.Create(request, baseUrl, html);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("User Name Required"));
        }

        [Test, Category("Services")]
        public async Task RequireUniqueEmail()
        {
            // Arrange
            var payload = new RegisterPayload()
            {
                UserName = "NewUser",
                FirstName = "New",
                LastName = "User",
                NickName = "New Guy",
                Email = "TestUser@example.com",
                Password = "T3stP@ssw0rd1"
            };

            request.Payload = payload;

            var baseUrl = "https://example.com";

            var emailMetaData = new EmailMetaData();

            var html = "c:/path/to/html";

            // Act
            var result = await sutEmailFailure.Create(request, baseUrl, html);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Email not Unique"));
        }

        [Test, Category("Services")]
        public async Task RequireEmail()
        {
            // Arrange
            var payload = new RegisterPayload()
            {
                UserName = "NewUser",
                FirstName = "New",
                LastName = "User",
                NickName = "New Guy",
                Password = "T3stP@ssw0rd"
            };

            request.Payload = payload;

            var baseUrl = "https://example.com";

            var html = "c:/path/to/html";

            // Act
            var result = await sut.Create(request, baseUrl, html);

            // Act and Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Email Required"));
        }

        [Test, Category("Services")]
        public async Task UpdateUser()
        {
            // Arrange
            var userId = 2;

            var payload = new UpdateUserPayload()
            {
                UserName = "TestUserUPDATED",
                FirstName = "Test",
                LastName = "User",
                NickName = "Test User UPDATED",
                Email = "TestUser@example.com"
            };

            request.Payload = payload;

            var baseUrl = "https://example.com";

            var html = "../../../../SudokuCollective.Api/Content/EmailTemplates/create-email-inlined.html";

            // Act
            var result = await sut.Update(userId, request, baseUrl, html);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("User Updated"));
            Assert.That(((UserResult)result.Payload[0]).User.UserName, Is.EqualTo("TestUserUPDATED"));
        }

        [Test, Category("Services")]
        public async Task RequestPasswordReset()
        {
            // Arrange
            var payload = new RequestPasswordResetPayload
            {
                License = context.Apps.Select(a => a.License).FirstOrDefault(),
                Email = context.Users.Select(u => u.Email).FirstOrDefault()
            };

            request.Payload = payload;

            var baseUrl = "https://example.com";

            var html = "../../../../SudokuCollective.Api/Content/EmailTemplates/confirm-old-email-inlined.html";

            // Act
            var result = await sutRequestPasswordReset.RequestPasswordReset(
                request,
                baseUrl,
                html);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Processed Password Reset Request"));
        }

        [Test, Category("Services")]
        public async Task ReturnsFalseIfRequestPasswordResetFails()
        {
            // Arrange
            var payload = new RequestPasswordResetPayload
            {
                License = context.Apps.Select(a => a.License).FirstOrDefault(),
                Email = "bademai@example.com"
            };

            request.Payload = payload;

            var baseUrl = "https://example.com";

            var html = "../../../../SudokuCollective.Api/Content/EmailTemplates/confirm-old-email-inlined.html";

            // Act
            var result = await sutFailure.RequestPasswordReset(
                request,
                baseUrl,
                html);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
        }

        [Test, Category("Services")]
        public async Task RequireUniqueUserNameForUpdates()
        {
            // Arrange
            var userId = 1;

            var payload = new UpdateUserPayload()
            {
                UserName = "TestUser",
                FirstName = "Test Super",
                LastName = "User",
                NickName = "Test Super User",
                Email = "TestSuperUser@example.com"
            };

            request.Payload = payload;

            var baseUrl = "https://example.com";

            var html = "../../../../SudokuCollective.Api/Content/EmailTemplates/confirm-old-email-inlined.html";

            // Act
            var result = await sutFailure.Update(userId, request, baseUrl, html);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("User Name not Unique"));
        }

        [Test, Category("Services")]
        public async Task RequireUserNameForUpdates()
        {
            // Arrange
            var userId = 1;

            var payload = new UpdateUserPayload()
            {
                UserName = null,
                FirstName = "Test Super",
                LastName = "User",
                NickName = "Test Super User",
                Email = "TestSuperUser@example.com"
            };

            request.Payload = payload;

            var baseUrl = "https://example.com";

            var html = "../../../../SudokuCollective.Api/Content/EmailTemplates/confirm-old-email-inlined.html";

            // Act
            var result = await sut.Update(userId, request, baseUrl, html);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("User Name Required"));
        }

        [Test, Category("Services")]
        public async Task RequireUniqueEmailWithUpdates()
        {
            // Arrange
            var userId = 1;

            var payload = new UpdateUserPayload()
            {
                UserName = "TestSuperUserUPDATED",
                FirstName = "Test Super",
                LastName = "User",
                NickName = "Test Super User",
                Email = "TestUser@example.com"
            };

            request.Payload = payload;

            var baseUrl = "https://example.com";

            var html = "../../../../SudokuCollective.Api/Content/EmailTemplates/confirm-old-email-inlined.html";

            // Act
            var result = await sutEmailFailure.Update(userId, request, baseUrl, html);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Email not Unique"));
        }

        [Test, Category("Services")]
        public async Task RequireEmailWithUpdates()
        {
            // Arrange
            var userId = 1;

            var payload = new UpdateUserPayload()
            {
                UserName = "TestSuperUserUPDATED",
                FirstName = "Test Super",
                LastName = "User",
                NickName = "Test Super User"
            };

            request.Payload = payload;

            var baseUrl = "https://example.com";

            var html = "../../../../SudokuCollective.Api/Content/EmailTemplates/confirm-old-email-inlined.html";

            // Act
            var result = await sut.Update(userId, request, baseUrl, html);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Email Required"));
        }

        [Test, Category("Services")]
        public async Task UpdateUserPassword()
        {
            // Arrange
            var user = context.Users.FirstOrDefault(u => u.Id == 2);
            user.ReceivedRequestToUpdatePassword = true;
            context.SaveChanges();

            var payload = new PasswordResetPayload()
            {
                UserId = user.Id,
                NewPassword = "T3stP@ssw0rd",
            };

            request.Payload = payload;

            var license = TestObjects.GetLicense();

            // Act
            var result = await sut.UpdatePassword(
                request,
                license);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Password Reset"));
        }

        [Test, Category("Services")]
        public async Task DeleteUsers()
        {
            // Arrange
            var userId = 2;
            var license = TestObjects.GetLicense();

            // Act
            var result = await sut.Delete(userId, license);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test, Category("Services")]
        public async Task ReturnErrorMessageIfUserNotFoundForDeletion()
        {
            // Arrange
            var userId = 4;
            var license = TestObjects.GetLicense();

            // Act
            var result = await sutFailure.Delete(userId, license);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("User not Found"));
        }

        [Test, Category("Services")]
        public async Task AddRolesToUsers()
        {
            // Arrange
            var userId = 2;

            var user = context.Users
                .Include(u => u.Roles)
                .FirstOrDefault(u => u.Id == userId);

            var payload = new UpdateUserRolePayload();
            payload.RoleIds.Add(3);
            var license = TestObjects.GetLicense();

            // Act
            var result = await sut.AddUserRoles(
                userId,
                payload.RoleIds,
                license);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Roles Added"));
        }

        [Test, Category("Services")]
        public async Task RemoveRolesFromUsers()
        {
            // Arrange
            var userId = 1;

            var user = context.Users
                .Include(u => u.Roles)
                .FirstOrDefault(u => u.Id == userId);

            var payload = new UpdateUserRolePayload();
            payload.RoleIds.Add(3);
            var license = TestObjects.GetLicense();

            // Act
            var result = await sut.RemoveUserRoles(
                userId,
                payload.RoleIds,
                license);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Roles Removed"));
        }

        [Test, Category("Services")]
        public async Task ActivateUsers()
        {
            // Arrange
            var userId = 1;

            // Act
            var result = await sut.Activate(userId);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("User Activated"));
        }

        [Test, Category("Services")]
        public async Task DeactivateUsers()
        {
            // Arrange
            var userId = 1;

            // Act
            var result = await sut.Deactivate(userId);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("User Deactivated"));
        }

        [Test, Category("Services")]
        public async Task InitiatePasswordReset()
        {
            // Arrange
            var passwordReset = context.PasswordResets.FirstOrDefault();

            // Act
            var result = await sutResetPassword.InitiatePasswordReset(
                passwordReset.Token,
                TestObjects.GetLicense());

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("User Found"));
        }

        [Test, Category("Services")]
        public async Task ReturnsFalseIfInitiatePasswordResetFails()
        {
            // Arrange
            var passwordReset = context.PasswordResets.FirstOrDefault();

            // Act
            var result = await sutFailure.InitiatePasswordReset(
                passwordReset.Token,
                TestObjects.GetLicense());

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Password Reset Request not Found"));
        }

        [Test, Category("Services")]
        public async Task ResendEmailConfirmations()
        {
            // Arrange
            var baseUrl = "https://example.com";

            var html = "../../../../SudokuCollective.Api/Content/EmailTemplates/confirm-old-email-inlined.html";

            var license = TestObjects.GetLicense();

            // Act
            var result = await sutResendEmailConfirmation.ResendEmailConfirmation(
                3,
                1,
                baseUrl,
                html,
                license);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Email Confirmation Email Resent"));
        }

        [Test, Category("Services")]
        public async Task ReturnsFalseForResendEmailConfirmationsIfUserEmailConfirmed()
        {
            // Arrange
            var baseUrl = "https://example.com";

            var html = "../../../../SudokuCollective.Api/Content/EmailTemplates/confirm-old-email-inlined.html";

            var license = TestObjects.GetLicense();

            // Act
            var result = await sutFailure.ResendEmailConfirmation(
                3,
                1,
                baseUrl,
                html,
                license);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
        }

        [Test, Category("Services")]
        public async Task CancelEmailConfirmationRequests()
        {
            // Arrange
            var user = context.Users.FirstOrDefault(u => u.Id == 1);
            var app = context.Apps.FirstOrDefault(a => a.Id == 1);

            // Act
            var result = await sut.CancelEmailConfirmationRequest(user.Id, app.Id);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Email Confirmation Request Cancelled"));
            Assert.That((UserResult)result.Payload[0], Is.TypeOf<UserResult>());
        }

        [Test, Category("Services")]
        public async Task ReturnsFalseIfCancelEmailConfirmationRequestsFails()
        {
            // Arrange
            var user = context.Users.FirstOrDefault(u => u.Id == 1);
            var app = context.Apps.FirstOrDefault(a => a.Id == 1);

            // Act
            var result = await sutFailure.CancelEmailConfirmationRequest(user.Id, app.Id);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("User not Found"));
        }

        [Test, Category("Services")]
        public async Task ResendPasswordResetEmail()
        {
            // Arrange
            var baseUrl = "https://example.com";

            var html = "../../../../SudokuCollective.Api/Content/EmailTemplates/confirm-old-email-inlined.html";

            // Act
            var result = await sutResetPassword.ResendPasswordReset(3, 1, baseUrl, html);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Password Reset Email Resent"));
        }

        [Test, Category("Services")]
        public async Task ReturnsFalseIfResendPasswordResetEmailFails()
        {
            // Arrange
            var baseUrl = "https://example.com";

            var html = "../../../../SudokuCollective.Api/Content/EmailTemplates/confirm-old-email-inlined.html";

            // Act
            var result = await sutFailure.ResendPasswordReset(1, 1, baseUrl, html);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
        }

        [Test, Category("Services")]
        public async Task CancelPasswordResetRequests()
        {
            // Arrange
            var user = context.Users.FirstOrDefault(u => u.Id == 1);
            var app = context.Apps.FirstOrDefault(a => a.Id == 1);

            // Act
            var result = await sut.CancelPasswordResetRequest(user.Id, app.Id);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Password Reset Request Cancelled"));
            Assert.That((UserResult)result.Payload[0], Is.TypeOf<UserResult>());
        }

        [Test, Category("Services")]
        public async Task ReturnsFalseIfCancelPasswordResetRequestFails()
        {
            // Arrange
            var user = context.Users.FirstOrDefault(u => u.Id == 1);
            var app = context.Apps.FirstOrDefault(a => a.Id == 1);

            // Act
            var result = await sutFailure.CancelPasswordResetRequest(user.Id, app.Id);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("User not Found"));
        }

        [Test, Category("Services")]
        public async Task CancelAllEmailRequests()
        {
            // Arrange
            var user = context.Users.FirstOrDefault(u => u.Id == 1);
            var app = context.Apps.FirstOrDefault(a => a.Id == 1);

            // Act
            var result = await sut.CancelAllEmailRequests(user.Id, app.Id);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Email Confirmation Request Cancelled and Password Reset Request Cancelled"));
            Assert.That((UserResult)result.Payload[0], Is.TypeOf<UserResult>());
        }

        [Test, Category("Services")]
        public async Task ReturnFalseIfCancelAllEmailRequestsFails()
        {
            // Arrange
            var user = context.Users.FirstOrDefault(u => u.Id == 1);
            var app = context.Apps.FirstOrDefault(a => a.Id == 1);

            // Act
            var result = await sutFailure.CancelAllEmailRequests(user.Id, app.Id);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
        }

        [Test, Category("Services")]
        public async Task SuccessfullyGetUserByPasswordToken()
        {
            // Arrange

            // Act
            var result = await sut.GetUserByPasswordToken(Guid.NewGuid().ToString());

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("User Found"));
            Assert.That((User)result.Payload[0], Is.TypeOf<User>());
        }

        [Test, Category("Services")]
        public async Task ReturnFalseIfGetUserByPasswordTokenFails()
        {
            // Arrange

            // Act
            var result = await sutFailure.GetUserByPasswordToken(Guid.NewGuid().ToString());

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("User not Found"));
        }

        [Test, Category("Services")]
        public async Task SuccessfullyGetLicenseByPasswordToken()
        {
            // Arrange

            // Act
            var result = await sut.GetAppLicenseByPasswordToken(Guid.NewGuid().ToString());

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("App Found"));
            Assert.That(result.License, Is.TypeOf<string>());
        }

        [Test, Category("Services")]
        public async Task ReturnFalseIfGetLicenseByPasswordTokenFails()
        {
            // Arrange

            // Act
            var result = await sutFailure.GetAppLicenseByPasswordToken(Guid.NewGuid().ToString());

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("No Outstanding Request to Reset Password"));
        }
    }
}
