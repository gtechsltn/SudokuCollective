using NUnit.Framework;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Data.Models.Requests;
using System;

namespace SudokuCollective.Test.TestCases.Requests
{
    public class RegisterRequestShould
    {
        private IRegisterRequest sut;

        [SetUp]
        public void Setup()
        {
            sut = new RegisterRequest();
        }

        [Test, Category("Requests")]
        public void HasRequiredProperties()
        {
            // Arrange and Act

            // Assert
            Assert.That(sut.UserName, Is.InstanceOf<string>());
            Assert.That(sut.FirstName, Is.InstanceOf<string>());
            Assert.That(sut.LastName, Is.InstanceOf<string>());
            Assert.That(sut.Email, Is.InstanceOf<string>());
            Assert.That(sut.Password, Is.InstanceOf<string>());
        }

        [Test, Category("Requests")]
        public void ThrowsAnExceptionIfUserNameInvalid()
        {
            // Arrange and Act

            // Act and Assert
            Assert.Throws<ArgumentException>(() => sut.UserName = "joe");
        }

        [Test, Category("Requests")]
        public void ThrowsAnExceptionIfEmailInvalid()
        {
            // Arrange

            // Act and Assert
            Assert.Throws<ArgumentException>(() => sut.Email = "invalidEmail@");
        }

        [Test, Category("Requests")]
        public void RejectInvalidPassword()
        {
            // Arrange

            // Act and Assert
            Assert.Throws<ArgumentException>(() => sut.Password = "password");
        }

        [Test, Category("Requests")]
        public void HasADefaultConstructor()
        {
            // Arrange and Act
            sut = new RegisterRequest();

            // Assert
            Assert.That(sut, Is.InstanceOf<RegisterRequest>());
        }

        [Test, Category("Requests")]
        public void HasAConstructorThatAcceptsParams()
        {
            // Arrange and Act
            sut = new RegisterRequest(
                "userName",
                "firstName", 
                "lastName", 
                string.Empty, 
                "testemail@example.com", 
                "t2stP@ssw0rd!");

            // Assert
            Assert.That(sut, Is.InstanceOf<RegisterRequest>());
        }
    }
}
