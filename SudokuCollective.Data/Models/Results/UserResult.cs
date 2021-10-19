using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Results;
using SudokuCollective.Core.Models;

namespace SudokuCollective.Data.Models.Results
{
    public class UserResult : IUserResult
    {
        public IUser User { get; set; }
        public bool? ConfirmationEmailSuccessfullySent { get; set; }
        public string Token { get; set; }

        public UserResult()
        {
            User = new User();
            ConfirmationEmailSuccessfullySent = null;
            Token = string.Empty;
        }

        public UserResult(IUser user, bool? confirmationEmailSuccessfullySent, string token)
        {
            User = user;
            ConfirmationEmailSuccessfullySent = confirmationEmailSuccessfullySent;
            Token = token;
        }
    }
}
