using System.Collections.Generic;

namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Results
{
    public interface IAuthenticatedUserNameResult : IDomainObject
    {
        public List<List<int>> SudokuMatrix { get; set; }
    }
}
