using System.Collections.Generic;
using System.Threading.Tasks;
using SudokuCollective.Core.Interfaces.ServiceModels;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;

namespace SudokuCollective.Core.Interfaces.Repositories
{
    public interface IUsersRepository<TEntity> : IRepository<TEntity> where TEntity : IUser
    {
        Task<IRepositoryResponse> GetByUserName(string username);
        Task<IRepositoryResponse> GetByEmail(string email);
        Task<IRepositoryResponse> AddRole(int userId, int roleId);
        Task<IRepositoryResponse> AddRoles(int userId, List<int> roleIds);
        Task<IRepositoryResponse> RemoveRole(int userId, int roleId);
        Task<IRepositoryResponse> RemoveRoles(int userId, List<int> roleIds);
        Task<IRepositoryResponse> ConfirmEmail(IEmailConfirmation emailConfirmation);
        Task<IRepositoryResponse> UpdateEmail(IEmailConfirmation emailConfirmation);
        Task<IRepositoryResponse> GetMyApps(int id);
        Task<string> GetAppLicense(int appId);
        Task<bool> Activate(int id);
        Task<bool> Deactivate(int id);
        Task<bool> PromoteToAdmin(int id);
        Task<bool> IsUserRegistered(int id);
        Task<bool> IsUserNameUnique(string username);
        Task<bool> IsEmailUnique(string email);
        Task<bool> IsUpdatedUserNameUnique(int userId, string username);
        Task<bool> IsUpdatedEmailUnique(int userId, string email);
    }
}
