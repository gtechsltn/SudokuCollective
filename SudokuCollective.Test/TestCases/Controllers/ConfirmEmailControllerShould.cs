﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SudokuCollective.Api.Controllers;
using SudokuCollective.Data.Models;
using SudokuCollective.Test.Services;
using SudokuCollective.Test.TestData;

namespace SudokuCollective.Test.TestCases.Controllers
{
    public class ConfirmEmailControllerShould
    {
        private DatabaseContext context;
        private ConfirmEmailController sutSuccess;
        private ConfirmEmailController sutFailure;
        private MockedUsersService mockedUsersService;
        private Mock<IWebHostEnvironment> mockedWebHostEnvironment;
        private string emailConfirmationToken;

        [SetUp]
        public async Task Setup()
        {
            context = await TestDatabase.GetDatabaseContext();

            mockedUsersService = new MockedUsersService(context);
            mockedWebHostEnvironment = new Mock<IWebHostEnvironment>();

            sutSuccess = new ConfirmEmailController(
                mockedUsersService.SuccessfulRequest.Object,
                mockedWebHostEnvironment.Object);
            sutFailure = new ConfirmEmailController(
                mockedUsersService.FailedRequest.Object,
                mockedWebHostEnvironment.Object);

            emailConfirmationToken = Guid.NewGuid().ToString();
        }

        [Test]
        [Category("Controllers")]
        public void SuccessfullyConfirmUserEmails()
        {
            // Arrange

            // Act
            var result = sutSuccess.Index(emailConfirmationToken);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
        }

        [Test]
        [Category("Controllers")]
        public void ProcessRequestIfConfirmEmailTokenAlreadyProcessed()
        {
            // Arrange

            // Act
            var result = sutFailure.Index(emailConfirmationToken);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ActionResult>());
        }
    }
}