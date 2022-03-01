using System.Text.Json.Serialization;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Results;
using SudokuCollective.Core.Models;

namespace SudokuCollective.Data.Models.Results
{
    public class UserCreatedResult : IUserCreatedResult
    {
        [JsonIgnore]
        IAuthenticatedUser IUserCreatedResult.User 
        {
            get => (IAuthenticatedUser)User;
            set 
            {
                User = (AuthenticatedUser)value;
            }
        }
        public AuthenticatedUser User { get; set; }
        public string Token { get; set; }
        public bool EmailConfirmationSent { get; set; }

        public UserCreatedResult()
        {
            User = new AuthenticatedUser();
            Token = string.Empty;
            EmailConfirmationSent = false;
        }

        public UserCreatedResult(
            IAuthenticatedUser user, 
            string token,
            bool emailConfirmationSent)
        {
            User = (AuthenticatedUser)user;
            Token = token;
            EmailConfirmationSent = emailConfirmationSent;
        }

        public UserCreatedResult(
            AuthenticatedUser user, 
            string token,
            bool emailConfirmationSent)
        {
            User = user;
            Token = token;
            EmailConfirmationSent = emailConfirmationSent;
        }
    }
}
