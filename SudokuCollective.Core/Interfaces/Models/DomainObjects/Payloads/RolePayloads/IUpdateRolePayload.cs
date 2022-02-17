using SudokuCollective.Core.Enums;

namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads
{
    public interface IUpdateRolePayload
    {
        int Id { get; set; }
        string Name { get; set; }
        RoleLevel RoleLevel { get; set; }
    }
}
