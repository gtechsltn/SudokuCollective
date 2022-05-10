using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Jobs;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Services;
using SudokuCollective.Test.Repositories;
using SudokuCollective.Test.TestData;

namespace SudokuCollective.Test.TestCases.Jobs
{
    public class DataJobsShould
    {
        private DatabaseContext context;
        private MockedSolutionsRepository mockedSolutionsRepository;
        private Game game;
        private Mock<ILogger<GamesService>> mockedLogger;
        private EventId eventId;

        [SetUp]
        public async Task Setup()
        {
            context = await TestDatabase.GetDatabaseContext();
            
            mockedSolutionsRepository = new MockedSolutionsRepository(context);
            game = new Game();
            game.SudokuMatrix.GenerateSolution();
            mockedLogger = new Mock<ILogger<GamesService>>();
            eventId = new EventId(401, "Hangfire Event Warning");
        }
        
        [Test, Category("Services")]
        public void AddSolutions()
        {
            try
            {
                // Arrange and Assert
                DataJobs.AddSolutionJob(
                    mockedSolutionsRepository.SuccessfulRequest.Object, 
                    game.SudokuSolution, 
                    mockedLogger.Object, 
                    eventId);

                Assert.That(true);
            }
            catch
            {
                Assert.That(false);
            }
        }
    }
}