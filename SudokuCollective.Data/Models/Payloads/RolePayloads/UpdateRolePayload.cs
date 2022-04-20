using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads;

namespace SudokuCollective.Data.Models.Payloads
{
    public class UpdateRolePayload : IUpdateRolePayload
    {
        [Required, JsonPropertyName("id")]
        public int Id { get; set; }
        [Required, JsonPropertyName("name")]
        public string Name { get; set; }
        [Required, JsonPropertyName("roleLevel")]
        public RoleLevel RoleLevel { get; set; }

        public UpdateRolePayload()
        {
            Id = 0;
            Name = string.Empty;
            RoleLevel = RoleLevel.NULL;
        }

        public UpdateRolePayload(int id, string name, int roleLevel)
        {
            Id = id;
            Name = name;
            RoleLevel = (RoleLevel)roleLevel;
        }

        public UpdateRolePayload(int id, string name, RoleLevel roleLevel)
        {
            Id = id;
            Name = name;
            RoleLevel = roleLevel;
        }

        public static implicit operator JsonElement(UpdateRolePayload v)
        {
            return JsonSerializer.SerializeToElement(v, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }
}
