using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Requests;
using SudokuCollective.Data.Models.Results;
using SudokuCollective.Data.Services;
using SudokuCollective.Test.Repositories;
using SudokuCollective.Test.TestData;

namespace SudokuCollective.Test.TestCases.Services
{
    public class GamesServiceShould
    {
        private DatabaseContext context;
        private MockedGamesRepository mockedGamesRepository;
        private MockedAppsRepository mockedAppsRepository;
        private MockedUsersRepository mockedUsersRepository;
        private MockedDifficultiesRepository mockedDifficultiesRepositorySuccessful;
        private MockedDifficultiesRepository mockedDifficultiesRepositoryFailed;
        private MockedSolutionsRepository mockedSolutionsRepository;
        private MemoryDistributedCache memoryCache;
        private IGamesService sut;
        private IGamesService sutFailure;
        private IGamesService sutSolved;
        private IGamesService sutAnonFailure;
        private IGamesService sutUpdateFailure;
        private Request request;
        private CreateGameRequest createGameRequest;
        private UpdateGameRequest updateGameRequest;
        private GamesRequest gamesRequest;

        [SetUp]
        public async Task Setup()
        {
            context = await TestDatabase.GetDatabaseContext();

            mockedGamesRepository = new MockedGamesRepository(context);
            mockedAppsRepository = new MockedAppsRepository(context);
            mockedUsersRepository = new MockedUsersRepository(context);
            mockedDifficultiesRepositorySuccessful = new MockedDifficultiesRepository(context);
            mockedDifficultiesRepositoryFailed = new MockedDifficultiesRepository(context);
            mockedSolutionsRepository = new MockedSolutionsRepository(context);
            memoryCache = new MemoryDistributedCache(
                Options.Create(new MemoryDistributedCacheOptions()));

            sut = new GamesService(
                mockedGamesRepository.SuccessfulRequest.Object,
                mockedAppsRepository.SuccessfulRequest.Object,
                mockedUsersRepository.SuccessfulRequest.Object,
                mockedDifficultiesRepositorySuccessful.SuccessfulRequest.Object,
                mockedSolutionsRepository.SuccessfulRequest.Object,
                memoryCache);

            sutFailure = new GamesService(
                mockedGamesRepository.FailedRequest.Object,
                mockedAppsRepository.SuccessfulRequest.Object,
                mockedUsersRepository.SuccessfulRequest.Object,
                mockedDifficultiesRepositorySuccessful.SuccessfulRequest.Object,
                mockedSolutionsRepository.SuccessfulRequest.Object,
                memoryCache);

            sutSolved = new GamesService(
                mockedGamesRepository.SolvedRequest.Object,
                mockedAppsRepository.SuccessfulRequest.Object,
                mockedUsersRepository.SuccessfulRequest.Object,
                mockedDifficultiesRepositorySuccessful.SuccessfulRequest.Object,
                mockedSolutionsRepository.SuccessfulRequest.Object,
                memoryCache);

            sutAnonFailure = new GamesService(
                mockedGamesRepository.SuccessfulRequest.Object,
                mockedAppsRepository.SuccessfulRequest.Object,
                mockedUsersRepository.SuccessfulRequest.Object,
                mockedDifficultiesRepositoryFailed.FailedRequest.Object,
                mockedSolutionsRepository.SuccessfulRequest.Object,
                memoryCache);

            sutUpdateFailure = new GamesService(
                mockedGamesRepository.UpdateFailedRequest.Object,
                mockedAppsRepository.SuccessfulRequest.Object,
                mockedUsersRepository.SuccessfulRequest.Object,
                mockedDifficultiesRepositorySuccessful.SuccessfulRequest.Object,
                mockedSolutionsRepository.SuccessfulRequest.Object,
                memoryCache);

            request = TestObjects.GetRequest();
            createGameRequest = TestObjects.GetCreateGameRequest();
            gamesRequest = TestObjects.GetGamesRequest();

        }

        [Test, Category("Services")]
        public async Task CreateGames()
        {
            // Arrange
            request.Payload = createGameRequest;

            // Act
            var result = await sut.Create(request);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Game Created"));
            Assert.That((Game)result.Payload[0], Is.TypeOf<Game>());
        }

