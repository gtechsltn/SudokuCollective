using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SudokuCollective.Core.Extensions;
using SudokuCollective.Core.Interfaces.Jobs;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models;
using SudokuCollective.Logs.Utilities;

namespace SudokuCollective.Data.Jobs
{
    public class DataJobs : IDataJobs
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<DataJobs> _logger;

        public DataJobs(DatabaseContext context, ILogger<DataJobs> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddSolutionJobAsync(List<int> intList)
        {
            try
            {
                var sudokuSolution = new SudokuSolution(intList);

                // Add solution to the database
                var solutions = await _context.SudokuSolutions.ToListAsync();

                var solutionInDB = false;

                if (solutions.Count > 0)
                {

                    foreach (var solution in solutions.Where(s => s.DateSolved > DateTime.MinValue))
                    {
                        if (solution.SolutionList.IsThisListEqual(sudokuSolution.SolutionList))
                        {
                            solutionInDB = true;
                        }
                    }
                }

                if (!solutionInDB)
                {
                    _context.Add(sudokuSolution);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
                _logger.LogWarning(LogsUtilities.GetControllerWarningEventId(), message);
            }
        }
    }
}
