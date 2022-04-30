using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Extensions;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Extensions;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Payloads;
using SudokuCollective.Data.Models.Results;
using SudokuCollective.Data.Utilities;
using SudokuCollective.Logs;
using SudokuCollective.Logs.Utilities;

namespace SudokuCollective.Data.Services
{
    public class GamesService : IGamesService
    {
        #region Fields
        private readonly IGamesRepository<Game> _gamesRepository;
        private readonly IAppsRepository<App> _appsRepository;
        private readonly IUsersRepository<User> _usersRepository;
        private readonly IDifficultiesRepository<Difficulty> _difficultiesRepository;
        private readonly ISolutionsRepository<SudokuSolution> _solutionsRepository;
        private readonly IRequestService _requestService;
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<GamesService> _logger;
        #endregion

        #region Constructor
        public GamesService(
            IGamesRepository<Game> gamesRepsitory,
            IAppsRepository<App> appsRepository,
            IUsersRepository<User> usersRepository, 
            IDifficultiesRepository<Difficulty> difficultiesRepository,
            ISolutionsRepository<SudokuSolution> solutionsRepository,
            IRequestService requestService,
            IDistributedCache distributedCache,
            ILogger<GamesService> logger)
        {
            _gamesRepository = gamesRepsitory;
            _appsRepository = appsRepository;
            _usersRepository = usersRepository;
            _difficultiesRepository = difficultiesRepository;
            _solutionsRepository = solutionsRepository;
            _requestService = requestService;
            _distributedCache = distributedCache;
            _logger = logger;
        }
        #endregion

