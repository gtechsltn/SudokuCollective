namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests.GameRequests
{
    public interface IGamesRequest : IDomainObject
    {
        int UserId { get; set; }
    }
}
