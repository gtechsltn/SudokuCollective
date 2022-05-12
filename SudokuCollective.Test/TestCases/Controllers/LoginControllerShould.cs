using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SudokuCollective.Api.Controllers.V1;
using SudokuCollective.Test.TestData;
using SudokuCollective.Test.Services;
using SudokuCollective.Data.Models.Authentication;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Requests;
using SudokuCollective.Data.Models.Results;

namespace SudokuCollective.Test.TestCases.Controllers
{
    public class LoginControllerShould
    {
        private LoginController sut;
        private LoginController sutInvalid;
        private LoginController sutInvalidUserName;
        private LoginController sutInvalidPassword;
        private LoginController sutUserNameNotFound;
        private MockedAuthenticateService mockedAuthenticateService;
        private MockedUserManagementService mockedUserManagementService;
        private MockedUserManagementService mockedUserManagementInvalidUserNameService;
        private MockedUserManagementService mockedUserManagementInvalidPasswordService;
        private MockedRequestService mockedRequestService;
        private Mock<ILogger<LoginController>> mockedLogger;
        private LoginRequest loginRequest;
        private string userName;
        private string password;
        private string email;

        [SetUp]
        public void Setup()
        {
            userName = "TestSuperUser";
            password = "T#stP@ssw0rd";
            email = "TestSuperUser@example.com";

            mockedAuthenticateService = new MockedAuthenticateService();
            mockedUserManagementService = new MockedUserManagementService();
            mockedUserManagementInvalidUserNameService = new MockedUserManagementService();
            mockedUserManagementInvalidPasswordService = new MockedUserManagementService();
            mockedRequestService = new MockedRequestService();
            mockedLogger = new Mock<ILogger<LoginController>>();

            loginRequest = new LoginRequest()
            {
                UserName = userName,
                Password = password,
                License = TestObjects.GetLicense()
            };

            sut = new LoginController(
                mockedAuthenticateService.SuccessfulRequest.Object,
                mockedUserManagementService.SuccssfulRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                mockedLogger.Object);
            sutInvalid = new LoginController(
                mockedAuthenticateService.FailedRequest.Object,
                mockedUserManagementService.FailedRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                mockedLogger.Object);
            sutInvalidUserName = new LoginController(
                mockedAuthenticateService.FailedRequest.Object,
                mockedUserManagementInvalidUserNameService.UserNameFailedRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                mockedLogger.Object);
            sutInvalidPassword = new LoginController(
                mockedAuthenticateService.FailedRequest.Object,
                mockedUserManagementInvalidPasswordService.PasswordFailedRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                mockedLogger.Object);
            sutUserNameNotFound = new LoginController(
                mockedAuthenticateService.FailedRequest.Object,
                mockedUserManagementService.FailedRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                mockedLogger.Object);
        }

        [Test, Category("Controllers")]
        public async Task AuthenticateUsers()
        {
            // Arrange

            // Act
            var result = await sut.PostAsync(loginRequest);
            var returnedValue = ((OkObjectResult)result).Value;
            var message = ((Result)((OkObjectResult)result).Value).Message;
            var statusCode = ((OkObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            Assert.That(returnedValue, Is.TypeOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 200: User Found"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public async Task ReturnBadRequestMessageWhenUserNameIsInvalid()
        {
            // Arrange

            // Act
            var result = await sutInvalidUserName.PostAsync(loginRequest);
            var message = ((Result)((NotFoundObjectResult)result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: No User Has This User Name"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task ReturnBadRequestMessageWhenPasswordIsInvalid()
        {
            // Arrange

            // Act
            var result = await sutInvalidPassword.PostAsync(loginRequest);
            var message = ((Result)((NotFoundObjectResult)result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Password Invalid"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task ReturnBadRequestMessageWhenUsersArentAuthenticated()
        {
            // Arrange

            // Act
            var result = await sutInvalid.PostAsync(loginRequest);
            var message = ((NotFoundObjectResult)result).Value;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: User not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public async Task ReturnUserName()
        {
            // Arrange
            var request = new ConfirmUserNameRequest
            {
                Email = email,
                License = TestObjects.GetLicense()
            };

            // Act
            var result = await sut.ConfirmUserNameAsync(request);
            var message = ((Result)((OkObjectResult)result).Value).Message;
            var username = ((AuthenticatedUserNameResult)((Result)((OkObjectResult)result).Value).Payload[0]).UserName;

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: User Name Confirmed"));
            Assert.That(username, Is.EqualTo(userName));
        }

        [Test, Category("Controllers")]
        public async Task ReturnErrorMessageIfUserNameNotFound()
        {
            // Arrange
            var request = new ConfirmUserNameRequest
            {
                Email = email,
                License = TestObjects.GetLicense()
            };

            // Act
            var result = await sutUserNameNotFound.ConfirmUserNameAsync(request);
            var message = ((Result)((NotFoundObjectResult)result).Value).Message;

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: No User is using this Email"));
        }
    }
}
