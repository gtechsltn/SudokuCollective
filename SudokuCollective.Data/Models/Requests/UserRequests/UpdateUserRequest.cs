using System;
using System.ComponentModel.DataAnnotations;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Core.Messages;
using SudokuCollective.Core.Validation.Attributes;

namespace SudokuCollective.Data.Models.Requests
{
    public class UpdateUserRequest : IUpdateUserRequest
    {
        private string _email = string.Empty;
        private readonly EmailValidatedAttribute _emailValidator = new();

        [Required]
        public string UserName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string NickName { get; set; }
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

        public UpdateUserRequest()
        {
            UserName = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            NickName = string.Empty;
        }

        public UpdateUserRequest(
            string userName, 
            string firstName, 
            string lastName, 
            string nickName, 
            string email)
        {
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            NickName = nickName;
            Email = email;
        }
    }
}
