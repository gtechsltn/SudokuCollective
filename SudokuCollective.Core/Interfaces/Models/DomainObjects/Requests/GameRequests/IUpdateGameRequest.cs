using System.Collections.Generic;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;

namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests
{
    public interface IUpdateGameRequest : IDomainObject
    {
        int GameId { get; set; }
        List<ISudokuCell> SudokuCells { get; set; }
    }
}
