using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;

namespace SudokuCollective.Data.Models.Requests
{
    public class CreateRoleRequest : ICreateRoleRequest
    {
        public string Name { get; set; }
        public RoleLevel RoleLevel { get; set; }

        public CreateRoleRequest()
        {
            Name = string.Empty;
            RoleLevel = RoleLevel.NULL;
        }

        public CreateRoleRequest(string name, int roleLevel)
        {
            Name = name;
            RoleLevel = (RoleLevel)roleLevel;
        }

        public CreateRoleRequest(string name, RoleLevel roleLevel)
        {
            Name = name;
            RoleLevel = roleLevel;
        }
    }
}
