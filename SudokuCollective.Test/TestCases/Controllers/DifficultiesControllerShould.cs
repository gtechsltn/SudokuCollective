using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SudokuCollective.Core.Enums;
using SudokuCollective.Data.Models;
using SudokuCollective.Core.Models;
using SudokuCollective.Test.TestData;
using SudokuCollective.Api.Controllers.V1;
using SudokuCollective.Test.Services;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Payloads;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SudokuCollective.Test.TestCases.Controllers
{
    public class DifficultiesControllerShould
    {
        private DatabaseContext context;
        private DifficultiesController sutSuccess;
        private DifficultiesController sutFailure;
        private MockedDifficultiesService mockedDifficultiesService;
        private MockedAppsService mockedAppsService;
        private MockedRequestService mockedRequestService;
        private Mock<IHttpContextAccessor> mockedHttpContextAccessor;
        private Mock<ILogger<DifficultiesController>> mockedLogger;
        private Request request;
        private CreateDifficultyPayload createDifficultyPayload;
        private UpdateDifficultyPayload updateDifficultyPayload;

        [SetUp]
        public async Task Setup()
        {
            context = await TestDatabase.GetDatabaseContext();
            mockedDifficultiesService = new MockedDifficultiesService(context);
            mockedAppsService = new MockedAppsService(context);
            mockedRequestService = new MockedRequestService();
            mockedHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockedLogger = new Mock<ILogger<DifficultiesController>>();

            request = TestObjects.GetRequest();

            createDifficultyPayload = new CreateDifficultyPayload()
            {
                Name = "Test Difficulty",
                DisplayName = "Test Difficulty",
                DifficultyLevel = DifficultyLevel.TEST
            };

            updateDifficultyPayload = new UpdateDifficultyPayload()
            {
                Id = 1,
                Name = "Test Difficulty",
                DisplayName = "Test Difficulty"
            };

            sutSuccess = new DifficultiesController(
                mockedDifficultiesService.SuccessfulRequest.Object,
                mockedAppsService.SuccessfulRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                mockedHttpContextAccessor.Object,
                mockedLogger.Object);

            sutFailure = new DifficultiesController(
                mockedDifficultiesService.FailedRequest.Object,
                mockedAppsService.SuccessfulRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                mockedHttpContextAccessor.Object,
                mockedLogger.Object);
        }

        [Test]
        [Category("Controllers")]
        public async Task SuccessfullyGetDifficulty()
        {
            // Arrange
            var difficultyId = 2;

            // Act
            var result = await sutSuccess.GetAsync(difficultyId);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;
            var difficulty = (Difficulty)((Result)((OkObjectResult)result.Result).Value).Payload[0];

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<Difficulty>>());
            Assert.That(message, Is.EqualTo("Status Code 200: Difficulty Found"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(difficulty, Is.InstanceOf<Difficulty>());
        }

        [Test]
        [Category("Controllers")]
        public async Task IssueErrorAndMessageShouldGetDifficultyFail()
        {
            // Arrange
            var difficultyId = 2;

            // Act
            var result = await sutFailure.GetAsync(difficultyId);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<Difficulty>>());
            Assert.That(message, Is.EqualTo("Status Code 404: Difficulty not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public async Task SuccessfullyGetDifficulties()
        {
            // Arrange

            // Act
            var result = await sutSuccess.GetDifficultiesAsync();
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;
            var difficulties = ((Result)((OkObjectResult)result.Result).Value).Payload.ConvertAll(d => (Difficulty)d);

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<IEnumerable<Difficulty>>>());
            Assert.That(message, Is.EqualTo("Status Code 200: Difficulties Found"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(difficulties.Count, Is.EqualTo(4));
        }

        [Test]
        [Category("Controllers")]
        public async Task IssueErrorAndMessageShouldGetDifficultiesFail()
        {
            // Arrange

            // Act
            var result = await sutFailure.GetDifficultiesAsync();
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<IEnumerable<Difficulty>>>());
            Assert.That(message, Is.EqualTo("Status Code 404: Difficulties not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public async Task SuccessfullyUpdateDifficulties()
        {
            // Arrange
            request.Payload = updateDifficultyPayload;

            // Act
            var result = await sutSuccess.UpdateAsync(1, request);
            var message = ((Result)((OkObjectResult)result).Value).Message;
            var statusCode = ((OkObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Difficulty Updated"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test]
        [Category("Controllers")]
        public async Task IssueErrorAndMessageShouldUpdateGamesFail()
        {
            // Arrange
            request.Payload = updateDifficultyPayload;

            // Act
            var result = await sutFailure.UpdateAsync(1, request);
            var message = ((Result)((NotFoundObjectResult)result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Difficulty not Updated"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public async Task SuccessfullyCreateDifficulties()
        {
            // Arrange
            request.Payload = createDifficultyPayload;

            // Act
            var result = await sutSuccess.PostAsync(request);
            var message = ((Result)((ObjectResult)result.Result).Value).Message;
            var statusCode = ((ObjectResult)result.Result).StatusCode;
            var difficulty = (Difficulty)((Result)((ObjectResult)result.Result).Value).Payload[0];

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<Difficulty>>());
            Assert.That(message, Is.EqualTo("Status Code 201: Difficulty Created"));
            Assert.That(statusCode, Is.EqualTo(201));
            Assert.That(difficulty, Is.InstanceOf<Difficulty>());
        }

        [Test]
        [Category("Controllers")]
        public async Task IssueErrorAndMessageShouldCreateDifficultiesFail()
        {
            // Arrange
            request.Payload = createDifficultyPayload;

            // Act
            var result = await sutFailure.PostAsync(request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<Difficulty>>());
            Assert.That(message, Is.EqualTo("Status Code 404: Difficulty not Created"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public async Task SuccessfullyDeleteDifficulties()
        {
            // Arrange
            var difficultyId = 2;

            // Act
            var result = await sutSuccess.DeleteAsync(difficultyId, request);
            var message = ((Result)((OkObjectResult)result).Value).Message;
            var statusCode = ((OkObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Difficulty Deleted"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test]
        [Category("Controllers")]
        public async Task IssueErrorAndMessageShouldDeleteDifficultiesFail()
        {
            // Arrange
            var difficultyId = 2;

            // Act
            var result = await sutFailure.DeleteAsync(difficultyId, request);
            var message = ((NotFoundObjectResult)result).Value;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Difficulty not Deleted"));
            Assert.That(statusCode, Is.EqualTo(404));
        }
    }
}
