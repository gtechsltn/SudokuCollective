using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SudokuCollective.Data.Models;
using SudokuCollective.Core.Models;
using SudokuCollective.Test.TestData;
using SudokuCollective.Api.Controllers.V1;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Test.Services;
using SudokuCollective.Data.Models.Requests;
using SudokuCollective.Data.Models.Results;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace SudokuCollective.Test.TestCases.Controllers
{
    public class SignupControllerShould
    {
        private DatabaseContext context;
        private SignupController sutSuccess;
        private SignupController sutFailure;
        private MockedUsersService mockedUsersService;
        private MockedAuthenticateService mockedAuthenticateService;
        private MockedRequestService mockedRequestService;
        private SignupRequest signupRequest;
        private ResendEmailConfirmationRequest Request;
        private Mock<IWebHostEnvironment> mockedWebHostEnvironment;
        private Mock<ILogger<SignupController>> mockedLogger;

        [SetUp]
        public async Task Setup()
        {
            context = await TestDatabase.GetDatabaseContext();

            mockedUsersService = new MockedUsersService(context);
            mockedAuthenticateService = new MockedAuthenticateService();
            mockedRequestService = new MockedRequestService();
            mockedWebHostEnvironment = new Mock<IWebHostEnvironment>();
            mockedLogger = new Mock<ILogger<SignupController>>();

            sutSuccess = new SignupController(
                mockedUsersService.SuccessfulRequest.Object,
                mockedAuthenticateService.SuccessfulRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                mockedWebHostEnvironment.Object,
                mockedLogger.Object);
            sutFailure = new SignupController(
                mockedUsersService.FailedRequest.Object,
                mockedAuthenticateService.FailedRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                mockedWebHostEnvironment.Object,
                mockedLogger.Object);

            signupRequest = new SignupRequest()
            {
                UserName = "TestUser3",
                FirstName = "Test",
                LastName = "User 3",
                NickName = "My Nickname",
                Email = "testuser3@example.com",
                Password = "T#stP@ssw0rd",
                License = TestObjects.GetLicense()
            };

            Request = new ResendEmailConfirmationRequest();
        }

        [Test, Category("Controllers")]
        public void SuccessfullyRegisterUsers()
        {
            // Arrange

            // Act
            var result = sutSuccess.PostAsync(signupRequest);
            var message = ((Result)((ObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((ObjectResult)result.Result.Result).StatusCode;
            var user = ((UserCreatedResult)((Result)((ObjectResult)result.Result.Result).Value).Payload[0]).User;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<User>>());
            Assert.That(message, Is.EqualTo("Status Code 201: User Created"));
            Assert.That(statusCode, Is.EqualTo(201));
            Assert.That(user, Is.InstanceOf<AuthenticatedUser>());
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldSuccessfullyRegisterUsersFail()
        {
            // Arrange

            // Act
            var result = sutFailure.PostAsync(signupRequest);
            var errorMessage = ((Result)((NotFoundObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<User>>());
            Assert.That(errorMessage, Is.EqualTo("Status Code 404: User not Created"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test, Category("Controllers")]
        public void SuccessfullyResendEmailConfirmation()
        {
            // Arrange

            // Act
            var result = sutSuccess.ResendEmailConfirmationAsync(Request);
            var message = ((Result)((ObjectResult)result.Result).Value).Message;
            var statusCode = ((ObjectResult)result.Result).StatusCode;
            var emailResent = ((UserResult)((Result)((ObjectResult)result.Result).Value).Payload[0]).ConfirmationEmailSuccessfullySent;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Email Confirmation Email Resent"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(emailResent, Is.True);
        }

        [Test, Category("Controllers")]
        public void IssueErrorAndMessageShouldSuccessfullyResendEmailConfirmationFail()
        {
            // Arrange

            // Act
            var result = sutFailure.ResendEmailConfirmationAsync(Request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Email Confirmation Email not Resent"));
            Assert.That(statusCode, Is.EqualTo(404));
        }
    }
}
