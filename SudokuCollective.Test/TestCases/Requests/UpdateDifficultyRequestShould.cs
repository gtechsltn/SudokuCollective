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
    }
}
