using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SudokuCollective.Api.Controllers;
using SudokuCollective.Data.Models.Params;

namespace SudokuCollective.Test.TestCases.Controllers
{
    public class HelloWorldControllerShould
    {
        private HelloWorldController sut;

        [SetUp]
        public void SetUp()
        {
            sut = new HelloWorldController();
        }

        [Test, Category("Controllers")]
        public void ReturnAMessage()
        {
            // Arrange

            // Act
            var result = sut.Get();
            var success = ((Result)((OkObjectResult)result).Value).IsSuccess;
            var message = ((Result)((OkObjectResult)result).Value).Message;
            var statusCode = ((OkObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            Assert.That(success, Is.True);
            Assert.That(message, Is.EqualTo("Status Code 200: Hello World from Sudoku Collective!"));
            Assert.That(statusCode, Is.EqualTo(200));
        }

        [Test, Category("Controllers")]
        public void EchoParams()
        {
            // Arrange

            // Act
            var result = sut.Get("hello_world");
            var success = ((Result)((OkObjectResult)result).Value).IsSuccess;
            var message = ((Result)((OkObjectResult)result).Value).Message;
            var statusCode = ((OkObjectResult)result).StatusCode;

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            Assert.That(success, Is.True);
            Assert.That(message, Is.EqualTo("Status Code 200: You Submitted: hello_world"));
            Assert.That(statusCode, Is.EqualTo(200));
        }
    }
}
