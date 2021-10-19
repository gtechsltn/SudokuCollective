using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;

namespace SudokuCollective.Data.Models.Requests
{
    public class UpdateRoleRequest : IUpdateRoleRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public RoleLevel RoleLevel { get; set; }

        public UpdateRoleRequest()
        {
            Id = 0;
            Name = string.Empty;
            RoleLevel = RoleLevel.NULL;
        }

        public UpdateRoleRequest(int id, string name, int roleLevel)
        {
            Id = id;
            Name = name;
            RoleLevel = (RoleLevel)roleLevel;
        }

        public UpdateRoleRequest(int id, string name, RoleLevel roleLevel)
        {
            Id = id;
            Name = name;
            RoleLevel = roleLevel;
        }
    }
}
