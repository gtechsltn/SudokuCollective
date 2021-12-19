using System;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Results;

namespace SudokuCollective.Data.Models.Results
{
    public class ConfirmEmailResult : IConfirmEmailResult
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime DateUpdated { get; set; }
        public string AppTitle { get; set; }
        public string Url { get; set; }
        public bool? IsUpdate { get; set; }
        public bool? NewEmailAddressConfirmed { get; set; }
        public bool? ConfirmationEmailSuccessfullySent { get; set; }

        public ConfirmEmailResult()
        {
            UserName = string.Empty;
            Email = string.Empty;
            DateUpdated = DateTime.MinValue;
            AppTitle = string.Empty;
            Url = string.Empty;
            IsUpdate = null;
            NewEmailAddressConfirmed = null;
            ConfirmationEmailSuccessfullySent = null;
        }

        public ConfirmEmailResult(
            string userName,
            string email,
            DateTime dateUpdated,
            string appTitle, 
            string url, 
            bool? isUpdate, 
            bool? newEmailAddressConfirmed, 
            bool? confirmationEmailSuccessfullySent)
        {
            UserName = userName;
            Email = email;
            DateUpdated = dateUpdated;
            AppTitle = appTitle;
            Url = url;
            IsUpdate = isUpdate;
            NewEmailAddressConfirmed = newEmailAddressConfirmed;
            ConfirmationEmailSuccessfullySent = confirmationEmailSuccessfullySent;
        }
    }
}
