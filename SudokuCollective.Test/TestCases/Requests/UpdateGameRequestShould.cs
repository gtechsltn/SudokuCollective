using System;
using System.Collections.Generic;
using NUnit.Framework;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Core.Models;
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
            Assert.That(sut.SudokuCells, Is.InstanceOf<List<SudokuCell>>());
        }

        [Test, Category("Requests")]
        public void AcceptsValidSudokuCells()
        {
            // Arrange and Act
            sut.SudokuCells = TestObjects.GetValidSudokuCells();

            // Assert
            Assert.That(sut.SudokuCells, Is.InstanceOf<List<SudokuCell>>());
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
                .GetValidSudokuCells();

            // Act
            sut = new UpdateGameRequest(1, sudokuCellList);

            // Assert
            Assert.That(sut, Is.InstanceOf<UpdateGameRequest>());
        }

        [Test, Category("Requests")]
        public void ThrowsAnExceptionIfSudokuCellsAreInvalid()
        {
            // Arrange
            var sudokuCellList = TestObjects
                .GetValidSudokuCells();

            sudokuCellList.Add(new SudokuCell());

            // Act and Assert
            Assert.Throws<ArgumentException>(() => 
                new UpdateGameRequest(1, sudokuCellList));
        }
    }
}
