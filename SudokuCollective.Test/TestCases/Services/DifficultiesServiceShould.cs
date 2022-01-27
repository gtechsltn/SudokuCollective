﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Services;
using SudokuCollective.Test.TestData;
using SudokuCollective.Test.Repositories;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Requests;

namespace SudokuCollective.Test.TestCases.Services
{
    public class DifficultiesServiceShould
    {
        private DatabaseContext context;
        private MockedDifficultiesRepository mockedDifficultiesRepository;
        private MemoryDistributedCache memoryCache;
        private IDifficultiesService sut;
        private IDifficultiesService sutCreateDifficulty;
        private string license;
        private Paginator paginator;

        [SetUp]
        public async Task Setup()
        {
            context = await TestDatabase.GetDatabaseContext();
            mockedDifficultiesRepository = new MockedDifficultiesRepository(context);
            memoryCache = new MemoryDistributedCache(
                Options.Create(new MemoryDistributedCacheOptions()));

            sut = new DifficultiesService(
                mockedDifficultiesRepository.SuccessfulRequest.Object,
                memoryCache);
            sutCreateDifficulty = new DifficultiesService(
                mockedDifficultiesRepository.CreateDifficultyRequest.Object,
                memoryCache);
            license = TestObjects.GetLicense();
            paginator = TestObjects.GetPaginator();
        }

        [Test]
        [Category("Services")]
        public async Task GetADifficulty()
        {
            // Arrange

            // Act
            var result = await sut.Get(1);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Difficulty Found"));
            Assert.That((Difficulty)result.DataPacket[0], Is.TypeOf<Difficulty>());
        }

        [Test]
        [Category("Services")]
        public async Task GetDifficulties()
        {
            // Arrange

            // Act
            var result = await sut.GetDifficulties();

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Difficulties Found"));
            Assert.That(result.DataPacket.Count, Is.EqualTo(4));
        }

        [Test]
        [Category("Services")]
        public async Task GetDifficultiesWithoutNullOrTestDifficultyLevel()
        {
            // Arrange

            // Act
            var result = await sut.GetDifficulties();
            var nullAndTestDifficultyLevelsBlocked = result.DataPacket
                .ConvertAll(d => (Difficulty)d)
                .Any(difficulty =>
                    difficulty.DifficultyLevel.Equals(DifficultyLevel.NULL)
                    || difficulty.DifficultyLevel.Equals(DifficultyLevel.TEST));

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Difficulties Found"));
            Assert.That(nullAndTestDifficultyLevelsBlocked, Is.False);
        }

        [Test]
        [Category("Services")]
        public async Task UpdateADifficulty()
        {
            // Arrange
            var request = new Request
            {
                License = license,
                RequestorId = 1,
                Paginator = paginator,
                DataPacket = new UpdateDifficultyRequest()
                {
                    Id = 1,
                    Name = "Medium UPDATED!",
                }
            };

            // Act
            var result = await sut.Update(1, request);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Difficulty Updated"));
            Assert.That(((Difficulty)result.DataPacket[0]).Name, Is.EqualTo("Medium UPDATED!"));
        }

        [Test]
        [Category("Services")]
        public async Task DeleteADifficulty()
        {
            // Arrange

            // Act
            var result = await sut.Delete(1);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Difficulty Deleted"));
        }

        [Test]
        [Category("Services")]
        public async Task CreateADifficulty()
        {
            // Arrange
            var request = new Request
            {
                License = license,
                RequestorId = 1,
                Paginator = new Paginator(),
                DataPacket = new CreateDifficultyRequest()
                {
                    Name = "new-difficulty",
                    DisplayName = "New Difficulty",
                    DifficultyLevel = DifficultyLevel.NULL
                }
            };

            // Act
            var result = await sutCreateDifficulty.Create(request);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Difficulty Created"));
            Assert.That((Difficulty)result.DataPacket[0], Is.InstanceOf<Difficulty>());
        }
    }
}