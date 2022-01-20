using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SudokuCollective.Core.Interfaces.Models;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models;

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
    }
}
