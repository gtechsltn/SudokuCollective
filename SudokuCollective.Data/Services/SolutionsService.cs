using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Hangfire;
using SudokuCollective.Core.Extensions;
using SudokuCollective.Core.Interfaces.Cache;
using SudokuCollective.Core.Interfaces.Jobs;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Extensions;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Payloads;
using SudokuCollective.Data.Utilities;

namespace SudokuCollective.Data.Services
{
    public class SolutionsService : ISolutionsService
    {
        #region Fields
        private readonly ISolutionsRepository<SudokuSolution> _solutionsRepository;
        private readonly IRequestService _requestService;
        private readonly IDistributedCache _distributedCache;
        private readonly ICacheService _cacheService;
        private readonly ICacheKeys _cacheKeys;
        private readonly IBackgroundJobClient _jobClient;
        private readonly IDataJobs _dataJobs;
        private readonly ILogger<SolutionsService> _logger;
        #endregion

        #region Constructor
        public SolutionsService(
            ISolutionsRepository<SudokuSolution> solutionsRepository,
            IRequestService requestService,
            IDistributedCache distributedCache,
            ICacheService cacheService,
            ICacheKeys cacheKeys,
            IBackgroundJobClient jobClient,
            IDataJobs dataJobs,
            ILogger<SolutionsService> logger)
        {
            _solutionsRepository = solutionsRepository;
            _requestService = requestService;
            _distributedCache = distributedCache;
            _cacheService = cacheService;
            _cacheKeys = cacheKeys;
            _jobClient = jobClient;
            _dataJobs = dataJobs;
            _logger = logger;
        }
        #endregion

        #region Methods

        public async Task<IResult> GetAsync(int id)
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
                var solutionResponse = await _solutionsRepository.GetAsync(id);

                if (solutionResponse.IsSuccess)
                {
                    var solution = (SudokuSolution)solutionResponse.Object;

                    result.IsSuccess = solutionResponse.IsSuccess;
                    result.Message = SolutionsMessages.SolutionFoundMessage;
                    result.Payload.Add(solution);

                    return result;
                }
                else if (!solutionResponse.IsSuccess && solutionResponse.Exception != null)
                {
                    result.IsSuccess = solutionResponse.IsSuccess;
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
            catch (Exception e)
            {
                return DataUtilities.ProcessException<SolutionsService>(
                    _requestService,
                    _logger,
                    result,
                    e);
            }
        }

