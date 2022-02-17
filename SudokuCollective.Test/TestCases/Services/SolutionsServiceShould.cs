using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using SudokuCollective.Cache;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Requests;
using SudokuCollective.Data.Services;
using SudokuCollective.Test.Cache;
using SudokuCollective.Test.Repositories;
using SudokuCollective.Test.TestData;

namespace SudokuCollective.Test.TestCases.Services
{
    public class SolutionsServiceShould
    {
        private DatabaseContext context;
        private MockedSolutionsRepository mockedSolutionsRepository;
        private MockedCacheService mockedCacheService;
        private MemoryDistributedCache memoryCache;
        private ISolutionsService sut;
        private ISolutionsService sutFailure;
        private Request request;

        [SetUp]
        public async Task Setup()
        {
            context = await TestDatabase.GetDatabaseContext();
            mockedSolutionsRepository = new MockedSolutionsRepository(context);
            mockedCacheService = new MockedCacheService(context);
            memoryCache = new MemoryDistributedCache(
                Options.Create(new MemoryDistributedCacheOptions()));

            sut = new SolutionsService(
                mockedSolutionsRepository.SuccessfulRequest.Object,
                memoryCache,
                mockedCacheService.SuccessfulRequest.Object,
                new CacheKeys());

            sutFailure = new SolutionsService(
                mockedSolutionsRepository.FailedRequest.Object,
                memoryCache,
                mockedCacheService.FailedRequest.Object,
                new CacheKeys());

            request = TestObjects.GetRequest();
        }

        [Test, Category("Services")]
        public async Task GetASolution()
        {
            // Arrange

            // Act
            var result = await sut.Get(1);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Solution Found"));
            Assert.That((SudokuSolution)result.Payload[0], Is.TypeOf<SudokuSolution>());
        }

        [Test, Category("Services")]
        public async Task IssueMessageIfGetSolutionFails()
        {
            // Arrange

            // Act
            var result = await sutFailure.Get(1);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Solution not Found"));
        }

        [Test, Category("Services")]
        public async Task GetSolutions()
        {
            // Arrange

            // Act
            var result = await sut.GetSolutions(request);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Solutions Found"));
            Assert.That(result.Payload.ConvertAll(s => (ISudokuSolution)s), Is.TypeOf<List<ISudokuSolution>>());
        }

        [Test, Category("Services")]
        public async Task IssueMessageIfGetSolutionsFails()
        {
            // Arrange

            // Act
            var result = await sutFailure.GetSolutions(request);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Solutions not Found"));
        }

        [Test, Category("Services")]
        public async Task SolveSudokuMatrices()
        {
            // Arrange
            var solutionRequest = new SolutionRequest()
            {
                FirstRow = new List<int> { 0, 2, 0, 5, 0, 0, 8, 7, 6 },
                SecondRow = new List<int> { 7, 0, 0, 1, 8, 0, 0, 5, 0 },
                ThirdRow = new List<int> { 8, 5, 9, 7, 0, 0, 0, 4, 0 },
                FourthRow = new List<int> { 5, 9, 0, 0, 0, 4, 6, 8, 1 },
                FifthRow = new List<int> { 0, 1, 0, 0, 3, 0, 0, 0, 0 },
                SixthRow = new List<int> { 0, 0, 0, 8, 6, 0, 0, 9, 5 },
                SeventhRow = new List<int> { 2, 0, 7, 0, 0, 8, 0, 0, 9 },
                EighthRow = new List<int> { 9, 0, 4, 0, 0, 7, 2, 0, 8 },
                NinthRow = new List<int> { 0, 0, 0, 0, 0, 2, 4, 6, 0 }
            };
            request.Payload = solutionRequest;

            // Act
            var result = await sut.Solve(request);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Sudoku Solution Found"));
            Assert.That((SudokuSolution)result.Payload[0], Is.TypeOf<SudokuSolution>());
        }

        [Test, Category("Services")]
        public async Task GenerateASolution()
        {
            // Arrange

            // Act
            var result = await sut.Generate();

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Solution Generated"));
            Assert.That((SudokuSolution)result.Payload[0], Is.TypeOf<SudokuSolution>());
        }

        [Test, Category("Services")]
        public async Task AddSolutions()
        {
            // Arrange

            // Act
            var result = await sut.Add(10);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Solutions Added"));
        }

        [Test, Category("Services")]
        public async Task IssueMessageIfAddSolutionsFails()
        {
            // Arrange

            // Act
            var result = await sutFailure.Add(10);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Solutions not Added"));
        }
    }
}
