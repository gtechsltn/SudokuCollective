using System;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;

namespace SudokuCollective.Core.Models
{
    public class AuthenticatedUser : IAuthenticatedUser
    {
        #region Properties
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool ReceivedRequestToUpdateEmail { get; set; }
        public bool ReceivedRequestToUpdatePassword { get; set; }
        public bool IsActive { get; set; }
        public bool IsSuperUser { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        #endregion

        #region Constructors
        public AuthenticatedUser()
        {
            var createdDate = DateTime.UtcNow;

            Id = 0;
            UserName = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            NickName = string.Empty;
            FullName = string.Empty;
            Email = string.Empty;
            IsEmailConfirmed = false;
            ReceivedRequestToUpdateEmail = false;
            ReceivedRequestToUpdatePassword = false;
            IsActive = false;
            IsSuperUser = false;
            IsAdmin = false;
            DateCreated = createdDate;
            DateUpdated = createdDate;
        }
        #endregion

        #region Methods
        public void UpdateWithUserInfo(IUser user)
        {

            Id = user.Id;
            UserName = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            NickName = user.NickName;
            FullName = user.FullName;
            Email = user.Email;
            IsEmailConfirmed = user.IsEmailConfirmed;
            ReceivedRequestToUpdateEmail = user.ReceivedRequestToUpdateEmail;
            ReceivedRequestToUpdatePassword = user.ReceivedRequestToUpdatePassword;
            IsActive = user.IsActive;
            IsSuperUser = user.IsSuperUser;
            IsAdmin = user.IsAdmin;
            DateCreated = user.DateCreated;
            DateUpdated = user.DateUpdated;
        }
        #endregion
    }
}
