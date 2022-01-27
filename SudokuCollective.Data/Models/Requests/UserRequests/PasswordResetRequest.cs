using System;
using System.ComponentModel.DataAnnotations;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Core.Validation.Attributes;

namespace SudokuCollective.Data.Models.Requests
{
    public class PasswordResetRequest : IPasswordResetRequest
    {
        private string _newPassword = string.Empty;
        private readonly PasswordValidatedAttribute _passwordValidator = new();

        [Required]
        public int UserId { get; set; }
        [Required, PasswordValidated(ErrorMessage = "Password must be between 4 and up to 20 characters with at least 1 capital letter, 1 lower case letter, and 1 special character of [! @ # $ % ^ & * + = ? - _ . ,]")]
        public string NewPassword
        {
            get
            {
                return _newPassword;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && _passwordValidator.IsValid(value))
                {
                    _newPassword = value;
                }
                else
                {
                    throw new ArgumentException("Password must be between 4 and up to 20 characters with at least 1 capital letter, 1 lower case letter, and 1 special character of [! @ # $ % ^ & * + = ? - _ . ,]");
                }
            }
        }

        public PasswordResetRequest()
        {
            UserId = 0;
        }

        public PasswordResetRequest(int userId, string newPassword)
        {
            UserId = userId;
            NewPassword = newPassword;
        }
    }
}
