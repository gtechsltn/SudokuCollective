using SudokuCollective.Core.Enums;

namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests.GameRequests
{
    public interface IAnnonymousGameRequest : IDomainObject
    {
        DifficultyLevel DifficultyLevel { get; set; }
    }
}
