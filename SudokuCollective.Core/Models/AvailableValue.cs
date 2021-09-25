using SudokuCollective.Core.Interfaces.Models.DomainEntities;

namespace SudokuCollective.Core.Models
{
    public class AvailableValue : IAvailableValue
    {
        public int Value { get; set; }
        public bool Available { get; set; }
    }
}
