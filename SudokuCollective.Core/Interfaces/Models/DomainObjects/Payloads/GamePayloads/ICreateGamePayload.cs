namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads
{
    public interface ICreateGamePayload
    {
        int UserId { get; set; }
        int DifficultyId { get; set; }
    }
}
