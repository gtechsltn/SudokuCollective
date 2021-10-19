using SudokuCollective.Core.Interfaces.Models.DomainObjects.Results;

namespace SudokuCollective.Data.Models.Results
{
    public class ConfirmEmailResult : IConfirmEmailResult
    {
        public string UserName { get; set; }
        public string AppTitle { get; set; }
        public string Url { get; set; }
        public bool? IsUpdate { get; set; }
        public bool? NewEmailAddressConfirmed { get; set; }
        public bool? ConfirmationEmailSuccessfullySent { get; set; }

        public ConfirmEmailResult()
        {
            UserName = string.Empty;
            AppTitle = string.Empty;
            Url = string.Empty;
            IsUpdate = null;
            NewEmailAddressConfirmed = null;
            ConfirmationEmailSuccessfullySent = null;
        }

        public ConfirmEmailResult(
            string userName, 
            string appTitle, 
            string url, 
            bool? isUpdate, 
            bool? newEmailAddressConfirmed, 
            bool? confirmationEmailSuccessfullySent)
        {
            UserName = userName;
            AppTitle = appTitle;
            Url = url;
            IsUpdate = isUpdate;
            NewEmailAddressConfirmed = newEmailAddressConfirmed;
            ConfirmationEmailSuccessfullySent = confirmationEmailSuccessfullySent;
        }
    }
}
