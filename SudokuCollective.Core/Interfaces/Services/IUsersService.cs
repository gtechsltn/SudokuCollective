using System.Collections.Generic;
using System.Threading.Tasks;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Results;

namespace SudokuCollective.Core.Interfaces.Services
{
    public interface IUsersService : IService
    {
        Task<IUserResult> Create(
            IRequest request,
            string baseUrl,
            string emailtTemplatePath);
        Task<IResult> Get(
            int id,
            string license);
        Task<IResult> GetUsers(
            int requestorId,
            string license,
            IPaginator paginator);
        Task<IUserResult> Update(
            int id,
            IRequest request,
            string baseUrl,
            string emailTemplatePath);
        Task<IResult> Delete(int id, string license);
        Task<IResult> RequestPasswordReset(
            IRequest request,
            string baseUrl,
            string emailTemplatePath);
        Task<IInitiatePasswordResetResult> InitiatePasswordReset(
            string token,
            string license);
        Task<IUserResult> GetUserByPasswordToken(string token);
        Task<ILicenseResult> GetAppLicenseByPasswordToken(string token);
        Task<IResult> UpdatePassword(
            IRequest request,
            string license);
        Task<IResult> AddUserRoles(
            int userid,
            List<int> roleIds,
            string license);
        Task<IResult> RemoveUserRoles(
            int userid,
            List<int> roleIds,
            string license);
        Task<IResult> Activate(int id);
        Task<IResult> Deactivate(int id);
        Task<IConfirmEmailResult> ConfirmEmail(
            string token,
            string baseUrl,
            string emailtTemplatePath);
        Task<IResult> ResendPasswordReset(
            int userId,
            int appId,
            string baseUrl,
            string emailTamplatePath);
        Task<IUserResult> ResendEmailConfirmation(
            int userId,
            int appId,
            string baseUrl,
            string emailTemplatePath,
            string license);
        Task<IUserResult> CancelEmailConfirmationRequest(int id, int appId);
        Task<IUserResult> CancelPasswordResetRequest(int id, int appId);
        Task<IUserResult> CancelAllEmailRequests(int id, int appId);
    }
}
