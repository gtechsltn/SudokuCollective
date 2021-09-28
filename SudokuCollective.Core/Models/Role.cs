﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;

namespace SudokuCollective.Core.Models
{
    public class Role : IRole
    {
        #region Properties
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public RoleLevel RoleLevel { get; set; }
        [IgnoreDataMember]
        ICollection<IUserRole> IRole.Users
        {

            get
            {
                return (ICollection<IUserRole>)Users;
            }

            set
            {
                Users = (ICollection<UserRole>)value;
            }
        }
        [IgnoreDataMember]
        public virtual ICollection<UserRole> Users { get; set; }
        #endregion

        #region Constructors
        public Role()
        {
            Id = 0;
            Name = string.Empty;
            RoleLevel = RoleLevel.NULL;
            Users = new List<UserRole>();
        }

        [JsonConstructor]
        public Role(int id, string name, RoleLevel roleLevel)
        {
            Id = id;
            Name = name;
            RoleLevel = roleLevel;
            Users = new List<UserRole>();
        }
        #endregion
    }
}