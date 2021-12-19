using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Requests;
using SudokuCollective.Data.Resiliency;

namespace SudokuCollective.Data.Services
{
    public class DifficultiesService : IDifficultiesService
    {
        #region Fields
        private readonly IDifficultiesRepository<Difficulty> _difficultiesRepository;
        private readonly IDistributedCache _distributedCache;
        #endregion

        #region Constructor
        public DifficultiesService(
            IDifficultiesRepository<Difficulty> difficultiesRepository,
            IDistributedCache distributedCache)
        {
            _difficultiesRepository = difficultiesRepository;
            _distributedCache = distributedCache;
        }
        #endregion
        
        #region Methods
        public async Task<IResult> Create(IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = new Result();

            CreateDifficultyRequest createDifficultyRequest;

            if (request.DataPacket is CreateDifficultyRequest r)
            {
                createDifficultyRequest = r;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = ServicesMesages.InvalidRequestMessage;

                return result;
            }
            
            if (string.IsNullOrEmpty(createDifficultyRequest.Name)) 
                throw new ArgumentNullException(nameof(createDifficultyRequest.DifficultyLevel));

            if (string.IsNullOrEmpty(createDifficultyRequest.DisplayName)) 
                throw new ArgumentNullException(nameof(createDifficultyRequest.DisplayName));

            try
            {
                if (!await CacheFactory.HasDifficultyLevelWithCacheAsync(
                    _difficultiesRepository,
                    _distributedCache,
                    string.Format(CacheKeys.GetDifficulty, createDifficultyRequest.DifficultyLevel),
                    CachingStrategy.Heavy,
                    createDifficultyRequest.DifficultyLevel))
                {

                    var difficulty = new Difficulty()
                    {
                        Name = createDifficultyRequest.Name,
                        DisplayName = createDifficultyRequest.DisplayName,
                        DifficultyLevel = createDifficultyRequest.DifficultyLevel
                    };

                    var response = await CacheFactory.AddWithCacheAsync<Difficulty>(
                        _difficultiesRepository,
                        _distributedCache,
                        CacheKeys.GetDifficulty,
                        CachingStrategy.Heavy,
                        difficulty);

                    if (response.Success)
                    {
                        result.IsSuccess = response.Success;
                        result.Message = DifficultiesMessages.DifficultyCreatedMessage;
                        result.DataPacket.Add(response.Object);

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
                var cacheFactoryResponse = await CacheFactory.GetWithCacheAsync(
                    _difficultiesRepository,
                    _distributedCache,
                    string.Format(CacheKeys.GetDifficulty, id),
                    CachingStrategy.Heavy,
                    id,
                    result);

                var response = (RepositoryResponse)cacheFactoryResponse.Item1;
                result = (Result)cacheFactoryResponse.Item2;

                if (response.Success)
                {
                    result.IsSuccess = response.Success;
                    result.Message = DifficultiesMessages.DifficultyFoundMessage;
                    result.DataPacket.Add(response.Object);

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
                var cacheFactoryResponse = await CacheFactory.GetAllWithCacheAsync(
                    _difficultiesRepository,
                    _distributedCache,
                    CacheKeys.GetDifficulties,
                    CachingStrategy.Heavy,
                    result);

                var response = (RepositoryResponse)cacheFactoryResponse.Item1;
                result = (Result)cacheFactoryResponse.Item2;

                if (response.Success)
                {
                    result.IsSuccess = response.Success;
                    result.Message = DifficultiesMessages.DifficultiesFoundMessage;

                    result.DataPacket.AddRange(response.Objects);


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

            UpdateDifficultyRequest updateDifficultyRequest;

            if (request.DataPacket is UpdateDifficultyRequest r)
            {
                updateDifficultyRequest = r;
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

                    difficulty.Name = updateDifficultyRequest.Name;
                    difficulty.DisplayName = updateDifficultyRequest.DisplayName;

                    var updateDifficultyResponse = await CacheFactory.UpdateWithCacheAsync(
                        _difficultiesRepository,
                        _distributedCache,
                        difficulty);

                    if (updateDifficultyResponse.Success)
                    {
                        result.IsSuccess = updateDifficultyResponse.Success;
                        result.Message = DifficultiesMessages.DifficultyUpdatedMessage;

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
                    var updateDeleteResponse = await CacheFactory.DeleteWithCacheAsync(
                        _difficultiesRepository,
                        _distributedCache,
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
