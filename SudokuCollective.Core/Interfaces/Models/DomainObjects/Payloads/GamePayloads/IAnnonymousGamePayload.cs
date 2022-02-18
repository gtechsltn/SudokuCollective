using SudokuCollective.Core.Enums;

namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads
{
    public interface IAnnonymousGamePayload : IPayload
    {
        DifficultyLevel DifficultyLevel { get; set; }
    }
}
