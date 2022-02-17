using System;
using NUnit.Framework;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads;
using SudokuCollective.Data.Models.Payloads;
using SudokuCollective.Test.TestData;

namespace SudokuCollective.Test.TestCases.Payloads
{
    public class ResetPasswordPayloadShould
    {
        private IResetPasswordPayload sut;

        [SetUp]
        public void Setup()
        {
            sut = new ResetPasswordPayload();
        }

        [Test, Category("Payloads")]
        public void HasRequiredProperties()
        {
            // Arrange and Act

            // Assert
            Assert.That(sut.Token, Is.InstanceOf<string>());
            Assert.That(sut.NewPassword, Is.InstanceOf<string>());
        }

        [Test, Category("Payloads")]
        public void HasADefaultConstructor()
        {
            // Arrange and Act
            sut = new ResetPasswordPayload();

            // Assert
            Assert.That(sut, Is.InstanceOf<ResetPasswordPayload>());
        }

        [Test, Category("Payloads")]
        public void HasAConstructorThatAcceptsParams()
        {
            // Arrange and Act
            sut = new ResetPasswordPayload(TestObjects.GetToken(), "T3stPass0rd?1");

            // Assert
            Assert.That(sut, Is.InstanceOf<ResetPasswordPayload>());
        }

        [Test, Category("Payloads")]
        public void ThrowExceptionForInvalidLicenses()
        {
            // Arrange

            // Act and Assert
            Assert.Throws<ArgumentException>(() => sut.Token = "InvalidLicense");
        }

        [Test, Category("Payloads")]
        public void ThrowExceptionForInvalidEmails()
        {
            // Arrange

            // Act and Assert
            Assert.Throws<ArgumentException>(() => sut.NewPassword = "invalidpassword");
        }
    }
}
