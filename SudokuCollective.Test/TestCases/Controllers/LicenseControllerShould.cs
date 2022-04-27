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
        public void SuccessfullyGetLicense()
        {
            // Arrange
            var appId = 1;

            // Act
            var result = sutSuccess.Get(appId, request);
            var message = ((LicenseResult)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;
            var license = ((LicenseResult)((OkObjectResult)result.Result).Value).License;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: App Found"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(license, Is.InstanceOf<string>());
        }

        [Test]
        [Category("Controllers")]
        public void IssueErrorAndMessageShouldGetLicenseFail()
        {
            // Arrange
            var appId = 1;

            // Act
            var result = sutFailure.Get(appId, request);
            var message = ((LicenseResult)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: App not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public void SuccessfullyPostLicenses()
        {
            // Arrange
            request.Payload = licensePayload;

            // Act
            var result = sutSuccess.Post(request);
            var app = (App)((Result)((ObjectResult)result.Result.Result).Value).Payload[0];
            var message = ((Result)((ObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((ObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<App>>());
            Assert.That(message, Is.EqualTo("Status Code 201: App Created"));
            Assert.That(statusCode, Is.EqualTo(201));
            Assert.That(app, Is.InstanceOf<App>());
        }

        [Test]
        [Category("Controllers")]
        public void IssueErrorAndMessageShouldPostLicenseFail()
        {
            // Arrange
            request.Payload = licensePayload;

            // Act
            var result = sutFailure.Post(request);
            var message = ((Result)((NotFoundObjectResult)result.Result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult<App>>());
            Assert.That(message, Is.EqualTo("Status Code 404: App not Created"));
            Assert.That(statusCode, Is.EqualTo(404));

        }
    }
}
