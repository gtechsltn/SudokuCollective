namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests.RegisterRequests
{
    public interface IConfirmUserNameRequest : IDomainObject
    {
        string Email { get; set; }
        string License { get; set; }
    }
}
