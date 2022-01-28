using NUnit.Framework;
using SudokuCollective.Core.Interfaces.Models.TokenModels;
using SudokuCollective.Data.Models.Authentication;
using SudokuCollective.Test.TestData;
using System;

namespace SudokuCollective.Test.TestCases.Authentication
{
    public class TokenRequestShould
    {
        private ITokenRequest sut;

        [SetUp]
        public void Setup()
        {
            sut = new TokenRequest();
        }

        [Test, Category("Authentication")]
        public void HasRequiredProperties()
        {
            // Arrange and Act

            // Assert
            Assert.That(sut.UserName, Is.InstanceOf<string>());
            Assert.That(sut.Password, Is.InstanceOf<string>());
            Assert.That(sut.License, Is.InstanceOf<string>());
        }

        [Test, Category("Authentication")]
        public void HasADefaultConstrutor()
        {
            // Arrange and Act
            sut = new TokenRequest();

            // Assert
            Assert.That(sut, Is.InstanceOf<TokenRequest>());
        }

        [Test, Category("Authentication")]
        public void HasAConstructorThatAcceptsParams()
        {
            // Arrange and Act
            sut = new TokenRequest(
                "username", 
                "T3stP4ssw0rd$", 
                TestObjects.GetToken());

            // Assert
            Assert.That(sut, Is.InstanceOf<TokenRequest>());
        }

        [Test, Category("Authentication")]
        public void ThrowAnExceptionIfUserNameIsInvalid()
        {
            // Arrange
            
            // Act and Assert
            Assert.Throws<ArgumentException>(
                () => new TokenRequest(
                    "usn", 
                    "T3stP4ssw0rd$", 
                    TestObjects.GetToken()));
        }

        [Test, Category("Authentication")]
        public void ThrowAnExceptionIfPasswordIsInvalid()
        {
            // Arrange

            // Act and Assert
            Assert.Throws<ArgumentException>(
                () => new TokenRequest(
                    "username",
                    "test",
                    TestObjects.GetToken()));
        }

        [Test, Category("Authentication")]
        public void ThrowAnExceptionIfLicenseIsInvalid()
        {
            // Arrange

            // Act and Assert
            Assert.Throws<ArgumentException>(
                () => new TokenRequest(
                    "username",
                    "test",
                    "license"));
        }
    }
}
