using System.Threading.Tasks;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;

namespace SudokuCollective.Core.Interfaces.Services
{
    public interface IUserManagementService : IService
    {
        Task<bool> IsValidUser(string username, string password);
        Task<UserAuthenticationErrorType> ConfirmAuthenticationIssue(string username, string password, string license);
        Task<IResult> ConfirmUserName(string email, string license);
    }
}
