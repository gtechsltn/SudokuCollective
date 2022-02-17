using SudokuCollective.Core.Enums;

namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads
{
    public interface IAnnonymousGamePayload
    {
        DifficultyLevel DifficultyLevel { get; set; }
    }
}
