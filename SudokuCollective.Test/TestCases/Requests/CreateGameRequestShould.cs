using NUnit.Framework;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Data.Models.Requests;

namespace SudokuCollective.Test.TestCases.Requests
{
    public class CreateGameRequestShould
    {
        private ICreateGameRequest sut;

        [SetUp]
        public void Setup()
        {
            sut = new CreateGameRequest();
        }

        [Test, Category("Requests")]
        public void HasRequiredProperties()
        {
            // Arrange and Act
            
            // Assert
            Assert.That(sut.UserId, Is.InstanceOf<int>());
            Assert.That(sut.DifficultyId, Is.InstanceOf<int>());
        }
    }
}