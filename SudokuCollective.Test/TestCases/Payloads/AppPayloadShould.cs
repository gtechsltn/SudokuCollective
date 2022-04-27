using NUnit.Framework;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads;
using SudokuCollective.Data.Models.Payloads;

namespace SudokuCollective.Test.TestCases.Payloads
{
    public class AppPayloadShould
    {
        private IAppPayload sut;

        [SetUp]
        public void Setup()
        {
            sut = new AppPayload();
        }

        [Test, Category("Payloads")]
        public void HasRequiredProperties()
        {
            // Arrange and Act
            
            // Assert
            Assert.That(sut.Name, Is.InstanceOf<string>());
            Assert.That(sut.LocalUrl, Is.InstanceOf<string>());
            Assert.That(sut.StagingUrl, Is.InstanceOf<string>());
            Assert.That(sut.QaUrl, Is.InstanceOf<string>());
            Assert.That(sut.ProdUrl, Is.InstanceOf<string>());
            Assert.That(sut.IsActive, Is.InstanceOf<bool>());
            Assert.That(sut.Environment, Is.InstanceOf<ReleaseEnvironment>());
            Assert.That(sut.PermitSuperUserAccess, Is.InstanceOf<bool>());
            Assert.That(sut.PermitCollectiveLogins, Is.InstanceOf<bool>());
            Assert.That(sut.DisableCustomUrls, Is.InstanceOf<bool>());
            Assert.That(sut.CustomEmailConfirmationAction, Is.InstanceOf<string>());
            Assert.That(sut.CustomPasswordResetAction, Is.InstanceOf<string>());
            Assert.That(sut.TimeFrame, Is.InstanceOf<TimeFrame>());
            Assert.That(sut.AccessDuration, Is.InstanceOf<int>());
        }

        [Test, Category("Payloads")]
        public void HaveADefaultConstructor()
        {
            // Arrange and Act
            sut = new AppPayload();

            // Assert
            Assert.That(sut, Is.InstanceOf<AppPayload>());
        }

        [Test, Category("Payloads")]
        public void HaveAConstructorThatAcceptsIntsForEnums()
        {
            // Arrange and Act
            sut = new AppPayload(
                "name",
                "http://localhost:8080",
                "https://example-dev.com",
                "https://example-qa.com",
                "https://www.example.com",
                true,
                1,
                true,
                false,
                false,
                "customEmailConfirmationAction",
                "customPasswordResetAction",
                4,
                1);

            // Assert
            Assert.That(sut, Is.InstanceOf<AppPayload>());
        }

        [Test, Category("Payloads")]
        public void HaveAConstructorThatAcceptsEnums()
        {
            // Arrange and Act
            sut = new AppPayload(
                "name",
                "http://localhost:8080",
                "https://example-dev.com",
                "https://example-qa.com",
                "https://www.example.com",
                true,
                ReleaseEnvironment.LOCAL,
                true,
                false,
                false,
                "customEmailConfirmationAction",
                "customPasswordResetAction",
                TimeFrame.DAYS,
                1);

            // Assert
            Assert.That(sut, Is.InstanceOf<AppPayload>());
        }
    }
}
