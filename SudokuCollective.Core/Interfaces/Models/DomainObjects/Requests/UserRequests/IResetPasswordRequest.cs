namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests
{
    public interface IResetPasswordRequest
    {
        string Token { get; set; }
        string NewPassword { get; set; }
    }
}
