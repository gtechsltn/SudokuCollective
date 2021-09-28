namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests
{
    public interface IConfirmUserNameRequest
    {
        string Email { get; set; }
        string License { get; set; }
    }
}
