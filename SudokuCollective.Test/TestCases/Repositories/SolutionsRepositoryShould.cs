using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models;
using SudokuCollective.Repos;
using SudokuCollective.Test.TestData;

namespace SudokuCollective.Test.TestCases.Repositories
{
    public class SolutionsRepositoryShould
    {
        private DatabaseContext context;
        private ISolutionsRepository<SudokuSolution> sut;
        private SudokuSolution newSolution;

        [SetUp]
        public async Task Setup()
        {
            context = await TestDatabase.GetDatabaseContext();
            sut = new SolutionsRepository<SudokuSolution>(context);

            newSolution = new SudokuSolution();
        }

        [Test, Category("Repository")]
        public async Task CreateSolutions()
        {
            // Arrange

            // Act
            var result = await sut.Add(newSolution);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That((SudokuSolution)result.Object, Is.InstanceOf<SudokuSolution>());
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfCreateSolutionsFails()
        {
            // Arrange
            newSolution.Id = 10;

            // Act
            var result = await sut.Add(newSolution);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
        }

        [Test, Category("Repository")]
        public async Task GetSolutionsById()
        {
            // Arrange

            // Act
            var result = await sut.Get(1);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That((SudokuSolution)result.Object, Is.InstanceOf<SudokuSolution>());
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfGetByIdFails()
        {
            // Arrange

            // Act
            var result = await sut.Get(5);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Object, Is.Null);
        }

        [Test, Category("Repository")]
        public async Task GetAllSolutions()
        {
            // Arrange

            // Act
            var result = await sut.GetAll();

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Objects.ConvertAll(s => (SudokuSolution)s), Is.InstanceOf<List<SudokuSolution>>());
        }

        [Test, Category("Repository")]
        public async Task AddSolutions()
        {
            // Arrange
            var solutions = new List<SudokuSolution>();
            solutions.Add(newSolution);

            // Act
            var result = await sut.AddSolutions(solutions.ConvertAll(s => (ISudokuSolution)s));

            // Assert
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test, Category("Repository")]
        public async Task GetSolvedSolutions()
        {
            // Arrange

            // Act
            var result = await sut.GetSolvedSolutions();

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Objects.ConvertAll(s => (SudokuSolution)s), Is.InstanceOf<List<SudokuSolution>>());
        }
    }
}
