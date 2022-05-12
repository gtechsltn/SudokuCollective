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
        public async Task SuccessfullyGetUser()
        {
            // Arrange
            var userId = 1;

            // Act
            var result = await sutSuccess.GetAsync(userId, request);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;
            var user = (User)((Result)((OkObjectResult)result.Result).Value).Payload[0];

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<User>>());
            Assert.That(message, Is.EqualTo("Status Code 200: User Found"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(user, Is.InstanceOf<User>());
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldGetUserFail()
        {
            // Arrange
            var userId = 1;

            // Act
            var result = await sutFailure.GetAsync(userId, request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<User>>());
            Assert.That(message, Is.EqualTo("Status Code 404: User not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyGetUsers()
        {
            // Arrange

            // Act
            var result = await sutSuccess.GetUsersAsync(request);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var users = ((Result)((OkObjectResult)result.Result).Value).Payload.ConvertAll(u => (User)u);
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(users, Is.InstanceOf<IEnumerable<User>>());
            Assert.That(message, Is.EqualTo("Status Code 200: Users Found"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldGetUsersFail()
        {
            // Arrange

            // Act
            var result = await sutFailure.GetUsersAsync(request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(message, Is.EqualTo("Status Code 404: Users not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyUpdateUsers()
        {
            // Arrange
            int userId = 1;
            request.Payload = updateUserPayload;

            // Act
            var result = await sutSuccess.UpdateAsync(userId, request);
            var message = ((Result)((OkObjectResult)result).Value).Message;
            var user = (User)((Result)((OkObjectResult)result).Value).Payload[0];
            var statusCode = ((OkObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: User Updated"));
            Assert.That(user, Is.InstanceOf<User>());
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldUpdateUserFail()
        {
            // Arrange
            int userId = 1;
            request.Payload = updateUserPayload;

            // Act
            var result = await sutFailure.UpdateAsync(userId, request);
            var message = ((Result)((NotFoundObjectResult)result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: User not Updated"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyUpdateUsersPasswords()
        {
            // Arrange and Act
            var result = await sutSuccess.RequestPasswordResetAsync(requestPasswordResetRequest);
            var message = ((Result)((OkObjectResult)result).Value).Message;
            var statusCode = ((OkObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Processed Password Reset Request"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldUpdateUsersPasswordsFail()
        {
            // Arrange and Act
            var result = await sutFailure.RequestPasswordResetAsync(requestPasswordResetRequest);
            var message = ((Result)((NotFoundObjectResult)result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Unable to Process Password Reset Request"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyDeleteUsers()
        {
            // Arrange
            int userId = 1;

            // Act
            var result = await sutSuccess.DeleteAsync(userId, request);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<User>>());
            Assert.That(message, Is.EqualTo("Status Code 200: User Deleted"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldDeleteUsersFail()
        {
            // Arrange
            int userId = 1;

            // Act
            var result = await sutFailure.DeleteAsync(userId, request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<User>>());
            Assert.That(message, Is.EqualTo("Status Code 404: User not Deleted"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyAddUsersRole()
        {
            // Arrange
            int userId = 1;
            request.Payload = updateUserRolePayload;

            // Act
            var result = await sutSuccess.AddRolesAsync(userId, request);
            var message = ((Result)((OkObjectResult)result).Value).Message;
            var statusCode = ((OkObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Roles Added"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldAddUsersRoleFail()
        {
            // Arrange
            int userId = 1;
            request.Payload = updateUserRolePayload;

            // Act
            var result = await sutFailure.AddRolesAsync(userId, request);
            var message = ((Result)((NotFoundObjectResult)result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Roles not Added"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyRemoveUsersRoles()
        {
            // Arrange
            int userId = 1;
            request.Payload = updateUserRolePayload;

            // Act
            var result = await sutSuccess.RemoveRolesAsync(userId, request);
            var message = ((Result)((OkObjectResult)result).Value).Message;
            var statusCode = ((OkObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Roles Removed"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldRemoveUsersRolesFail()
        {
            // Arrange
            int userId = 1;
            request.Payload = updateUserRolePayload;

            // Act
            var result = await sutFailure.RemoveRolesAsync(userId, request);
            var message = ((Result)((NotFoundObjectResult)result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Roles not Removed"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyActivateUsers()
        {
            // Arrange
            int userId = 1;

            // Act
            var result = await sutSuccess.ActivateAsync(userId);
            var message = ((Result)((OkObjectResult)result).Value).Message;
            var statusCode = ((OkObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: User Activated"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldActivateUsersFail()
        {
            // Arrange
            int userId = 1;

            // Act
            var result = await sutFailure.ActivateAsync(userId);
            var message = ((Result)((NotFoundObjectResult)result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: User not Activated"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyDeactivateUsers()
        {
            // Arrange
            int userId = 1;

            // Act
            var result = await sutSuccess.DeactivateAsync(userId);
            var message = ((Result)((OkObjectResult)result).Value).Message;
            var statusCode = ((OkObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: User Deactivated"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldDeactivateUsersFail()
        {
            // Arrange
            int userId = 1;

            // Act
            var result = await sutFailure.DeactivateAsync(userId);
            var message = ((Result)((NotFoundObjectResult)result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: User not Deactivated"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyResendPasswordResetEmails()
        {
            // Arrange

            // Act
            var result = await sutSuccess.ResendRequestPasswordResetAsync(resendRequestPasswordRequest);
            var message = ((Result)((ObjectResult)result).Value).Message;
            var statusCode = ((ObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Password Reset Email Resent"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyResendEmailConfirmationFail()
        {
            // Arrange

            // Act
            var result = await sutFailure.ResendRequestPasswordResetAsync(resendRequestPasswordRequest);
            var message = ((Result)((NotFoundObjectResult)result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: User not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyCancelEmailConfirmation()
        {
            // Arrange

            // Act
            var result = await sutSuccess.CancelEmailConfirmationAsync(request);
            var message = ((Result)((ObjectResult)result).Value).Message;
            var statusCode = ((ObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Email Confirmation Request Cancelled"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyCancelEmailConfirmationFail()
        {
            // Arrange

            // Act
            var result = await sutFailure.CancelEmailConfirmationAsync(request);
            var message = ((Result)((NotFoundObjectResult)result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Email Confirmation Request not Cancelled"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyCancelPasswordRequest()
        {
            // Arrange

            // Act
            var result = await sutSuccess.CancelPasswordResetAsync(request);
            var message = ((Result)((ObjectResult)result).Value).Message;
            var statusCode = ((ObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Password Reset Request Cancelled"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyCancelPasswordRequestFail()
        {
            // Arrange

            // Act
            var result = await sutFailure.CancelPasswordResetAsync(request);
            var message = ((Result)((NotFoundObjectResult)result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Password Reset Request not Cancelled"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyCancelAllEmailRequests()
        {
            // Arrange

            // Act
            var result = await sutSuccess.CancelAllEmailRequestsAsync(request);
            var message = ((Result)((ObjectResult)result).Value).Message;
            var statusCode = ((ObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Email Confirmation Request Cancelled and Password Reset Request Cancelled"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSuccessfullyCancelAllEmailRequestsFail()
        {
            // Arrange

            // Act
            var result = await sutFailure.CancelAllEmailRequestsAsync(request);
            var message = ((Result)((NotFoundObjectResult)result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Email Requests not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyConfirmUserEmail()
        {
            // Arrange
            var emailConfirmationToken = Guid.NewGuid().ToString();

            // Act
            var result = await sutSuccess.ConfirmEmailAsync(emailConfirmationToken);
            var message = ((Result)((OkObjectResult)result).Value).Message;
            var statusCode = ((OkObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Email Confirmed"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorMessageShouldConfirmUserEmailFails()
        {
            // Arrange
            var emailConfirmationToken = Guid.NewGuid().ToString();

            // Act
            var result = await sutFailure.ConfirmEmailAsync(emailConfirmationToken);
            var message = ((Result)((NotFoundObjectResult)result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Email not Confirmed"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task SuccessfullyResetPassword()
        {
            // Arrange
            var request = new ResetPasswordRequest
            {
                Token = Guid.NewGuid().ToString(),
                NewPassword = "P@ssword2"
            };
                

            // Act
            var result = await sutSuccess.ResetPasswordAsync(request);
            var message = ((Result)((OkObjectResult)result).Value).Message;
            var statusCode = ((OkObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Password Reset"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task IssueErrorMessageShouldResetPasswordFails()
        {
            // Arrange
            var request = new ResetPasswordRequest
            {
                Token = Guid.NewGuid().ToString(),
                NewPassword = "P@ssword2"
            };

            // Act
            var result = await sutFailureResetPassword.ResetPasswordAsync(request);
            var message = ((Result)((ObjectResult)result).Value).Message;
            var statusCode = ((ObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Password not Reset"));
            Assert.That(statusCode, Is.EqualTo(404));
        }
    }
}
