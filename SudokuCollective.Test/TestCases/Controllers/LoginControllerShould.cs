using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

            loginRequest = new LoginRequest()
            {
                UserName = userName,
                Password = password,
                License = TestObjects.GetLicense()
            };

            sut = new LoginController(
                mockedAuthenticateService.SuccessfulRequest.Object,
                mockedUserManagementService.SuccssfulRequest.Object);
            sutInvalid = new LoginController(
                mockedAuthenticateService.FailedRequest.Object,
                mockedUserManagementService.FailedRequest.Object);
            sutInvalidUserName = new LoginController(
                mockedAuthenticateService.FailedRequest.Object,
                mockedUserManagementInvalidUserNameService.UserNameFailedRequest.Object);
            sutInvalidPassword = new LoginController(
                mockedAuthenticateService.FailedRequest.Object,
                mockedUserManagementInvalidPasswordService.PasswordFailedRequest.Object);
            sutUserNameNotFound = new LoginController(
                mockedAuthenticateService.FailedRequest.Object,
                mockedUserManagementService.FailedRequest.Object);
        }

        [Test]
        [Category("Controllers")]
        public void AuthenticateUsers()
        {
            // Arrange

            // Act
            var result = sut.Post(loginRequest);
            var returnedValue = ((OkObjectResult)result.Result).Value;
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.TypeOf<Task<ActionResult>>());
            Assert.That(returnedValue, Is.TypeOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 200: User Found"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test]
        [Category("Controllers")]
        public void ReturnBadRequestMessageWhenUserNameIsInvalid()
        {
            // Arrange

            // Act
            var result = sutInvalidUserName.Post(loginRequest);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.TypeOf<Task<ActionResult>>());
            Assert.That(message, Is.EqualTo("Status Code 404: No User Has This User Name"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public void ReturnBadRequestMessageWhenPasswordIsInvalid()
        {
            // Arrange

            // Act
            var result = sutInvalidPassword.Post(loginRequest);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.TypeOf<Task<ActionResult>>());
            Assert.That(message, Is.EqualTo("Status Code 404: Password Invalid"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public void ReturnBadRequestMessageWhenUsersArentAuthenticated()
        {
            // Arrange

            // Act
            var result = sutInvalid.Post(loginRequest);
            var message = ((NotFoundObjectResult)result.Result).Value;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.TypeOf<Task<ActionResult>>());
            Assert.That(message, Is.EqualTo("Status Code 404: Bad Request"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public void ReturnUserName()
        {
            // Arrange
            var request = new ConfirmUserNameRequest
            {
                Email = email,
                License = TestObjects.GetLicense()
            };

            // Act
            var result = sut.ConfirmUserName(request);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var username = ((AuthenticatedUserNameResult)((Result)((OkObjectResult)result.Result).Value).Payload[0]).UserName;

            // Assert
            Assert.That(result, Is.TypeOf<Task<ActionResult>>());
            Assert.That(message, Is.EqualTo("Status Code 200: User Name Confirmed"));
            Assert.That(username, Is.EqualTo(userName));
        }

        [Test]
        [Category("Controllers")]
        public void ReturnErrorMessageIfUserNameNotFound()
        {
            // Arrange
            var request = new ConfirmUserNameRequest
            {
                Email = email,
                License = TestObjects.GetLicense()
            };

            // Act
            var result = sutUserNameNotFound.ConfirmUserName(request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;

            // Assert
            Assert.That(result, Is.TypeOf<Task<ActionResult>>());
            Assert.That(message, Is.EqualTo("Status Code 404: No User is using this Email"));
        }
    }
}