        public async Task<IResult> GetSolutionsAsync(IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = new Result();
            var response = new RepositoryResponse();

            try
            {
                var cacheServiceResponse = await _cacheService.GetAllWithCacheAsync<SudokuSolution>(
                    _solutionsRepository,
                    _distributedCache,
                    _cacheKeys.GetSolutionsCacheKey,
                    DateTime.Now.AddHours(1),
                    result);

                response = (RepositoryResponse)cacheServiceResponse.Item1;
                result = (Result)cacheServiceResponse.Item2;

                if (response.IsSuccess)
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
                        result.Payload.AddRange(response
                            .Objects
                            .OrderBy(s => ((ISudokuSolution)s).Id)
                            .ToList()
                            .ConvertAll(s => (object)s));
                    }

                    result.IsSuccess = response.IsSuccess;
                    result.Message = SolutionsMessages.SolutionsFoundMessage;

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
                    result.Message = SolutionsMessages.SolutionsNotFoundMessage;

                    return result;
                }
            }
            catch (Exception e)
            {
                return DataUtilities.ProcessException<SolutionsService>(
                    _requestService,
                    _logger,
                    result,
                    e);
            }
        }

        public async Task<IResult> SolveAsync(
            IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = new Result();

            try
            {
                SolutionPayload payload;

                if (request.Payload.ConvertToPayloadSuccessful(typeof(SolutionPayload), out IPayload conversionResult))
                {
                    payload = (SolutionPayload)conversionResult;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = ServicesMesages.InvalidRequestMessage;

                    return result;
                }

                var intList = new List<int>();

                intList.AddRange(payload.FirstRow);
                intList.AddRange(payload.SecondRow);
                intList.AddRange(payload.ThirdRow);
                intList.AddRange(payload.FourthRow);
                intList.AddRange(payload.FifthRow);
                intList.AddRange(payload.SixthRow);
                intList.AddRange(payload.SeventhRow);
                intList.AddRange(payload.EighthRow);
                intList.AddRange(payload.NinthRow);

                var sudokuSolver = new SudokuMatrix(intList);

                await sudokuSolver.Solve();

                if (sudokuSolver.IsValid())
                {
                    _jobClient.Enqueue(() => _dataJobs.AddSolutionJobAsync(sudokuSolver.ToIntList()));

                    var solution = new SudokuSolution(sudokuSolver.ToIntList());

                    result.IsSuccess = true;
                    result.Payload.Add(solution);
                    result.Message = SolutionsMessages.SudokuSolutionFoundMessage;
                }
                else
                {
                    var response = await _solutionsRepository.GetAllAsync();

                    var solvedSolutions = response
                        .Objects
                        .ConvertAll(s => (SudokuSolution)s)
                        .ToList();

                    intList = sudokuSolver.ToIntList();

                    if (solvedSolutions.Count > 0)
                    {
                        var solutonInDB = false;

                        foreach (var solution in solvedSolutions)
                        {
                            var possibleSolution = true;

                            for (var i = 0; i < intList.Count - 1; i++)
                            {
                                if (intList[i] != 0 && intList[i] != solution.SolutionList[i])
                                {
                                    possibleSolution = false;
                                    break;
                                }
                            }

                            if (possibleSolution)
                            {
                                solutonInDB = possibleSolution;
                                result.IsSuccess = possibleSolution;
                                result.Payload.Add(solution);
                                result.Message = SolutionsMessages.SudokuSolutionFoundMessage;
                                break;
                            }
                        }

                        if (!solutonInDB)
                        {
                            result.IsSuccess = false;
                            result.Message = SolutionsMessages.SudokuSolutionNotFoundMessage;
                        }
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = SolutionsMessages.SudokuSolutionNotFoundMessage;
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                return DataUtilities.ProcessException<SolutionsService>(
                    _requestService,
                    _logger,
                    result,
                    e);
            }
        }

        public async Task<IResult> GenerateAsync()
        {
            try
            {
                var result = new Result();

                var continueLoop = true;

                do
                {
                    var matrix = new SudokuMatrix();

                    matrix.GenerateSolution();

                    var cacheServiceResponse = await _cacheService.GetAllWithCacheAsync<SudokuSolution>(
                        _solutionsRepository,
                        _distributedCache,
                        _cacheKeys.GetSolutionsCacheKey,
                        DateTime.Now.AddHours(1),
                        result);

                    var response = cacheServiceResponse.Item1;
                    result = (Result)cacheServiceResponse.Item2;

                    var matrixNotInDB = true;

                    if (response.IsSuccess)
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
                        result.Payload.Add(new SudokuSolution(
                            matrix.ToIntList()));

                        continueLoop = false;
                    }

                } while (continueLoop);

                var solutionResponse = await _solutionsRepository.AddAsync((SudokuSolution)result.Payload[0]);

                if (solutionResponse.IsSuccess)
                {
                    result.IsSuccess = solutionResponse.IsSuccess;
                    result.Message = SolutionsMessages.SolutionGeneratedMessage;

                    return result;
                }
                else
                {
                    result.IsSuccess = solutionResponse.IsSuccess;
                    result.Message = SolutionsMessages.SolutionNotGeneratedMessage;

                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<IResult> Async(int limitArg)
        {
            var result = new Result();

            if (limitArg == 0)
            {
                result.IsSuccess = false;
                result.Message = SolutionsMessages.SolutionsNotAddedMessage;

                return result;
            }

            var limit = 1000;

            if (limitArg <= limit)
            {
                limit = limitArg;
            }

            var reduceLimitBy = 0;

            var solutionsInDB = new List<List<int>>();

            try
            {
                var solutions = (await _solutionsRepository.GetSolvedSolutionsAsync())
                    .Objects.ConvertAll(s => (SudokuSolution)s);

                foreach (var solution in solutions)
                {
                    solutionsInDB.Add(solution.SolutionList);
                }
            }
            catch (Exception e)
            {
                return DataUtilities.ProcessException<SolutionsService>(
                    _requestService,
                    _logger,
                    result,
                    e);
            }

            var matrix = new SudokuMatrix();

            try
            {
                List<List<int>> solutionsList = new();
                List<SudokuSolution> newSolutions = new();

                var continueLoop = true;

                do
                {
                    for (var i = 0; i < limit - reduceLimitBy; i++)
                    {
                        matrix.GenerateSolution();

                        if (!solutionsInDB.Contains(matrix.ToIntList()))
                        {
                            solutionsList.Add(matrix.ToIntList());
                        }
                    }

                    solutionsList = solutionsList
                        .Distinct()
                        .ToList();

                    if (limit == solutionsList.Count)
                    {
                        continueLoop = false;

                    }
                    else
                    {
                        reduceLimitBy = limit - solutionsList.Count;
                    }

                } while (continueLoop);

                foreach (var solutionList in solutionsList)
                {
                    newSolutions.Add(new SudokuSolution(solutionList));
                }

                var solutionsResponse = await _solutionsRepository
                    .AddSolutionsAsync(newSolutions.ConvertAll(s => (ISudokuSolution)s));

                if (solutionsResponse.IsSuccess)
                {
                    result.IsSuccess = solutionsResponse.IsSuccess;
                    result.Message = SolutionsMessages.SolutionsAddedMessage;

                    return result;
                }
                else if (!solutionsResponse.IsSuccess && solutionsResponse.Exception != null)
                {
                    result.IsSuccess = solutionsResponse.IsSuccess;
                    result.Message = solutionsResponse.Exception.Message;

                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = SolutionsMessages.SolutionsNotAddedMessage;

                    return result;
                }

            }
            catch (Exception e)
            {
                return DataUtilities.ProcessException<SolutionsService>(
                    _requestService,
                    _logger,
                    result,
                    e);
            }
        }
        #endregion
    }
}
