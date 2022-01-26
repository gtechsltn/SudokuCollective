using NUnit.Framework;
using SudokuCollective.Core.Interfaces.Models.TokenModels;
using SudokuCollective.Data.Models.Authentication;

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
            sut = new TokenRequest("username", "password", "license");

            // Assert
            Assert.That(sut, Is.InstanceOf<TokenRequest>());
        }
    }
}