        [Test, Category("Services")]
        public async Task FailToCreateGameIfUserDoesNotExist()
        {
            // Arrange
            var badRequest = new CreateGameRequest()
            {
                UserId = 5,
                DifficultyId = 4
            };
            request.Payload = badRequest;

            // Act
            var result = await sutFailure.Create(request);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Game not Created"));
        }

        [Test, Category("Services")]
        public async Task UpdateGames()
        {
            // Arrange
            var gameId = 1;
            var updatedValue = 6;
            updateGameRequest = TestObjects.GetUpdateGameRequest(updatedValue);
            request.Payload = updateGameRequest;

            // Act
            var result = await sut.Update(gameId, request);

            var checkValue = ((Game)result.Payload[0])
                .SudokuMatrix.SudokuCells
                .OrderBy(cell => cell.Index)
                .Where(cell => cell.Index == 2)
                .Select(cell => cell.DisplayedValue)
                .FirstOrDefault();

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Game Updated"));
            Assert.That((Game)result.Payload[0], Is.TypeOf<Game>());
            Assert.That(checkValue, Is.EqualTo(updatedValue));
        }

        [Test, Category("Services")]
        public async Task RejectUpdateIfCellsAreInvalid()
        {
            // Arrange
            var gameId = 1;
            var updatedValue = 6;
            updateGameRequest = TestObjects.GetInvalidUpdateGameRequest(updatedValue);
            request.Payload = updateGameRequest;

            // Act
            var result = await sutUpdateFailure.Update(gameId, request);

            var updatedGame = await context.Games
                .FirstOrDefaultAsync(game => game.Id == gameId);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Game not Updated"));
        }

        [Test, Category("Services")]
        public async Task DeleteGames()
        {
            // Arrange
            var gameId = 1;

            // Act
            var result = await sut.Delete(gameId);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Game Deleted"));
        }

        [Test, Category("Services")]
        public async Task DeleteReturnsErrorMessageIfGameNotFound()
        {
            // Arrange
            var gameId = 5;

            // Act
            var result = await sutFailure.Delete(gameId);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Game not Found"));
        }

        [Test, Category("Services")]
        public async Task GetAGame()
        {
            // Arrange
            var gameId = 1;
            var appId = 1;

            // Act
            var result = await sut.GetGame(gameId, appId);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Game Found"));
            Assert.That((Game)result.Payload[0], Is.TypeOf<Game>());
        }

        [Test, Category("Services")]
        public async Task ReturnErrorMessageIfGameNotFound()
        {
            // Arrange
            var gameId = 5;
            var appId = 1;

            // Act
            var result = await sutFailure.GetGame(gameId, appId);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Game not Found"));
        }

        [Test, Category("Services")]
        public async Task GetGames()
        {
            // Arrange
            request.Payload = gamesRequest;

            // Act
            var result = await sut.GetGames(request);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Games Found"));
            Assert.That(result.Payload.ConvertAll(g => (IGame)g), Is.TypeOf<List<IGame>>());
        }

        [Test, Category("Services")]
        public async Task GetUsersGame()
        {
            // Arrange
            var gameId = 1;
            request.Payload = gamesRequest;

            // Act
            var result = await sut.GetMyGame(gameId, request);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Game Found"));
            Assert.That((Game)result.Payload[0], Is.TypeOf<Game>());
        }

        [Test, Category("Services")]
        public async Task GetUsersGames()
        {
            // Arrange
            request.Payload = gamesRequest;

            // Act
            var result = await sut.GetMyGames(request);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Games Found"));
            Assert.That(result.Payload.ConvertAll(g => (IGame)g), Is.TypeOf<List<IGame>>());
        }

        [Test, Category("Services")]
        public async Task DeleteAUsersGame()
        {
            // Arrange
            var gameId = 1;
            request.Payload = gamesRequest;

            // Act
            var result = await sut.DeleteMyGame(gameId, request);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Game Deleted"));
        }

