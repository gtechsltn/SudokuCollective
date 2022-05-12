using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SudokuCollective.Api.Controllers.V1;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Test.Services;
using SudokuCollective.Test.TestData;

namespace SudokuCollective.Test.TestCases.Controllers
{
    public class AppsControllerShould
    {
        private DatabaseContext context;
        private AppsController sutSuccess;
        private AppsController sutFailure;
        private AppsController sutInvalid;
        private AppsController sutPromoteUserFailure;
        private MockedAppsService mockAppsService;
        private MockedRequestService mockedRequestService;
        private Mock<IHttpContextAccessor> mockedHttpContextAccessor;
        private Mock<ILogger<AppsController>> mockedLogger;
        private Request request;

        [SetUp]
        public async Task Setup()
        {
            context = await TestDatabase.GetDatabaseContext();
            mockAppsService = new MockedAppsService(context);
            mockedRequestService = new MockedRequestService();
            mockedHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockedLogger = new Mock<ILogger<AppsController>>();

            request = new Request();

            sutSuccess = new AppsController(
                mockAppsService.SuccessfulRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                mockedHttpContextAccessor.Object,
                mockedLogger.Object);
            sutFailure = new AppsController(
                mockAppsService.FailedRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                mockedHttpContextAccessor.Object,
                mockedLogger.Object);
            sutInvalid = new AppsController(
                mockAppsService.InvalidRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                mockedHttpContextAccessor.Object,
                mockedLogger.Object);
            sutPromoteUserFailure = new AppsController(
                mockAppsService.PromoteUserFailsRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                mockedHttpContextAccessor.Object,
                mockedLogger.Object);
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyGetApp()
        {
            // Arrange
            var appId = 1;

            // Act
            var result = await sutSuccess.GetAsync(appId, request);
            var app = (App)((Result)((OkObjectResult)result.Result).Value).Payload[0];
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<App>>());
            Assert.That(message, Is.EqualTo("Status Code 200: App Found"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(app, Is.InstanceOf<App>());
            Assert.That(app.Id, Is.EqualTo(1));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldGetAppFail()
        {
            // Arrange
            var appId = 1;

            // Act
            var result = await sutFailure.GetAsync(appId, request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<App>>());
            Assert.That(message, Is.EqualTo("Status Code 404: App not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyGetAppByLicense()
        {
            // Arrange

            // Act
            var result = await sutSuccess.GetByLicenseAsync(
                TestObjects.GetLicense(), 
                request);
            var app = (App)((Result)((OkObjectResult)result.Result).Value).Payload[0];
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<App>>());
            Assert.That(message, Is.EqualTo("Status Code 200: App Found"));
            Assert.That(app, Is.InstanceOf<App>());
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldGetAppByLicenseFail()
        {
            // Arrange

            // Act
            var result = await sutFailure.GetByLicenseAsync(
                TestObjects.GetInvalidLicense(),
                request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<App>>());
            Assert.That(message, Is.EqualTo("Status Code 404: App not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyGetApps()
        {
            // Arrange

            // Act
            var result = await sutSuccess.GetAppsAsync(request);
            var apps = ((Result)((OkObjectResult)result.Result).Value).Payload.ConvertAll(a => (App)a);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<IEnumerable<App>>>());
            Assert.That(message, Is.EqualTo("Status Code 200: Apps Found"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(apps, Is.InstanceOf<List<App>>());
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyGetAppsFail()
        {
            // Arrange

            // Act
            var result = await sutFailure.GetAppsAsync(request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<IEnumerable<App>>>());
            Assert.That(message, Is.EqualTo("Status Code 404: Apps not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyUpdateApps()
        {
            // Arrange
            request.Payload = TestObjects.GetAppPayload();

            // Act
            var result = await sutSuccess.UpdateAsync(1, request);
            var message = ((Result)((OkObjectResult)result).Value).Message;
            var statusCode = ((OkObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: App Updated"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyUpdateAppsFail()
        {
            // Arrange
            request.Payload = TestObjects.GetInvalidAppPayload();

            // Act
            var result = await sutFailure.UpdateAsync(1, request);
            var message = ((Result)((NotFoundObjectResult)result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: App not Updated"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyGetAppUsers()
        {
            // Arrange

            // Act
            var result = await sutSuccess.GetAppUsersAsync(
                1,
                request);

            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;
            var users = ((Result)((OkObjectResult)result.Result).Value).Payload.ConvertAll(u => (User)u);

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<IEnumerable<User>>>());
            Assert.That(message, Is.EqualTo("Status Code 200: Users Found"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(users.Count, Is.EqualTo(2));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyGetAppUsersFail()
        {
            // Arrange

            // Act
            var result = await sutFailure.GetAppUsersAsync(
                1,
                request);

            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<IEnumerable<User>>>());
            Assert.That(message, Is.EqualTo("Status Code 404: Users not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyAddUserToApp()
        {
            // Arrange

            // Act
            var result = await sutSuccess.AddUserAsync(1, 3, request);
            var message = ((Result)((OkObjectResult)result).Value).Message;
            var user = (User)((Result)((OkObjectResult)result).Value).Payload[0];
            var statusCode = ((OkObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: User Added to App"));
            Assert.That(user, Is.InstanceOf<User>());
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyAddUserToAppFail()
        {
            // Arrange

            // Act
            var result = await sutFailure.AddUserAsync(1, 3, request);
            var message = ((Result)((NotFoundObjectResult)result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: User not Added to App"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyRemoveUserFromApp()
        {
            // Arrange

            // Act
            var result = await sutSuccess.RemoveUserAsync(1, 3, request);
            var message = ((Result)((OkObjectResult)result).Value).Message;
            var user = (User)((Result)((OkObjectResult)result).Value).Payload[0];
            var statusCode = ((OkObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: User Removed from App"));
            Assert.That(user, Is.InstanceOf<User>());
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyRemoveUserFromAppFail()
        {
            // Arrange

            // Act
            var result = await sutFailure.RemoveUserAsync(1, 3, request);
            var message = ((Result)((NotFoundObjectResult)result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: User not Removed from App"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyActivateAnApp()
        {
            // Arrange

            // Act
            var result = await sutSuccess.ActivateAsync(1, request);
            var message = ((Result)((OkObjectResult)result).Value).Message;
            var app = (App)((Result)((OkObjectResult)result).Value).Payload[0];
            var statusCode = ((OkObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: App Activated"));
            Assert.That(app, Is.InstanceOf<App>());
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyActivateAnAppFail()
        {
            // Arrange

            // Act
            var result = await sutFailure.ActivateAsync(1, request);
            var message = ((Result)((NotFoundObjectResult)result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: App not Activated"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyDeactivateAnApp()
        {
            // Arrange

            // Act
            var result = await sutSuccess.DeactivateAsync(1, request);
            var message = ((Result)((OkObjectResult)result).Value).Message;
            var app = (App)((Result)((OkObjectResult)result).Value).Payload[0];
            var statusCode = ((OkObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: App Deactivated"));
            Assert.That(app, Is.InstanceOf<App>());
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyDeactivateAnAppFail()
        {
            // Arrange

            // Act
            var result = await sutFailure.DeactivateAsync(1, request);
            var message = ((Result)((NotFoundObjectResult)result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: App not Deactivated"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task ReturnBadRequestResponseShouldLicenseValidationFail()
        {
            // Arrange
            var appId = 1;

            // Act
            var resultOne = await sutInvalid.GetAsync(appId, request);
            var messageOne = ((Result)
                ((BadRequestObjectResult)resultOne.Result)
                    .Value)
                    .Message;
            var statusCodeOne = ((BadRequestObjectResult)resultOne.Result).StatusCode;

            var resultTwo = await sutInvalid.GetAppsAsync(request);
            var messageTwo = ((Result)
                ((BadRequestObjectResult)resultTwo.Result)
                    .Value)
                    .Message;
            var statusCodeTwo = ((BadRequestObjectResult)resultTwo.Result).StatusCode;

            var resultThree = await sutInvalid.UpdateAsync(1, request);
            var messageThree = ((Result)
                ((BadRequestObjectResult)resultThree)
                    .Value)
                    .Message;
            var statusCodeThree = ((BadRequestObjectResult)resultThree).StatusCode;

            var resultFour = await sutInvalid.GetAppUsersAsync(1, request);
            var messageFour = ((Result)
                ((BadRequestObjectResult)resultFour.Result)
                    .Value)
                    .Message;
            var statusCodeFour = ((BadRequestObjectResult)resultFour.Result).StatusCode;

            var resultFive = await sutInvalid.AddUserAsync(1, 3, request);
            var messageFive = ((Result)
                ((BadRequestObjectResult)resultFive)
                    .Value)
                    .Message;
            var statusCodeFive = ((BadRequestObjectResult)resultFour.Result).StatusCode;

            var resultSix = await sutInvalid.RemoveUserAsync(1, 3, request);
            var messageSix = ((Result)
                ((BadRequestObjectResult)resultSix)
                    .Value)
                    .Message;
            var statusCodeSix = ((BadRequestObjectResult)resultFour.Result).StatusCode;

            // Assert
            Assert.That(resultOne, Is.InstanceOf<ActionResult<App>>());
            Assert.That(messageOne, Is.EqualTo("Status Code 400: Invalid Request on this Token"));
            Assert.That(statusCodeOne, Is.EqualTo(400));
            Assert.That(resultTwo, Is.InstanceOf<ActionResult<IEnumerable<App>>>());
            Assert.That(messageTwo, Is.EqualTo("Status Code 400: Invalid Request on this Token"));
            Assert.That(statusCodeTwo, Is.EqualTo(400));
            Assert.That(resultThree, Is.InstanceOf<ActionResult>());
            Assert.That(messageThree, Is.EqualTo("Status Code 400: Invalid Request on this Token"));
            Assert.That(statusCodeThree, Is.EqualTo(400));
            Assert.That(resultFour, Is.InstanceOf<ActionResult<IEnumerable<User>>>());
            Assert.That(messageFour, Is.EqualTo("Status Code 400: Invalid Request on this Token"));
            Assert.That(statusCodeFour, Is.EqualTo(400));
            Assert.That(resultFive, Is.InstanceOf<ActionResult>());
            Assert.That(messageFive, Is.EqualTo("Status Code 400: Invalid Request on this Token"));
            Assert.That(statusCodeFive, Is.EqualTo(400));
            Assert.That(resultSix, Is.InstanceOf<ActionResult>());
            Assert.That(messageSix, Is.EqualTo("Status Code 400: Invalid Request on this Token"));
            Assert.That(statusCodeSix, Is.EqualTo(400));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyAllowSuperuserToDeleteApps()
        {
            // Arrange

            // Act
            var result = await sutSuccess.DeleteAsync(
                2, 
                TestObjects.GetSecondLicense(),
                request);
            var message = ((Result)((OkObjectResult)result).Value).Message;
            var statusCode = ((OkObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: App Deleted"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyAllowSuperuserToDeleteAppsFail()
        {
            // Arrange

            // Act
            var result = await sutFailure.DeleteAsync(
                2,
                TestObjects.GetSecondLicense(),
                request);

            var errorMessage = ((Result)
                ((BadRequestObjectResult)result)
                    .Value)
                    .Message;
            var statusCode = ((BadRequestObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(errorMessage, Is.EqualTo("Status Code 400: You are not the Owner of this App"));
            Assert.That(statusCode, Is.EqualTo(400));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyPromoteUserToAppAdmin()
        {
            // Arrange

            // Act
            var result = await sutSuccess.ActivateAdminPrivilegesAsync(1, 3, request);
            var message = ((Result)((OkObjectResult)result).Value).Message;
            var user = (User)((Result)((OkObjectResult)result).Value).Payload[0];
            var statusCode = ((OkObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: User has been Promoted to Admin"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(user, Is.InstanceOf<User>());
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyPromoteUserToAppAdminFail()
        {
            // Arrange

            // Act
            var result = await sutPromoteUserFailure.ActivateAdminPrivilegesAsync(1, 3, request);
            var message = ((Result)((NotFoundObjectResult)result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: User has not been Promoted to Admin"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyDeactivateAdminPrivileges()
        {
            // Arrange

            // Act
            var result = await sutSuccess.DeactivateAdminPrivilegesAsync(1, 3, request);
            var message = ((Result)((OkObjectResult)result).Value).Message;
            var user = (User)((Result)((OkObjectResult)result).Value).Payload[0];
            var statusCode = ((OkObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Admin Privileges Deactivated"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(user, Is.InstanceOf<User>());
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyDeactivateAdminPrivilegesFail()
        {
            // Arrange

            // Act
            var result = await sutPromoteUserFailure.DeactivateAdminPrivilegesAsync(1, 3, request);
            var message = ((Result)((NotFoundObjectResult)result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Deactivation of Admin Privileges Failed"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyGetMyApps()
        {
            // Arrange

            // Act
            var result = await sutSuccess.GetMyAppsAsync(request);
            var apps = ((Result)((OkObjectResult)result.Result).Value).Payload.ConvertAll(a => (App)a);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<IEnumerable<App>>>());
            Assert.That(message, Is.EqualTo("Status Code 200: Apps Found"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(apps, Is.InstanceOf<List<App>>());
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyGetMyAppsFail()
        {
            // Arrange

            // Act
            var result = await sutFailure.GetMyAppsAsync(request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<IEnumerable<App>>>());
            Assert.That(message, Is.EqualTo("Status Code 404: Apps not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyGetMyRegisteredApps()
        {
            // Arrange

            // Act
            var result = await sutSuccess.GetMyRegisteredAppsAsync(request);
            var apps = ((Result)((OkObjectResult)result).Value).Payload.ConvertAll(a =>(App)a);
            var message = ((Result)((OkObjectResult)result).Value).Message;
            var statusCode = ((OkObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Apps Found"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(apps, Is.InstanceOf<List<App>>());
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyGetRegisteredAppsFail()
        {
            // Arrange

            // Act
            var result = await sutFailure.GetMyRegisteredAppsAsync(request);
            var message = ((Result)((NotFoundObjectResult)result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Apps not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }
    }
}
