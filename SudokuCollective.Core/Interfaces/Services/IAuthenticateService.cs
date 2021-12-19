using System.Threading.Tasks;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Core.Interfaces.Models.TokenModels;

namespace SudokuCollective.Core.Interfaces.Services
{
    public interface IAuthenticateService : IService
    {
        Task<IResult> IsAuthenticated(ITokenRequest request);
    }
}
