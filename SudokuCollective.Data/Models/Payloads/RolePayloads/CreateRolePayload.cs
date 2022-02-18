using System.Text.Json;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads;

namespace SudokuCollective.Data.Models.Payloads
{
    public class CreateRolePayload : ICreateRolePayload
    {
        public string Name { get; set; }
        public RoleLevel RoleLevel { get; set; }

        public CreateRolePayload()
        {
            Name = string.Empty;
            RoleLevel = RoleLevel.NULL;
        }

        public CreateRolePayload(string name, int roleLevel)
        {
            Name = name;
            RoleLevel = (RoleLevel)roleLevel;
        }

        public CreateRolePayload(string name, RoleLevel roleLevel)
        {
            Name = name;
            RoleLevel = roleLevel;
        }

        public static implicit operator JsonElement(CreateRolePayload v)
        {
            return JsonSerializer.SerializeToElement(v, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }
}
