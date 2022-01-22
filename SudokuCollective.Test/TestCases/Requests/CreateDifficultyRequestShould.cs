using NUnit.Framework;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Data.Models.Requests;

namespace SudokuCollective.Test.TestCases.Requests
{
    public class CreateDifficultyRequestShould
    {
        private ICreateDifficultyRequest sut;

        [SetUp]
        public void Setup()
        {
            sut = new CreateDifficultyRequest();
        }

        [Test, Category("Requests")]
        public void HasRequiredProperties()
        {
            // Arrange and Act
            
            // Assert
            Assert.That(sut.Name, Is.InstanceOf<string>());
            Assert.That(sut.DisplayName, Is.InstanceOf<string>());
            Assert.That(sut.DifficultyLevel, Is.InstanceOf<DifficultyLevel>());
        }
    }
}
