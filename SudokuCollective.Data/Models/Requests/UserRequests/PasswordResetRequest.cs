using System;
using System.ComponentModel.DataAnnotations;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Core.Messages;
using SudokuCollective.Core.Validation.Attributes;

namespace SudokuCollective.Data.Models.Requests
{
    public class PasswordResetRequest : IPasswordResetRequest
    {
        private string _newPassword = string.Empty;
        private readonly PasswordValidatedAttribute _passwordValidator = new();

        [Required]
        public int UserId { get; set; }
        [Required, PasswordValidated(ErrorMessage = AttributeMessages.InvalidPassword)]
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
                    throw new ArgumentException(AttributeMessages.InvalidPassword);
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
