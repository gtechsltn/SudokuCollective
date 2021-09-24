namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests.UserRequests
{
    public interface IUpdateUserRequest : IDomainObject
    {
        string UserName { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string NickName { get; set; }
        string Email { get; set; }
    }
}
