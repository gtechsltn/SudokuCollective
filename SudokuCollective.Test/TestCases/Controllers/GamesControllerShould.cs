using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SudokuCollective.Data.Models;
using SudokuCollective.Test.TestData;
using SudokuCollective.Api.V1.Controllers;
using SudokuCollective.Core.Models;
using SudokuCollective.Core.Enums;
using SudokuCollective.Test.Services;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Payloads;

namespace SudokuCollective.Test.TestCases.Controllers
{
    public class GamesControllerShould
    {
        private DatabaseContext context;
        private GamesController sutSuccess;
        private GamesController sutFailure;
        private MockedGamesService mockGamesService;
        private MockedAppsService mockAppsService;
        private MockedRequestService mockedRequestService;
        private Mock<IHttpContextAccessor> mockedHttpContextAccessor;
        private Mock<ILogger<GamesController>> mockedLogger;
        private Request request;
        private CreateGamePayload createGamePayload;
        private GamePayload updateGamePayload;

        [SetUp]
        public async Task Setup()
        {
            context = await TestDatabase.GetDatabaseContext();
            mockGamesService = new MockedGamesService(context);
            mockAppsService = new MockedAppsService(context);
            mockedRequestService = new MockedRequestService();
            mockedHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockedLogger = new Mock<ILogger<GamesController>>();

            request = new Request();

            createGamePayload = new CreateGamePayload();

            updateGamePayload = TestObjects.GetGamePayload(6);

            sutSuccess = new GamesController(
                mockGamesService.SuccessfulRequest.Object,
                mockAppsService.SuccessfulRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                mockedHttpContextAccessor.Object,
                mockedLogger.Object);

            sutFailure = new GamesController(
                mockGamesService.FailedRequest.Object,
                mockAppsService.FailedRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                mockedHttpContextAccessor.Object,
                mockedLogger.Object);
        }

