using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Resiliency;
using SudokuCollective.Data.Utilities;

namespace SudokuCollective.Data.Services
{
    public class SolutionsService : ISolutionsService
    {
        #region Fields
        private readonly ISolutionsRepository<SudokuSolution> _solutionsRepository;
        private readonly IDistributedCache _distributedCache;
        #endregion

        #region Constructor
        public SolutionsService(
            ISolutionsRepository<SudokuSolution> solutionsRepository,
            IDistributedCache distributedCache)
        {
            _solutionsRepository = solutionsRepository;
            _distributedCache = distributedCache;
        }
        #endregion

        #region Methods
        public async Task<IResult> Generate()
        {
            try
            {
                var result = new Result();

                var continueLoop = true;

                do
                {
                    var matrix = new SudokuMatrix();

                    matrix.GenerateSolution();

                    var cacheFactoryResponse = await CacheFactory.GetAllWithCacheAsync<SudokuSolution>(
                        _solutionsRepository,
                        _distributedCache,
                        CacheKeys.GetSolutionsCacheKey,
                        DateTime.Now.AddHours(1),
                        result);

                    var response = cacheFactoryResponse.Item1;
                    result = (Result)cacheFactoryResponse.Item2;

                    var matrixNotInDB = true;

                    if (response.Success)
                    {
                        foreach (var solution in response
                            .Objects
                            .ConvertAll(s => (SudokuSolution)s)
                            .Where(s => s.DateSolved > DateTime.MinValue))
                        {
                            if (solution.SolutionList.Count > 0 && solution.ToString().Equals(matrix))
                            {
                                matrixNotInDB = false;
                            }
                        }
                    }

                    if (matrixNotInDB)
                    {
                        result.DataPacket.Add(new SudokuSolution(
                            matrix.ToIntList()));

                        continueLoop = false;
                    }

                } while (continueLoop);

                var solutionResponse = await _solutionsRepository.Add((SudokuSolution)result.DataPacket[0]);

                if (solutionResponse.Success)
                {
                    result.IsSuccess = solutionResponse.Success;
                    result.Message = SolutionsMessages.SolutionGeneratedMessage;

                    return result;
                }
                else
                {
                    result.IsSuccess = solutionResponse.Success;
                    result.Message = SolutionsMessages.SolutionNotGeneratedMessage;

                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<IResult> Get(int id)
        {
            var result = new Result();

            if (id == 0)
            {
                result.IsSuccess = false;
                result.Message = SolutionsMessages.SolutionNotFoundMessage;

                return result;
            }

            try
            {
                var solutionResponse = await _solutionsRepository.Get(id);

                if (solutionResponse.Success)
                {
                    var solution = (SudokuSolution)solutionResponse.Object;

                    result.IsSuccess = solutionResponse.Success;
                    result.Message = SolutionsMessages.SolutionFoundMessage;
                    result.DataPacket.Add(solution);

                    return result;
                }
                else if (!solutionResponse.Success && solutionResponse.Exception != null)
                {
                    result.IsSuccess = solutionResponse.Success;
                    result.Message = solutionResponse.Exception.Message;

                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = SolutionsMessages.SolutionNotFoundMessage;

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

        public async Task<IResult> GetSolutions(IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = new Result();
            var response = new RepositoryResponse();

            try
            {
                var cacheFactoryResponse = await CacheFactory.GetAllWithCacheAsync<SudokuSolution>(
                    _solutionsRepository,
                    _distributedCache,
                    CacheKeys.GetSolutionsCacheKey,
                    DateTime.Now.AddHours(1),
                    result);

                response = (RepositoryResponse)cacheFactoryResponse.Item1;
                result = (Result)cacheFactoryResponse.Item2;

                if (response.Success)
                {
                    if (request.Paginator != null)
                    {
                        if (DataUtilities.IsPageValid(request.Paginator, response.Objects))
                        {
                            result = PaginatorUtilities.PaginateSolutions(request.Paginator, response, result);

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
                        result.DataPacket.AddRange(response
                            .Objects
                            .OrderBy(s => ((ISudokuSolution)s).Id)
                            .ToList()
                            .ConvertAll(s => (object)s));
                    }

                    result.IsSuccess = response.Success;
                    result.Message = SolutionsMessages.SolutionsFoundMessage;

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
                    result.Message = SolutionsMessages.SolutionsNotFoundMessage;

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

        public Task<IResult> Solve(IRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<IResult> Add(int limit)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
