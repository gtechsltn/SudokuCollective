using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads;
using SudokuCollective.Core.Messages;
using SudokuCollective.Core.Models;
using SudokuCollective.Core.Validation.Attributes;

namespace SudokuCollective.Data.Models.Payloads
{
    public class UpdateGamePayload : IUpdateGamePayload
    {
        private List<SudokuCell> _sudokuCells = new();
        private readonly SudokuCellsValidatedAttribute _sudokuCellsValidator = new();

        [Required, JsonPropertyName("gameId")]
        public int GameId { get; set; }
        [Required, SudokuCellsValidated(ErrorMessage = AttributeMessages.InvalidSudokuCells), JsonPropertyName("sudokuCells")]
        public List<SudokuCell> SudokuCells
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
                else
                {
                    throw new ArgumentException(AttributeMessages.InvalidSudokuCells);
                }
            }
        }
        
        public UpdateGamePayload()
        {
            GameId = 0;
            SudokuCells = new List<SudokuCell>(); ;
        }

        public UpdateGamePayload(int gameId, SudokuCell[] sudokuCells)
        {
            GameId = gameId;
            SudokuCells = sudokuCells.ToList(); ;
        }

        public UpdateGamePayload(int gameId, List<SudokuCell> sudokuCells)
        {
            GameId = gameId;
            SudokuCells = sudokuCells;
        }

        public static implicit operator JsonElement(UpdateGamePayload v)
        {
            return JsonSerializer.SerializeToElement(v, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }
}
