using System.Collections.Generic;
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
using SudokuCollective.Test.Services;
using SudokuCollective.Test.TestData;

namespace SudokuCollective.Test.TestCases.Controllers
{
    public class RolesControllerShould
    {
        private DatabaseContext context;
        private RolesController sutSuccess;
        private RolesController sutFailure;
        private MockedRolesService mockedRolesService;
        private MockedAppsService mockedAppsService;
        private MockedRequestService mockedRequestService;
        private Mock<IHttpContextAccessor> mockedHttpContextAccessor;
        private Mock<ILogger<RolesController>> mockedLogger;
        private Request request;
        private CreateRolePayload createRolePayload;
        private UpdateRolePayload updateRolePayload;


        [SetUp]
        public async Task Setup()
        {
            context = await TestDatabase.GetDatabaseContext();
            mockedRolesService = new MockedRolesService(context);
            mockedAppsService = new MockedAppsService(context);
            mockedRequestService = new MockedRequestService();
            mockedHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockedLogger = new Mock<ILogger<RolesController>>();

            request = TestObjects.GetRequest();
            
            updateRolePayload = TestObjects.GetUpdateRolePayload();
            
            createRolePayload = TestObjects.GetCreateRolePayload();

            sutSuccess = new RolesController(
                mockedRolesService.SuccessfulRequest.Object,
                mockedAppsService.SuccessfulRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                mockedHttpContextAccessor.Object,
                mockedLogger.Object);

            sutFailure = new RolesController(
                mockedRolesService.FailedRequest.Object,
                mockedAppsService.SuccessfulRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                mockedHttpContextAccessor.Object,
                mockedLogger.Object);
        }

        [Test]
        [Category("Controllers")]
        public async Task SuccessfullyGetRole()
        {
            // Arrange
            var roleId = 1;

            // Act
            var result = await sutSuccess.GetAsync(roleId);
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;
            var role = (Role)((Result)((OkObjectResult)result.Result).Value).Payload[0];

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<Role>>());
            Assert.That(message, Is.EqualTo("Status Code 200: Role Found"));
            Assert.That(statusCode, Is.EqualTo(200));
            Assert.That(role, Is.InstanceOf<Role>());
        }

        [Test]
        [Category("Controllers")]
        public async Task IssueErrorAndMessageShouldGetRoleFail()
        {
            // Arrange
            var roleId = 2;

            // Act
            var result = await sutFailure.GetAsync(roleId);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<Role>>());
            Assert.That(message, Is.EqualTo("Status Code 404: Role not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public async Task SuccessfullyGetRoles()
        {
            // Arrange

            // Act
            var result = await sutSuccess.GetRolesAsync( );
            var message = ((Result)((OkObjectResult)result.Result).Value).Message;
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<IEnumerable<Role>>>());
            Assert.That(message, Is.EqualTo("Status Code 200: Roles Found"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test]
        [Category("Controllers")]
        public async Task IssueErrorAndMessageShouldGetRolesFail()
        {
            // Arrange

            // Act
            var result = await sutFailure.GetRolesAsync();
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<IEnumerable<Role>>>());
            Assert.That(message, Is.EqualTo("Status Code 404: Roles not Found"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public async Task SuccessfullyUpdateRoles()
        {
            // Arrange
            request.Payload = updateRolePayload;

            // Act
            var result = await sutSuccess.UpdateAsync(1, request);
            var message = ((Result)((OkObjectResult)result).Value).Message;
            var statusCode = ((OkObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Role Updated"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test]
        [Category("Controllers")]
        public async Task IssueErrorAndMessageShouldUpdateRoleFail()
        {
            // Arrange
            request.Payload = updateRolePayload;

            // Act
            var result = await sutFailure.UpdateAsync(1, request);
            var message = ((Result)((NotFoundObjectResult)result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Role not Updated"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public async Task SuccessfullyCreateRoles()
        {
            // Arrange
            request.Payload = createRolePayload;

            // Act
            var result = await sutSuccess.PostAsync(request);
            var message = ((Result)((ObjectResult)result.Result).Value).Message;
            var statusCode = ((ObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<Role>>());
            Assert.That(message, Is.EqualTo("Status Code 201: Role Created"));
            Assert.That(statusCode, Is.EqualTo(201));
        }

        [Test]
        [Category("Controllers")]
        public async Task IssueErrorAndMessageShouldCreateRolesFail()
        {
            // Arrange
            request.Payload = createRolePayload;

            // Act
            var result = await sutFailure.PostAsync(request);
            var message = ((Result)((NotFoundObjectResult)result.Result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Role not Created"));
            Assert.That(statusCode, Is.EqualTo(404));
        }

        [Test]
        [Category("Controllers")]
        public async Task SuccessfullyDeleteRoles()
        {
            // Arrange
            var roleId = 1;

            // Act
            var result = await sutSuccess.DeleteAsync(roleId, request);
            var message = ((Result)((OkObjectResult)result).Value).Message;
            var statusCode = ((OkObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 200: Role Deleted"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test]
        [Category("Controllers")]
        public async Task IssueErrorAndMessageShouldDeleteRolesFail()
        {
            // Arrange
            var roleId = 1;

            // Act
            var result = await sutFailure.DeleteAsync(roleId, request);
            var message = ((Result)((NotFoundObjectResult)result).Value).Message;
            var statusCode = ((NotFoundObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult>());
            Assert.That(message, Is.EqualTo("Status Code 404: Role not Deleted"));
            Assert.That(statusCode, Is.EqualTo(404));
        }
    }
}