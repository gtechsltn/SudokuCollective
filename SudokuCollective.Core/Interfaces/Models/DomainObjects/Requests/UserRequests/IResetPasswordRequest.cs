namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests.UserRequests
{
    public interface IResetPasswordRequest : IDomainObject
    {
        string Token { get; set; }
        string NewPassword { get; set; }
    }
}
