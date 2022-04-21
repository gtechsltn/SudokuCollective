using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SudokuCollective.Api.Controllers.V1;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Test.TestData;
using SudokuCollective.Test.Services;
using SudokuCollective.Data.Models.Requests;
using SudokuCollective.Data.Models.Payloads;

namespace SudokuCollective.Test.TestCases.Controllers
{
    public class UsersControllerShould
    {
        private DatabaseContext context;
        private UsersController sutSuccess;
        private UsersController sutFailure;
        private UsersController sutFailureResetPassword;
        private MockedUsersService mockedUsersService;
        private MockedAppsService mockedAppsService;
        private MockedRequestService mockedRequestService;
        private Request request;
        private UpdateUserPayload updateUserPayload;
        private RequestPasswordResetRequest requestPasswordResetRequest;
        private UpdateUserRolePayload updateUserRolePayload;
        private ResendRequestPasswordRequest resendRequestPasswordRequest;
        private Mock<IWebHostEnvironment> mockWebHostEnvironment;
        private Mock<IHttpContextAccessor> mockedHttpContextAccessor;
        private Mock<ILogger<UsersController>> mockedLogger;

        [SetUp]
        public async Task Setup()
        {
            context = await TestDatabase.GetDatabaseContext();
            mockedUsersService = new MockedUsersService(context);
            mockedAppsService = new MockedAppsService(context);
            mockedRequestService = new MockedRequestService();
            mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            mockedHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockedLogger = new Mock<ILogger<UsersController>>();

            request = TestObjects.GetRequest();

            updateUserPayload = new UpdateUserPayload()
            {
                UserName = "Test Username",
                FirstName = "FirstName",
                LastName = "LastName",
                NickName = "MyNickname",
                Email = "testemail@example.com"
            };

            requestPasswordResetRequest = new RequestPasswordResetRequest()
            {
                License = TestObjects.GetLicense(),
                Email = "TestSuperUser@example.com"
            };

            updateUserRolePayload = new UpdateUserRolePayload()
            {
                RoleIds = new List<int>() { 3 }
            };

            resendRequestPasswordRequest = new ResendRequestPasswordRequest()
            {
                UserId = 1,
                AppId = 1
            };

            sutSuccess = new UsersController(
                mockedUsersService.SuccessfulRequest.Object,
                mockedAppsService.SuccessfulRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                mockWebHostEnvironment.Object,
                mockedHttpContextAccessor.Object,
                mockedLogger.Object);

            sutFailure = new UsersController(
                mockedUsersService.FailedRequest.Object,
                mockedAppsService.SuccessfulRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                mockWebHostEnvironment.Object,
                mockedHttpContextAccessor.Object,
                mockedLogger.Object);

            sutFailureResetPassword = new UsersController(
                mockedUsersService.FailedResetPasswordRequest.Object,
                mockedAppsService.SuccessfulRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                mockWebHostEnvironment.Object,
                mockedHttpContextAccessor.Object,
                mockedLogger.Object);
        }

