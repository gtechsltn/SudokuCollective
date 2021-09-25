namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests
{
    public interface ICreateGameRequest : IDomainObject
    {
        int UserId { get; set; }
        int DifficultyId { get; set; }
    }
}