        #region Methods
        public async Task<IResult> CreateAsync(IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = new Result();

            CreateGamePayload payload;

            if (request.Payload.ConvertToPayloadSuccessful(typeof(CreateGamePayload), out IPayload conversionResult))
            {
                payload = (CreateGamePayload)conversionResult;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = ServicesMesages.InvalidRequestMessage;

                return result;
            }

            try
            {
                var userResponse = await _usersRepository.GetAsync(payload.UserId);

                if (userResponse.IsSuccess)
                {
                    var difficultyResponse = await _difficultiesRepository.GetAsync(payload.DifficultyId);

                    if (difficultyResponse.IsSuccess)
                    {
                        var user = (User)userResponse.Object;
                        var difficulty = (Difficulty)difficultyResponse.Object;

                        var game = new Game(
                            user,
                            new SudokuMatrix(),
                            difficulty,
                            request.AppId);

                        game.SudokuMatrix.GenerateSolution();

                        var gameResponse = await _gamesRepository.AddAsync(game);

                        if (gameResponse.IsSuccess)
                        {
                            game = (Game)gameResponse.Object;

                            game.User = null;
                            game.SudokuMatrix.SudokuCells.OrderBy(cell => cell.Index);

                            result.IsSuccess = gameResponse.IsSuccess;
                            result.Message = GamesMessages.GameCreatedMessage;
                            result.Payload.Add(game);

                            return result;
                        }
                        else if (!gameResponse.IsSuccess && gameResponse.Exception != null)
                        {
                            result.IsSuccess = gameResponse.IsSuccess;
                            result.Message = gameResponse.Exception.Message;

                            return result;
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Message = GamesMessages.GameNotCreatedMessage;

                            return result;
                        }
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = DifficultiesMessages.DifficultyDoesNotExistMessage;

                        return result;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = UsersMessages.UserDoesNotExistMessage;

                    return result;
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Message = e.Message;

                SudokuCollectiveLogger.LogError<GamesService>(
                    _logger,
                    LogsUtilities.GetServiceErrorEventId(), 
                    string.Format(LoggerMessages.ErrorThrownMessage, result.Message),
                    e,
                    (SudokuCollective.Logs.Models.Request)_requestService.Get());

                return result;
            }
        }

        public async Task<IResult> GetGameAsync(int id, int appId)
        {
            var result = new Result();

            if (id == 0 || appId == 0)
            {
                result.IsSuccess = false;
                result.Message = GamesMessages.GameNotFoundMessage;
            }

            try
            {
                if (await _appsRepository.HasEntityAsync(appId))
                {
                    var gameResponse = await _gamesRepository.GetAppGameAsync(id, appId);

                    if (gameResponse.IsSuccess)
                    {
                        var game = (Game)gameResponse.Object;

                        result.IsSuccess = true;
                        result.Message = GamesMessages.GameFoundMessage;
                        result.Payload.Add(game);

                        return result;
                    }
                    else if (!gameResponse.IsSuccess && gameResponse.Exception != null)
                    {
                        result.IsSuccess = gameResponse.IsSuccess;
                        result.Message = gameResponse.Exception.Message;

                        return result;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = GamesMessages.GameNotFoundMessage;

                        return result;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = AppsMessages.AppNotFoundMessage;

                    return result;
                }
            }
            catch (Exception e)
            {
                return DataUtilities.ProcessException<GamesService>(
                    _requestService,
                    _logger,
                    result,
                    e);
            }
        }

        public async Task<IResult> GetGamesAsync(IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = new Result();

            try
            {
                if (await _appsRepository.HasEntityAsync(request.AppId))
                {
                    var response = await _gamesRepository.GetAppGamesAsync(request.AppId);

                    if (response.IsSuccess)
                    {
                        if (request.Paginator != null)
                        {
                            if (DataUtilities.IsPageValid(request.Paginator, response.Objects))
                            {
                                result = PaginatorUtilities.PaginateGames(
                                    request.Paginator, 
                                    response, 
                                    result);

                                if (result.Message.Equals(
                                    ServicesMesages.SortValueNotImplementedMessage))
                                {
                                    return result;
                                }
                            }
                            else
                            {
                                result.IsSuccess = false;
                                result.Message = ServicesMesages.PageNotFoundMessage;

                                return result;
                            }
                        }
                        else
                        {
                            result.Payload.AddRange(response.Objects);
                        }

                        result.IsSuccess = response.IsSuccess;
                        result.Message = GamesMessages.GamesFoundMessage;

                        return result;
                    }
                    else if (!response.IsSuccess && response.Exception != null)
                    {
                        result.IsSuccess = response.IsSuccess;
                        result.Message = response.Exception.Message;

                        return result;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = GamesMessages.GamesNotFoundMessage;

                        return result;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = AppsMessages.AppNotFoundMessage;

                    return result;
                }
            }
            catch (Exception e)
            {
                return DataUtilities.ProcessException<GamesService>(
                    _requestService,
                    _logger,
                    result,
                    e);
            }
        }

        public async Task<IResult> GetMyGameAsync(int id, IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = new Result();

            if (id == 0)
            {
                result.IsSuccess = false;
                result.Message = GamesMessages.GameNotFoundMessage;

                return result;
            }

            GamesPayload payload;

            if (request.Payload.ConvertToPayloadSuccessful(typeof(GamesPayload), out IPayload conversionResult))
            {
                payload = (GamesPayload)conversionResult;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = ServicesMesages.InvalidRequestMessage;

                return result;
            }

            try
            {
                if (await _appsRepository.HasEntityAsync(request.AppId))
                {
                    var gameResponse = await _gamesRepository.GetMyGameAsync(
                        payload.UserId, 
                        id,
                        request.AppId);

                    if (gameResponse.IsSuccess)
                    {
                        var game = (Game)gameResponse.Object;

                        result.IsSuccess = true;
                        result.Message = GamesMessages.GameFoundMessage;
                        result.Payload.Add(game);

                        return result;
                    }
                    else if (!gameResponse.IsSuccess && gameResponse.Exception != null)
                    {
                        result.IsSuccess = gameResponse.IsSuccess;
                        result.Message = gameResponse.Exception.Message;

                        return result;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = GamesMessages.GameNotFoundMessage;

                        return result;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = AppsMessages.AppNotFoundMessage;

                    return result;
                }
            }
            catch (Exception e)
            {
                return DataUtilities.ProcessException<GamesService>(
                    _requestService,
                    _logger,
                    result,
                    e);
            }
        }

        public async Task<IResult> GetMyGamesAsync(IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = new Result();

            GamesPayload payload;

            if (request.Payload.ConvertToPayloadSuccessful(typeof(GamesPayload), out IPayload conversionResult))
            {
                payload = (GamesPayload)conversionResult;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = ServicesMesages.InvalidRequestMessage;

                return result;
            }

            try
            {
                if (await _appsRepository.HasEntityAsync(request.AppId))
                {
                    var response = await _gamesRepository.GetMyGamesAsync(
                        payload.UserId,
                        request.AppId);

                    if (response.IsSuccess)
                    {
                        if (request.Paginator != null)
                        {
                            if (DataUtilities.IsPageValid(request.Paginator, response.Objects))
                            {
                                result = PaginatorUtilities.PaginateGames(
                                    request.Paginator, 
                                    response, 
                                    result);

                                if (result.Message.Equals(
                                    ServicesMesages.SortValueNotImplementedMessage))
                                {
                                    return result;
                                }
                            }
                            else
                            {
                                result.IsSuccess = false;
                                result.Message = ServicesMesages.PageNotFoundMessage;

                                return result;
                            }
                        }
                        else
                        {
                            result.Payload.AddRange(response.Objects);
                        }

                        result.IsSuccess = response.IsSuccess;
                        result.Message = GamesMessages.GamesFoundMessage;

                        return result;
                    }
                    else if (!response.IsSuccess && response.Exception != null)
                    {
                        result.IsSuccess = response.IsSuccess;
                        result.Message = response.Exception.Message;

                        return result;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = GamesMessages.GamesNotFoundMessage;

                        return result;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = AppsMessages.AppNotFoundMessage;

                    return result;
                }
            }
            catch (Exception e)
            {
                return DataUtilities.ProcessException<GamesService>(
                    _requestService,
                    _logger,
                    result,
                    e);
            }
        }

        public async Task<IResult> UpdateAsync(int id, IRequest request)
        {
            var result = new Result();

            UpdateGamePayload payload;

            if (request.Payload.ConvertToPayloadSuccessful(typeof(UpdateGamePayload), out IPayload conversionResult))
            {
                payload = (UpdateGamePayload)conversionResult;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = ServicesMesages.InvalidRequestMessage;

                return result;
            }

            try
            {
                if (await _gamesRepository.HasEntityAsync(id))
                {
                    var gameResponse = await _gamesRepository.GetAsync(id);

                    if (gameResponse.IsSuccess)
                    {
                        foreach (var cell in payload.SudokuCells)
                        {
                            foreach (var savedCell in ((Game)gameResponse.Object).SudokuMatrix.SudokuCells)
                            {
                                if (savedCell.Id == cell.Id && savedCell.Hidden)
                                {
                                    savedCell.DisplayedValue = cell.DisplayedValue;
                                }
                            }
                        }

                        var updateGameResponse = await _gamesRepository.UpdateAsync((Game)gameResponse.Object);

                        if (updateGameResponse.IsSuccess)
                        {
                            result.IsSuccess = updateGameResponse.IsSuccess;
                            result.Message = GamesMessages.GameUpdatedMessage;
                            result.Payload.Add((Game)updateGameResponse.Object);

                            return result;
                        }
                        else if (!updateGameResponse.IsSuccess && updateGameResponse.Exception != null)
                        {
                            result.IsSuccess = updateGameResponse.IsSuccess;
                            result.Message = updateGameResponse.Exception.Message;

                            return result;
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Message = GamesMessages.GameNotUpdatedMessage;

                            return result;
                        }
                    }
                    else if (!gameResponse.IsSuccess && gameResponse.Exception != null)
                    {
                        result.IsSuccess = gameResponse.IsSuccess;
                        result.Message = gameResponse.Exception.Message;

                        return result;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = GamesMessages.GameNotFoundMessage;

                        return result;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = GamesMessages.GameNotFoundMessage;

                    return result;
                }
            }
            catch (Exception e)
            {
                return DataUtilities.ProcessException<GamesService>(
                    _requestService,
                    _logger,
                    result,
                    e);
            }
        }

        public async Task<IResult> DeleteAsync(int id)
        {
            var result = new Result();

            if (id == 0)
            {
                result.IsSuccess = false;
                result.Message = GamesMessages.GameNotFoundMessage;

                return result;
            }

            try
            {
                var gameResponse = await _gamesRepository.GetAsync(id);

                if (gameResponse.IsSuccess)
                {
                    var deleteGameResponse = await _gamesRepository.DeleteAsync((Game)gameResponse.Object);

                    if (deleteGameResponse.IsSuccess)
                    {
                        result.IsSuccess = deleteGameResponse.IsSuccess;
                        result.Message = GamesMessages.GameDeletedMessage;

                        return result;
                    }
                    else if (!deleteGameResponse.IsSuccess && deleteGameResponse.Exception != null)
                    {
                        result.IsSuccess = deleteGameResponse.IsSuccess;
                        result.Message = deleteGameResponse.Exception.Message;

                        return result;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = GamesMessages.GameNotDeletedMessage;

                        return result;
                    }
                }
                else if (!gameResponse.IsSuccess && gameResponse.Exception != null)
                {
                    result.IsSuccess = gameResponse.IsSuccess;
                    result.Message = gameResponse.Exception.Message;

                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = GamesMessages.GameNotFoundMessage;

                    return result;
                }
            }
            catch (Exception e)
            {
                return DataUtilities.ProcessException<GamesService>(
                    _requestService,
                    _logger,
                    result,
                    e);
            }
        }

        public async Task<IResult> DeleteMyGameAsync(int id, IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = new Result();

            if (id == 0)
            {
                result.IsSuccess = false;
                result.Message = GamesMessages.GameNotFoundMessage;

                return result;
            }

            GamesPayload payload;

            if (request.Payload.ConvertToPayloadSuccessful(typeof(GamesPayload), out IPayload conversionResult))
            {
                payload = (GamesPayload)conversionResult;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = ServicesMesages.InvalidRequestMessage;

                return result;
            }

            try
            {
                if (await _appsRepository.HasEntityAsync(request.AppId))
                {
                    var response = await _gamesRepository.DeleteMyGameAsync(
                        payload.UserId, 
                        id, 
                        request.AppId);

                    if (response.IsSuccess)
                    {
                        result.IsSuccess = true;
                        result.Message = GamesMessages.GameDeletedMessage;

                        return result;
                    }
                    else if (!response.IsSuccess && response.Exception != null)
                    {
                        result.IsSuccess = response.IsSuccess;
                        result.Message = response.Exception.Message;

                        return result;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = GamesMessages.GameNotDeletedMessage;

                        return result;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = AppsMessages.AppNotFoundMessage;

                    return result;
                }
            }
            catch (Exception e)
            {
                return DataUtilities.ProcessException<GamesService>(
                    _requestService,
                    _logger,
                    result,
                    e);
            }
        }

        public async Task<IResult> CheckAsync(int id, IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = new Result();

            if (id == 0)
            {
                result.IsSuccess = false;
                result.Message = GamesMessages.GameNotFoundMessage;

                return result;
            }

            UpdateGamePayload payload;

            if (request.Payload.ConvertToPayloadSuccessful(typeof(UpdateGamePayload), out IPayload conversionResult))
            {
                payload = (UpdateGamePayload)conversionResult;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = ServicesMesages.InvalidRequestMessage;

                return result;
            }

            try
            {
                var gameResponse = await _gamesRepository.GetAsync(id);

                if (gameResponse.IsSuccess)
                {
                    foreach (var cell in payload.SudokuCells)
                    {
                        foreach (var savedCell in ((Game)gameResponse.Object).SudokuMatrix.SudokuCells)
                        {
                            if (savedCell.Id == cell.Id && savedCell.Hidden)
                            {
                                savedCell.DisplayedValue = cell.DisplayedValue;
                            }
                        }
                    }

                    if (((Game)gameResponse.Object).IsSolved())
                    {
                        result.Message = GamesMessages.GameSolvedMessage;
                    }
                    else
                    {
                        result.Message = GamesMessages.GameNotSolvedMessage;
                    }

                    var updateGameResponse = await _gamesRepository.UpdateAsync((Game)gameResponse.Object);

                    if (updateGameResponse.IsSuccess)
                    {
                        result.IsSuccess = updateGameResponse.IsSuccess;
                        result.Payload.Add((Game)updateGameResponse.Object);

                        return result;
                    }
                    else if (!updateGameResponse.IsSuccess && updateGameResponse.Exception != null)
                    {
                        result.IsSuccess = updateGameResponse.IsSuccess;
                        result.Message = updateGameResponse.Exception.Message;

                        return result;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = GamesMessages.GameNotUpdatedMessage;

                        return result;
                    }
                }
                else if (!gameResponse.IsSuccess && gameResponse.Exception != null)
                {
                    result.IsSuccess = gameResponse.IsSuccess;
                    result.Message = gameResponse.Exception.Message;

                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = GamesMessages.GameNotFoundMessage;

                    return result;
                }
            }
            catch (Exception e)
            {
                return DataUtilities.ProcessException<GamesService>(
                    _requestService,
                    _logger,
                    result,
                    e);
            }
        }

        public async Task<IResult> CreateAnnonymousAsync(DifficultyLevel difficultyLevel)
        {
            var result = new Result();
            var gameResult = new AnnonymousGameResult();

            try
            {
                if (await _difficultiesRepository.HasDifficultyLevelAsync(difficultyLevel))
                {
                    var game = new Game(new Difficulty { DifficultyLevel = difficultyLevel });

                    game.SudokuMatrix.GenerateSolution();

                    var sudokuMatrix = new List<List<int>>();

                    for (var i = 0; i < 73; i += 9)
                    {
                        gameResult.SudokuMatrix.Add(game.SudokuMatrix.ToDisplayedIntList().GetRange(i, 9));
                    }

                    result.IsSuccess = true;
                    result.Message = GamesMessages.GameCreatedMessage;
                    result.Payload.Add(gameResult);
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = DifficultiesMessages.DifficultyNotFoundMessage;
                }

                return result;
            }
            catch (Exception e)
            {
                return DataUtilities.ProcessException<GamesService>(
                    _requestService,
                    _logger,
                    result,
                    e);
            }
        }

        public async Task<IResult> CheckAnnonymousAsync(List<int> intList)
        {
            if (intList == null) throw new ArgumentNullException(nameof(intList));

            try
            {
                var result = new Result();

                if (intList.Count != 81 || intList.Contains(0))
                {
                    result.IsSuccess = false;
                    result.Message = GamesMessages.GameNotSolvedMessage;

                    return result;
                }

                var game = new Game(
                    new Difficulty
                    {
                        DifficultyLevel = DifficultyLevel.TEST
                    },
                    intList);

                result.IsSuccess = game.IsSolved();

                if (result.IsSuccess)
                {
                    // Add solution to the database
                    var response = await _solutionsRepository.GetAllAsync();

                    if (response.IsSuccess)
                    {
                        var solutionInDB = false;

                        foreach (var solution in response
                            .Objects
                            .ConvertAll(s => (SudokuSolution)s)
                            .Where(s => s.DateSolved > DateTime.MinValue))
                        {
                            if (solution.SolutionList.IsThisListEqual(game.SudokuSolution.SolutionList))
                            {
                                solutionInDB = true;
                            }
                        }

                        if (!solutionInDB)
                        {
                            _ = _solutionsRepository.AddAsync(game.SudokuSolution);
                        }
                    }

                    result.Message = GamesMessages.GameSolvedMessage;
                }
                else
                {
                    result.Message = GamesMessages.GameNotSolvedMessage;
                }

                return result;
            }
            catch (Exception e)
            {
                SudokuCollectiveLogger.LogError<GamesService>(
                    _logger,
                    LogsUtilities.GetServiceErrorEventId(), 
                    string.Format(LoggerMessages.ErrorThrownMessage, e.Message),
                    e,
                    (SudokuCollective.Logs.Models.Request)_requestService.Get());

                throw;
            }
        }
        #endregion
    }
}