        [Test, Category("Services")]
        public async Task CheckGames()
        {
            // Arrange
            var gameId = 1;
            var updatedValue = 6;
            updateGameRequest = TestObjects.GetUpdateGameRequest(updatedValue);
            request.Payload = updateGameRequest;

            // Act
            var result = await sut.Check(gameId, request);

            var updatedGame = await context.Games
                .FirstOrDefaultAsync(game => game.Id == gameId);

            var checkValue = updatedGame.SudokuMatrix.SudokuCells
                .Where(cell => cell.Id == 58)
                .Select(cell => cell.DisplayedValue)
                .FirstOrDefault();

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Game not Solved"));
            Assert.That((Game)result.Payload[0], Is.TypeOf<Game>());
            Assert.That(checkValue, Is.EqualTo(updatedValue));
        }

        [Test, Category("Services")]
        public async Task NoteWhenGameIsSolvedOnUpdate()
        {
            // Arrange
            var gameId = 2;
            updateGameRequest = TestObjects.GetSolvedUpdateGameRequest();
            request.Payload = updateGameRequest;

            // Act
            var result = await sutSolved.Check(gameId, request);

            var updatedGame = await context.Games
                .FirstOrDefaultAsync(game => game.Id == gameId);

            var game = context.Games.FirstOrDefault(g => g.Id == gameId);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Game Solved"));
            Assert.That((Game)result.Payload[0], Is.TypeOf<Game>());
            Assert.That(game.IsSolved(), Is.True);
        }

        [Test, Category("Services")]
        public async Task CheckGameShouldReturnMessageIfGameNotFound()
        {
            // Arrange
            var gameId = 5;
            updateGameRequest = TestObjects.GetGameNotFoundUpdateGameRequest();
            request.Payload = updateGameRequest;

            // Act
            var result = await sutFailure.Check(gameId, request);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Game not Found"));
        }

        [Test, Category("Services")]
        public async Task CreateAnnonymousGames()
        {
            // Arrange

            // Act
            var result = await sut.CreateAnnonymous(DifficultyLevel.TEST);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Game Created"));
            Assert.That(((AnnonymousGameResult)result.Payload[0]).SudokuMatrix, Is.TypeOf<List<List<int>>>());
        }

        [Test, Category("Services")]
        public async Task CreateAnnonymousGamesShouldReturnMessageIfDifficultyNotFound()
        {
            // Arrange
            var difficulty = await context
                .Difficulties
                .FirstOrDefaultAsync(d => d.DifficultyLevel == DifficultyLevel.TEST);

            context.Difficulties.Remove(difficulty);

            // Act
            var result = await sutAnonFailure.CreateAnnonymous(difficulty.DifficultyLevel);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Difficulty not Found"));
        }

        [Test, Category("Services")]
        public async Task CheckAnnonymousGames()
        {
            // Arrange
            var intList = new List<int> {
                2, 9, 8, 1, 3, 4, 6, 7, 5,
                3, 1, 6, 5, 8, 7, 2, 9, 4,
                4, 5, 7, 6, 9, 2, 1, 8, 3,
                9, 7, 1, 2, 4, 3, 5, 6, 8,
                5, 8, 3, 7, 6, 1, 4, 2, 9,
                6, 2, 4, 9, 5, 8, 3, 1, 7,
                7, 3, 5, 8, 2, 6, 9, 4, 1,
                8, 4, 2, 3, 1, 9, 7, 5, 6,
                1, 6, 9, 4, 7, 5, 8, 3, 2 };

            // Act
            var result = await sut.CheckAnnonymous(intList);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Game Solved"));
        }

        [Test, Category("Services")]
        public async Task CheckAnnonymousGamesShouldReturnMessageIfSolutionNotFound()
        {
            // Arrange
            var intList = new List<int> {
                5, 9, 8, 1, 3, 4, 6, 7, 2,
                3, 1, 6, 5, 8, 7, 2, 9, 4,
                4, 5, 7, 6, 9, 2, 1, 8, 3,
                9, 7, 1, 2, 4, 3, 5, 6, 8,
                5, 8, 3, 7, 6, 1, 4, 2, 9,
                6, 2, 4, 9, 5, 8, 3, 1, 7,
                7, 3, 5, 8, 2, 6, 9, 4, 1,
                8, 4, 2, 3, 1, 9, 7, 5, 6,
                1, 6, 9, 4, 7, 5, 8, 3, 2 };

            // Act
            var result = await sut.CheckAnnonymous(intList);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Game not Solved"));
        }
    }
}
