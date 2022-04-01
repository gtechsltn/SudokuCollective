using System.Collections.Generic;
using System.Threading.Tasks;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;

namespace SudokuCollective.Core.Interfaces.Services
{
    public interface IGamesService : IService
    {
        Task<IResult> Create(IRequest request);
        Task<IResult> Update(int id, IRequest request);
        Task<IResult> Delete(int id);
        Task<IResult> GetGame(int id, int appId);
        Task<IResult> GetGames(IRequest request);
        Task<IResult> GetMyGame(int id, IRequest request);
        Task<IResult> GetMyGames(IRequest request);
        Task<IResult> DeleteMyGame(int id, IRequest request);
        Task<IResult> Check(int id, IRequest request);
        Task<IResult> CreateAnnonymous(DifficultyLevel difficultyLevel);
        Task<IResult> CheckAnnonymous(List<int> intList);
    }
}
