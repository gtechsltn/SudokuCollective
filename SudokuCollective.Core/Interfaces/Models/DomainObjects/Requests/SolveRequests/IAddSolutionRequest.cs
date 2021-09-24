namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests.SolveRequests
{
    public interface IAddSolutionRequest : IDomainObject
    {
        int Limit { get; set; }
    }
}
