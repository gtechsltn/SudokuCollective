using NUnit.Framework;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Data.Models.Requests;
using SudokuCollective.Test.TestData;

namespace SudokuCollective.Test.TestCases.Requests
{
    public class ResetPasswordRequestShould
    {
        private IResetPasswordRequest sut;

        [SetUp]
        public void Setup()
        {
            sut = new ResetPasswordRequest();
        }

        [Test, Category("Requests")]
        public void HasRequiredProperties()
        {
            // Arrange and Act

            // Assert
            Assert.That(sut.Token, Is.InstanceOf<string>());
            Assert.That(sut.NewPassword, Is.InstanceOf<string>());
        }

        [Test, Category("Requests")]
        public void HasADefaultConstructor()
        {
            // Arrange and Act
            sut = new ResetPasswordRequest();

            // Assert
            Assert.That(sut, Is.InstanceOf<ResetPasswordRequest>());
        }

        [Test, Category("Requests")]
        public void HasAConstructorThatAcceptsParams()
        {
            // Arrange and Act
            sut = new ResetPasswordRequest(TestObjects.GetLicense(), "T3stPass0rd?1");

            // Assert
            Assert.That(sut, Is.InstanceOf<ResetPasswordRequest>());
        }

        [Test, Category("Requests")]
        public void RejectInvalidLicenses()
        {
            // Arrange and Act
            sut.Token = "InvalidLicense";

            // Assert
            Assert.That(sut.Token, Is.EqualTo(string.Empty));
        }

        [Test, Category("Requests")]
        public void RejectInvalidEmails()
        {
            // Arrange and Act
            sut.NewPassword = "invalidpassword";

            // Assert
            Assert.That(sut.NewPassword, Is.EqualTo(string.Empty));
        }
    }
}
