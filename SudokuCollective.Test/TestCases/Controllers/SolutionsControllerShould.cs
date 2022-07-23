using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SudokuCollective.Data.Models.Payloads;
using SudokuCollective.Core.Models;
using SudokuCollective.Test.TestData;
using SudokuCollective.Api.V1.Controllers;
using SudokuCollective.Data.Models;
using SudokuCollective.Test.Services;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Requests;

namespace SudokuCollective.Test.TestCases.Controllers
{
    public class SolutionsControllerShould
    {
        private DatabaseContext context;
        private SolutionsController sutSuccess;
        private SolutionsController sutFailure;
        private SolutionsController sutSolvedFailure;
        private MockedSolutionsService mockSolutionsService;
        private MockedAppsService mockAppsService;
        private MockedRequestService mockedRequestService;
        private Mock<IHttpContextAccessor> mockedHttpContextAccessor;
        private Mock<ILogger<SolutionsController>> mockedLogger;
        private Request request;
        private AnnonymousCheckRequest annonymousCheckRequest;
        private AddSolutionsPayload addSolutionPayload;
        private AddSolutionsPayload invalidAddSolutionPayload;

        [SetUp]
        public async Task Setup()
        {
            context = await TestDatabase.GetDatabaseContext();
            mockSolutionsService = new MockedSolutionsService(context);
            mockAppsService = new MockedAppsService(context);
            mockedRequestService = new MockedRequestService();
            mockedHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockedLogger = new Mock<ILogger<SolutionsController>>();

            request = TestObjects.GetRequest();

            annonymousCheckRequest = new AnnonymousCheckRequest()
            {
                FirstRow = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                SecondRow = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                ThirdRow = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                FourthRow = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                FifthRow = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                SixthRow = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                SeventhRow = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                EighthRow = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                NinthRow = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            };

            addSolutionPayload = new AddSolutionsPayload()
            {
                Limit = 1000
            };

            invalidAddSolutionPayload = new AddSolutionsPayload()
            {
                Limit = 1001
            };

            sutSuccess = new SolutionsController(
                mockSolutionsService.SuccessfulRequest.Object,
                mockAppsService.SuccessfulRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                mockedHttpContextAccessor.Object,
                mockedLogger.Object);

            sutFailure = new SolutionsController(
                mockSolutionsService.FailedRequest.Object,
                mockAppsService.SuccessfulRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                mockedHttpContextAccessor.Object,
                mockedLogger.Object);

            sutSolvedFailure = new SolutionsController(
                mockSolutionsService.SolveFailedRequest.Object,
                mockAppsService.SuccessfulRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                mockedHttpContextAccessor.Object,
                mockedLogger.Object);
        }

        [Test]
        [Category("Controllers")]
        public async Task SuccessfullyGetSolution()
        {
            // Arrange
            var solutionId = 1;

            // Act
            var actionResult = await sutSuccess.GetAsync(solutionId, request);
            var result = (Result)((OkObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var solution = (SudokuSolution)result.Payload[0];
            var statusCode = ((OkObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 200: Solution Found"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(solution, Is.InstanceOf<SudokuSolution>());
        }

        [Test]
        [Category("Controllers")]
        public async Task IssueErrorAndMessageShouldGetSolutionFail()
        {
            // Arrange
            var solutionId = 2;

            // Act
            var actionResult = await sutFailure.GetAsync(solutionId, request);
            var result = (Result)((NotFoundObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var statusCode = ((NotFoundObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 404: Solution not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public async Task SuccessfullyGetSolutions()
        {
            // Arrange

            // Act
            var actionResult = await sutSuccess.GetSolutionsAsync(request);
            var result = (Result)((OkObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var statusCode = ((OkObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 200: Solutions Found"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test]
        [Category("Controllers")]
        public async Task IssueErrorAndMessageShouldGetSolutionsFail()
        {
            // Arrange

            // Act
            var actionResult = await sutFailure.GetSolutionsAsync(request);
            var result = (Result)((NotFoundObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var statusCode = ((NotFoundObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 404: Solutions not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public async Task SuccessfullySolveSolution()
        {
            // Arrange

            // Act
            var actionResult = await sutSuccess.SolveAsync(annonymousCheckRequest);
            var result = (Result)((OkObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var statusCode = ((OkObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 200: Sudoku Solution Found"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test]
        [Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSolveSolutionFail()
        {
            // Arrange

            // Act
            var actionResult = await sutFailure.SolveAsync(annonymousCheckRequest);
            var result = (Result)((BadRequestObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var statusCode = ((BadRequestObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 400: Sudoku Solution not Found"));
            Assert.That(statusCode, Is.EqualTo(400));
        }

        [Test]
        [Category("Controllers")]
        public async Task SuccessfullyGenerateSolution()
        {
            // Arrange

            // Act
            var actionResult = await sutSuccess.GenerateAsync();
            var result = (Result)((OkObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var statusCode = ((OkObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 200: Solution Generated"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test]
        [Category("Controllers")]
        public async Task IssueErrorAndMessageShouldGenerateSolutionFail()
        {
            // Arrange

            // Act
            var actionResult = await sutFailure.GenerateAsync();
            var result = (Result)((BadRequestObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var statusCode = ((BadRequestObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 400: Solution not Generated"));
            Assert.That(statusCode, Is.EqualTo(400));
        }

        [Test]
        [Category("Controllers")]
        public async Task IssueMessageIfSudokuSolutionSolveFailed()
        {
            // Arrange

            // Act
            var actionResult =  await sutSolvedFailure.SolveAsync(annonymousCheckRequest);
            var result = (Result)((OkObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var statusCode = ((OkObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 200: Sudoku Solution not Found"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test]
        [Category("Controllers")]
        public async Task SuccessfullyAddSolutions()
        {
            // Arrange
            request.Payload = addSolutionPayload;

            // Act
            var actionResult = await sutSuccess.AddSolutionsAsync(request);
            var result = (Result)((OkObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var statusCode = ((OkObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 200: Solutions Added"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test]
        [Category("Controllers")]
        public async Task IssueMessageIfAddSolutionsFailed()
        {
            // Arrange
            request.Payload = invalidAddSolutionPayload;

            // Act
            var actionResult = await sutFailure.AddSolutionsAsync(request);
            var result = (Result)((BadRequestObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var statusCode = ((BadRequestObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 400: Solutions not Added"));
            Assert.That(statusCode, Is.EqualTo(400));
        }

        [Test]
        [Category("Controllers")]
        public async Task IssueErrorAndMessageShouldAddSolutionsFail()
        {
            // Arrange
            request.Payload = addSolutionPayload;

            // Act
            var actionResult = await sutFailure.AddSolutionsAsync(request);
            var result = (Result)((BadRequestObjectResult)actionResult.Result).Value;
            var message = result.Message;
            var statusCode = ((BadRequestObjectResult)actionResult.Result).StatusCode;

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ActionResult<Result>>());
            Assert.That(result, Is.InstanceOf<Result>());
            Assert.That(message, Is.EqualTo("Status Code 400: Solutions not Added"));
            Assert.That(statusCode, Is.EqualTo(400));
        }
    }
}
