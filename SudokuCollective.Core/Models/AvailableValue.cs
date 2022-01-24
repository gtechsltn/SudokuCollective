using System.ComponentModel.DataAnnotations;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;

namespace SudokuCollective.Core.Models
{
    public class AvailableValue : IAvailableValue
    {
        [Required]
        public int Value { get; set; }
        [Required]
        public bool Available { get; set; }
    }
}
