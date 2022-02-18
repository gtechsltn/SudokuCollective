namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads
{
    public interface ICreateGamePayload : IPayload
    {
        int UserId { get; set; }
        int DifficultyId { get; set; }
    }
}
