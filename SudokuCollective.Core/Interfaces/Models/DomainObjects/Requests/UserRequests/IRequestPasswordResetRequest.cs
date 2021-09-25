namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests
{
    public interface IRequestPasswordResetRequest : IDomainObject
    {
        string License { get; set; }
        string Email { get; set; }
    }
}
