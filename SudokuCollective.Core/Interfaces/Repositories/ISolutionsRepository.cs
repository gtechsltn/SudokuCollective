using System.Collections.Generic;
using System.Threading.Tasks;
using SudokuCollective.Core.Interfaces.DataModels;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;

namespace SudokuCollective.Core.Interfaces.Repositories
{
    public interface ISolutionsRepository<TEntity> : IRepository<TEntity> where TEntity : ISudokuSolution
    {
        Task<IRepositoryResponse> AddSolutions(List<ISudokuSolution> solutions);
        Task<IRepositoryResponse> GetSolvedSolutions();
    }
}