        [Test]
        [Category("Controllers")]
        public void SuccessfullyGetGame()
        {
            // Arrange
            var gameId = 1;

            // Act
            var result = sutSuccess.GetGameAsync(gameId, request);
            var message = ((Result)((OkObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result.Result).StatusCode;
            var game = (Game)((Result)((OkObjectResult)result.Result.Result).Value).Payload[0];

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<Game>>());
            Assert.That(message, Is.EqualTo("Status Code 200: Game Found"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(game, Is.InstanceOf<Game>());
        }

        [Test]
        [Category("Controllers")]
        public void IssueErrorAndMessageShouldGetGameFail()
        {
            // Arrange
            var gameId = 1;

            // Act
            var result = sutFailure.GetGameAsync(gameId, request);
            var message = ((Result)((NotFoundObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<Game>>());
            Assert.That(message, Is.EqualTo("Status Code 404: Game not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public void SuccessfullyGetGames()
        {
            // Arrange

            // Act
            var result = sutSuccess.GetGamesAsync(request);
            var message = ((Result)((OkObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<IEnumerable<Game>>>());
            Assert.That(message, Is.EqualTo("Status Code 200: Games Found"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test]
        [Category("Controllers")]
        public void IssueErrorAndMessageShouldGetGamesFail()
        {
            // Arrange

            // Act
            var result = sutFailure.GetGamesAsync(request);
            var message = ((Result)((NotFoundObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<IEnumerable<Game>>>());
            Assert.That(message, Is.EqualTo("Status Code 404: Games not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public void SuccessfullyDeleteGames()
        {
            // Arrange

            // Act
            var result = sutSuccess.DeleteAsync(1, request);
            var message = ((Result)((OkObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<Game>>());
            Assert.That(message, Is.EqualTo("Status Code 200: Game Deleted"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test]
        [Category("Controllers")]
        public void IssueErrorAndMessageShouldDeleteGamesFail()
        {
            // Arrange

            // Act
            var result = sutFailure.DeleteAsync(1, request);
            var message = ((Result)((NotFoundObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<Game>>());
            Assert.That(message, Is.EqualTo("Status Code 404: Game not Deleted"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public void SuccessfullyUpdateGames()
        {
            // Arrange
            request.Payload = updateGamePayload;

            // Act
            var result = sutSuccess.UpdateAsync(1, request);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Game Updated"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test]
        [Category("Controllers")]
        public void IssueErrorAndMessageShouldUpdateGamesFail()
        {
            // Arrange
            request.Payload = updateGamePayload;

            // Act
            var result = sutFailure.UpdateAsync(1, request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Game not Updated"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public void SuccessfullyCreateGames()
        {
            // Arrange
            request.Payload = createGamePayload;

            // Act
            var result = sutSuccess.PostAsync(request);
            var message = ((Result)((ObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((ObjectResult)result.Result.Result).StatusCode;
            var game = (Game)((Result)((ObjectResult)result.Result.Result).Value).Payload[0];

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<Game>>());
            Assert.That(message, Is.EqualTo("Status Code 201: Game Created"));
            Assert.That(statusCode, Is.EqualTo(201));
            Assert.That(game, Is.InstanceOf<Game>());
        }

        [Test]
        [Category("Controllers")]
        public void IssueErrorAndMessageShouldCreateGamesFail()
        {
            // Arrange
            request.Payload = createGamePayload;

            // Act
            var result = sutFailure.PostAsync(request);
            var message = ((Result)((NotFoundObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<Game>>());
            Assert.That(message, Is.EqualTo("Status Code 404: Game not Created"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public void SuccessfullyCheckGames()
        {
            // Arrange
            request.Payload = updateGamePayload;

            // Act
            var result = sutSuccess.CheckAsync(1, request);
            var message = ((Result)((OkObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result.Result).StatusCode;
            var game = (Game)((Result)((OkObjectResult)result.Result.Result).Value).Payload[0];

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<Game>>());
            Assert.That(message, Is.EqualTo("Status Code 200: Game Solved"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(game, Is.InstanceOf<Game>());
        }

        [Test]
        [Category("Controllers")]
        public void IssueErrorAndMessageShouldCheckGamesFail()
        {
            // Arrange
            request.Payload = updateGamePayload;

            // Act
            var result = sutFailure.CheckAsync(1, request);
            var message = ((Result)((NotFoundObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<Game>>());
            Assert.That(message, Is.EqualTo("Status Code 404: Game not Updated"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public void SuccessfullyGetGameByUserId()
        {
            // Arrange
            var userId = 1;
            request.Payload = updateGamePayload;

            // Act
            var result = sutSuccess.GetMyGameAsync(userId, request);
            var message = ((Result)((OkObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result.Result).StatusCode;
            var game = (Game)((Result)((OkObjectResult)result.Result.Result).Value).Payload[0];

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<Game>>());
            Assert.That(message, Is.EqualTo("Status Code 200: Game Found"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(game, Is.InstanceOf<Game>());
        }

        [Test]
        [Category("Controllers")]
        public void IssueErrorAndMessageShouldGetGameByUserIdFail()
        {
            // Arrange
            var userId = 1;
            request.Payload = updateGamePayload;

            // Act
            var result = sutFailure.GetMyGameAsync(userId, request);
            var message = ((Result)((NotFoundObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<Game>>());
            Assert.That(message, Is.EqualTo("Status Code 404: Game not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public void SuccessfullyGetGamesByUserId()
        {
            // Arrange
            request.Payload = updateGamePayload;

            // Act
            var result = sutSuccess.GetMyGamesAsync(request);
            var message = ((Result)((OkObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<IEnumerable<Game>>>());
            Assert.That(message, Is.EqualTo("Status Code 200: Games Found"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test]
        [Category("Controllers")]
        public void IssueErrorAndMessageShouldGetGamesByUserIdFail()
        {
            // Arrange
            request.Payload = updateGamePayload;

            // Act
            var result = sutFailure.GetMyGamesAsync(request);
            var message = ((Result)((NotFoundObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<IEnumerable<Game>>>());
            Assert.That(message, Is.EqualTo("Status Code 404: Games not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public void SuccessfullyUpdateGamesByUserId()
        {
            // Arrange
            request.Payload = updateGamePayload;

            // Act
            var result = sutSuccess.UpdateMyGameAsync(1, request);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Game Updated"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test]
        [Category("Controllers")]
        public void IssueErrorAndMessageShouldUpdateGamesByUserIdFail()
        {
            // Arrange
            request.Payload = updateGamePayload;

            // Act
            var result = sutFailure.UpdateMyGameAsync(1, request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Game not Updated"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public void SuccessfullyDeleteGameByUserId()
        {
            // Arrange
            var userId = 1;
            request.Payload = updateGamePayload;

            // Act
            var result = sutSuccess.DeleteMyGameAsync(userId, request);
            var message = ((Result)((OkObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<Game>>());
            Assert.That(message, Is.EqualTo("Status Code 200: Game Deleted"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test]
        [Category("Controllers")]
        public void IssueErrorAndMessageShouldDeleteGameByUserIdFail()
        {
            // Arrange
            var userId = 1;
            request.Payload = updateGamePayload;

            // Act
            var result = sutFailure.DeleteMyGameAsync(userId, request);
            var message = ((Result)((NotFoundObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<Game>>());
            Assert.That(message, Is.EqualTo("Status Code 404: Game not Deleted"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public void SuccessfullyCreateAnnonymousGames()
        {
            // Arrange

            // Assert
            var result = sutSuccess.CreateAnnonymousAsync(
                new AnnonymousGameRequest { 
                    DifficultyLevel = DifficultyLevel.TEST 
                });
            var message = ((Result)((ObjectResult)result.Result).Value).Message;
            var statusCode = ((ObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Game Created"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test]
        [Category("Controllers")]
        public void IssueMessageShouldCreateAnnonymousGamesFail()
        {
            // Arrange

            // Assert
            var result = sutFailure.CreateAnnonymousAsync(
                new AnnonymousGameRequest
                {
                    DifficultyLevel = DifficultyLevel.TEST
                });
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Game not Created"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public void SuccessfullyCheckAnnonymousGames()
        {
            // Arrange

            // Assert
            var result = sutSuccess.CheckAnnonymous(
                new AnnonymousCheckRequest
                {
                    FirstRow = new List<int> { 2, 9, 8, 1, 3, 4, 6, 7, 5 },
                    SecondRow = new List<int> { 3, 1, 6, 5, 8, 7, 2, 9, 4 },
                    ThirdRow = new List<int> { 4, 5, 7, 6, 9, 2, 1, 8, 3  },
                    FourthRow = new List<int> { 9, 7, 1, 2, 4, 3, 5, 6, 8 },
                    FifthRow = new List<int> { 5, 8, 3, 7, 6, 1, 4, 2, 9 },
                    SixthRow = new List<int> { 6, 2, 4, 9, 5, 8, 3, 1, 7 },
                    SeventhRow = new List<int> { 7, 3, 5, 8, 2, 6, 9, 4, 1 },
                    EighthRow = new List<int> { 8, 4, 2, 3, 1, 9, 7, 5, 6 },
                    NinthRow = new List<int> { 1, 6, 9, 4, 7, 5, 8, 3, 2 }
                });

            var success = ((Result)((ObjectResult)result).Value).IsSuccess;
            var message = ((Result)((ObjectResult)result).Value).Message;
            var statusCode = ((ObjectResult)result).StatusCode;

            // Assert
            Assert.That(success, Is.True);
            Assert.That(message, Is.EqualTo("Status Code 200: Game Solved"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test]
        [Category("Controllers")]
        public void IssueMessageShouldCheckAnnonymousGamesFail()
        {
            // Arrange

            // Assert
            var result = sutFailure.CheckAnnonymous(
                new AnnonymousCheckRequest
                {
                    FirstRow = new List<int> { 5, 9, 8, 1, 3, 4, 6, 7, 2 },
                    SecondRow = new List<int> { 3, 1, 6, 5, 8, 7, 2, 9, 4 },
                    ThirdRow = new List<int> { 4, 5, 7, 6, 9, 2, 1, 8, 3 },
                    FourthRow = new List<int> { 9, 7, 1, 2, 4, 3, 5, 6, 8 },
                    FifthRow = new List<int> { 5, 8, 3, 7, 6, 1, 4, 2, 9 },
                    SixthRow = new List<int> { 6, 2, 4, 9, 5, 8, 3, 1, 7 },
                    SeventhRow = new List<int> { 7, 3, 5, 8, 2, 6, 9, 4, 1 },
                    EighthRow = new List<int> { 8, 4, 2, 3, 1, 9, 7, 5, 6 },
                    NinthRow = new List<int> { 1, 6, 9, 4, 7, 5, 8, 3, 2 }
                });

            var success = ((Result)((ObjectResult)result).Value).IsSuccess;
            var message = ((Result)((ObjectResult)result).Value).Message;
            var statusCode = ((ObjectResult)result).StatusCode;

            // Assert
            Assert.That(success, Is.False);
            Assert.That(message, Is.EqualTo("Status Code 404: Game not Solved"));
            Assert.That(statusCode, Is.EqualTo(404));
        }
    }
}
