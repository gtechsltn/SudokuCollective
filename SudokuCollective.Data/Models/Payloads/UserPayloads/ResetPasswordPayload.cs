using System;
using System.ComponentModel.DataAnnotations;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads;
using SudokuCollective.Core.Messages;
using SudokuCollective.Core.Validation.Attributes;

namespace SudokuCollective.Data.Models.Payloads
{
    public class ResetPasswordPayload : IResetPasswordPayload
    {
        private string _token = string.Empty;
        private string _newPassword = string.Empty;
        private readonly GuidValidatedAttribute _guidValidator = new();
        private readonly PasswordValidatedAttribute _passwordValidator = new();

        [Required, GuidValidated(ErrorMessage = AttributeMessages.InvalidToken)]
        public string Token
        {
            get
            {
                return _token;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && _guidValidator.IsValid(value))
                {
                    _token = value;
                }
                else
                {
                    throw new ArgumentException(AttributeMessages.InvalidToken);
                }
            }
        }
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

        public ResetPasswordPayload() {}

        public ResetPasswordPayload(string token, string newPassword)
        {
            Token = token;
            NewPassword = newPassword;
        }
    }
}
