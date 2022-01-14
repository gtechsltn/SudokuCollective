using System.Collections.Generic;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Models;

namespace SudokuCollective.Core.Interfaces.Models.DomainEntities
{
    public interface IRole : IDomainEntity
    {
        string Name { get; set; }
        RoleLevel RoleLevel { get; set; }
        List<UserRole> Users { get; set; }
    }
}
