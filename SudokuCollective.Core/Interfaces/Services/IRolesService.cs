using System.Threading.Tasks;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;

namespace SudokuCollective.Core.Interfaces.Services
{
    public interface IRolesService : IService
    {
        Task<IResult> Create(IRequest request);
        Task<IResult> Get(int id);
        Task<IResult> Update(int id, IRequest request);
        Task<IResult> Delete(int id);
        Task<IResult> GetRoles();
    }
}
