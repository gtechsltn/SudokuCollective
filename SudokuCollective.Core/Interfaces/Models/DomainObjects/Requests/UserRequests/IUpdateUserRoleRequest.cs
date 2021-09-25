using System.Collections.Generic;

namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests
{
    public interface IUpdateUserRoleRequest : IDomainObject
    {
        List<int> RoleIds { get; set; }
    }
}
