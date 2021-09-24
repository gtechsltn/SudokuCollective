namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests.DifficultyRequests
{
    public interface IUpdateDifficultyRequest : IDomainObject
    {
        int Id { get; set; }
        string Name { get; set; }
        string DisplayName { get; set; }
    }
}
