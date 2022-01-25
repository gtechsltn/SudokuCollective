using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Validation;
using SudokuCollective.Core.Validation.Attributes;

namespace SudokuCollective.Core.Models
{
    public class User : IUser
    {
        #region Fields
        private string _userName = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private bool _isSuperUser;
        private bool _isAdmin;
        private readonly EmailValidatedAttribute _emailValidator = new();
        private readonly PasswordValidatedAttribute _passwordValidator = new();
        #endregion

        #region Properties
        [Required]
        public int Id { get; set; }
        [Required, UserNameValidated(ErrorMessage = "User name must be at least 4 characters and can contain alphanumeric characters and special characters of [! @ # $ % ^ & * + = ? - _ . ,]")]
        public string UserName
        {
            get
            {
                return _userName;
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var regex = new Regex(RegexValidators.UserNameRegexPattern);

                    if (regex.IsMatch(value))
                    {
                        _userName = value;
                    }
                }
                else
                {
                    _userName = string.Empty;
                }
            }
        }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string NickName { get; set; }
        public string FullName
        {
            get => string.Format("{0} {1}", FirstName, LastName);
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
                    IsEmailConfirmed = false;
                }
            }
        }
        [Required]
        public bool IsEmailConfirmed { get; set; }
        public bool ReceivedRequestToUpdateEmail { get; set; }
        [IgnoreDataMember, PasswordValidated(ErrorMessage = "Password must be between 4 and up to 20 characters with at least 1 capital letter, 1 lower case letter, and 1 special character of[! @ # $ % ^ & * + = ? - _ . ,]")]
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
        public bool ReceivedRequestToUpdatePassword { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public bool IsSuperUser
        {
            get
            {
                _isSuperUser = false;

                if (Roles != null)
                {
                    foreach (var role in Roles)
                    {
                        if (role.Role.RoleLevel == RoleLevel.SUPERUSER)
                        {
                            _isSuperUser = true;
                        }
                    }
                }

                return _isSuperUser;
            }

            set
            {

                _isSuperUser = value;
            }
        }
        [Required]
        public bool IsAdmin
        {
            get
            {
                _isAdmin = false;

                if (Roles != null)
                {
                    foreach (var role in Roles)
                    {
                        if (role.Role.RoleLevel == RoleLevel.ADMIN)
                        {
                            _isAdmin = true;
                        }
                    }
                }

                return _isAdmin;
            }

            set
            {
                _isAdmin = value;
            }
        }
        [Required]
        public DateTime DateCreated { get; set; }
        [Required]
        public DateTime DateUpdated { get; set; }
        [IgnoreDataMember]
        ICollection<IGame> IUser.Games
        {
            get
            {
                return Games.ConvertAll(g => (IGame)g);
            }
            set
            {
                Games = value.ToList().ConvertAll(g => (Game)g);
            }
        }
        [Required]
        public virtual List<Game> Games { get; set; }
        [IgnoreDataMember]
        ICollection<IUserRole> IUser.Roles
        {
            get
            {
                return Roles.ConvertAll(ur => (IUserRole)ur);
            }
            set
            {
                Roles = value.ToList().ConvertAll(ur => (UserRole)ur);
            }
        }
        [Required]
        public virtual List<UserRole> Roles { get; set; }
        [IgnoreDataMember]
        ICollection<IUserApp> IUser.Apps
        {
            get
            {
                return Apps.ConvertAll(a => (IUserApp)a);
            }
            set
            {
                Apps = value.ToList().ConvertAll(a => (UserApp)a);
            }
        }
        [Required]
        public virtual List<UserApp> Apps { get; set; }
        #endregion

        #region Constructors
        public User(
            string firstName,
            string lastName,
            string password) : this()
        {
            var dateUserCreated = DateTime.UtcNow;

            FirstName = firstName;
            LastName = lastName;
            Password = password;
            ReceivedRequestToUpdatePassword = false;
            DateCreated = dateUserCreated;
            IsActive = true;
            IsSuperUser = false;
            IsAdmin = false;
            IsEmailConfirmed = false;
        }

        public User()
        {
            Games = new List<Game>();
            Roles = new List<UserRole>();
            Apps = new List<UserApp>();

            Id = 0;
            UserName = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            NickName = string.Empty;
            Email = string.Empty;
            IsEmailConfirmed = false;
            ReceivedRequestToUpdateEmail = false;
            Password = string.Empty;
            ReceivedRequestToUpdatePassword = false;
            DateCreated = DateTime.MinValue;
            DateUpdated = DateTime.MinValue;
            IsActive = false;
        }

        [JsonConstructor]
        public User(
            int id,
            string userName,
            string firstName,
            string lastName,
            string nickName,
            string email,
            bool emailConfirmed,
            bool receivedRequestToUpdateEmail,
            string password,
            bool receivedRequestToUpdatePassword,
            bool isActive,
            DateTime dateCreated,
            DateTime dateUpdated)
        {
            Games = new List<Game>();
            Roles = new List<UserRole>();
            Apps = new List<UserApp>();

            Id = id;
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            NickName = nickName;
            Email = email;
            IsEmailConfirmed = emailConfirmed;
            ReceivedRequestToUpdateEmail = receivedRequestToUpdateEmail;
            Password = password;
            ReceivedRequestToUpdatePassword = receivedRequestToUpdatePassword;
            IsActive = isActive;
            DateCreated = dateCreated;
            DateUpdated = dateUpdated;
        }
        #endregion

        #region Methods
        public void ActivateUser()
        {
            IsActive = true;
        }

        public void DeactiveUser()
        {
            IsActive = false;
        }

        public void UpdateRoles()
        {
            if (Roles != null)
            {
                foreach (var role in Roles)
                {
                    if (role.Role.RoleLevel == RoleLevel.SUPERUSER)
                    {
                        IsSuperUser = true;
                    }
                    else if (role.Role.RoleLevel == RoleLevel.ADMIN)
                    {
                        _isAdmin = true;
                    }
                    else
                    {
                        // do nothing...
                    }
                }
            }
        }
        #endregion
    }
}
