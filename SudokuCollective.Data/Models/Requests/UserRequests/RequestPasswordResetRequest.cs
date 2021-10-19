using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;

namespace SudokuCollective.Data.Models.Requests
{
    public class RequestPasswordResetRequest : IRequestPasswordResetRequest
    {
        public string License { get; set; }
        public string Email { get; set; }

        public RequestPasswordResetRequest()
        {
            License = string.Empty;
            Email = string.Empty;
        }

        public RequestPasswordResetRequest(string license, string email)
        {
            License = license;
            Email = email;
        }
    }
}
