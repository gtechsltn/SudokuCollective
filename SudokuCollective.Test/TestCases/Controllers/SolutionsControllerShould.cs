using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SudokuCollective.Core.Models;
using SudokuCollective.Test.TestData;
using SudokuCollective.Api.V1.Controllers;
using SudokuCollective.Data.Models;
using SudokuCollective.Test.Services;
using SudokuCollective.Data.Models.Params;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SudokuCollective.Data.Models.Payloads;
using SudokuCollective.Data.Models.Results;

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
            var result = await sutSuccess.GetAsync(solutionId, request);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;
            var solution = (SudokuSolution)((Result)((OkObjectResult)result.Result).Value).Payload[0];

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<SudokuSolution>>());
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
            var result = await sutFailure.GetAsync(solutionId, request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<SudokuSolution>>());
            Assert.That(message, Is.EqualTo("Status Code 404: Solution not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public async Task SuccessfullyGetSolutions()
        {
            // Arrange

            // Act
            var result = await sutSuccess.GetSolutionsAsync(request);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<IEnumerable<SudokuSolution>>>());
            Assert.That(message, Is.EqualTo("Status Code 200: Solutions Found"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test]
        [Category("Controllers")]
        public async Task IssueErrorAndMessageShouldGetSolutionsFail()
        {
            // Arrange

            // Act
            var result = await sutFailure.GetSolutionsAsync(request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<IEnumerable<SudokuSolution>>>());
            Assert.That(message, Is.EqualTo("Status Code 404: Solutions not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public async Task SuccessfullySolveSolution()
        {
            // Arrange

            // Act
            var result = await sutSuccess.SolveAsync(annonymousCheckRequest);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<AnnonymousGameResult>>());
            Assert.That(message, Is.EqualTo("Status Code 200: Sudoku Solution Found"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test]
        [Category("Controllers")]
        public async Task IssueErrorAndMessageShouldSolveSolutionFail()
        {
            // Arrange

            // Act
            var result = await sutFailure.SolveAsync(annonymousCheckRequest);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<AnnonymousGameResult>>());
            Assert.That(message, Is.EqualTo("Status Code 404: Sudoku Solution not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public async Task SuccessfullyGenerateSolution()
        {
            // Arrange

            // Act
            var result = await sutSuccess.GenerateAsync();
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<AnnonymousGameResult>>());
            Assert.That(message, Is.EqualTo("Status Code 200: Solution Generated"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test]
        [Category("Controllers")]
        public async Task IssueErrorAndMessageShouldGenerateSolutionFail()
        {
            // Arrange

            // Act
            var result = await sutFailure.GenerateAsync();
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<AnnonymousGameResult>>());
            Assert.That(message, Is.EqualTo("Status Code 404: Solution not Generated"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public async Task IssueMessageIfSudokuSolutionSolveFailed()
        {
            // Arrange

            // Act
            var result =  await sutSolvedFailure.SolveAsync(annonymousCheckRequest);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<AnnonymousGameResult>>());
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
            var result = await sutSuccess.AddSolutionsAsync(request);
            var message = ((Result)((OkObjectResult)result).Value).Message;
            var statusCode = ((OkObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
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
            var result = await sutFailure.AddSolutionsAsync(request);
            var message = ((Result)((NotFoundObjectResult)result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Solutions not Added"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public async Task IssueErrorAndMessageShouldAddSolutionsFail()
        {
            // Arrange
            request.Payload = addSolutionPayload;

            // Act
            var result = await sutFailure.AddSolutionsAsync(request);
            var message = ((Result)((NotFoundObjectResult)result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Solutions not Added"));
            Assert.That(statusCode, Is.EqualTo(404));
        }
    }
}
