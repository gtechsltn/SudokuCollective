using NUnit.Framework;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Data.Models.Requests;

namespace SudokuCollective.Test.TestCases.Requests
{
    public class AppRequestShould
    {
        private IAppRequest sut;

        [SetUp]
        public void Setup()
        {
            sut = new AppRequest();
        }

        [Test, Category("Requests")]
        public void HasRequiredProperties()
        {
            // Arrange and Act
            
            // Assert
            Assert.That(sut.Name, Is.InstanceOf<string>());
            Assert.That(sut.LocalUrl, Is.InstanceOf<string>());
            Assert.That(sut.DevUrl, Is.InstanceOf<string>());
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

        [Test, Category("Requests")]
        public void HaveADefaultConstructor()
        {
            // Arrange and Act
            sut = new AppRequest();

            // Assert
            Assert.That(sut, Is.InstanceOf<AppRequest>());
        }

        [Test, Category("Requests")]
        public void HaveAConstructorThatAcceptsIntsForEnums()
        {
            // Arrange and Act
            sut = new AppRequest(
                "name",
                "localUrl",
                "devUrl",
                "qaUrl",
                "prodUrl",
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
            Assert.That(sut, Is.InstanceOf<AppRequest>());
        }

        [Test, Category("Requests")]
        public void HaveAConstructorThatAcceptsEnums()
        {
            // Arrange and Act
            sut = new AppRequest(
                "name",
                "localUrl",
                "devUrl",
                "qaUrl",
                "prodUrl",
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
            Assert.That(sut, Is.InstanceOf<AppRequest>());
        }
    }
}
