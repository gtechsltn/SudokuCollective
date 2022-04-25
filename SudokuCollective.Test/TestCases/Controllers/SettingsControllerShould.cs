using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SudokuCollective.Api.Controllers.V1;
using SudokuCollective.Core.Enums;

namespace SudokuCollective.Test.TestCases.Controllers
{
    public class SettingsControllerShould
    {
        private SettingsController sut;

        [SetUp]
        public void SetUp()
        {
            sut = new SettingsController();
        }

        [Test, Category("Controller")]
        public void HaveEnumListItemsWithRequiredProperties()
        {
            // Arrange and Act
            var result = new EnumListItem();

            // Assert
            Assert.That(result.Label, Is.InstanceOf<string>());
            Assert.That(result.Value, Is.InstanceOf<int>());
            Assert.That(result.AppliesTo, Is.InstanceOf<List<string>>());
        }

        [Test, Category("Controller")]
        public void GetAListOfReleaseEnvironments()
        {
            // Arrange and Act
            var result = sut.GetReleaseEnvironments();
            var environments = ((IEnumerable<EnumListItem>)((OkObjectResult)result.Result).Value)
                .Cast<EnumListItem>()
                .ToList();
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.TypeOf<ActionResult<List<EnumListItem>>>());
            Assert.That(environments.Count, Is.EqualTo(4));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controller")]
        public void GetAListOfTimeFrames()
        {
            // Arrange and Act
            var result = sut.GetTimeFrames();
            var timeFrames = ((IEnumerable<EnumListItem>)((OkObjectResult)result.Result).Value)
                .Cast<EnumListItem>()
                .ToList();
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.TypeOf<ActionResult<List<EnumListItem>>>());
            Assert.That(timeFrames.Count, Is.EqualTo(6));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controller")]
        public void GetAListOfSortValues()
        {
            // Arrange and Act
            var result = sut.GetSortValues();
            var sortValues = ((IEnumerable<EnumListItem>)((OkObjectResult)result.Result).Value)
                .Cast<EnumListItem>()
                .ToList();
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.TypeOf<ActionResult<List<EnumListItem>>>());
            Assert.That(sortValues.Count, Is.EqualTo(14));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controller")]
        public void HaveAListOfSortValuesFilterableForApps()
        {
            // Arrange and Act
            var result = sut.GetSortValues();
            var sortValues = ((IEnumerable<EnumListItem>)((OkObjectResult)result.Result).Value)
                .Cast<EnumListItem>()
                .ToList();
            var appValues = sortValues.Where(s => s.AppliesTo.Contains("apps"));
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.TypeOf<ActionResult<List<EnumListItem>>>());
            Assert.That(appValues.Count, Is.EqualTo(5));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controller")]
        public void HaveAListOfSortValuesFilterableForUsers()
        {
            // Arrange and Act
            var result = sut.GetSortValues();
            var sortValues = ((IEnumerable<EnumListItem>)((OkObjectResult)result.Result).Value)
                .Cast<EnumListItem>()
                .ToList();
            var userValues = sortValues.Where(s => s.AppliesTo.Contains("users"));
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.TypeOf<ActionResult<List<EnumListItem>>>());
            Assert.That(userValues.Count, Is.EqualTo(10));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controller")]
        public void HaveAListOfSortValuesFilterableForGames()
        {
            // Arrange and Act
            var result = sut.GetSortValues();
            var sortValues = ((IEnumerable<EnumListItem>)((OkObjectResult)result.Result).Value)
                .Cast<EnumListItem>()
                .ToList();
            var gameValues = sortValues.Where(s => s.AppliesTo.Contains("games"));
            var statusCode = ((OkObjectResult)result.Result).StatusCode;

            // Assert
            Assert.That(result, Is.TypeOf<ActionResult<List<EnumListItem>>>());
            Assert.That(gameValues.Count, Is.EqualTo(5));
            Assert.That(statusCode, Is.EqualTo(200));
        }
    }
}
