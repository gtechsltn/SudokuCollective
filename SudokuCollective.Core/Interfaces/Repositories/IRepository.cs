using System.Collections.Generic;
using System.Threading.Tasks;
using SudokuCollective.Core.Interfaces.ServiceModels;
using SudokuCollective.Core.Interfaces.Models;

namespace SudokuCollective.Core.Interfaces.Repositories
{
    public interface IRepository<TEntity> where TEntity : IDomainEntity
    {
        Task<IRepositoryResponse> Add(TEntity entity);
        Task<IRepositoryResponse> Get(int id);
        Task<IRepositoryResponse> GetAll();
        Task<IRepositoryResponse> Update(TEntity entity);
        Task<IRepositoryResponse> UpdateRange(List<TEntity> entities);
        Task<IRepositoryResponse> Delete(TEntity entity);
        Task<IRepositoryResponse> DeleteRange(List<TEntity> entities);
        Task<bool> HasEntity(int id);
    }
}
