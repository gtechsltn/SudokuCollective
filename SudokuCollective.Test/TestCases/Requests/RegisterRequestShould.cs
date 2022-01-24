using NUnit.Framework;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Data.Models.Requests;

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
        public void RejectInvalidUserNames()
        {
            // Arrange and Act
            sut.UserName = "joe";

            // Assert
            Assert.That(sut.UserName, Is.EqualTo(string.Empty));
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
        public void RejectInvalidPassword()
        {
            // Arrange and Act
            sut.Password = "password";

            // Assert
            Assert.That(sut.Password, Is.EqualTo(string.Empty));
        }
    }
}
