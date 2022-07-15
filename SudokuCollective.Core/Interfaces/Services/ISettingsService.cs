using System.Collections.Generic;
using System.Threading.Tasks;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Settings;

namespace SudokuCollective.Core.Interfaces.Services
{
    public interface ISettingsService
    {
        Task<IResult> GetAsync();
        List<IEnumListItem> GetReleaseEnvironments();
        List<IEnumListItem> GetSortValues();
        List<IEnumListItem> GetTimeFrames();
    }
}
