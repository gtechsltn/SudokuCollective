using NUnit.Framework;
using SudokuCollective.Core.Enums;
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
    }
}
