using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;

namespace SudokuCollective.Core.Models
{
    public class User : IUser
    {
        #region Fields
        private string _userName;
        private string _email;
        private bool _isSuperUser;
        private bool _isAdmin;
        #endregion

        #region Properties
        public int Id { get; set; }
        [Required]
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
                    var regex = new Regex("^[^-]{1}?[^\"\']*$");

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
        [Required]
        public string Email
        {
            get
            {
                return _email;
            }

            set
            {
                _email = value;
                IsEmailConfirmed = false;
            }
        }
        public bool IsEmailConfirmed { get; set; }
        public bool ReceivedRequestToUpdateEmail { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string Password { get; set; }
        public bool ReceivedRequestToUpdatePassword { get; set; }
        public bool IsActive { get; set; }
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
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        ICollection<IGame> IUser.Games
        {
            get
            {
                return (ICollection<IGame>)Games;
            }
            set
            {
                Games = (ICollection<Game>)value;
            }
        }
        public virtual ICollection<Game> Games { get; set; }
        ICollection<IUserRole> IUser.Roles
        {
            get
            {
                return (ICollection<IUserRole>)Roles;
            }
            set
            {
                Roles = (ICollection<UserRole>)value;
            }
        }
        public virtual ICollection<UserRole> Roles { get; set; }
        ICollection<IUserApp> IUser.Apps
        {
            get
            {
                return (ICollection<IUserApp>)Apps;
            }
            set
            {
                Apps = (ICollection<UserApp>)value;
            }
        }
        public virtual ICollection<UserApp> Apps { get; set; }
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
