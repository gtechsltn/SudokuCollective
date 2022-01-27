﻿using System;
using NUnit.Framework;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Data.Models.Requests;
using SudokuCollective.Test.TestData;

namespace SudokuCollective.Test.TestCases.Requests
{
    public class RequestPasswordResetRequestShould
    {
        private IRequestPasswordResetRequest sut;

        [SetUp]
        public void Setup()
        {
            sut = new RequestPasswordResetRequest();
        }

        [Test, Category("Requests")]
        public void HasRequiredProperties()
        {
            // Arrange and Act

            // Assert
            Assert.That(sut.License, Is.InstanceOf<string>());
            Assert.That(sut.Email, Is.InstanceOf<string>());
        }

        [Test, Category("Requests")]
        public void HasADefaultConstructor()
        {
            // Arrange and Act
            sut = new RequestPasswordResetRequest();

            // Assert
            Assert.That(sut, Is.InstanceOf<RequestPasswordResetRequest>());
        }

        [Test, Category("Requests")]
        public void HasAConstructorThatAcceptsParams()
        {
            // Arrange and Act
            sut = new RequestPasswordResetRequest(TestObjects.GetLicense(), "testEmail@example.com");

            // Assert
            Assert.That(sut, Is.InstanceOf<RequestPasswordResetRequest>());
        }

        [Test, Category("Requests")]
        public void ThrowsExceptionForInvalidLicenses()
        {
            // Arrange

            // Act and Assert
            Assert.Throws<ArgumentException>(() => sut.License = "InvalidLicense");
        }

        [Test, Category("Requests")]
        public void ThrowsExceptionForInvalidEmails()
        {
            // Arrange

            // Act and Assert
            Assert.Throws<ArgumentException>(() => sut.Email = "InvalidEmail@");
        }
    }
}
