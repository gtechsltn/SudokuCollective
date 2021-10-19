using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;

namespace SudokuCollective.Data.Models.Requests
{
    public class ResetPasswordRequest : IResetPasswordRequest
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }

        public ResetPasswordRequest()
        {
            Token = string.Empty;
            NewPassword = string.Empty;
        }

        public ResetPasswordRequest(string token, string newPassword)
        {
            Token = token;
            NewPassword = newPassword;
        }
    }
}
