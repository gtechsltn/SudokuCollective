using System.Threading.Tasks;
using SudokuCollective.Core.Interfaces.DataModels;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;

namespace SudokuCollective.Core.Interfaces.Repositories
{
    public interface IEmailConfirmationsRepository<TEntity> where TEntity : IEmailConfirmation
    {
        Task<IRepositoryResponse> Create(TEntity entity);
        Task<IRepositoryResponse> Get(string token);
        Task<IRepositoryResponse> GetAll();
        Task<IRepositoryResponse> Update(TEntity entity);
        Task<IRepositoryResponse> Delete(TEntity entity);
        Task<bool> HasEntity(int id);
        Task<bool> HasOutstandingEmailConfirmation(int userId, int appid);
        Task<IRepositoryResponse> RetrieveEmailConfirmation(int userId, int appid);
    }
}
