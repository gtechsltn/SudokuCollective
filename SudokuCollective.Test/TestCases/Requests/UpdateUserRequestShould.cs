using System;
using NUnit.Framework;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Data.Models.Requests;

namespace SudokuCollective.Test.TestCases.Requests
{
    public class UpdateUserRequestShould
    {
        private IUpdateUserRequest sut;

        [SetUp]
        public void Setup()
        {
            sut = new UpdateUserRequest();
        }

        [Test, Category("Requests")]
        public void HasRequiredProperties()
        {
            // Arrange and Act

            // Assert
            Assert.That(sut.UserName, Is.InstanceOf<string>());
            Assert.That(sut.FirstName, Is.InstanceOf<string>());
            Assert.That(sut.LastName, Is.InstanceOf<string>());
            Assert.That(sut.NickName, Is.InstanceOf<string>());
            Assert.That(sut.Email, Is.InstanceOf<string>());
        }

        [Test, Category("Requests")]
        public void HasADefaultConstructor()
        {
            // Arrange and Act
            sut = new UpdateUserRequest();

            // Assert
            Assert.That(sut, Is.InstanceOf<UpdateUserRequest>());
        }

        [Test, Category("Requests")]
        public void HasAConstructorThatAcceptsParams()
        {
            // Arrange and Act
            sut = new UpdateUserRequest(
                "UserName",
                "FirstName",
                "LastName",
                "NickName",
                "testEmail@example.com");

            // Assert
            Assert.That(sut, Is.InstanceOf<UpdateUserRequest>());
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
