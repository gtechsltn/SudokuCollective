using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Results;
using SudokuCollective.Core.Models;

namespace SudokuCollective.Data.Models.Results
{
    public class AuthenticationResult : IAuthenticationResult
    {
        public IAuthenticatedUser User { get; set; }
        public string Token { get; set; }

        public AuthenticationResult()
        {
            User = new AuthenticatedUser();
            Token = string.Empty;
        }

        public AuthenticationResult(IAuthenticatedUser user, string token)
        {
            User = user;
            Token = token;
        }
    }
}
