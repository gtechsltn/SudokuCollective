using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;

namespace SudokuCollective.Data.Models.Requests
{
    public class UpdateUserRequest : IUpdateUserRequest
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public string Email { get; set; }

        public UpdateUserRequest()
        {
            UserName = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            NickName = string.Empty;
            Email = string.Empty;
        }

        public UpdateUserRequest(
            string userName, 
            string firstName, 
            string lastName, 
            string nickName, 
            string email)
        {
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            NickName = nickName;
            Email = email;
        }
    }
}