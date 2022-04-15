using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SudokuCollective.Core.Interfaces.Models;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models;
using SudokuCollective.Logs;
using SudokuCollective.Logs.Utilities;

namespace SudokuCollective.Data.Utilities
{
    public static class DataUtilities
    {
        public async static Task AttachSudokuCells(
            this ISudokuMatrix matrix,
            DatabaseContext context)
        {

            var cells = await context.SudokuCells
                .Where(cell => cell.SudokuMatrixId == matrix.Id)
                .ToListAsync();

            ((SudokuMatrix)matrix).SudokuCells = cells.ConvertAll(cell => cell);
        }

        public async static Task<bool> IsGameInActiveApp(this IGame game, DatabaseContext context)
        {
            var app = await context.Apps.FirstOrDefaultAsync(a => a.Id == game.AppId);

            return app.IsActive;
        }

        public static bool IsPageValid(IPaginator paginator, List<IDomainEntity> entities)
        {
            if (paginator.ItemsPerPage * paginator.Page > entities.Count && paginator.Page == 1)
            {
                return true;
            }
            else if (paginator.ItemsPerPage * paginator.Page > entities.Count && paginator.Page > 1)
            {
                return false;
            }
            else
            {
                return paginator.ItemsPerPage * paginator.Page <= entities.Count;
            }
        }

        public static IResult ProcessException<T>(
            IRequestService requestService, 
            ILogger<T> logger, 
            IResult result, 
            Exception e,
            string message = null)
        {
            if (string.IsNullOrEmpty(message))
            {
                message = string.Format(LoggerMessages.ErrorThrownMessage, result.Message);
            }

            result.IsSuccess = false;
            result.Message = e.Message;

            SudokuCollectiveLogger.LogError<T>(
                logger,
                LogsUtilities.GetServiceErrorEventId(), 
                message,
                e,
                (SudokuCollective.Logs.Models.Request)requestService.Get());

            return result;
        }
    }
}
