using System.Collections.Generic;
using NUnit.Framework;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Data.Models.Requests;
using SudokuCollective.Test.TestData;

namespace SudokuCollective.Test.TestCases.Requests
{
    public class SolutionRequestShould
    {
        private ISolutionRequest sut;
        private ISolutionRequest sutInvalid;

        [SetUp]
        public void Setup()
        {
            sut = TestObjects.GetValidSolutionRequest();
            sutInvalid = TestObjects.GetInvalidSolutionRequest();
        }

        [Test, Category("Requests")]
        public void HasRequiredProperties()
        {
            // Arrange and Act

            // Assert
            Assert.That(sut.FirstRow, Is.InstanceOf<List<int>>());
            Assert.That(sut.SecondRow, Is.InstanceOf<List<int>>());
            Assert.That(sut.ThirdRow, Is.InstanceOf<List<int>>());
            Assert.That(sut.FourthRow, Is.InstanceOf<List<int>>());
            Assert.That(sut.FifthRow, Is.InstanceOf<List<int>>());
            Assert.That(sut.SixthRow, Is.InstanceOf<List<int>>());
            Assert.That(sut.SeventhRow, Is.InstanceOf<List<int>>());
            Assert.That(sut.EighthRow, Is.InstanceOf<List<int>>());
            Assert.That(sut.NinthRow, Is.InstanceOf<List<int>>());
        }

        [Test, Category("Requests")]
        public void OnlyAcceptsValidLists()
        {
            // Arrange and Act

            // Assert
            Assert.That(sut.FirstRow.Count, Is.EqualTo(9));
            Assert.That(sut.SecondRow.Count, Is.EqualTo(9));
            Assert.That(sut.ThirdRow.Count, Is.EqualTo(9));
            Assert.That(sut.FourthRow.Count, Is.EqualTo(9));
            Assert.That(sut.FifthRow.Count, Is.EqualTo(9));
            Assert.That(sut.SixthRow.Count, Is.EqualTo(9));
            Assert.That(sut.SeventhRow.Count, Is.EqualTo(9));
            Assert.That(sut.EighthRow.Count, Is.EqualTo(9));
            Assert.That(sut.NinthRow.Count, Is.EqualTo(9));
        }

        [Test, Category("Requests")]
        public void RejectsInvalidLists()
        {
            // Arrange and Act

            // Assert
            Assert.That(sutInvalid.FirstRow.Count, Is.EqualTo(0));
            Assert.That(sutInvalid.SecondRow.Count, Is.EqualTo(9));
            Assert.That(sutInvalid.ThirdRow.Count, Is.EqualTo(9));
            Assert.That(sutInvalid.FourthRow.Count, Is.EqualTo(9));
            Assert.That(sutInvalid.FifthRow.Count, Is.EqualTo(9));
            Assert.That(sutInvalid.SixthRow.Count, Is.EqualTo(9));
            Assert.That(sutInvalid.SeventhRow.Count, Is.EqualTo(9));
            Assert.That(sutInvalid.EighthRow.Count, Is.EqualTo(9));
            Assert.That(sutInvalid.NinthRow.Count, Is.EqualTo(9));
        }

        [Test, Category("Requests")]
        public void HasADefaultConstructor()
        {
            // Arrange and Act
            sut = new SolutionRequest();

            // Assert
            Assert.That(sut, Is.InstanceOf<SolutionRequest>());
        }

        [Test, Category("Requests")]
        public void HasAConstructorThatAcceptsIntArrays()
        {
            // Arrange and Act
            sut = new SolutionRequest(
                new int[9],
                new int[9],
                new int[9],
                new int[9],
                new int[9],
                new int[9],
                new int[9],
                new int[9],
                new int[9]);

            // Assert
            Assert.That(sut, Is.InstanceOf<SolutionRequest>());
        }

        [Test, Category("Requests")]
        public void HasAConstructorThatAcceptsIntLists()
        {
            // Arrange and Act
            sut = new SolutionRequest(
                new List<int> { 1, 7, 4, 6, 8, 2, 5, 3, 9 },
                new List<int> { 3, 9, 2, 1, 5, 7, 6, 4, 8 },
                new List<int> { 5, 6, 3, 4, 5, 6, 1, 7, 2 },
                new List<int> { 7, 8, 5, 4, 1, 3, 2, 9, 6 },
                new List<int> { 2, 1, 6, 5, 7, 9, 4, 8, 3 },
                new List<int> { 9, 4, 3, 8, 2, 6, 7, 5, 1 },
                new List<int> { 4, 5, 9, 2, 6, 8, 3, 1, 7 },
                new List<int> { 6, 3, 7, 9, 4, 1, 8, 2, 5 },
                new List<int> { 8, 2, 1, 7, 3, 5, 9, 6, 4 }); ;

            // Assert
            Assert.That(sut, Is.InstanceOf<SolutionRequest>());
        }
    }
}
