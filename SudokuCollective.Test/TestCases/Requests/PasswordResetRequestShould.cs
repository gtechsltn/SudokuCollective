using System;
using NUnit.Framework;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Data.Models.Requests;

namespace SudokuCollective.Test.TestCases.Requests
{
    public class PasswordResetRequestShould
    {
        private IPasswordResetRequest sut;

        [SetUp]
        public void Setup()
        {
            sut = new PasswordResetRequest();
        }

        [Test, Category("Requests")]
        public void HasRequiredProperties()
        {
            // Arrange and Act

            // Assert
            Assert.That(sut.UserId, Is.InstanceOf<int>());
            Assert.That(sut.NewPassword, Is.InstanceOf<string>());
        }

        [Test, Category("Requests")]
        public void HasADefaultConstructor()
        {
            // Arrange and Act
            sut = new PasswordResetRequest();

            // Assert
            Assert.That(sut, Is.InstanceOf<PasswordResetRequest>());
        }

        [Test, Category("Requests")]
        public void HasAConstructorThatAcceptsParams()
        {
            // Arrange and Act
            sut = new PasswordResetRequest(1, "T3stPass0rd?1");

            // Assert
            Assert.That(sut, Is.InstanceOf<PasswordResetRequest>());
        }

        [Test, Category("Requests")]
        public void ThrowsExceptionForInvalidPasswords()
        {
            // Arrange

            // Act and Assert
            Assert.Throws<ArgumentException>(() => sut.NewPassword = "invalidpassword");
        }
    }
}
