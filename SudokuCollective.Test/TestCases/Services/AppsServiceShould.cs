using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NUnit.Framework;
using SudokuCollective.Cache;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Payloads;
using SudokuCollective.Data.Models.Requests;
using SudokuCollective.Data.Services;
using SudokuCollective.Test.Cache;
using SudokuCollective.Test.Repositories;
using SudokuCollective.Test.Services;
using SudokuCollective.Test.TestData;

namespace SudokuCollective.Test.TestCases.Services
{
    public class AppsServiceShould
    {
        private DatabaseContext context;
        private MockedAppsRepository mockedAppsRepository;
        private MockedUsersRepository mockedUsersRepository;
        private MockedAppAdminsRepository mockedAppAdminsRepository;
        private MockedRolesRepository mockedRolesRepository;
        private MockedRequestService mockedRequestService;
        private MockedCacheService mockedCacheService;
        private MemoryDistributedCache memoryCache;
        private Mock<IHttpContextAccessor> mockedHttpContextAccessor;
        private Mock<ILogger<AppsService>> mockedLogger;
        private IAppsService sut;
        private IAppsService sutAppRepoFailure;
        private IAppsService sutUserRepoFailure;
        private IAppsService sutPromoteUser;
        private DateTime dateCreated;
        private string license;
        private Paginator paginator;
        private int userId;
        private int appId;
        private User user;
        private App app;

        [SetUp]
        public async Task Setup()
        {
            context = await TestDatabase.GetDatabaseContext();

            mockedAppsRepository = new MockedAppsRepository(context);
            mockedUsersRepository = new MockedUsersRepository(context);
            mockedAppAdminsRepository = new MockedAppAdminsRepository(context);
            mockedRolesRepository = new MockedRolesRepository(context);
            mockedRequestService = new MockedRequestService();
            mockedCacheService = new MockedCacheService(context);
            memoryCache = new MemoryDistributedCache(
                Options.Create(new MemoryDistributedCacheOptions()));
            mockedHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockedLogger = new Mock<ILogger<AppsService>>();

            dateCreated = DateTime.UtcNow;
            license = TestObjects.GetLicense();
            paginator = TestObjects.GetPaginator();
            userId = 1;
            appId = 1;
            user = context.Users.FirstOrDefault(u => u.Id == userId);
            app = context.Apps.FirstOrDefault(a => a.Id == appId);

            mockedHttpContextAccessor = TestObjects.GetHttpContextAccessor(user, app);

            sut = new AppsService(
                mockedAppsRepository.SuccessfulRequest.Object,
                mockedUsersRepository.SuccessfulRequest.Object,
                mockedAppAdminsRepository.SuccessfulRequest.Object,
                mockedRolesRepository.SuccessfulRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                memoryCache,
                mockedCacheService.SuccessfulRequest.Object,
                new CacheKeys(),
                new CachingStrategy(),
                mockedHttpContextAccessor.Object,
                mockedLogger.Object);

            sutAppRepoFailure = new AppsService(
                mockedAppsRepository.FailedRequest.Object,
                mockedUsersRepository.SuccessfulRequest.Object,
                mockedAppAdminsRepository.FailedRequest.Object,
                mockedRolesRepository.FailedRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                memoryCache,
                mockedCacheService.FailedRequest.Object,
                new CacheKeys(),
                new CachingStrategy(),
                mockedHttpContextAccessor.Object,
                mockedLogger.Object);

            sutUserRepoFailure = new AppsService(
                mockedAppsRepository.SuccessfulRequest.Object,
                mockedUsersRepository.FailedRequest.Object,
                mockedAppAdminsRepository.FailedRequest.Object,
                mockedRolesRepository.SuccessfulRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                memoryCache,
                mockedCacheService.FailedRequest.Object,
                new CacheKeys(),
                new CachingStrategy(),
                mockedHttpContextAccessor.Object,
                mockedLogger.Object);

            sutPromoteUser = new AppsService(
                mockedAppsRepository.SuccessfulRequest.Object,
                mockedUsersRepository.InitiatePasswordSuccessfulRequest.Object,
                mockedAppAdminsRepository.PromoteUserRequest.Object,
                mockedRolesRepository.SuccessfulRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                memoryCache,
                mockedCacheService.SuccessfulRequest.Object,
                new CacheKeys(),
                new CachingStrategy(),
                mockedHttpContextAccessor.Object,
                mockedLogger.Object);
        }

