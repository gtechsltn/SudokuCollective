using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;

namespace SudokuCollective.Data.Models.Requests
{
    public class ConfirmUserNameRequest : IConfirmUserNameRequest
    {
        public string Email { get; set; }
        public string License { get; set; }

        public ConfirmUserNameRequest()
        {
            Email = string.Empty;
            License = string.Empty;
        }

        public ConfirmUserNameRequest(string email, string license)
        {
            Email = email;
            License = license;
        }
    }
}
