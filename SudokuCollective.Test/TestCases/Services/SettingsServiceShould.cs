using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using SudokuCollective.Cache;
using SudokuCollective.Core.Interfaces.Models;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Settings;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Services;
using SudokuCollective.Test.Cache;
using SudokuCollective.Test.Repositories;
using SudokuCollective.Test.TestData;

namespace SudokuCollective.Test.TestCases.Services
{
    public class SettingsServiceShould
    {
        private SettingsService sut;
        private DatabaseContext context;
        private MockedDifficultiesRepository mockedDifficultiesRepository;
        private MockedCacheService mockedCacheService;
        private MemoryDistributedCache memoryCache;

        [SetUp]
        public async Task Setup()
        {
            context = await TestDatabase.GetDatabaseContext();

            mockedDifficultiesRepository = new MockedDifficultiesRepository(context);
            mockedCacheService = new MockedCacheService(context);
            memoryCache = new MemoryDistributedCache(
                Options.Create(new MemoryDistributedCacheOptions()));

            sut = new SettingsService(mockedDifficultiesRepository.SuccessfulRequest.Object,
                memoryCache,
                mockedCacheService.SuccessfulRequest.Object,
                new CacheKeys(),
                new CachingStrategy()
            );
        }

        public async Task GetSettings()
        {
            // Arrange and Act
            var result = await sut.GetAsync();
            var settings = (ISettings)result.Payload[0];

            Assert.That(result, Is.InstanceOf<IResult>());
            Assert.That(settings.Difficulties, Is.InstanceOf<List<IDifficulty>>());
            Assert.That(settings.ReleaseEnvironments, Is.InstanceOf<List<IEnumListItem>>());
            Assert.That(settings.SortValues, Is.InstanceOf<List<IEnumListItem>>());
            Assert.That(settings.TimeFrames, Is.InstanceOf<List<IEnumListItem>>());
        }

        [Test, Category("Services")]
        public void ReturnListOfReleaseEnvironments()
        {
            // Arrange and Act
            var result = sut.GetReleaseEnvironments();
            var releaseEnvironments = result.Payload[0] as List<IEnumListItem>;

            // Assert
            Assert.That(result, Is.InstanceOf<IResult>());
            Assert.That(releaseEnvironments.Count, Is.EqualTo(4));
        }

        [Test, Category("Services")]
        public void ReturnListOfSortValues()
        {
            // Arrange and Act
            var result = sut.GetSortValues();
            var sortValues = result.Payload[0] as List<IEnumListItem>;

            // Assert
            Assert.That(result, Is.InstanceOf<IResult>());
            Assert.That(sortValues.Count, Is.EqualTo(14));
        }

        [Test, Category("Services")]
        public void ReturnListOfTimeFrames()
        {
            // Arrange and Act
            var result = sut.GetTimeFrames();
            var timeFrames = result.Payload[0] as List<IEnumListItem>;

            // Assert
            Assert.That(result, Is.InstanceOf<IResult>());
            Assert.That(timeFrames.Count, Is.EqualTo(6));
        }
    }
}
