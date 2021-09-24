namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests.UserRequests
{
    public interface IPasswordResetRequest : IDomainObject
    {
        int UserId { get; set; }
        string NewPassword { get; set; }
    }
}
