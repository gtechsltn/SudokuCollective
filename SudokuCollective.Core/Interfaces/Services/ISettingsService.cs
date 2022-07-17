using System.Threading.Tasks;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;

namespace SudokuCollective.Core.Interfaces.Services
{
    public interface ISettingsService
    {
        Task<IResult> GetAsync();
        IResult GetReleaseEnvironments();
        IResult GetSortValues();
        IResult GetTimeFrames();
    }
}
