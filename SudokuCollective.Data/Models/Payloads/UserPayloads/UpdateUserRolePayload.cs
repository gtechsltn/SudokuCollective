using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads;

namespace SudokuCollective.Data.Models.Payloads
{
    public class UpdateUserRolePayload : IUpdateUserRolePayload
    {
        [Required]
        public List<int> RoleIds { get; set; }

        public UpdateUserRolePayload()
        {
            RoleIds = new List<int>();
        }

        public UpdateUserRolePayload(int[] roleIds)
        {
            RoleIds = roleIds.ToList();
        }

        public UpdateUserRolePayload(List<int> roleIds)
        {
            RoleIds = roleIds;
        }

        public static implicit operator JsonElement(UpdateUserRolePayload v)
        {
            return JsonSerializer.SerializeToElement(v, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }
}
