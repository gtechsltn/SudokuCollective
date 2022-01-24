﻿using NUnit.Framework;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Data.Models.Requests;

namespace SudokuCollective.Test.TestCases.Requests
{
    public class ConfirmUserNameRequestShould
    {
        private IConfirmUserNameRequest sut;

        [SetUp]
        public void Setup()
        {
            sut = new ConfirmUserNameRequest();
        }

        [Test, Category("Requests")]
        public void HasRequiredProperties()
        {
            // Arrange and Act

            // Assert
            Assert.That(sut.Email, Is.InstanceOf<string>());
            Assert.That(sut.License, Is.InstanceOf<string>());
        }

        [Test, Category("Requests")]
        public void RejectInvalidEmails()
        {
            // Arrange and Act
            sut.Email = "invalidEmail@";

            // Assert
            Assert.That(sut.Email, Is.EqualTo(string.Empty));
        }

        [Test, Category("Requests")]
        public void RejectInvalidLicenses()
        {
            // Arrange and Act
            sut.License = "invalidLicense";

            // Assert
            Assert.That(sut.License, Is.EqualTo(string.Empty));
        }
    }
}
