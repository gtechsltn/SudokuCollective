using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads;

namespace SudokuCollective.Data.Models.Payloads
{
    public class LicensePayload : ILicensePayload
    {
        [Required, JsonPropertyName("name")]
        public string Name { get; set; }
        [Required, JsonPropertyName("ownerId")]
        public int OwnerId { get; set; }
        [JsonPropertyName("localUrl")]
        public string LocalUrl { get; set; }
        [JsonPropertyName("stagingUrl")]
        public string StagingUrl { get; set; }
        [JsonPropertyName("qaUrl")]
        public string QaUrl { get; set; }
        [JsonPropertyName("prodUrl")]
        public string ProdUrl { get; set; }

        public LicensePayload()
        {
            Name = string.Empty;
            OwnerId = 0;
            LocalUrl = string.Empty;
            StagingUrl = string.Empty;
            QaUrl = string.Empty;
            ProdUrl = string.Empty;
        }

        public LicensePayload(
            string name,
            int ownerId,
            string localUrl,
            string stagingUrl,
            string qaUrl,
            string prodUrl)
        {
            Name = name;
            OwnerId = ownerId;
            LocalUrl = localUrl;
            StagingUrl = stagingUrl;
            QaUrl = qaUrl;
            ProdUrl = prodUrl;
        }

        public static implicit operator JsonElement(LicensePayload v)
        {
            return JsonSerializer.SerializeToElement(v, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }
}
