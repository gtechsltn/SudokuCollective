using System.Threading.Tasks;
using SudokuCollective.Core.Interfaces.DataModels;
using SudokuCollective.Core.Interfaces.Models;

namespace SudokuCollective.Core.Interfaces.Repositories
{
    public interface IPasswordResetsRepository<TEntity> where TEntity : IDomainEntity
    {
        Task<IRepositoryResponse> Create(TEntity entity);
        Task<IRepositoryResponse> Get(string token);
        Task<IRepositoryResponse> GetAll();
        Task<IRepositoryResponse> Update(TEntity entity);
        Task<IRepositoryResponse> Delete(TEntity entity);
        Task<bool> HasEntity(int id);
        Task<bool> HasOutstandingPasswordReset(int userId, int appid);
        Task<IRepositoryResponse> RetrievePasswordReset(int userId, int appid);
    }
}
