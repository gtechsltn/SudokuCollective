using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;

namespace SudokuCollective.Core.Models
{
    public class Role : IRole
    {
        #region Properties
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public RoleLevel RoleLevel { get; set; }
        [JsonIgnore]
        ICollection<IUserRole> IRole.Users
        {
            get
            {
                return Users.ConvertAll(ur => (IUserRole)ur);
            }
            set
            {
                Users = value.ToList().ConvertAll(ur => (UserRole)ur);
            }
        }
        [JsonIgnore]
        public virtual List<UserRole> Users { get; set; }
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