        [Test, Category("Services")]
        public async Task GetAppByID()
        {
            // Arrange

            // Act
            var result = await sut.Get(1);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("App Found"));
            Assert.That(result.Payload[0], Is.TypeOf<App>());
        }

        [Test, Category("Services")]
        public async Task GetAppByIDReturnsFalseIfNotFound()
        {
            // Arrange

            // Act
            var result = await sutAppRepoFailure.Get(3);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("App not Found"));
            Assert.That(result.Payload.Count, Is.EqualTo(0));
        }

        [Test, Category("Services")]
        public async Task GetApps()
        {
            // Arrange

            // Act
            var result = await sut.GetApps(new Paginator(), 1);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Apps Found"));
            Assert.That(result.Payload.Count, Is.EqualTo(2));
        }

        [Test, Category("Services")]
        public async Task CreateApps()
        {
            // Arrange

            // Act
            var result = await sut.Create(new LicenseRequest()
            {

                Name = "Test App 3",
                OwnerId = 1,
                LocalUrl = "https://localhost:8081",
                StagingUrl = "https://testapp3-dev.com",
                ProdUrl = "https://testapp3.com"
            });

            var apps = context.Apps.ToList();

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("App Created"));
            Assert.That(((App)result.Payload[0]).IsActive, Is.True);
        }

        [Test, Category("Services")]
        public async Task NotCreateAppsIfOwnerDoesNotExist()
        {
            // Arrange

            // Act
            var result = await sutUserRepoFailure.Create(new LicenseRequest()
            {

                Name = "Test App 3",
                OwnerId = 4,
                LocalUrl = "https://localhost:8081",
                StagingUrl = "https://testapp3-dev.com",
                ProdUrl = "https://testapp3.com"
            });

            var apps = context.Apps.ToList();

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("User does not Exist"));
            Assert.That(apps.Count, Is.EqualTo(2));
        }

        [Test, Category("Services")]
        public async Task GetAppByLicense()
        {
            // Arrange

            // Act
            var result = await sut.GetByLicense(license);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("App Found"));
            Assert.That(((App)result.Payload[0]).Id, Is.EqualTo(1));
            Assert.That(result.Payload[0], Is.TypeOf<App>());
        }

        [Test, Category("Services")]
        public async Task NotGetAppByLicenseIfInvalid()
        {
            // Arrange
            var invalidLicense = "5CDBFC8F-F304-4703-831B-750A7B7F8531";

            // Act
            var result = await sutAppRepoFailure.GetByLicense(invalidLicense);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("App not Found"));
        }

        [Test, Category("Services")]
        public async Task RetrieveLicense()
        {
            // Arrange

            // Act
            var result = await sut.GetLicense(1);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("App Found"));
            Assert.That(result.License, Is.EqualTo(license));
        }

        [Test, Category("Services")]
        public async Task NotRetrieveLicenseIfAppDoesNotExist()
        {
            // Arrange

            // Act
            var result = await sutAppRepoFailure.GetLicense(5);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("App not Found"));
            Assert.That(result.License, Is.Not.EqualTo(license));
        }

        [Test, Category("Services")]
        public async Task GetUsersByApp()
        {
            // Arrange

            // Act
            var result = await sut.GetAppUsers(1, 1, paginator);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Users Found"));
            Assert.That(result.Payload.Count, Is.EqualTo(2));
        }

        [Test, Category("Services")]
        public async Task UpdateApps()
        {
            // Arrange

            var payload = new AppPayload()
                {
                    Name = "Test App 1... UPDATED!",
                    LocalUrl = "https://localhost:4200",
                    StagingUrl = "https://testapp-dev.com",
                    QaUrl = string.Empty,
                    ProdUrl = "https://testapp.com",
                    IsActive = true,
                    Environment = ReleaseEnvironment.LOCAL,
                    PermitSuperUserAccess = true,
                    PermitCollectiveLogins = false,
                    DisableCustomUrls = false,
                    CustomEmailConfirmationAction = "ConfirmEmail",
                    CustomPasswordResetAction = "ResetPassword",
                    TimeFrame = TimeFrame.DAYS,
                    AccessDuration = 1
                };
            var request = new Request { Payload = payload };

            // Act
            var result = await sut.Update(1, request);

            var name = ((App)result.Payload[0]).Name;
            var apps = context.Apps.ToList();

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("App Updated"));
            Assert.That(name, Is.EqualTo("Test App 1... UPDATED!"));
        }

        [Test, Category("Services")]
        public async Task AddUsersToApp()
        {
            // Arrange

            // Act
            var result = await sut.AddAppUser(1, 3);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("User Added to App"));
        }

        [Test, Category("Services")]
        public async Task RemoveUsersFromApps()
        {
            // Arrange

            // Act
            var result = await sut.RemoveAppUser(1, 2);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("User Removed from App"));
        }

        [Test, Category("Services")]
        public async Task DeleteApps()
        {
            // Arrange

            // Act
            var result = await sut.DeleteOrReset(2);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("App Deleted"));
        }

        [Test, Category("Services")]
        public async Task ActivateApps()
        {
            // Arrange

            // Act
            var result = await sut.Activate(1);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("App Activated"));
        }

        [Test, Category("Services")]
        public async Task DeactivateApps()
        {
            // Arrange

            // Act
            var result = await sut.Deactivate(1);
            var app = context.Apps.FirstOrDefault(a => a.Id == 1);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("App Deactivated"));
        }

        [Test, Category("Services")]
        public async Task PermitValidRequests()
        {
            // Arrange

            // Act
            var result = await sut.IsRequestValidOnThisToken(
                mockedHttpContextAccessor.Object, 
                license, 
                appId, 
                userId);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test, Category("Services")]
        public async Task DenyInvalidRequests()
        {
            // Arrange
            var invalidLicense = "5CDBFC8F-F304-4703-831B-750A7B7F8531";

            // Act
            var result = await sutAppRepoFailure.IsRequestValidOnThisToken(
                mockedHttpContextAccessor.Object, 
                invalidLicense, 
                appId, 
                userId);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test, Category("Services")]
        public async Task DenyRequestWhereUserIsNotRegisteredToApp()
        {
            // Arrange
            var invalidLicense = "5CDBFC8F-F304-4703-831B-750A7B7F8531";

            appId = 1;

            var user = new User()
            {
                Id = 4,
                UserName = "TestUser3",
                FirstName = "John",
                LastName = "Doe",
                NickName = "Johnny Boy",
                Email = "testuser3@example.com",
                Password = "password1",
                DateCreated = dateCreated,
                DateUpdated = dateCreated
            };

            var invalidmockedHttpContextAccessor = TestObjects.GetInvalidHttpContextAccessor(user);

            // Act
            var result = await sutUserRepoFailure.IsRequestValidOnThisToken(
                invalidmockedHttpContextAccessor.Object, 
                invalidLicense, 
                appId, 
                user.Id);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test, Category("Services")]
        public async Task PermitSuperUserSystemWideAccess()
        {
            // Arrange
            var app = context.Apps.FirstOrDefault(a => a.Id == 2);
            var licenseResult =await sut.GetLicense(app.Id);
            var superUser = context.Users.Where(user => user.Id == 1).FirstOrDefault();

            // Act
            var superUserIsInApp = app.Users
                .Any(ua => ua.UserId == superUser.Id);

            var mockSuperUserHttpContextAccessor = new Mock<IHttpContextAccessor>();

            var claim = new List<Claim> {

                new Claim(ClaimTypes.Name, "TestSuperUser"),
                new Claim(ClaimTypes.Name, superUser.Id.ToString()),
                new Claim(ClaimTypes.Name, app.Id.ToString()),
            };
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("865542af-e02f-446d-ad34-b121554f37be"));
            
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expirationLimit = DateTime.UtcNow.AddDays(1);

            var jwtInvalidToken = new JwtSecurityToken(
                    "test",
                    "test",
                    claim.ToArray(),
                    notBefore: DateTime.UtcNow,
                    expires: expirationLimit,
                    signingCredentials: credentials
                );

            mockSuperUserHttpContextAccessor.Setup(
                mock => mock.HttpContext.Request.Headers["Authorization"])
                .Returns(string.Format("bearer {0}", new JwtSecurityTokenHandler().WriteToken(jwtInvalidToken)));

            var result = await sut.IsRequestValidOnThisToken(
                mockSuperUserHttpContextAccessor.Object, 
                licenseResult.License, 
                app.Id, 
                superUser.Id);

            // Assert
            Assert.That(superUserIsInApp, Is.False);
            Assert.That(superUser.IsSuperUser, Is.True);
            Assert.That(result, Is.True);
        }

        [Test, Category("Services")]
        public async Task PermitOwnerRequests()
        {
            // Arrange

            // Act
            var result = await sut.IsUserOwnerOThisfApp(
                mockedHttpContextAccessor.Object, 
                license,
                userId,
                license,
                appId, 
                userId);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test, Category("Services")]
        public async Task DenyInvalidOwnerRequests()
        {
            // Arrange
            var invalidLicense = "5CDBFC8F-F304-4703-831B-750A7B7F8531";

            // Act
            var result = await sutUserRepoFailure.IsUserOwnerOThisfApp(
                mockedHttpContextAccessor.Object, 
                invalidLicense,
                userId,
                license,
                appId, 
                userId);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test, Category("Services")]
        public async Task PromoteUsersToAdmin()
        {
            // Arrange

            // Act
            var result = await sutPromoteUser.ActivateAdminPrivileges(1, 3);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("User has been Promoted to Admin"));
        }

        [Test, Category("Services")]
        public async Task ReturnFalseIfPromoteUsersToAdminFails()
        {
            // Arrange

            // Act
            var result = await sutAppRepoFailure.ActivateAdminPrivileges(1, 3);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("App not Found"));
        }

        [Test, Category("Services")]
        public async Task DeactivateUserAdminPrivileges()
        {
            // Arrange

            // Act
            var result = await sut
                .DeactivateAdminPrivileges(1, 3);

            // Assert
            Assert.That(result, Is.InstanceOf<Core.Interfaces.Models.DomainObjects.Params.IResult>());
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Admin Privileges Deactivated"));
            Assert.That(result.Payload[0], Is.TypeOf<User>());

        }

        [Test, Category("Services")]
        public async Task ReturnFalseIfDeactivateUserAdminPrivilegesFails()
        {
            // Arrange

            // Act
            var result = await sutAppRepoFailure
                .DeactivateAdminPrivileges(1, 3);

            // Assert
            Assert.That(result, Is.InstanceOf<Core.Interfaces.Models.DomainObjects.Params.IResult>());
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("App not Found"));
        }

        [Test, Category("Services")]
        public async Task GetMyApps()
        {
            // Arrange

            // Act
            var result = await sut.GetMyApps(1, new Paginator());

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Apps Found"));
            Assert.That(result.Payload.Count, Is.EqualTo(2));
        }

        [Test, Category("Services")]
        public async Task ReturnFalseIfGetMyAppsFails()
        {
            // Arrange

            // Act
            var result = await sutAppRepoFailure.GetMyApps(1, new Paginator());

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Apps not Found"));
        }

        [Test, Category("Services")]
        public async Task GetRegisteredApps()
        {
            // Arrange

            // Act
            var result = await sut.GetRegisteredApps(2, new Paginator());

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Apps Found"));
            Assert.That(result.Payload.Count, Is.EqualTo(2));
        }

        [Test, Category("Services")]
        public async Task ReturnFalseIfGetRegisteredAppsFails()
        {
            // Arrange

            // Act
            var result = await sutAppRepoFailure.GetRegisteredApps(1, new Paginator());

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Apps not Found"));
        }
    }
}
