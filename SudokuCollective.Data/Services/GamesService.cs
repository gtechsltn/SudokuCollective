using System;
using System.Collections.Generic;
using  System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Extensions;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Results;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Requests;
using SudokuCollective.Data.Models.Results;
using SudokuCollective.Data.Resiliency;
using SudokuCollective.Data.Utilities;

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
        private readonly IDistributedCache _distributedCache;
        #endregion

        #region Constructor
        public GamesService(
            IGamesRepository<Game> gamesRepsitory,
            IAppsRepository<App> appsRepository,
            IUsersRepository<User> usersRepository, 
            IDifficultiesRepository<Difficulty> difficultiesRepository,
            ISolutionsRepository<SudokuSolution> solutionsRepository,
            IDistributedCache distributedCache)
        {
            _gamesRepository = gamesRepsitory;
            _appsRepository = appsRepository;
            _usersRepository = usersRepository;
            _difficultiesRepository = difficultiesRepository;
            _solutionsRepository = solutionsRepository;
            _distributedCache = distributedCache;
        }
        #endregion

        #region Methods
        public async Task<IResult> Create(IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = new Result();

            CreateGameRequest gameRequest;

            if (request.DataPacket is CreateGameRequest r)
            {
                gameRequest = r;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = ServicesMesages.InvalidRequestMessage;

                return result;
            }

            try
            {
                var userResponse = await _usersRepository.Get(gameRequest.UserId);

                if (userResponse.Success)
                {
                    var difficultyResponse = await _difficultiesRepository.Get(gameRequest.DifficultyId);

                    if (difficultyResponse.Success)
                    {
                        var user = (User)userResponse.Object;
                        var difficulty = (Difficulty)difficultyResponse.Object;

                        var game = new Game(
                            user,
                            new SudokuMatrix(),
                            difficulty,
                            request.AppId);

                        game.SudokuMatrix.GenerateSolution();

                        var gameResponse = await _gamesRepository.Add(game);

                        if (gameResponse.Success)
                        {
                            game = (Game)gameResponse.Object;

                            game.User = null;
                            game.SudokuMatrix.SudokuCells.OrderBy(cell => cell.Index);

                            result.IsSuccess = gameResponse.Success;
                            result.Message = GamesMessages.GameCreatedMessage;
                            result.DataPacket.Add(game);

                            return result;
                        }
                        else if (!gameResponse.Success && gameResponse.Exception != null)
                        {
                            result.IsSuccess = gameResponse.Success;
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
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> GetGame(int id, int appId)
        {
            var result = new Result();

            if (id == 0 || appId == 0)
            {
                result.IsSuccess = false;
                result.Message = GamesMessages.GameNotFoundMessage;
            }

            try
            {
                if (await _appsRepository.HasEntity(appId))
                {
                    var gameResponse = await _gamesRepository.GetAppGame(id, appId);

                    if (gameResponse.Success)
                    {
                        var game = (Game)gameResponse.Object;

                        result.IsSuccess = true;
                        result.Message = GamesMessages.GameFoundMessage;
                        result.DataPacket.Add(game);

                        return result;
                    }
                    else if (!gameResponse.Success && gameResponse.Exception != null)
                    {
                        result.IsSuccess = gameResponse.Success;
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
            catch (Exception exp)
            {
                result.IsSuccess = true;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> GetGames(IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = new Result();

            try
            {
                if (await _appsRepository.HasEntity(request.AppId))
                {
                    var response = await _gamesRepository.GetAppGames(request.AppId);

                    if (response.Success)
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
                            result.DataPacket.AddRange(response.Objects);
                        }

                        result.IsSuccess = response.Success;
                        result.Message = GamesMessages.GamesFoundMessage;

                        return result;
                    }
                    else if (!response.Success && response.Exception != null)
                    {
                        result.IsSuccess = response.Success;
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
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> GetMyGame(int id, IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = new Result();

            if (id == 0)
            {
                result.IsSuccess = false;
                result.Message = GamesMessages.GameNotFoundMessage;

                return result;
            }

            GamesRequest gameRequest;

            if (request.DataPacket is GamesRequest r)
            {
                gameRequest = r;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = ServicesMesages.InvalidRequestMessage;

                return result;
            }

            try
            {
                if (await _appsRepository.HasEntity(request.AppId))
                {
                    var gameResponse = await _gamesRepository.GetMyGame(
                        gameRequest.UserId, 
                        id,
                        request.AppId);

                    if (gameResponse.Success)
                    {
                        var game = (Game)gameResponse.Object;

                        result.IsSuccess = true;
                        result.Message = GamesMessages.GameFoundMessage;
                        result.DataPacket.Add(game);

                        return result;
                    }
                    else if (!gameResponse.Success && gameResponse.Exception != null)
                    {
                        result.IsSuccess = gameResponse.Success;
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
            catch (Exception exp)
            {
                result.IsSuccess = true;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> GetMyGames(IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = new Result();

            GamesRequest gamesRequest;

            if (request.DataPacket is GamesRequest r)
            {
                gamesRequest = r;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = ServicesMesages.InvalidRequestMessage;

                return result;
            }

            try
            {
                if (await _appsRepository.HasEntity(request.AppId))
                {
                    var response = await _gamesRepository.GetMyGames(
                        gamesRequest.UserId,
                        request.AppId);

                    if (response.Success)
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
                            result.DataPacket.AddRange(response.Objects);
                        }

                        result.IsSuccess = response.Success;
                        result.Message = GamesMessages.GamesFoundMessage;

                        return result;
                    }
                    else if (!response.Success && response.Exception != null)
                    {
                        result.IsSuccess = response.Success;
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
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> Update(int id, IRequest request)
        {
            var result = new Result();

            UpdateGameRequest updateRequest;

            if (request.DataPacket is UpdateGameRequest r)
            {
                updateRequest = r;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = ServicesMesages.InvalidRequestMessage;

                return result;
            }

            try
            {
                if (await _gamesRepository.HasEntity(id))
                {
                    var gameResponse = await _gamesRepository.Get(id);

                    if (gameResponse.Success)
                    {
                        foreach (var cell in updateRequest.SudokuCells)
                        {
                            foreach (var savedCell in ((Game)gameResponse.Object).SudokuMatrix.SudokuCells)
                            {
                                if (savedCell.Id == cell.Id && savedCell.Hidden)
                                {
                                    savedCell.DisplayedValue = cell.DisplayedValue;
                                }
                            }
                        }

                        var updateGameResponse = await _gamesRepository.Update((Game)gameResponse.Object);

                        if (updateGameResponse.Success)
                        {
                            result.IsSuccess = updateGameResponse.Success;
                            result.Message = GamesMessages.GameUpdatedMessage;
                            result.DataPacket.Add((Game)updateGameResponse.Object);

                            return result;
                        }
                        else if (!updateGameResponse.Success && updateGameResponse.Exception != null)
                        {
                            result.IsSuccess = updateGameResponse.Success;
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
                    else if (!gameResponse.Success && gameResponse.Exception != null)
                    {
                        result.IsSuccess = gameResponse.Success;
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
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> Delete(int id)
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
                var gameResponse = await _gamesRepository.Get(id);

                if (gameResponse.Success)
                {
                    var deleteGameResponse = await _gamesRepository.Delete((Game)gameResponse.Object);

                    if (deleteGameResponse.Success)
                    {
                        result.IsSuccess = deleteGameResponse.Success;
                        result.Message = GamesMessages.GameDeletedMessage;

                        return result;
                    }
                    else if (!deleteGameResponse.Success && deleteGameResponse.Exception != null)
                    {
                        result.IsSuccess = deleteGameResponse.Success;
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
                else if (!gameResponse.Success && gameResponse.Exception != null)
                {
                    result.IsSuccess = gameResponse.Success;
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
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> DeleteMyGame(int id, IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = new Result();

            if (id == 0)
            {
                result.IsSuccess = false;
                result.Message = GamesMessages.GameNotFoundMessage;

                return result;
            }

            GamesRequest deleteRequest;

            if (request.DataPacket is GamesRequest r)
            {
                deleteRequest = r;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = ServicesMesages.InvalidRequestMessage;

                return result;
            }

            try
            {
                if (await _appsRepository.HasEntity(request.AppId))
                {
                    var response = await _gamesRepository.DeleteMyGame(
                        deleteRequest.UserId, 
                        id, 
                        request.AppId);

                    if (response.Success)
                    {
                        result.IsSuccess = true;
                        result.Message = GamesMessages.GameDeletedMessage;

                        return result;
                    }
                    else if (!response.Success && response.Exception != null)
                    {
                        result.IsSuccess = response.Success;
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
            catch (Exception exp)
            {
                result.IsSuccess = true;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> Check(int id, IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = new Result();

            if (id == 0)
            {
                result.IsSuccess = false;
                result.Message = GamesMessages.GameNotFoundMessage;

                return result;
            }

            UpdateGameRequest checkRequest;

            if (request.DataPacket is UpdateGameRequest r)
            {
                checkRequest = r;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = ServicesMesages.InvalidRequestMessage;

                return result;
            }

            try
            {
                var gameResponse = await _gamesRepository.Get(id);

                if (gameResponse.Success)
                {
                    foreach (var cell in checkRequest.SudokuCells)
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

                    var updateGameResponse = await _gamesRepository.Update((Game)gameResponse.Object);

                    if (updateGameResponse.Success)
                    {
                        result.IsSuccess = updateGameResponse.Success;
                        result.DataPacket.Add((Game)updateGameResponse.Object);

                        return result;
                    }
                    else if (!updateGameResponse.Success && updateGameResponse.Exception != null)
                    {
                        result.IsSuccess = updateGameResponse.Success;
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
                else if (!gameResponse.Success && gameResponse.Exception != null)
                {
                    result.IsSuccess = gameResponse.Success;
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
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> CreateAnnonymous(DifficultyLevel difficultyLevel)
        {
            var result = new Result();
            var gameResult = new AnnonymousGameResult();

            try
            {
                if (await _difficultiesRepository.HasDifficultyLevel(difficultyLevel))
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
                    result.DataPacket.Add(gameResult);
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = DifficultiesMessages.DifficultyNotFoundMessage;
                }

                return result;
            }
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> CheckAnnonymous(List<int> intList)
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
                    var response = await _solutionsRepository.GetAll();

                    if (response.Success)
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
                            _ = _solutionsRepository.Add(game.SudokuSolution);
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
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
