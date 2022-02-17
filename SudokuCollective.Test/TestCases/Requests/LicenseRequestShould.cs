using NUnit.Framework;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Data.Models.Requests;

namespace SudokuCollective.Test.TestCases.Requests
{
    public class LicenseRequestShould
    {
        private ILicenseRequest sut;

        [SetUp]
        public void Setup()
        {
            sut = new LicenseRequest();
        }

        [Test, Category("Requests")]
        public void HasRequiredProperties()
        {
            // Arrange and Act

            // Assert
            Assert.That(sut.Name, Is.InstanceOf<string>());
            Assert.That(sut.OwnerId, Is.InstanceOf<int>());
            Assert.That(sut.LocalUrl, Is.InstanceOf<string>());
            Assert.That(sut.DevUrl, Is.InstanceOf<string>());
            Assert.That(sut.QaUrl, Is.InstanceOf<string>());
            Assert.That(sut.ProdUrl, Is.InstanceOf<string>());
        }

        [Test, Category("Requests")]
        public void HaveADefaultConstructor()
        {
            // Arrange and Act
            sut = new LicenseRequest();

            // Assert
            Assert.That(sut, Is.InstanceOf<LicenseRequest>());
        }

        [Test, Category("Requests")]
        public void HaveAConstructorThatAcceptsIntsForEnums()
        {
            // Arrange and Act
            sut = new LicenseRequest(
                "name",
                0,
                "localUrl",
                "devUrl",
                "qaUrl",
                "prodUrl");

            // Assert
            Assert.That(sut, Is.InstanceOf<LicenseRequest>());
        }
    }
}
