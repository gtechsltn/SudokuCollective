using System.Threading.Tasks;
using SudokuCollective.Core.Interfaces.DataModels;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;

namespace SudokuCollective.Core.Interfaces.Repositories
{
    public interface IAppsRepository<TEntity> : IRepository<TEntity> where TEntity : IApp
    {
        Task<IRepositoryResponse> GetByLicense(string license);
        Task<IRepositoryResponse> GetAppUsers(int id);
        Task<IRepositoryResponse> GetNonAppUsers(int id);
        Task<IRepositoryResponse> GetMyApps(int ownerId);
        Task<IRepositoryResponse> GetMyRegisteredApps(int userId);
        Task<IRepositoryResponse> Reset(TEntity entity);
        Task<IRepositoryResponse> AddAppUser(int userId, string license);
        Task<IRepositoryResponse> RemoveAppUser(int userId, string license);
        Task<IRepositoryResponse> Activate(int id);
        Task<IRepositoryResponse> Deactivate(int id);
        Task<bool> IsAppLicenseValid(string license);
        Task<bool> IsUserRegisteredToApp(int id, string license, int userId);
        Task<bool> IsUserOwnerOfApp(int id, string license, int userId);
        Task<string> GetLicense(int id);
    }
}
