using SudokuCollective.Core.Enums;

namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests
{
    public interface ICreateRoleRequest
    {
        string Name { get; set; }
        RoleLevel RoleLevel { get; set; }
    }
}
