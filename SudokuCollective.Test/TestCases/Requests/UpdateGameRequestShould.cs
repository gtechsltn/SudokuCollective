using System.Collections.Generic;
using NUnit.Framework;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Data.Models.Requests;
using SudokuCollective.Test.TestData;

namespace SudokuCollective.Test.TestCases.Requests
{
    public class UpdateGameRequestShould
    {
        private IUpdateGameRequest sut;

        [SetUp]
        public void Setup()
        {
            sut = new UpdateGameRequest();
        }

        [Test, Category("Requests")]
        public void HasRequiredProperties()
        {
            // Arrange and Act

            // Assert
            Assert.That(sut.GameId, Is.InstanceOf<int>());
            Assert.That(sut.SudokuCells, Is.InstanceOf<List<ISudokuCell>>());
        }

        [Test, Category("Requests")]
        public void AcceptsValidSudokuCells()
        {
            // Arrange and Act
            sut.SudokuCells = TestObjects.GetValidSudokuCells().ConvertAll(c => (ISudokuCell)c);

            // Assert
            Assert.That(sut.SudokuCells, Is.InstanceOf<List<ISudokuCell>>());
            Assert.That(sut.SudokuCells.Count, Is.EqualTo(81));
        }
    }
}
