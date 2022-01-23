using NUnit.Framework;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Data.Models.Requests;

namespace SudokuCollective.Test.TestCases.Requests
{
    public class AnnonymousGameRequestShould
    {
        private IAnnonymousGameRequest sut;

        [SetUp]
        public void Setup()
        {
            sut = new AnnonymousGameRequest();
        }

        [Test, Category("Requests")]
        public void HasRequiredProperties()
        {
            // Arrange and Act
            
            // Assert
            Assert.That(sut.DifficultyLevel, Is.InstanceOf<DifficultyLevel>());
        }
    }
}
