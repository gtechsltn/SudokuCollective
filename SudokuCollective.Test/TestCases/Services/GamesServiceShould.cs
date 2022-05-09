﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Payloads;
using SudokuCollective.Data.Models.Results;
using SudokuCollective.Data.Services;
using SudokuCollective.Test.Repositories;
using SudokuCollective.Test.Services;
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
        private MockedRequestService mockedRequestService;
        private MemoryDistributedCache memoryCache;
        private Mock<ILogger<GamesService>> mockedLogger;

        private IGamesService sut;
        private IGamesService sutFailure;
        private IGamesService sutSolved;
        private IGamesService sutAnonFailure;
        private IGamesService sutUpdateFailure;
        private Request request;
        private CreateGamePayload createGamePayload;
        private GamePayload GamePayload;
        private GamesPayload gamesPayload;

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
            mockedRequestService = new MockedRequestService();
            memoryCache = new MemoryDistributedCache(
                Options.Create(new MemoryDistributedCacheOptions()));
            mockedLogger = new Mock<ILogger<GamesService>>();

            sut = new GamesService(
                mockedGamesRepository.SuccessfulRequest.Object,
                mockedAppsRepository.SuccessfulRequest.Object,
                mockedUsersRepository.SuccessfulRequest.Object,
                mockedDifficultiesRepositorySuccessful.SuccessfulRequest.Object,
                mockedSolutionsRepository.SuccessfulRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                memoryCache,
                mockedLogger.Object);

            sutFailure = new GamesService(
                mockedGamesRepository.FailedRequest.Object,
                mockedAppsRepository.SuccessfulRequest.Object,
                mockedUsersRepository.SuccessfulRequest.Object,
                mockedDifficultiesRepositorySuccessful.SuccessfulRequest.Object,
                mockedSolutionsRepository.SuccessfulRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                memoryCache,
                mockedLogger.Object);

            sutSolved = new GamesService(
                mockedGamesRepository.SolvedRequest.Object,
                mockedAppsRepository.SuccessfulRequest.Object,
                mockedUsersRepository.SuccessfulRequest.Object,
                mockedDifficultiesRepositorySuccessful.SuccessfulRequest.Object,
                mockedSolutionsRepository.SuccessfulRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                memoryCache,
                mockedLogger.Object);

            sutAnonFailure = new GamesService(
                mockedGamesRepository.SuccessfulRequest.Object,
                mockedAppsRepository.SuccessfulRequest.Object,
                mockedUsersRepository.SuccessfulRequest.Object,
                mockedDifficultiesRepositoryFailed.FailedRequest.Object,
                mockedSolutionsRepository.SuccessfulRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                memoryCache,
                mockedLogger.Object);

            sutUpdateFailure = new GamesService(
                mockedGamesRepository.UpdateFailedRequest.Object,
                mockedAppsRepository.SuccessfulRequest.Object,
                mockedUsersRepository.SuccessfulRequest.Object,
                mockedDifficultiesRepositorySuccessful.SuccessfulRequest.Object,
                mockedSolutionsRepository.SuccessfulRequest.Object,
                mockedRequestService.SuccessfulRequest.Object,
                memoryCache,
                mockedLogger.Object);

            request = TestObjects.GetRequest();
            createGamePayload = TestObjects.GetCreateGamePayload();
            gamesPayload = TestObjects.GetGamesPayload();

        }

        [Test, Category("Services")]
        public async Task CreateGames()
        {
            // Arrange
            request.Payload = createGamePayload;

            // Act
            var result = await sut.CreateAsync(request);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Game Created"));
            Assert.That((Game)result.Payload[0], Is.TypeOf<Game>());
        }

        [Test, Category("Services")]
        public async Task FailToCreateGameIfUserDoesNotExist()
        {
            // Arrange
            var payload = new CreateGamePayload()
            {
                DifficultyId = 4
            };
            request.Payload = payload;

            // Act
            var result = await sutFailure.CreateAsync(request);

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
            GamePayload = TestObjects.GetGamePayload(updatedValue);
            request.Payload = GamePayload;

            // Act
            var result = await sut.UpdateAsync(gameId, request);

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
            GamePayload = TestObjects.GetInvalidGamePayload(updatedValue);
            request.Payload = GamePayload;

            // Act
            var result = await sutUpdateFailure.UpdateAsync(gameId, request);

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
            var result = await sut.DeleteAsync(gameId);

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
            var result = await sutFailure.DeleteAsync(gameId);

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
            var result = await sut.GetGameAsync(gameId, appId);

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
            var result = await sutFailure.GetGameAsync(gameId, appId);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Game not Found"));
        }

        [Test, Category("Services")]
        public async Task GetGames()
        {
            // Arrange
            request.Payload = gamesPayload;

            // Act
            var result = await sut.GetGamesAsync(request);

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
            request.Payload = gamesPayload;

            // Act
            var result = await sut.GetMyGameAsync(gameId, request);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Game Found"));
            Assert.That((Game)result.Payload[0], Is.TypeOf<Game>());
        }

        [Test, Category("Services")]
        public async Task GetUsersGames()
        {
            // Arrange
            request.Payload = gamesPayload;

            // Act
            var result = await sut.GetMyGamesAsync(request);

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
            request.Payload = gamesPayload;

            // Act
            var result = await sut.DeleteMyGameAsync(gameId, request);

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
            GamePayload = TestObjects.GetGamePayload(updatedValue);
            request.Payload = GamePayload;

            // Act
            var result = await sut.CheckAsync(gameId, request);

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
            GamePayload = TestObjects.GetSolvedGamePayload();
            request.Payload = GamePayload;

            // Act
            var result = await sutSolved.CheckAsync(gameId, request);

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
            GamePayload = TestObjects.GetGameNotFoundGamePayload();
            request.Payload = GamePayload;

            // Act
            var result = await sutFailure.CheckAsync(gameId, request);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Game not Found"));
        }

        [Test, Category("Services")]
        public async Task CreateAnnonymousGames()
        {
            // Arrange

            // Act
            var result = await sut.CreateAnnonymousAsync(DifficultyLevel.TEST);

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
            var result = await sutAnonFailure.CreateAnnonymousAsync(difficulty.DifficultyLevel);

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
            var result = await sut.CheckAnnonymousAsync(intList);

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
            var result = await sut.CheckAnnonymousAsync(intList);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Game not Solved"));
        }
    }
}
