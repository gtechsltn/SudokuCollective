using System.Threading.Tasks;
using SudokuCollective.Core.Interfaces.DataModels;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;

namespace SudokuCollective.Core.Interfaces.Repositories
{
    public interface IGamesRepository<TEntity> : IRepository<TEntity> where TEntity : IGame
    {
        Task<IRepositoryResponse> GetAppGame(int id, int appid);
        Task<IRepositoryResponse> GetAppGames(int appid);
        Task<IRepositoryResponse> GetMyGame(int userid, int gameid, int appid);
        Task<IRepositoryResponse> GetMyGames(int userid, int appid);
        Task<IRepositoryResponse> DeleteMyGame(int userid, int gameid, int appid);
    }
}
