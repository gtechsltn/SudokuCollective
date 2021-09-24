using System.Threading.Tasks;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Results;
using SudokuCollective.Core.Interfaces.Models.TokenModels;

namespace SudokuCollective.Core.Interfaces.Services
{
    public interface IAuthenticateService : IService
    {
        Task<IAuthenticationResult> IsAuthenticated(ITokenRequest request);
    }
}
