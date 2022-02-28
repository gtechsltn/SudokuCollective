using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using SudokuCollective.Core.Interfaces.ServiceModels;

namespace SudokuCollective.Data.Models.Authentication
{
    public class TokenManagement : ITokenManagement
    {
        [JsonPropertyName("Secret"), Required]
        public string Secret { get; set; }
        [JsonPropertyName("Issuer"), Required]
        public string Issuer { get; set; }
        [JsonPropertyName("Audience"), Required]
        public string Audience { get; set; }
        [JsonPropertyName("AccessExpiration"), Required]
        public int AccessExpiration { get; set; }
        [JsonPropertyName("RefreshExpiration"), Required]
        public int RefreshExpiration { get; set; }

        public TokenManagement()
        {
            Secret = string.Empty;
            Issuer = string.Empty;
            Audience = string.Empty;
            AccessExpiration = 0;
            RefreshExpiration = 0;
        }
    }
}