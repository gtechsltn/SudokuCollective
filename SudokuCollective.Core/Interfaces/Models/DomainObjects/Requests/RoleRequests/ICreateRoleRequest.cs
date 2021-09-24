using SudokuCollective.Core.Enums;

namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests.RoleRequests
{
    public interface ICreateRoleRequest : IDomainObject
    {
        string Name { get; set; }
        RoleLevel RoleLevel { get; set; }
    }
}
