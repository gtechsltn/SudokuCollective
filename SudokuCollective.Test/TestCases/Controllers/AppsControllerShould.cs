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
            var actionResult = await sutSuccess.GetAsync(appId, request);
            var result = (Result)((OkObjectResult)actionResult.Result).Value;
            var app = (App)result.Payload[0];
            var message = result.Message;
            var statusCode = ((OkObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
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
            var actionResult = await sutFailure.GetAsync(appId, request);
            var result = (Result)((NotFoundObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var statusCode = ((NotFoundObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 404: App not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyGetAppByLicense()
        {
            // Arrange

            // Act
            var actionResult = await sutSuccess.GetByLicenseAsync(
                TestObjects.GetLicense(), 
                request);
            var result = (Result)((OkObjectResult)actionResult.Result).Value;
            var app = (App)result.Payload[0];
            var message = result.Message;
            var statusCode = ((OkObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 200: App Found"));
            Assert.That(app, Is.InstanceOf<App>());
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldGetAppByLicenseFail()
        {
            // Arrange

            // Act
            var actionResult = await sutFailure.GetByLicenseAsync(
                TestObjects.GetInvalidLicense(),
                request);
            var result = (Result)((NotFoundObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var statusCode = ((NotFoundObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 404: App not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyGetApps()
        {
            // Arrange

            // Act
            var actionResult = await sutSuccess.GetAppsAsync(request);
            var result = (Result)((OkObjectResult)actionResult.Result).Value;
            var apps = result.Payload.ConvertAll(a => (App)a);
            var message = result.Message;
            var statusCode = ((OkObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 200: Apps Found"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(apps, Is.InstanceOf<List<App>>());
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyGetAppsFail()
        {
            // Arrange

            // Act
            var actionResult = await sutFailure.GetAppsAsync(request);
            var result = (Result)((NotFoundObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var statusCode = ((NotFoundObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 404: Apps not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyUpdateApps()
        {
            // Arrange
            request.Payload = TestObjects.GetAppPayload();

            // Act
            var actionResult = await sutSuccess.UpdateAsync(1, request);
            var result = (Result)((OkObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var statusCode = ((OkObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 200: App Updated"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyUpdateAppsFail()
        {
            // Arrange
            request.Payload = TestObjects.GetInvalidAppPayload();

            // Act
            var actionResult = await sutFailure.UpdateAsync(1, request);
            var result = (Result)((NotFoundObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var statusCode = ((NotFoundObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 404: App not Updated"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyGetAppUsers()
        {
            // Arrange

            // Act
            var actionResult = await sutSuccess.GetAppUsersAsync(
                1,
                request);
            var result = (Result)((OkObjectResult)actionResult.Result).Value;
            var users = result.Payload.ConvertAll(u => (User)u);
            var message = result.Message;
            var statusCode = ((OkObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 200: Users Found"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(users.Count, Is.EqualTo(2));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyGetAppUsersFail()
        {
            // Arrange

            // Act
            var actionResult = await sutFailure.GetAppUsersAsync(
                1,
                request);
            var result = (Result)((NotFoundObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var statusCode = ((NotFoundObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 404: Users not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyAddUserToApp()
        {
            // Arrange

            // Act
            var actionResult = await sutSuccess.AddUserAsync(1, 3, request);
            var result = (Result)((OkObjectResult)actionResult.Result).Value;
            var user = (User)result.Payload[0];
            var message = result.Message;
            var statusCode = ((OkObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 200: User Added to App"));
            Assert.That(user, Is.InstanceOf<User>());
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyAddUserToAppFail()
        {
            // Arrange

            // Act
            var actionResult = await sutFailure.AddUserAsync(1, 3, request);
            var result = (Result)((NotFoundObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var statusCode = ((NotFoundObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 404: User not Added to App"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyRemoveUserFromApp()
        {
            // Arrange

            // Act
            var actionResult = await sutSuccess.RemoveUserAsync(1, 3, request);
            var result = (Result)((OkObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var user = (User)result.Payload[0];
            var statusCode = ((OkObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 200: User Removed from App"));
            Assert.That(user, Is.InstanceOf<User>());
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyRemoveUserFromAppFail()
        {
            // Arrange

            // Act
            var actionResult = await sutFailure.RemoveUserAsync(1, 3, request);
            var result = (Result)((NotFoundObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var statusCode = ((NotFoundObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 404: User not Removed from App"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyActivateAnApp()
        {
            // Arrange

            // Act
            var actionResult = await sutSuccess.ActivateAsync(1, request);
            var result = (Result)((OkObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var app = (App)result.Payload[0];
            var statusCode = ((OkObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 200: App Activated"));
            Assert.That(app, Is.InstanceOf<App>());
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyActivateAnAppFail()
        {
            // Arrange

            // Act
            var actionResult = await sutFailure.ActivateAsync(1, request);
            var result = (Result)((NotFoundObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var statusCode = ((NotFoundObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 404: App not Activated"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyDeactivateAnApp()
        {
            // Arrange

            // Act
            var actionResult = await sutSuccess.DeactivateAsync(1, request);
            var result = (Result)((OkObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var app = (App)result.Payload[0];
            var statusCode = ((OkObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 200: App Deactivated"));
            Assert.That(app, Is.InstanceOf<App>());
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyDeactivateAnAppFail()
        {
            // Arrange

            // Act
            var actionResult = await sutFailure.DeactivateAsync(1, request);
            var result = (Result)((NotFoundObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var statusCode = ((NotFoundObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 404: App not Deactivated"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task ReturnBadRequestResponseShouldLicenseValidationFail()
        {
            // Arrange
            var appId = 1;

            // Act
            var actionResultOne = await sutInvalid.GetAsync(appId, request);
            var resultOne = (Result)
                ((BadRequestObjectResult)actionResultOne.Result)
                    .Value;
            var messageOne = resultOne.Message;
            var statusCodeOne = ((BadRequestObjectResult)actionResultOne.Result).StatusCode;

            var actionResultTwo = await sutInvalid.GetAppsAsync(request);
            var resultTwo = (Result)
                ((BadRequestObjectResult)actionResultTwo.Result)
                    .Value;
            var messageTwo = resultTwo.Message;
            var statusCodeTwo = ((BadRequestObjectResult)actionResultTwo.Result).StatusCode;

            var actionResultThree = await sutInvalid.UpdateAsync(1, request);
            var resultThree = (Result)
                ((BadRequestObjectResult)actionResultThree.Result)
                    .Value;
            var messageThree = resultThree.Message;
            var statusCodeThree = ((BadRequestObjectResult)actionResultThree.Result).StatusCode;

            var actionResultFour = await sutInvalid.GetAppUsersAsync(1, request);
            var resultFour = (Result)
                ((BadRequestObjectResult)actionResultFour.Result)
                    .Value;
            var messageFour = resultFour.Message;
            var statusCodeFour = ((BadRequestObjectResult)actionResultFour.Result).StatusCode;

            var actionResultFive = await sutInvalid.AddUserAsync(1, 3, request);
            var resultFive = (Result)
                ((BadRequestObjectResult)actionResultFive.Result)
                    .Value;
            var messageFive = resultFive.Message;
            var statusCodeFive = ((BadRequestObjectResult)actionResultFour.Result).StatusCode;

            var actionResultSix = await sutInvalid.RemoveUserAsync(1, 3, request);
            var resultSix = (Result)
                ((BadRequestObjectResult)actionResultSix.Result)
                    .Value;
            var messageSix = resultSix.Message;
            var statusCodeSix = ((BadRequestObjectResult)actionResultFour.Result).StatusCode;

            // Assert
            Assert.That(actionResultOne, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(resultOne, Is.InstanceOf<Result>());
            Assert.That(messageOne, Is.EqualTo("Status Code 400: Invalid Request on this Token"));
            Assert.That(statusCodeOne, Is.EqualTo(400));
            Assert.That(actionResultTwo, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(resultTwo, Is.InstanceOf<Result>());
            Assert.That(messageTwo, Is.EqualTo("Status Code 400: Invalid Request on this Token"));
            Assert.That(statusCodeTwo, Is.EqualTo(400));
            Assert.That(actionResultThree, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(resultThree, Is.InstanceOf<Result>());
            Assert.That(messageThree, Is.EqualTo("Status Code 400: Invalid Request on this Token"));
            Assert.That(statusCodeThree, Is.EqualTo(400));
            Assert.That(actionResultFour, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(resultFour, Is.InstanceOf<Result>());
            Assert.That(messageFour, Is.EqualTo("Status Code 400: Invalid Request on this Token"));
            Assert.That(statusCodeFour, Is.EqualTo(400));
            Assert.That(actionResultFive, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(resultFive, Is.InstanceOf<Result>());
            Assert.That(messageFive, Is.EqualTo("Status Code 400: Invalid Request on this Token"));
            Assert.That(statusCodeFive, Is.EqualTo(400));
            Assert.That(actionResultSix, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(resultSix, Is.InstanceOf<Result>());
            Assert.That(messageSix, Is.EqualTo("Status Code 400: Invalid Request on this Token"));
            Assert.That(statusCodeSix, Is.EqualTo(400));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyAllowSuperuserToDeleteApps()
        {
            // Arrange

            // Act
            var actionResult = await sutSuccess.DeleteAsync(
                2, 
                TestObjects.GetSecondLicense(),
                request);
            var result = (Result)((OkObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var statusCode = ((OkObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 200: App Deleted"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyAllowSuperuserToDeleteAppsFail()
        {
            // Arrange

            // Act
            var actionResult = await sutFailure.DeleteAsync(
                2,
                TestObjects.GetSecondLicense(),
                request);
            var result = (Result)
                ((BadRequestObjectResult)actionResult.Result)
                    .Value;
            var message = result.Message;
            var statusCode = ((BadRequestObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 400: You are not the Owner of this App"));
            Assert.That(statusCode, Is.EqualTo(400));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyPromoteUserToAppAdmin()
        {
            // Arrange

            // Act
            var actionResult = await sutSuccess.ActivateAdminPrivilegesAsync(1, 3, request);
            var result = (Result)((OkObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var user = (User)result.Payload[0];
            var statusCode = ((OkObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 200: User has been Promoted to Admin"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(user, Is.InstanceOf<User>());
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyPromoteUserToAppAdminFail()
        {
            // Arrange

            // Act
            var actionResult = await sutPromoteUserFailure.ActivateAdminPrivilegesAsync(1, 3, request);
            var result = (Result)((NotFoundObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var statusCode = ((NotFoundObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 404: User has not been Promoted to Admin"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyDeactivateAdminPrivileges()
        {
            // Arrange

            // Act
            var actionResult = await sutSuccess.DeactivateAdminPrivilegesAsync(1, 3, request);
            var result = (Result)((OkObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var user = (User)result.Payload[0];
            var statusCode = ((OkObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 200: Admin Privileges Deactivated"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(user, Is.InstanceOf<User>());
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyDeactivateAdminPrivilegesFail()
        {
            // Arrange

            // Act
            var actionResult = await sutPromoteUserFailure.DeactivateAdminPrivilegesAsync(1, 3, request);
            var result = (Result)((NotFoundObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var statusCode = ((NotFoundObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 404: Deactivation of Admin Privileges Failed"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyGetMyApps()
        {
            // Arrange

            // Act
            var actionResult = await sutSuccess.GetMyAppsAsync(request);
            var result = (Result)((OkObjectResult)actionResult.Result).Value;
            var apps = result.Payload.ConvertAll(a => (App)a);
            var message = result.Message;
            var statusCode = ((OkObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(message, Is.EqualTo("Status Code 200: Apps Found"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(apps, Is.InstanceOf<List<App>>());
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyGetMyAppsFail()
        {
            // Arrange

            // Act
            var actionResult = await sutFailure.GetMyAppsAsync(request);
            var result = (Result)((NotFoundObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var statusCode = ((NotFoundObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 404: Apps not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyGetMyRegisteredApps()
        {
            // Arrange

            // Act
            var actionResult = await sutSuccess.GetMyRegisteredAppsAsync(request);
            var result = (Result)((OkObjectResult)actionResult.Result).Value;
            var apps = result.Payload.ConvertAll(a =>(App)a);
            var message = result.Message;
            var statusCode = ((OkObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 200: Apps Found"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(apps, Is.InstanceOf<List<App>>());
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyGetRegisteredAppsFail()
        {
            // Arrange

            // Act
            var actionResult = await sutFailure.GetMyRegisteredAppsAsync(request);
            var result = (Result)((NotFoundObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var statusCode = ((NotFoundObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 404: Apps not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }
    }
}
