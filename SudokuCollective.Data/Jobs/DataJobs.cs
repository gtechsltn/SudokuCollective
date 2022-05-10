using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using SudokuCollective.Core.Extensions;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Models;

namespace SudokuCollective.Data.Jobs
{
    public static class DataJobs
    {
        public static void AddSolutionJob<T>(
            ISolutionsRepository<SudokuSolution> repository, 
            SudokuSolution sudokuSolution,
            ILogger<T> logger,
            EventId eventId)
        {
            try
            {
                // Add solution to the database
                var response = repository.GetAll();

                if (response.IsSuccess)
                {
                    var solutionInDB = false;

                    foreach (var solution in response
                        .Objects
                        .ConvertAll(s => (SudokuSolution)s)
                        .Where(s => s.DateSolved > DateTime.MinValue))
                    {
                        if (solution.SolutionList.IsThisListEqual(sudokuSolution.SolutionList))
                        {
                            solutionInDB = true;
                        }
                    }

                    if (!solutionInDB)
                    {
                        _ = repository.Add(sudokuSolution);
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogWarning(eventId, e.Message);
            }
        }
    }
}
