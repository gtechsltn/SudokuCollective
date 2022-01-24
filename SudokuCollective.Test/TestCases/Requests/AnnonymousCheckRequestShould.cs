using System.Collections.Generic;
using NUnit.Framework;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Data.Models.Requests;
using SudokuCollective.Test.TestData;

namespace SudokuCollective.Test.TestCases
{
    public class AnnonymousCheckRequestShould
    {
        private IAnnonymousCheckRequest sut;
        private IAnnonymousCheckRequest sutInvalid;

        [SetUp]
        public void Setup()
        {
            sut = TestObjects.GetValidAnnonymousCheckRequest();
            sutInvalid = TestObjects.GetInValidAnnonymousCheckRequest();
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
    }
}
