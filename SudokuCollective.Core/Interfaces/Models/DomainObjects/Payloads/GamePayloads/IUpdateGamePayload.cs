using System.Collections.Generic;
using SudokuCollective.Core.Models;

namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads
{
    public interface IUpdateGamePayload : IPayload
    {
        int GameId { get; set; }
        List<SudokuCell> SudokuCells { get; set; }
    }
}