        [Test, Category("Controllers")]
        public void SuccessfullyGetUser()
        {
            // Arrange
            var userId = 1;

            // Act
            var result = sutSuccess.Get(userId, request);
            var message = ((Result)((OkObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result.Result).StatusCode;
            var user = (User)((Result)((OkObjectResult)result.Result.Result).Value).Payload[0];

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<User>>());
            Assert.That(message, Is.EqualTo("Status Code 200: User Found"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(user, Is.InstanceOf<User>());
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldGetUserFail()
        {
            // Arrange
            var userId = 1;

            // Act
            var result = sutFailure.Get(userId, request);
            var message = ((Result)((NotFoundObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<User>>());
            Assert.That(message, Is.EqualTo("Status Code 404: User not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyGetUsers()
        {
            // Arrange

            // Act
            var result = sutSuccess.GetUsers(request);
            var message = ((Result)((OkObjectResult)result.Result.Result).Value).Message;
            var users = ((Result)((OkObjectResult)result.Result.Result).Value).Payload.ConvertAll(u => (User)u);
            var statusCode = ((OkObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(users, Is.InstanceOf<IEnumerable<User>>());
            Assert.That(message, Is.EqualTo("Status Code 200: Users Found"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldGetUsersFail()
        {
            // Arrange

            // Act
            var result = sutFailure.GetUsers(request);
            var message = ((Result)((NotFoundObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(message, Is.EqualTo("Status Code 404: Users not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyUpdateUsers()
        {
            // Arrange
            int userId = 1;
            request.Payload = updateUserPayload;

            // Act
            var result = sutSuccess.Update(userId, request);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var user = (User)((Result)((OkObjectResult)result.Result).Value).Payload[0];
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: User Updated"));
            Assert.That(user, Is.InstanceOf<User>());
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldUpdateUserFail()
        {
            // Arrange
            int userId = 1;
            request.Payload = updateUserPayload;

            // Act
            var result = sutFailure.Update(userId, request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: User not Updated"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyUpdateUsersPasswords()
        {
            // Arrange and Act
            var result = sutSuccess.RequestPasswordReset(requestPasswordResetRequest);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Processed Password Reset Request"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldUpdateUsersPasswordsFail()
        {
            // Arrange and Act
            var result = sutFailure.RequestPasswordReset(requestPasswordResetRequest);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Unable to Process Password Reset Request"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyDeleteUsers()
        {
            // Arrange
            int userId = 1;

            // Act
            var result = sutSuccess.Delete(userId, request);
            var message = ((Result)((OkObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<User>>());
            Assert.That(message, Is.EqualTo("Status Code 200: User Deleted"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldDeleteUsersFail()
        {
            // Arrange
            int userId = 1;

            // Act
            var result = sutFailure.Delete(userId, request);
            var message = ((Result)((NotFoundObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<User>>());
            Assert.That(message, Is.EqualTo("Status Code 404: User not Deleted"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyAddUsersRole()
        {
            // Arrange
            int userId = 1;
            request.Payload = updateUserRolePayload;

            // Act
            var result = sutSuccess.AddRoles(userId, request);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Roles Added"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldAddUsersRoleFail()
        {
            // Arrange
            int userId = 1;
            request.Payload = updateUserRolePayload;

            // Act
            var result = sutFailure.AddRoles(userId, request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Roles not Added"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyRemoveUsersRoles()
        {
            // Arrange
            int userId = 1;
            request.Payload = updateUserRolePayload;

            // Act
            var result = sutSuccess.RemoveRoles(userId, request);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Roles Removed"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldRemoveUsersRolesFail()
        {
            // Arrange
            int userId = 1;
            request.Payload = updateUserRolePayload;

            // Act
            var result = sutFailure.RemoveRoles(userId, request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Roles not Removed"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyActivateUsers()
        {
            // Arrange
            int userId = 1;

            // Act
            var result = sutSuccess.Activate(userId);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: User Activated"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldActivateUsersFail()
        {
            // Arrange
            int userId = 1;

            // Act
            var result = sutFailure.Activate(userId);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: User not Activated"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyDeactivateUsers()
        {
            // Arrange
            int userId = 1;

            // Act
            var result = sutSuccess.Deactivate(userId);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: User Deactivated"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldDeactivateUsersFail()
        {
            // Arrange
            int userId = 1;

            // Act
            var result = sutFailure.Deactivate(userId);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: User not Deactivated"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyResendPasswordResetEmails()
        {
            // Arrange

            // Act
            var result = sutSuccess.ResendRequestPasswordReset(resendRequestPasswordRequest);
            var message = ((Result)((ObjectResult)result.Result).Value).Message;
            var statusCode = ((ObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Password Reset Email Resent"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldSuccessfullyResendEmailConfirmationFail()
        {
            // Arrange

            // Act
            var result = sutFailure.ResendRequestPasswordReset(resendRequestPasswordRequest);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: User not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyCancelEmailConfirmation()
        {
            // Arrange

            // Act
            var result = sutSuccess.CancelEmailConfirmation(request);
            var message = ((Result)((ObjectResult)result.Result).Value).Message;
            var statusCode = ((ObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Email Confirmation Request Cancelled"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldSuccessfullyCancelEmailConfirmationFail()
        {
            // Arrange

            // Act
            var result = sutFailure.CancelEmailConfirmation(request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Email Confirmation Request not Cancelled"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyCancelPasswordRequest()
        {
            // Arrange

            // Act
            var result = sutSuccess.CancelPasswordReset(request);
            var message = ((Result)((ObjectResult)result.Result).Value).Message;
            var statusCode = ((ObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Password Reset Request Cancelled"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldSuccessfullyCancelPasswordRequestFail()
        {
            // Arrange

            // Act
            var result = sutFailure.CancelPasswordReset(request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Password Reset Request not Cancelled"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyCancelAllEmailRequests()
        {
            // Arrange

            // Act
            var result = sutSuccess.CancelAllEmailRequests(request);
            var message = ((Result)((ObjectResult)result.Result).Value).Message;
            var statusCode = ((ObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Email Confirmation Request Cancelled and Password Reset Request Cancelled"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldSuccessfullyCancelAllEmailRequestsFail()
        {
            // Arrange

            // Act
            var result = sutFailure.CancelAllEmailRequests(request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Email Requests not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyConfirmUserEmail()
        {
            // Arrange
            var emailConfirmationToken = Guid.NewGuid().ToString();

            // Act
            var result = sutSuccess.ConfirmEmail(emailConfirmationToken);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Email Confirmed"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public void IssueErrorMessageShouldConfirmUserEmailFails()
        {
            // Arrange
            var emailConfirmationToken = Guid.NewGuid().ToString();

            // Act
            var result = sutFailure.ConfirmEmail(emailConfirmationToken);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Email not Confirmed"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyResetPassword()
        {
            // Arrange
            var request = new ResetPasswordRequest
            {
                Token = Guid.NewGuid().ToString(),
                NewPassword = "P@ssword2"
            };
                

            // Act
            var result = sutSuccess.ResetPassword(request);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Password Reset"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public void IssueErrorMessageShouldResetPasswordFails()
        {
            // Arrange
            var request = new ResetPasswordRequest
            {
                Token = Guid.NewGuid().ToString(),
                NewPassword = "P@ssword2"
            };

            // Act
            var result = sutFailureResetPassword.ResetPassword(request);
            var message = ((Result)((ObjectResult)result.Result).Value).Message;
            var statusCode = ((ObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Password not Reset"));
            Assert.That(statusCode, Is.EqualTo(404));
        }
    }
}
