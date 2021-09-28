using System.Collections.Generic;

namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests
{
    public interface IUpdateUserRoleRequest
    {
        List<int> RoleIds { get; set; }
    }
}
