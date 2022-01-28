using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SudokuCollective.Core.Extensions;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Requests;
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

        public async Task<IResult> Solve(
            IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = new Result();

            try
            {
                var response = await _solutionsRepository.GetAll();

                var solvedSolutions = response
                    .Objects
                    .ConvertAll(s => (SudokuSolution)s)
                    .ToList();

                var intList = new List<int>();

                intList.AddRange(((SolutionRequest)request.DataPacket).FirstRow);
                intList.AddRange(((SolutionRequest)request.DataPacket).SecondRow);
                intList.AddRange(((SolutionRequest)request.DataPacket).ThirdRow);
                intList.AddRange(((SolutionRequest)request.DataPacket).FourthRow);
                intList.AddRange(((SolutionRequest)request.DataPacket).FifthRow);
                intList.AddRange(((SolutionRequest)request.DataPacket).SixthRow);
                intList.AddRange(((SolutionRequest)request.DataPacket).SeventhRow);
                intList.AddRange(((SolutionRequest)request.DataPacket).EighthRow);
                intList.AddRange(((SolutionRequest)request.DataPacket).NinthRow);

                var sudokuSolver = new SudokuMatrix(intList);

                await sudokuSolver.Solve();

                if (sudokuSolver.IsValid())
                {
                    var solution = new SudokuSolution(sudokuSolver.ToIntList());

                    var addResultToDataContext = true;

                    if (solvedSolutions.Count > 0)
                    {
                        foreach (var solvedSolution in solvedSolutions)
                        {
                            if (solvedSolution.ToString().Equals(solution.ToString()))
                            {
                                addResultToDataContext = false;
                            }
                        }
                    }

                    if (addResultToDataContext)
                    {
                        solution = (SudokuSolution)(await _solutionsRepository.Add(solution)).Object;
                    }
                    else
                    {
                        solution = solvedSolutions.Where(s => s.SolutionList.IsThisListEqual(solution.SolutionList)).FirstOrDefault();
                    }

                    result.IsSuccess = true;
                    result.DataPacket.Add(solution);
                    result.Message = SolutionsMessages.SudokuSolutionFoundMessage;
                }
                else
                {
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
                                result.DataPacket.Add(solution);
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
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
            }
        }

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

        public async Task<IResult> Add(int limitArg)
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
                var solutions = (await _solutionsRepository.GetSolvedSolutions())
                    .Objects.ConvertAll(s => (SudokuSolution)s);

                foreach (var solution in solutions)
                {
                    solutionsInDB.Add(solution.SolutionList);
                }
            }
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
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
                    .AddSolutions(newSolutions.ConvertAll(s => (ISudokuSolution)s));

                if (solutionsResponse.Success)
                {
                    result.IsSuccess = solutionsResponse.Success;
                    result.Message = SolutionsMessages.SolutionsAddedMessage;

                    return result;
                }
                else if (!solutionsResponse.Success && solutionsResponse.Exception != null)
                {
                    result.IsSuccess = solutionsResponse.Success;
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
