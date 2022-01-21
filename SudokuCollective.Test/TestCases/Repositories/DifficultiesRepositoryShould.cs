using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Repositories;
using SudokuCollective.Test.TestData;

namespace SudokuCollective.Test.TestCases.Repositories
{
    public class DifficultiesRepositoryShould
    {
        private DatabaseContext context;
        private IDifficultiesRepository<Difficulty> sut;
        private Difficulty newDifficutly;

        [SetUp]
        public async Task Setup()
        {
            context = await TestDatabase.GetDatabaseContext();
            sut = new DifficultiesRepository<Difficulty>(context);

            newDifficutly = new Difficulty()
            {
                Name = "New Test",
                DisplayName = "New Test",
                DifficultyLevel = DifficultyLevel.TEST
            };
        }

        [Test, Category("Repository")]
        public async Task CreateDifficulties()
        {
            // Arrange
            var testDifficulty = context.Difficulties.FirstOrDefault(d => d.DifficultyLevel == DifficultyLevel.TEST);
            context.Difficulties.Remove(testDifficulty);
            context.SaveChanges();

            // Act
            var result = await sut.Add(newDifficutly);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That((Difficulty)result.Object, Is.InstanceOf<Difficulty>());
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfCreateDifficutliesFails()
        {
            // Arrange and Act
            var result = await sut.Add(newDifficutly);

            // Assert
            Assert.That(result.Success, Is.False);
        }

        [Test, Category("Repository")]
        public async Task GetDifficultiesById()
        {
            // Arrange and Act
            var result = await sut.Get(1);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That((Difficulty)result.Object, Is.InstanceOf<Difficulty>());
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfGetByIdFails()
        {
            // Arrange and Act
            var result = await sut.Get(7);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Object, Is.Null);
        }

        [Test, Category("Repository")]
        public async Task GetAllDifficulties()
        {
            // Arrange and Act
            var result = await sut.GetAll();

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Objects.ConvertAll(d => (Difficulty)d), Is.InstanceOf<List<Difficulty>>());
        }

        [Test, Category("Repository")]
        public async Task UpdateDifficulties()
        {
            // Arrange
            var difficulty = context.Difficulties.FirstOrDefault(d => d.Id == 1);
            difficulty.Name = string.Format("{0} UPDATED!", difficulty.Name);

            // Act
            var result = await sut.Update(difficulty);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Object, Is.InstanceOf<Difficulty>());
            Assert.That(((Difficulty)result.Object).Name, Is.EqualTo(difficulty.Name));
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfUpdateDifficultiesFails()
        {
            // Arrange and Act
            var result = await sut.Update(newDifficutly);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Object, Is.Null);
        }

        [Test, Category("Repository")]
        public async Task DeleteDifficulties()
        {
            // Arrange
            var difficulty = context.Difficulties.FirstOrDefault(d => d.Id == 1);

            // Act
            var result = await sut.Delete(difficulty);

            // Assert
            Assert.That(result.Success, Is.True);
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfDeleteDifficultiesFails()
        {
            // Arrange and Act
            var result = await sut.Delete(newDifficutly);

            // Assert
            Assert.That(result.Success, Is.False);
        }

        [Test, Category("Repository")]
        public async Task ConfirmItHasAnDifficulty()
        {
            // Arrange and Act
            var result = await sut.HasEntity(1);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfConfirmItHasAnDifficultyFails()
        {
            // Arrange
            var id = context
                .Difficulties
                .ToList()
                .OrderBy(d => d.Id)
                .Last<Difficulty>()
                .Id + 1;

            // Act
            var result = await sut.HasEntity(id);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test, Category("Repository")]
        public async Task ConfirmItHasAnDifficultyLevel()
        {
            // Arrange and Act
            var result = await sut.HasDifficultyLevel(DifficultyLevel.TEST);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test, Category("Repository")]
        public async Task ReturnFalseIfConfirmItHasAnDifficultyLevelFails()
        {
            // Arrange
            var testDifficulty = context.Difficulties.FirstOrDefault(d => d.DifficultyLevel == DifficultyLevel.TEST);
            context.Difficulties.Remove(testDifficulty);
            context.SaveChanges();

            // Act
            var result = await sut.HasDifficultyLevel(DifficultyLevel.TEST);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}
