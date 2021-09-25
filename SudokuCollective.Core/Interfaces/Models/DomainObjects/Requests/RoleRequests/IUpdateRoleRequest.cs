using SudokuCollective.Core.Enums;

namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests
{
    public interface IUpdateRoleRequest : IDomainObject
    {
        int Id { get; set; }
        string Name { get; set; }
        RoleLevel RoleLevel { get; set; }
    }
}
