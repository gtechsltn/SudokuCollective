﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;

namespace SudokuCollective.Core.Models
{
    public class UserRole : IUserRole
    {
        #region Properties
        [Required]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [JsonIgnore]
        IUser IUserRole.User
        {
            get
            {
                return User;
            }
            set
            {
                User = (User)value;
            }
        }
        [JsonIgnore]
        public virtual User User { get; set; }
        [Required]
        public int RoleId { get; set; }
        [JsonIgnore]
        IRole IUserRole.Role
        {
            get
            {
                return Role;
            }
            set
            {
                Role = (Role)value;
            }
        }
        [Required]
        public virtual Role Role { get; set; }
        #endregion

        #region Constructors
        public UserRole()
        {
            Id = 0;
            UserId = 0;
            User = null;
            RoleId = 0;
            Role = null;
        }

        public UserRole(int userId, int roleId)
        {
            Id = 0;
            UserId = userId;
            RoleId = roleId;
        }

        [JsonConstructor]
        public UserRole(
            int id,
            int userId,
            int roleId)
        {
            Id = id;
            UserId = userId;
            RoleId = roleId;
        }
        #endregion

        #region Methods
        public override string ToString() => string.Format(base.ToString() + ".Id:{0}.RoleId:{1}.UserId:{2}", Id, RoleId, UserId);

        public string ToJson() => JsonSerializer.Serialize(
            this,
            new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });
        #endregion
    }
}
