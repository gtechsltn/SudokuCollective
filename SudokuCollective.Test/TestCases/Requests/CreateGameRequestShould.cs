using NUnit.Framework;
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

        [Test, Category("Requests")]
        public void HasADefaultConstructor()
        {
            // Arrange and Act
            sut = new CreateGameRequest();

            // Assert
            Assert.That(sut, Is.InstanceOf<CreateGameRequest>());
        }

        [Test, Category("Requests")]
        public void HasAConstructorThatAcceptsParams()
        {
            // Arrange and Act
            sut = new CreateGameRequest(1, 2);

            // Assert
            Assert.That(sut, Is.InstanceOf<CreateGameRequest>());
        }
    }
}
