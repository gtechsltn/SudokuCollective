namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests
{
    public interface IUpdatePasswordRequest
    {
        int UserId { get; set; }
        string NewPassword { get; set; }
        string License { get; set; }
    }
}
