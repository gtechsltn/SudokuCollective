namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads
{
    public interface IPasswordResetPayload
    {
        int UserId { get; set; }
        string NewPassword { get; set; }
    }
}
