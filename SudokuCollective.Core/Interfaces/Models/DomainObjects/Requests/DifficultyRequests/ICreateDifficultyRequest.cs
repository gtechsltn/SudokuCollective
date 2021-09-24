using SudokuCollective.Core.Enums;

namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests.DifficultyRequests
{
    public interface ICreateDifficultyRequest : IDomainObject
    {
        string Name { get; set; }
        string DisplayName { get; set; }
        DifficultyLevel DifficultyLevel { get; set; }
    }
}
