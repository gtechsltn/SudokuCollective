using System;
using System.ComponentModel.DataAnnotations;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Core.Validation.Attributes;

namespace SudokuCollective.Data.Models.Requests
{
    public class RequestPasswordResetRequest : IRequestPasswordResetRequest
    {
        private string _license = string.Empty;
        private string _email = string.Empty;
        private readonly GuidValidatedAttribute _guidValidator = new();
        private readonly EmailValidatedAttribute _emailValidator = new();

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
                else
                {
                    throw new ArgumentException("Guid must be in the pattern of d36ddcfd-5161-4c20-80aa-b312ef161433 with hexadecimal characters");
                }
            }
        }
        [Required, EmailValidated(ErrorMessage = "Email must be in a valid format")]
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
                    throw new ArgumentException("Email must be in a valid format");
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
