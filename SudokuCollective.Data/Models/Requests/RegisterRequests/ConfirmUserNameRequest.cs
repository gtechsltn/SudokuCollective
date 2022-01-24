using System.ComponentModel.DataAnnotations;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Core.Validation.Attributes;

namespace SudokuCollective.Data.Models.Requests
{
    public class ConfirmUserNameRequest : IConfirmUserNameRequest
    {
        private string _email = string.Empty;
        private string _license = string.Empty;
        private readonly EmailValidatedAttribute _emailRegexAttribute = new();
        private readonly GuidValidatedAttribute _guidRegexAttribute = new();

        [Required, EmailValidated(ErrorMessage = "Email must be in a valid format")]
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                if (value != null && _emailRegexAttribute.IsValid(value))
                {
                    _email = value;
                }
            }
        }
        [Required, GuidValidated(ErrorMessage = "License must be in the pattern of d36ddcfd-5161-4c20-80aa-b312ef161433 with hexadecimal characters")]
        public string License
        {
            get
            {
                return _license;
            }
            set
            {
                if (value != null && _guidRegexAttribute.IsValid(value))
                {
                    _license = value;
                }
            }
        }

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
