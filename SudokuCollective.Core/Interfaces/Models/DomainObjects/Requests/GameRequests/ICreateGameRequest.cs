namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests
{
    public interface ICreateGameRequest
    {
        int UserId { get; set; }
        int DifficultyId { get; set; }
    }
}
