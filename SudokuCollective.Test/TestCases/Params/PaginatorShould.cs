using NUnit.Framework;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.DataModels;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Data.Models.Params;

namespace SudokuCollective.Test.TestCases.Params
{
    public class PaginatorShould
    {
        private IPaginator sut;

        [SetUp]
        public void Setup()
        {
            sut = new Paginator();
        }

        [Test, Category("Params")]
        public void HasRequiredProperties()
        {
            // Arrange and Act
            
            // Assert
            Assert.That(sut.Page, Is.InstanceOf<int>());
            Assert.That(sut.ItemsPerPage, Is.InstanceOf<int>());
            Assert.That(sut.SortBy, Is.InstanceOf<SortValue>());
            Assert.That(sut.OrderByDescending, Is.InstanceOf<bool>());
            Assert.That(sut.IncludeCompletedGames, Is.InstanceOf<bool>());
        }

        [Test, Category("Params")]
        public void IsNullReturnsFalseIfSortValueIsSet()
        {
            // Arrange
            sut.SortBy = SortValue.ID;

            // Act
            var result = sut.IsNull();
            
            // Assert
            Assert.That(sut.SortBy, Is.Not.EqualTo(SortValue.NULL));
            Assert.That(result, Is.False);
        }

        [Test, Category("Params")]
        public void IsNullReturnsTrueIfSortValueIsNotSet()
        {
            // Arrange and Act
            var result = sut.IsNull();
            
            // Assert
            Assert.That(sut.SortBy, Is.EqualTo(SortValue.NULL));
            Assert.That(result, Is.True);
        }
    }
}