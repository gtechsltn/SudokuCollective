using System.Collections.Generic;

namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads
{
    public interface IUpdateUserRolePayload
    {
        List<int> RoleIds { get; set; }
    }
}
