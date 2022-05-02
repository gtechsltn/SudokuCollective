using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public void SuccessfullyGetApp()
        {
            // Arrange
            var appId = 1;

            // Act
            var result = sutSuccess.GetAsync(appId, request);
            var app = (App)((Result)((OkObjectResult)result.Result.Result).Value).Payload[0];
            var message = ((Result)((OkObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<App>>());
            Assert.That(message, Is.EqualTo("Status Code 200: App Found"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(app, Is.InstanceOf<App>());
            Assert.That(app.Id, Is.EqualTo(1));
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldGetAppFail()
        {
            // Arrange
            var appId = 1;

            // Act
            var result = sutFailure.GetAsync(appId, request);
            var message = ((Result)((NotFoundObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<App>>());
            Assert.That(message, Is.EqualTo("Status Code 404: App not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyGetAppByLicense()
        {
            // Arrange

            // Act
            var result = sutSuccess.GetByLicenseAsync(
                TestObjects.GetLicense(), 
                request);
            var app = (App)((Result)((OkObjectResult)result.Result.Result).Value).Payload[0];
            var message = ((Result)((OkObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<App>>());
            Assert.That(message, Is.EqualTo("Status Code 200: App Found"));
            Assert.That(app, Is.InstanceOf<App>());
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldGetAppByLicenseFail()
        {
            // Arrange

            // Act
            var result = sutFailure.GetByLicenseAsync(
                TestObjects.GetInvalidLicense(),
                request);
            var message = ((Result)((NotFoundObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<App>>());
            Assert.That(message, Is.EqualTo("Status Code 404: App not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyGetApps()
        {
            // Arrange

            // Act
            var result = sutSuccess.GetAppsAsync(request);
            var apps = ((Result)((OkObjectResult)result.Result.Result).Value).Payload.ConvertAll(a => (App)a);
            var message = ((Result)((OkObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<IEnumerable<App>>>());
            Assert.That(message, Is.EqualTo("Status Code 200: Apps Found"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(apps, Is.InstanceOf<List<App>>());
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldSuccessfullyGetAppsFail()
        {
            // Arrange

            // Act
            var result = sutFailure.GetAppsAsync(request);
            var message = ((Result)((NotFoundObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<IEnumerable<App>>>());
            Assert.That(message, Is.EqualTo("Status Code 404: Apps not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyUpdateApps()
        {
            // Arrange
            request.Payload = TestObjects.GetAppPayload();

            // Act
            var result = sutSuccess.UpdateAsync(1, request);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: App Updated"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldSuccessfullyUpdateAppsFail()
        {
            // Arrange
            request.Payload = TestObjects.GetInvalidAppPayload();

            // Act
            var result = sutFailure.UpdateAsync(1, request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: App not Updated"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyGetAppUsers()
        {
            // Arrange

            // Act
            var result = sutSuccess.GetAppUsersAsync(
                1,
                request);

            var message = ((Result)((OkObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result.Result).StatusCode;
            var users = ((Result)((OkObjectResult)result.Result.Result).Value).Payload.ConvertAll(u => (User)u);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<IEnumerable<User>>>());
            Assert.That(message, Is.EqualTo("Status Code 200: Users Found"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(users.Count, Is.EqualTo(2));
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldSuccessfullyGetAppUsersFail()
        {
            // Arrange

            // Act
            var result = sutFailure.GetAppUsersAsync(
                1,
                request);

            var message = ((Result)((NotFoundObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<IEnumerable<User>>>());
            Assert.That(message, Is.EqualTo("Status Code 404: Users not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyAddUserToApp()
        {
            // Arrange

            // Act
            var result = sutSuccess.AddUserAsync(1, 3, request);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var user = (User)((Result)((OkObjectResult)result.Result).Value).Payload[0];
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: User Added to App"));
            Assert.That(user, Is.InstanceOf<User>());
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldSuccessfullyAddUserToAppFail()
        {
            // Arrange

            // Act
            var result = sutFailure.AddUserAsync(1, 3, request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: User not Added to App"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyRemoveUserFromApp()
        {
            // Arrange

            // Act
            var result = sutSuccess.RemoveUserAsync(1, 3, request);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var user = (User)((Result)((OkObjectResult)result.Result).Value).Payload[0];
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: User Removed from App"));
            Assert.That(user, Is.InstanceOf<User>());
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldSuccessfullyRemoveUserFromAppFail()
        {
            // Arrange

            // Act
            var result = sutFailure.RemoveUserAsync(1, 3, request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: User not Removed from App"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyActivateAnApp()
        {
            // Arrange

            // Act
            var result = sutSuccess.ActivateAsync(1, request);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var app = (App)((Result)((OkObjectResult)result.Result).Value).Payload[0];
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: App Activated"));
            Assert.That(app, Is.InstanceOf<App>());
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldSuccessfullyActivateAnAppFail()
        {
            // Arrange

            // Act
            var result = sutFailure.ActivateAsync(1, request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: App not Activated"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyDeactivateAnApp()
        {
            // Arrange

            // Act
            var result = sutSuccess.DeactivateAsync(1, request);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var app = (App)((Result)((OkObjectResult)result.Result).Value).Payload[0];
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: App Deactivated"));
            Assert.That(app, Is.InstanceOf<App>());
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldSuccessfullyDeactivateAnAppFail()
        {
            // Arrange

            // Act
            var result = sutFailure.DeactivateAsync(1, request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: App not Deactivated"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public void ReturnBadRequestResponseShouldLicenseValidationFail()
        {
            // Arrange
            var appId = 1;

            // Act
            var resultOne = sutInvalid.GetAsync(appId, request);
            var messageOne = ((Result)
                ((BadRequestObjectResult)resultOne.Result.Result)
                    .Value)
                    .Message;
            var statusCodeOne = ((BadRequestObjectResult)resultOne.Result.Result).StatusCode;

            var resultTwo = sutInvalid.GetAppsAsync(request);
            var messageTwo = ((Result)
                ((BadRequestObjectResult)resultOne.Result.Result)
                    .Value)
                    .Message;
            var statusCodeTwo = ((BadRequestObjectResult)resultTwo.Result.Result).StatusCode;

            var resultThree = sutInvalid.UpdateAsync(1, request);
            var messageThree = ((Result)
                ((BadRequestObjectResult)resultOne.Result.Result)
                    .Value)
                    .Message;
            var statusCodeThree = ((BadRequestObjectResult)resultThree.Result).StatusCode;

            var resultFour = sutInvalid.GetAppUsersAsync(1, request);
            var messageFour = ((Result)
                ((BadRequestObjectResult)resultOne.Result.Result)
                    .Value)
                    .Message;
            var statusCodeFour = ((BadRequestObjectResult)resultFour.Result.Result).StatusCode;

            var resultFive = sutInvalid.AddUserAsync(1, 3, request);
            var messageFive = ((Result)
                ((BadRequestObjectResult)resultOne.Result.Result)
                    .Value)
                    .Message;
            var statusCodeFive = ((BadRequestObjectResult)resultFour.Result.Result).StatusCode;

            var resultSix = sutInvalid.RemoveUserAsync(1, 3, request);
            var messageSix = ((Result)
                ((BadRequestObjectResult)resultOne.Result.Result)
                    .Value)
                    .Message;
            var statusCodeSix = ((BadRequestObjectResult)resultFour.Result.Result).StatusCode;

            // Assert
            Assert.That(resultOne.Result, Is.InstanceOf<ActionResult<App>>());
            Assert.That(messageOne, Is.EqualTo("Status Code 400: Invalid Request on this Token"));
            Assert.That(statusCodeOne, Is.EqualTo(400));
            Assert.That(resultTwo.Result, Is.InstanceOf<ActionResult<IEnumerable<App>>>());
            Assert.That(messageTwo, Is.EqualTo("Status Code 400: Invalid Request on this Token"));
            Assert.That(statusCodeTwo, Is.EqualTo(400));
            Assert.That(resultThree.Result, Is.InstanceOf<ActionResult>());
            Assert.That(messageThree, Is.EqualTo("Status Code 400: Invalid Request on this Token"));
            Assert.That(statusCodeThree, Is.EqualTo(400));
            Assert.That(resultFour.Result, Is.InstanceOf<ActionResult<IEnumerable<User>>>());
            Assert.That(messageFour, Is.EqualTo("Status Code 400: Invalid Request on this Token"));
            Assert.That(statusCodeFour, Is.EqualTo(400));
            Assert.That(resultFive.Result, Is.InstanceOf<ActionResult>());
            Assert.That(messageFive, Is.EqualTo("Status Code 400: Invalid Request on this Token"));
            Assert.That(statusCodeFive, Is.EqualTo(400));
            Assert.That(resultSix.Result, Is.InstanceOf<ActionResult>());
            Assert.That(messageSix, Is.EqualTo("Status Code 400: Invalid Request on this Token"));
            Assert.That(statusCodeSix, Is.EqualTo(400));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyAllowSuperuserToDeleteApps()
        {
            // Arrange

            // Act
            var result = sutSuccess.DeleteAsync(
                2, 
                TestObjects.GetSecondLicense(),
                request);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: App Deleted"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldSuccessfullyAllowSuperuserToDeleteAppsFail()
        {
            // Arrange

            // Act
            var result = sutFailure.DeleteAsync(
                2,
                TestObjects.GetSecondLicense(),
                request);
            var errorMessage = ((Result)
                ((BadRequestObjectResult)result.Result)
                    .Value)
                    .Message;
            var statusCode = ((BadRequestObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
            Assert.That(errorMessage, Is.EqualTo("Status Code 400: You are not the Owner of this App"));
            Assert.That(statusCode, Is.EqualTo(400));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyPromoteUserToAppAdmin()
        {
            // Arrange

            // Act
            var result = sutSuccess.ActivateAdminPrivilegesAsync(1, 3, request);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var user = (User)((Result)((OkObjectResult)result.Result).Value).Payload[0];
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: User has been Promoted to Admin"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(user, Is.InstanceOf<User>());
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldSuccessfullyPromoteUserToAppAdminFail()
        {
            // Arrange

            // Act
            var result = sutPromoteUserFailure.ActivateAdminPrivilegesAsync(1, 3, request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: User has not been Promoted to Admin"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyDeactivateAdminPrivileges()
        {
            // Arrange

            // Act
            var result = sutSuccess.DeactivateAdminPrivilegesAsync(1, 3, request);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var user = (User)((Result)((OkObjectResult)result.Result).Value).Payload[0];
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Admin Privileges Deactivated"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(user, Is.InstanceOf<User>());
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldSuccessfullyDeactivateAdminPrivilegesFail()
        {
            // Arrange

            // Act
            var result = sutPromoteUserFailure.DeactivateAdminPrivilegesAsync(1, 3, request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Deactivation of Admin Privileges Failed"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyGetMyApps()
        {
            // Arrange

            // Act
            var result = sutSuccess.GetMyAppsAsync(request);
            var apps = ((Result)((OkObjectResult)result.Result.Result).Value).Payload.ConvertAll(a => (App)a);
            var message = ((Result)((OkObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<IEnumerable<App>>>());
            Assert.That(message, Is.EqualTo("Status Code 200: Apps Found"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(apps, Is.InstanceOf<List<App>>());
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldSuccessfullyGetMyAppsFail()
        {
            // Arrange

            // Act
            var result = sutFailure.GetMyAppsAsync(request);
            var message = ((Result)((NotFoundObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<IEnumerable<App>>>());
            Assert.That(message, Is.EqualTo("Status Code 404: Apps not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyGetMyRegisteredApps()
        {
            // Arrange

            // Act
            var result = sutSuccess.GetMyRegisteredAppsAsync(request);
            var apps = ((Result)((OkObjectResult)result.Result).Value).Payload.ConvertAll(a =>(App)a);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Apps Found"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(apps, Is.InstanceOf<List<App>>());
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldSuccessfullyGetRegisteredAppsFail()
        {
            // Arrange

            // Act
            var result = sutFailure.GetMyRegisteredAppsAsync(request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Apps not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }
    }
}
