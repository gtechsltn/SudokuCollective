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
using SudokuCollective.Data.Models.Payloads;
using SudokuCollective.Data.Models.Results;
using SudokuCollective.Test.Services;
using SudokuCollective.Test.TestData;

namespace SudokuCollective.Test.TestCases.Controllers
{
    public class LicenseControllerShould
    {
        private DatabaseContext context;
        private LicensesController sutSuccess;
        private LicensesController sutFailure;
        private MockedAppsService mockedAppsService;
        private MockedRequestService mockedRequestService;
        private Mock<IHttpContextAccessor> mockedHttpContextAccessor;
        private Mock<ILogger<LicensesController>> mockedLogger;
        private Request request;
        private LicensePayload licensePayload;

        [SetUp]
        public async Task Setup()
        {
            context = await TestDatabase.GetDatabaseContext();
            mockedAppsService = new MockedAppsService(context);
            mockedRequestService = new MockedRequestService();
            mockedHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockedLogger = new Mock<ILogger<LicensesController>>();

            request = TestObjects.GetRequest();

            licensePayload = TestObjects.GetLicensePayload();

            sutSuccess = new LicensesController(
                mockedAppsService.SuccessfulRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                mockedHttpContextAccessor.Object,
                mockedLogger.Object);
            sutFailure = new LicensesController(
                mockedAppsService.FailedRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                mockedHttpContextAccessor.Object,
                mockedLogger.Object);
        }

        [Test]
        [Category("Controllers")]
        public async Task SuccessfullyGetLicense()
        {
            // Arrange
            var appId = 1;

            // Act
            var result = await sutSuccess.GetAsync(appId, request);
            var message = ((LicenseResult)((OkObjectResult)result).Value).Message;
            var statusCode = ((OkObjectResult)result).StatusCode;
            var license = ((LicenseResult)((OkObjectResult)result).Value).License;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: App Found"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(license, Is.InstanceOf<string>());
        }

        [Test]
        [Category("Controllers")]
        public async Task IssueErrorAndMessageShouldGetLicenseFail()
        {
            // Arrange
            var appId = 1;

            // Act
            var result = await sutFailure.GetAsync(appId, request);
            var message = ((LicenseResult)((NotFoundObjectResult)result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: App not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public async Task SuccessfullyPostLicenses()
        {
            // Arrange
            request.Payload = licensePayload;

            // Act
            var result = await sutSuccess.PostAsync(request);
            var app = (App)((Result)((ObjectResult)result.Result).Value).Payload[0];
            var message = ((Result)((ObjectResult)result.Result).Value).Message;
            var statusCode = ((ObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<App>>());
            Assert.That(message, Is.EqualTo("Status Code 201: App Created"));
            Assert.That(statusCode, Is.EqualTo(201));
            Assert.That(app, Is.InstanceOf<App>());
        }

        [Test]
        [Category("Controllers")]
        public async Task IssueErrorAndMessageShouldPostLicenseFail()
        {
            // Arrange
            request.Payload = licensePayload;

            // Act
            var result = await sutFailure.PostAsync(request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<App>>());
            Assert.That(message, Is.EqualTo("Status Code 404: App not Created"));
            Assert.That(statusCode, Is.EqualTo(404));

        }
    }
}
