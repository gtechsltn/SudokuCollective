namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests.GameRequests
{
    public interface ICreateGameRequest : IDomainObject
    {
        int UserId { get; set; }
        int DifficultyId { get; set; }
    }
}
