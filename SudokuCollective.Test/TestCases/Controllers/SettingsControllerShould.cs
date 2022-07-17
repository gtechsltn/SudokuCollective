using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SudokuCollective.Api.Controllers.V1;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Settings;
using SudokuCollective.Test.Services;
using SudokuCollective.Test.TestData;

namespace SudokuCollective.Test.TestCases.Controllers
{
    public class SettingsControllerShould
    {
        private DatabaseContext context;
        private SettingsController sut;
        private MockedSettingsService mockedSettingsService;

        [SetUp]
        public async Task SetUp()
        {
            context = await TestDatabase.GetDatabaseContext();
            mockedSettingsService = new MockedSettingsService(context);
            sut = new SettingsController(mockedSettingsService.Request.Object);
        }

        [Test, Category("Controller")]
        public async Task GetSettings()
        {
            // Arrange and Act
            var result = await sut.GetAsync();
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.TypeOf<ActionResult<Result>>());
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controller")]
        public void GetAListOfReleaseEnvironments()
        {
            // Arrange and Act
            var result = sut.GetReleaseEnvironments();
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.TypeOf<ActionResult<Result>>());
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controller")]
        public void GetAListOfTimeFrames()
        {
            // Arrange and Act
            var result = sut.GetTimeFrames();
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.TypeOf<ActionResult<Result>>());
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controller")]
        public void GetAListOfSortValues()
        {
            // Arrange and Act
            var result = sut.GetSortValues();
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.TypeOf<ActionResult<Result>>());
            Assert.That(statusCode, Is.EqualTo(200));
        }
    }
}
