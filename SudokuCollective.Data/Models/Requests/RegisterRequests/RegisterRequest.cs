using System;
using System.ComponentModel.DataAnnotations;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Core.Validation.Attributes;

namespace SudokuCollective.Data.Models.Requests
{
    public class RegisterRequest : IRegisterRequest
    {
        private string _userName = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private readonly UserNameValidatedAttribute _userNameValidator = new();
        private readonly EmailValidatedAttribute _emailValidator = new();
        private readonly PasswordValidatedAttribute _passwordValidator = new();

        [Required, UserNameValidated(ErrorMessage = "User name must be at least 4 characters and can contain alphanumeric characters and special characters of [! @ # $ % ^ & * + = ? - _ . ,]")]
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && _userNameValidator.IsValid(value))
                {
                    _userName = value;
                }
                else
                {
                    throw new ArgumentException("User name must be at least 4 characters and can contain alphanumeric characters and special characters of [! @ # $ % ^ & * + = ? - _ . ,]");
                }
            }
        }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string NickName { get; set; }
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
                else
                {
                    throw new ArgumentException("Password must be between 4 and up to 20 characters with at least 1 capital letter, 1 lower case letter, and 1 special character of [! @ # $ % ^ & * + = ? - _ . ,]");
                }
            }
        }

        public RegisterRequest()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            NickName = string.Empty;
        }

        public RegisterRequest(
            string userName, 
            string firstName, 
            string lastName, 
            string nickName, 
            string email, 
            string password)
        {
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            NickName = nickName;
            Email = email;
            Password = password;
        }
    }
}
