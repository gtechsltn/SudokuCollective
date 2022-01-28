using System.Threading.Tasks;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;

namespace SudokuCollective.Core.Interfaces.Services
{
    public interface ISolutionsService : IService
    {
        Task<IResult> Get(int id);
        Task<IResult> GetSolutions(IRequest request);
        Task<IResult> Solve(IRequest request);
        Task<IResult> Generate();
        Task<IResult> Add(int limitArg);
    }
}
