namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests
{
    public interface IPasswordResetRequest
    {
        int UserId { get; set; }
        string NewPassword { get; set; }
    }
}
