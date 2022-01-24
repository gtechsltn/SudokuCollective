using NUnit.Framework;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Data.Models.Requests;

namespace SudokuCollective.Test.TestCases.Requests
{
    public class UpdateDifficultyRequestShould
    {
        private IUpdateDifficultyRequest sut;

        [SetUp]
        public void Setup()
        {
            sut = new UpdateDifficultyRequest();
        }

        [Test, Category("Requests")]
        public void HasRequiredProperties()
        {
            // Arrange and Act
            
            // Assert
            Assert.That(sut.Id, Is.InstanceOf<int>());
            Assert.That(sut.Name, Is.InstanceOf<string>());
            Assert.That(sut.DisplayName, Is.InstanceOf<string>());
        }

        [Test, Category("Requests")]
        public void HaveADefaultConstructor()
        {
            // Arrange and Act
            sut = new UpdateDifficultyRequest();

            // Assert
            Assert.That(sut, Is.InstanceOf<UpdateDifficultyRequest>());
        }

        [Test, Category("Requests")]
        public void HaveAConstructorThatAcceptsParams()
        {
            // Arrange and Act
            sut = new UpdateDifficultyRequest(
                1,
                "name",
                "displayName");

            // Assert
            Assert.That(sut, Is.InstanceOf<UpdateDifficultyRequest>());
        }
    }
}
