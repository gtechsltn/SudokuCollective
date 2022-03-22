using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Messages;
using SudokuCollective.Core.Utilities;
using SudokuCollective.Core.Validation.Attributes;

namespace SudokuCollective.Core.Models
{
    public class User : IUser
    {
        #region Fields
        private string _userName = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private bool _isActive = false;
        private bool _isSuperUser;
        private bool _isAdmin;
        private readonly UserNameValidatedAttribute _userNameValidatedAttribute = new();
        private readonly EmailValidatedAttribute _emailValidator = new();
        #endregion

        #region Properties
        [Required]
        public int Id { get; set; }
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
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string NickName { get; set; }
        [JsonIgnore]
        public string FullName
        {
            get => string.Format("{0} {1}", FirstName, LastName);
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
                    IsEmailConfirmed = false;
                }
                else
                {
                    throw new ArgumentException(AttributeMessages.InvalidEmail);
                }
            }
        }
        [Required]
        public bool IsEmailConfirmed { get; set; }
        public bool ReceivedRequestToUpdateEmail { get; set; }
        [JsonIgnore]
        public string Password
        {
            get
            {
                return _password;
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _password = value;
                }
            }
        }
        public bool ReceivedRequestToUpdatePassword { get; set; }
        [Required]
        public bool IsActive { get => _isActive; }
        [JsonIgnore]
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
        }
        [JsonIgnore]
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
        }
        [Required]
        public DateTime DateCreated { get; set; }
        [Required]
        public DateTime DateUpdated { get; set; }
        [JsonIgnore]
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
        [Required, JsonConverter(typeof(IDomainEntityListConverter<List<Game>>))]
        public virtual List<Game> Games { get; set; }
        [JsonIgnore]
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
        [Required, JsonConverter(typeof(IDomainEntityListConverter<List<UserRole>>))]
        public virtual List<UserRole> Roles { get; set; }
        [JsonIgnore]
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
        [Required, JsonConverter(typeof(IDomainEntityListConverter<List<UserApp>>))]
        public virtual List<UserApp> Apps { get; set; }
        #endregion

        #region Constructors
        public User()
        {
            Games = new List<Game>();
            Roles = new List<UserRole>();
            Apps = new List<UserApp>();

            Id = 0;
            FirstName = string.Empty;
            LastName = string.Empty;
            NickName = string.Empty;
            IsEmailConfirmed = false;
            ReceivedRequestToUpdateEmail = false;
            ReceivedRequestToUpdatePassword = false;
            DateCreated = DateTime.MinValue;
            DateUpdated = DateTime.MinValue;
        }

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
            IsEmailConfirmed = false;
            _isActive = true;
        }

        [JsonConstructor]
        public User(
            int id,
            string userName,
            string firstName,
            string lastName,
            string nickName,
            string email,
            bool isEmailConfirmed,
            bool receivedRequestToUpdateEmail,
            string password,
            bool receivedRequestToUpdatePassword,
            bool isActive,
            DateTime dateCreated,
            DateTime dateUpdated,
            List<Game> games = null,
            List<UserRole> roles = null,
            List<UserApp> apps = null)
        {
            Id = id;
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            NickName = nickName;
            Email = email;
            IsEmailConfirmed = isEmailConfirmed;
            ReceivedRequestToUpdateEmail = receivedRequestToUpdateEmail;
            Password = password;
            ReceivedRequestToUpdatePassword = receivedRequestToUpdatePassword;
            DateCreated = dateCreated;
            DateUpdated = dateUpdated;
            _isActive = isActive;

            if (games != null)
            {
                Games = games;
            }
            else
            {
                Games = new List<Game>();
            }

            if (roles != null)
            {
                Roles = roles;
            }
            else
            {
                Roles = new List<UserRole>();
            }

            if (apps != null)
            {
                Apps = apps;
            }
            else
            {
                Apps = new List<UserApp>();
            }
        }
        #endregion

        #region Methods
        public void ActivateUser()
        {
            _isActive = true;
        }

        public void DeactiveUser()
        {
            _isActive = false;
        }

        public void UpdateRoles()
        {
            if (Roles != null)
            {
                foreach (var role in Roles)
                {
                    if (role.Role.RoleLevel == RoleLevel.SUPERUSER)
                    {
                        _isSuperUser = true;
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

        public void HideEmail()
        {
            _email = null;
        }

        public override string ToString() => string.Format(base.ToString() + ".Id:{0}.UserName:{1}", Id, UserName);

        public string ToJson() => JsonSerializer.Serialize(
            this,
            new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });
        #endregion
    }
}
