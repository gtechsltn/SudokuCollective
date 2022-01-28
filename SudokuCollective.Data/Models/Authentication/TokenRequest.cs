using System;
using System.ComponentModel.DataAnnotations;
using SudokuCollective.Core.Interfaces.Models.TokenModels;
using SudokuCollective.Core.Messages;
using SudokuCollective.Core.Validation.Attributes;

namespace SudokuCollective.Data.Models.Authentication
{
    public class TokenRequest : ITokenRequest
    {
        private string _userName = string.Empty;
        private string _password = string.Empty;
        private string _license = string.Empty;
        private readonly UserNameValidatedAttribute _userNameValidatedAttribute = new();
        private readonly PasswordValidatedAttribute _passwordValidator = new();
        private readonly GuidValidatedAttribute _guidValidator = new();

        [Required, UserNameValidated(ErrorMessage = AttributeMessages.InvalidUserName)]
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && _userNameValidatedAttribute.IsValid(value))
                {
                    _userName = value;
                }
                else
                {
                    throw new ArgumentException(AttributeMessages.InvalidUserName);
                }
            }
        }

        [Required, PasswordValidated(ErrorMessage = AttributeMessages.InvalidPassword)]
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && _passwordValidator.IsValid(value))
                {
                    _password = value;
                }
                else
                {
                    throw new ArgumentException(AttributeMessages.InvalidPassword);
                }
            }
        }

        [Required, GuidValidated(ErrorMessage = AttributeMessages.InvalidLicense)]
        public string License
        {
            get
            {
                return _license;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && _guidValidator.IsValid(value))
                {
                    _license = value;
                }
                else
                {
                    throw new ArgumentException(AttributeMessages.InvalidLicense);
                }
            }
        }

        public TokenRequest() { }

        public TokenRequest(string userName, string password, string license)
        {
            UserName = userName;
            Password = password;
            License = license;
        }
    }
}
