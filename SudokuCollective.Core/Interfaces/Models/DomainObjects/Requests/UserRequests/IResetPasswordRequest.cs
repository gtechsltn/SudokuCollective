namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests
{
    public interface IResetPasswordRequest : IDomainObject
    {
        string Token { get; set; }
        string NewPassword { get; set; }
    }
}
