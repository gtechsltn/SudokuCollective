using System;
using System.ComponentModel.DataAnnotations;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Core.Messages;
using SudokuCollective.Core.Validation.Attributes;

namespace SudokuCollective.Data.Models.Requests
{
    public class ConfirmUserNameRequest : IConfirmUserNameRequest
    {
        private string _email = string.Empty;
        private string _license = string.Empty;
        private readonly EmailValidatedAttribute _emailRegexAttribute = new();
        private readonly GuidValidatedAttribute _guidRegexAttribute = new();

        [Required, EmailValidated(ErrorMessage = AttributeMessages.InvalidEmail)]
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && _emailRegexAttribute.IsValid(value))
                {
                    _email = value;
                }
                else
                {
                    throw new ArgumentException(AttributeMessages.InvalidEmail);
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
                if (!string.IsNullOrEmpty(value) && _guidRegexAttribute.IsValid(value))
                {
                    _license = value;
                }
                else
                {
                    throw new ArgumentException(AttributeMessages.InvalidLicense);
                }
            }
        }

        public ConfirmUserNameRequest() { }

        public ConfirmUserNameRequest(string email, string license)
        {
            Email = email;
            License = license;
        }
    }
}
