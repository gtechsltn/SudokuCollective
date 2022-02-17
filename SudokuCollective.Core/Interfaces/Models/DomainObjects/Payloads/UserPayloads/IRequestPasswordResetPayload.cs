namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads
{
    public interface IRequestPasswordResetPayload
    {
        string License { get; set; }
        string Email { get; set; }
    }
}
