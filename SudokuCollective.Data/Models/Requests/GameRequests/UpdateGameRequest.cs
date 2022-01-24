using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Core.Validation.Attributes;

namespace SudokuCollective.Data.Models.Requests
{
    public class UpdateGameRequest : IUpdateGameRequest
    {
        private List<ISudokuCell> _sudokuCells = new();
        private readonly SudokuCellsValidatedAttribute _sudokuCellsValidator = new();

        [Required]
        public int GameId { get; set; }
        [Required, SudokuCellsValidated(ErrorMessage = "The list of sudoku values is not valid")]
        public List<ISudokuCell> SudokuCells
        {
            get
            {
                return _sudokuCells;
            }
            set
            {
                if (value != null && _sudokuCellsValidator.IsValid(value))
                {
                    _sudokuCells = value;
                }
            }
        }
        public UpdateGameRequest()
        {
            GameId = 0;
            SudokuCells = new List<ISudokuCell>(); ;
        }

        public UpdateGameRequest(int gameId, ISudokuCell[] sudokuCells)
        {
            GameId = gameId;
            SudokuCells = sudokuCells.ToList(); ;
        }

        public UpdateGameRequest(int gameId, List<ISudokuCell> sudokuCells)
        {
            GameId = gameId;
            SudokuCells = sudokuCells;
        }
    }
}
