namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests
{
    public interface IAddSolutionRequest : IDomainObject
    {
        int Limit { get; set; }
    }
}
