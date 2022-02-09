using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;

namespace SudokuCollective.Core.Models
{
    public class AuthenticatedUser : IAuthenticatedUser
    {
        #region Properties
        [Required]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string NickName { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public bool IsEmailConfirmed { get; set; }
        [Required]
        public bool ReceivedRequestToUpdateEmail { get; set; }
        [Required]
        public bool ReceivedRequestToUpdatePassword { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public bool IsSuperUser { get; set; }
        [Required]
        public bool IsAdmin { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }
        [Required]
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
