using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SudokuCollective.Core.Interfaces.Cache;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Payloads;

namespace SudokuCollective.Data.Services
{
    public class DifficultiesService : IDifficultiesService
    {
        #region Fields
        private readonly IDifficultiesRepository<Difficulty> _difficultiesRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly ICacheService _cacheService;
        private readonly ICacheKeys _cacheKeys;
        private readonly ICachingStrategy _cachingStrategy;
        #endregion

        #region Constructor
        public DifficultiesService(
            IDifficultiesRepository<Difficulty> difficultiesRepository,
            IDistributedCache distributedCache,
            ICacheService cacheService,
            ICacheKeys cacheKeys,
            ICachingStrategy cachingStrategy)
        {
            _difficultiesRepository = difficultiesRepository;
            _distributedCache = distributedCache;
            _cacheService = cacheService;
            _cacheKeys = cacheKeys;
            _cachingStrategy = cachingStrategy;
        }
        #endregion
        
        #region Methods
        public async Task<IResult> Create(IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = new Result();

            CreateDifficultyPayload payload;

            if (request.Payload is CreateDifficultyPayload r)
            {
                payload = r;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = ServicesMesages.InvalidRequestMessage;

                return result;
            }
            
            if (string.IsNullOrEmpty(payload.Name)) 
                throw new ArgumentNullException(nameof(payload.DifficultyLevel));

            if (string.IsNullOrEmpty(payload.DisplayName)) 
                throw new ArgumentNullException(nameof(payload.DisplayName));

            try
            {
                if (!await _cacheService.HasDifficultyLevelWithCacheAsync(
                    _difficultiesRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetDifficultyCacheKey, payload.DifficultyLevel),
                    _cachingStrategy.Heavy,
                    payload.DifficultyLevel))
                {

                    var difficulty = new Difficulty()
                    {
                        Name = payload.Name,
                        DisplayName = payload.DisplayName,
                        DifficultyLevel = payload.DifficultyLevel
                    };

                    var response = await _cacheService.AddWithCacheAsync<Difficulty>(
                        _difficultiesRepository,
                        _distributedCache,
                        _cacheKeys.GetDifficultyCacheKey,
                        _cachingStrategy.Heavy,
                        _cacheKeys,
                        difficulty);

                    if (response.Success)
                    {
                        result.IsSuccess = response.Success;
                        result.Message = DifficultiesMessages.DifficultyCreatedMessage;
                        result.Payload.Add(response.Object);

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
                        result.Message = DifficultiesMessages.DifficultyNotCreatedMessage;

                        return result;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = DifficultiesMessages.DifficultyAlreadyExistsMessage;

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

        public async Task<IResult> Get(int id)
        {
            var result = new Result();

            if (id == 0)
            {
                result.IsSuccess = false;
                result.Message = DifficultiesMessages.DifficultiesNotFoundMessage;

                return result;
            }

            try
            {
                var cacheServiceResponse = await _cacheService.GetWithCacheAsync(
                    _difficultiesRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetDifficultyCacheKey, id),
                    _cachingStrategy.Heavy,
                    id,
                    result);

                var response = (RepositoryResponse)cacheServiceResponse.Item1;
                result = (Result)cacheServiceResponse.Item2;

                if (response.Success)
                {
                    result.IsSuccess = response.Success;
                    result.Message = DifficultiesMessages.DifficultyFoundMessage;
                    result.Payload.Add(response.Object);

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
                    result.Message = DifficultiesMessages.DifficultyNotFoundMessage;

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

        public async Task<IResult> GetDifficulties()
        {
            var result = new Result();

            try
            {
                var cacheServiceResponse = await _cacheService.GetAllWithCacheAsync(
                    _difficultiesRepository,
                    _distributedCache,
                    _cacheKeys.GetDifficultiesCacheKey,
                    _cachingStrategy.Heavy,
                    result);

                var response = (RepositoryResponse)cacheServiceResponse.Item1;
                result = (Result)cacheServiceResponse.Item2;

                if (response.Success)
                {
                    result.IsSuccess = response.Success;
                    result.Message = DifficultiesMessages.DifficultiesFoundMessage;

                    result.Payload.AddRange(response.Objects);


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
                    result.Message = DifficultiesMessages.DifficultiesNotFoundMessage;

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
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = new Result();

            UpdateDifficultyPayload payload;

            if (request.Payload is UpdateDifficultyPayload r)
            {
                payload = r;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = ServicesMesages.InvalidRequestMessage;

                return result;
            }

            if (id == 0)
            {
                result.IsSuccess = false;
                result.Message = DifficultiesMessages.DifficultiesNotFoundMessage;

                return result;
            }

            try
            {
                var response = await _difficultiesRepository.Get(id);

                if (response.Success)
                {
                    var difficulty = (Difficulty)response.Object;

                    difficulty.Name = payload.Name;
                    difficulty.DisplayName = payload.DisplayName;

                    var updateDifficultyResponse = await _cacheService.UpdateWithCacheAsync(
                        _difficultiesRepository,
                        _distributedCache,
                        _cacheKeys,
                        difficulty);

                    if (updateDifficultyResponse.Success)
                    {
                        result.IsSuccess = updateDifficultyResponse.Success;
                        result.Message = DifficultiesMessages.DifficultyUpdatedMessage;
                        result.Payload.Add(updateDifficultyResponse.Object);

                        return result;
                    }
                    else if (!updateDifficultyResponse.Success && updateDifficultyResponse.Exception != null)
                    {
                        result.IsSuccess = updateDifficultyResponse.Success;
                        result.Message = updateDifficultyResponse.Exception.Message;

                        return result;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = DifficultiesMessages.DifficultyNotUpdatedMessage;

                        return result;
                    }

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
                    result.Message = DifficultiesMessages.DifficultyNotFoundMessage;

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
                result.Message = DifficultiesMessages.DifficultiesNotFoundMessage;

                return result;
            }

            try
            {
                var response = await _difficultiesRepository.Get(id);

                if (response.Success)
                {
                    var updateDeleteResponse = await _cacheService.DeleteWithCacheAsync(
                        _difficultiesRepository,
                        _distributedCache,
                        _cacheKeys,
                        (Difficulty)response.Object);

                    if (updateDeleteResponse.Success)
                    {
                        result.IsSuccess = updateDeleteResponse.Success;
                        result.Message = DifficultiesMessages.DifficultyDeletedMessage;

                        return result;
                    }
                    else if (!updateDeleteResponse.Success && updateDeleteResponse.Exception != null)
                    {
                        result.IsSuccess = updateDeleteResponse.Success;
                        result.Message = updateDeleteResponse.Exception.Message;

                        return result;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = DifficultiesMessages.DifficultyNotDeletedMessage;

                        return result;
                    }

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
                    result.Message = DifficultiesMessages.DifficultyNotFoundMessage;

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
        #endregion
    }
}
