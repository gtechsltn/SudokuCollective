using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;

namespace SudokuCollective.Data.Models.Requests
{
    public class UpdateUserRoleRequest : IUpdateUserRoleRequest
    {
        [Required]
        public List<int> RoleIds { get; set; }

        public UpdateUserRoleRequest()
        {
            RoleIds = new List<int>();
        }

        public UpdateUserRoleRequest(int[] roleIds)
        {
            RoleIds = roleIds.ToList();
        }

        public UpdateUserRoleRequest(List<int> roleIds)
        {
            RoleIds = roleIds;
        }
    }
}
