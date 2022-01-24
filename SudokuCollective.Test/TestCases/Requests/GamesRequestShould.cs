using NUnit.Framework;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Data.Models.Requests;

namespace SudokuCollective.Test.TestCases.Requests
{
    public class GamesRequestShould
    {
        private IGamesRequest sut;

        [SetUp]
        public void Setup()
        {
            sut = new GamesRequest();
        }

        [Test, Category("Requests")]
        public void HasRequiredProperties()
        {
            // Arrange and Act
            
            // Assert
            Assert.That(sut.UserId, Is.InstanceOf<int>());
        }

        [Test, Category("Requests")]
        public void HasADefaultConstructor()
        {
            // Arrange and Act
            sut = new GamesRequest();

            // Assert
            Assert.That(sut, Is.InstanceOf<GamesRequest>());
        }

        [Test, Category("Requests")]
        public void HasAConstructorThatAcceptsParams()
        {
            // Arrange and Act
            sut = new GamesRequest(1);

            // Assert
            Assert.That(sut, Is.InstanceOf<GamesRequest>());
        }
    }
}
