using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Results;
using SudokuCollective.Core.Models;

namespace SudokuCollective.Data.Models.Results
{
    public class InitiatePasswordResetResult : IInitiatePasswordResetResult
    {
        public IApp App { get; set; }
        public IUser User { get; set; }
        public bool? ConfirmationEmailSuccessfullySent { get; set; }
        public string Token { get; set; }

        public InitiatePasswordResetResult()
        {
            App = new App();
            User = new User();
            ConfirmationEmailSuccessfullySent = null;
            Token = string.Empty;
        }

        public InitiatePasswordResetResult(
            IApp app, 
            IUser user, 
            bool? confirmationEmailSuccessfullySent, 
            string token)
        {
            App = app;
            User = user;
            ConfirmationEmailSuccessfullySent = confirmationEmailSuccessfullySent;
            Token = token;
        }
    }
}
