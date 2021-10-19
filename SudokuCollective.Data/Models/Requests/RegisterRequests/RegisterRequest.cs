using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;

namespace SudokuCollective.Data.Models.Requests
{
    public class RegisterRequest : IRegisterRequest
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public RegisterRequest()
        {
            UserName = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            NickName = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
        }

        public RegisterRequest(
            string userName, 
            string firstName, 
            string lastName, 
            string nickName, 
            string email, 
            string password)
        {
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            NickName = nickName;
            Email = email;
            Password = password;
        }
    }
}
