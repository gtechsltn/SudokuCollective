using System.Threading.Tasks;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Results;

namespace SudokuCollective.Core.Interfaces.Services
{
    public interface IUserManagementService : IService
    {
        Task<bool> IsValidUser(string userName, string password);
        Task<UserAuthenticationErrorType> ConfirmAuthenticationIssue(string username, string password, string license);
        Task<IAuthenticationResult> ConfirmUserName(string email, string license);
    }
}
