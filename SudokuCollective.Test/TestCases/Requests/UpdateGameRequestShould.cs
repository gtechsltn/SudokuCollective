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

        [Test, Category("Requests")]
        public void HasADefaultConstructor()
        {
            // Arrange and Act
            sut = new UpdateGameRequest();

            // Assert
            Assert.That(sut, Is.InstanceOf<UpdateGameRequest>());
        }

        [Test, Category("Requests")]
        public void HasAConstructorThatAcceptsISudokuCellArray()
        {
            // Arrange
            var sudokuCellArray = TestObjects
                .GetValidSudokuCells()
                .ConvertAll(c => (ISudokuCell)c)
                .ToArray();

            // Act
            sut = new UpdateGameRequest(1, sudokuCellArray);

            // Assert
            Assert.That(sut, Is.InstanceOf<UpdateGameRequest>());
        }

        [Test, Category("Requests")]
        public void HasAConstructorThatAcceptsISudokuCellList()
        {
            // Arrange
            var sudokuCellList = TestObjects
                .GetValidSudokuCells()
                .ConvertAll(c => (ISudokuCell)c);

            // Act
            sut = new UpdateGameRequest(1, sudokuCellList);

            // Assert
            Assert.That(sut, Is.InstanceOf<UpdateGameRequest>());
        }
    }
}
