namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads
{
    public interface IResetPasswordPayload
    {
        string Token { get; set; }
        string NewPassword { get; set; }
    }
}
