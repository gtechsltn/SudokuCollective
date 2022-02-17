using System;
using System.Collections.Generic;
using NUnit.Framework;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models.Payloads;
using SudokuCollective.Test.TestData;

namespace SudokuCollective.Test.TestCases.Payloads
{
    public class UpdateGamePayloadShould
    {
        private IUpdateGamePayload sut;

        [SetUp]
        public void Setup()
        {
            sut = new UpdateGamePayload();
        }

        [Test, Category("Payloads")]
        public void HasRequiredProperties()
        {
            // Arrange and Act

            // Assert
            Assert.That(sut.GameId, Is.InstanceOf<int>());
            Assert.That(sut.SudokuCells, Is.InstanceOf<List<SudokuCell>>());
        }

        [Test, Category("Payloads")]
        public void AcceptsValidSudokuCells()
        {
            // Arrange and Act
            sut.SudokuCells = TestObjects.GetValidSudokuCells();

            // Assert
            Assert.That(sut.SudokuCells, Is.InstanceOf<List<SudokuCell>>());
            Assert.That(sut.SudokuCells.Count, Is.EqualTo(81));
        }

        [Test, Category("Payloads")]
        public void HasADefaultConstructor()
        {
            // Arrange and Act
            sut = new UpdateGamePayload();

            // Assert
            Assert.That(sut, Is.InstanceOf<UpdateGamePayload>());
        }

        [Test, Category("Payloads")]
        public void HasAConstructorThatAcceptsISudokuCellArray()
        {
            // Arrange
            var sudokuCellArray = TestObjects
                .GetValidSudokuCells()
                .ToArray();

            // Act
            sut = new UpdateGamePayload(1, sudokuCellArray);

            // Assert
            Assert.That(sut, Is.InstanceOf<UpdateGamePayload>());
        }

        [Test, Category("Payloads")]
        public void HasAConstructorThatAcceptsISudokuCellList()
        {
            // Arrange
            var sudokuCellList = TestObjects
                .GetValidSudokuCells();

            // Act
            sut = new UpdateGamePayload(1, sudokuCellList);

            // Assert
            Assert.That(sut, Is.InstanceOf<UpdateGamePayload>());
        }

        [Test, Category("Payloads")]
        public void ThrowsAnExceptionIfSudokuCellsAreInvalid()
        {
            // Arrange
            var sudokuCellList = TestObjects
                .GetValidSudokuCells();

            sudokuCellList.Add(new SudokuCell());

            // Act and Assert
            Assert.Throws<ArgumentException>(() => 
                new UpdateGamePayload(1, sudokuCellList));
        }
    }
}
