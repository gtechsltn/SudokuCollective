using System.Collections.Generic;
using System.Linq;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;

namespace SudokuCollective.Data.Models.Requests
{
    public class UpdateGameRequest : IUpdateGameRequest
    {
        public int GameId { get; set; }
        public List<ISudokuCell> SudokuCells { get; set; }

        public UpdateGameRequest()
        {
            GameId = 0;
            SudokuCells = new List<ISudokuCell>();;
        }

        public UpdateGameRequest(int gameId, ISudokuCell[] sudokuCells)
        {
            GameId = gameId;
            SudokuCells = sudokuCells.ToList();;
        }

        public UpdateGameRequest(int gameId, List<ISudokuCell> sudokuCells)
        {
            GameId = gameId;
            SudokuCells = sudokuCells;
        }
    }
}
