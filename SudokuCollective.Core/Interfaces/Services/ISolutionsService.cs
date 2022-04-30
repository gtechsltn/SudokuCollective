using System.Threading.Tasks;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;

namespace SudokuCollective.Core.Interfaces.Services
{
    public interface ISolutionsService : IService
    {
        Task<IResult> GetAsync(int id);
        Task<IResult> GetSolutionsAsync(IRequest request);
        Task<IResult> SolveAsync(IRequest request);
        Task<IResult> GenerateAsync();
        Task<IResult> Async(int limitArg);
    }
}
