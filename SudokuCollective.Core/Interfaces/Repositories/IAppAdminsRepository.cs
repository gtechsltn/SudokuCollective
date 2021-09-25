using System.Threading.Tasks;
using SudokuCollective.Core.Interfaces.DataModels;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;

namespace SudokuCollective.Core.Interfaces.Repositories
{
    public interface IAppAdminsRepository<TEntity> : IRepository<TEntity> where TEntity : IAppAdmin
    {
        Task<bool> HasAdminRecord(int appId, int userId);
        Task<IRepositoryResponse> GetAdminRecord(int appId, int userId);
    }
}
