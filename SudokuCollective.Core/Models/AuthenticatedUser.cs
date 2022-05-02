﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Utilities;

namespace SudokuCollective.Core.Models
{
    public class AuthenticatedUser : IAuthenticatedUser
    {
        #region Properties
        [Required, JsonPropertyName("id")]
        public int Id { get; set; }
        [Required, JsonPropertyName("userName")]
        public string UserName { get; set; }
        [Required, JsonPropertyName("firstName")]
        public string FirstName { get; set; }
        [Required, JsonPropertyName("lastName")]
        public string LastName { get; set; }
        [Required, JsonPropertyName("nickName")]
        public string NickName { get; set; }
        [Required, JsonPropertyName("fullName")]
        public string FullName { get; set; }
        [Required, JsonPropertyName("email")]
        public string Email { get; set; }
        [Required, JsonPropertyName("isEmailConfirmed")]
        public bool IsEmailConfirmed { get; set; }
        [Required, JsonPropertyName("receivedRequestToUpdateEmail")]
        public bool ReceivedRequestToUpdateEmail { get; set; }
        [Required, JsonPropertyName("receivedRequestToUpdatePassword")]
        public bool ReceivedRequestToUpdatePassword { get; set; }
        [Required, JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
        [Required, JsonPropertyName("isSuperUser")]
        public bool IsSuperUser { get; set; }
        [Required, JsonPropertyName("isAdmin")]
        public bool IsAdmin { get; set; }
        [Required, JsonPropertyName("dateCreated")]
        public DateTime DateCreated { get; set; }
        [Required, JsonPropertyName("dateUpdated")]
        public DateTime DateUpdated { get; set; }
        [JsonIgnore]
        ICollection<IGame> IAuthenticatedUser.Games
        {
            get => Games.ConvertAll(g => (IGame)g);
            set => Games = value.ToList().ConvertAll(g => (Game)g);
        }
        [Required, JsonPropertyName("games"), JsonConverter(typeof(IDomainEntityListConverter<List<Game>>))]
        public virtual List<Game> Games { get; set; }
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
            Games = new List<Game>();
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
            Games = ((User)user).Games;
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
