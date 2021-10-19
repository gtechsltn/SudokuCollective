using SudokuCollective.Core.Interfaces.Models.DomainObjects.Results;

namespace SudokuCollective.Data.Models.Results
{
    public class AuthenticatedUserNameResult : IAuthenticatedUserNameResult
    {
        public string UserName { get; set; }

        public AuthenticatedUserNameResult()
        {
            UserName = string.Empty;
        }

        public AuthenticatedUserNameResult(string userName)
        {
            UserName = userName;
        }
    }
}
