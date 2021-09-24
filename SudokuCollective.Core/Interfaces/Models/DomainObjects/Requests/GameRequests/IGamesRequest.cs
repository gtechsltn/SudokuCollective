namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests
{
    public interface IGamesRequest : IDomainObject
    {
        int UserId { get; set; }
    }
}
