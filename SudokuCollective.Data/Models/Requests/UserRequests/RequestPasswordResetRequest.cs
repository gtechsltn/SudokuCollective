using System;
using System.ComponentModel.DataAnnotations;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Core.Messages;
using SudokuCollective.Core.Validation.Attributes;

namespace SudokuCollective.Data.Models.Requests
{
    public class RequestPasswordResetRequest : IRequestPasswordResetRequest
    {
        private string _license = string.Empty;
        private string _email = string.Empty;
        private readonly GuidValidatedAttribute _guidValidator = new();
        private readonly EmailValidatedAttribute _emailValidator = new();

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
        [Required, EmailValidated(ErrorMessage = AttributeMessages.InvalidEmail)]
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && _emailValidator.IsValid(value))
                {
                    _email = value;
                }
                else
                {
                    throw new ArgumentException(AttributeMessages.InvalidEmail);
                }
            }
        }

        public RequestPasswordResetRequest() { }

        public RequestPasswordResetRequest(string license, string email)
        {
            License = license;
            Email = email;
        }
    }
}
