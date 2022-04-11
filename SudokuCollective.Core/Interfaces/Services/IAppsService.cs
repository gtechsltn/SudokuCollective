using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;

namespace SudokuCollective.Core.Interfaces.Services
{
    public interface IAppsService : IService
    {
        Task<IResult> Create(ILicenseRequest request);
        Task<IResult> Get(int id);
        Task<IResult> Update(int id, IRequest request);
        Task<IResult> DeleteOrReset(int id, bool isReset = false);
        Task<IResult> GetApps(IPaginator paginator, int requestorId);
        Task<IResult> GetMyApps(int ownerId, IPaginator paginator);
        Task<IResult> GetRegisteredApps(int userId, IPaginator paginator);
        Task<IResult> GetByLicense(string license, int requestorId);
        Task<ILicenseResult> GetLicense(int id);
        Task<IResult> GetAppUsers(int id, int requestorId, IPaginator paginator, bool appUsers = true);
        Task<IResult> AddAppUser(int appId, int userId);
        Task<IResult> RemoveAppUser(int appId, int userId);
        Task<IResult> Activate(int id);
        Task<IResult> Deactivate(int id);
        Task<IResult> ActivateAdminPrivileges(int appId, int userId);
        Task<IResult> DeactivateAdminPrivileges(int appId, int userId);
        Task<bool> IsOwnerOfThisLicense(IHttpContextAccessor httpContextAccessor, string license, int appId, int userId, int requestorId);
        Task<bool> IsRequestValidOnThisLicense(IHttpContextAccessor httpContextAccessor, string license, int appId, int userId);
    }
}
