using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;

namespace SudokuCollective.Data.Models.Requests
{
    public class PasswordResetRequest : IPasswordResetRequest
    {
        public int UserId { get; set; }
        public string NewPassword { get; set; }

        public PasswordResetRequest()
        {
            UserId = 0;
            NewPassword = string.Empty;
        }

        public PasswordResetRequest(int userId, string newPassword)
        {
            UserId = userId;
            NewPassword = newPassword;
        }
    }
}
