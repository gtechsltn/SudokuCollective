using SudokuCollective.Core.Enums;

namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads
{
    public interface ICreateRolePayload
    {
        string Name { get; set; }
        RoleLevel RoleLevel { get; set; }
    }
}
