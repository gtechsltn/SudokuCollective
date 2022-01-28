using System.Collections.Generic;
using SudokuCollective.Core.Models;

namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests
{
    public interface IUpdateGameRequest
    {
        int GameId { get; set; }
        List<SudokuCell> SudokuCells { get; set; }
    }
}
