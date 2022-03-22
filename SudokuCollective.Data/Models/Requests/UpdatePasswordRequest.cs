using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Core.Messages;
using SudokuCollective.Core.Validation.Attributes;

namespace SudokuCollective.Data.Models.Requests
{
    public class UpdatePasswordRequest : IUpdatePasswordRequest
    {
        private string _newPassword = string.Empty;
        private string _license = string.Empty;
        private readonly PasswordValidatedAttribute _passwordValidator = new();
        private readonly GuidValidatedAttribute _guidValidator = new();

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

        public UpdatePasswordRequest() {}

        public UpdatePasswordRequest(
            int userId, 
            string newPassword,
            string license)
        {
            UserId = userId;
            NewPassword = newPassword;
            License = license;
        }

        public static implicit operator JsonElement(UpdatePasswordRequest v)
        {
            return JsonSerializer.SerializeToElement(v, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }
}
