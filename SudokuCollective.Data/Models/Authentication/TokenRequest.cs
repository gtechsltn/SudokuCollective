using System.ComponentModel.DataAnnotations;
using SudokuCollective.Core.Interfaces.Models.TokenModels;
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

        [Required, UserNameValidated(ErrorMessage = "User name must be at least 4 characters and can contain alphanumeric characters and special characters of [! @ # $ % ^ & * + = ? - _ . ,]")]
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
            }
        }

        [Required, PasswordValidated(ErrorMessage = "Password must be between 4 and up to 20 characters with at least 1 capital letter, 1 lower case letter, and 1 special character of [! @ # $ % ^ & * + = ? - _ . ,]")]
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
            }
        }

        [Required, GuidValidated(ErrorMessage = "Guid must be in the pattern of d36ddcfd-5161-4c20-80aa-b312ef161433 with hexadecimal characters")]
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
