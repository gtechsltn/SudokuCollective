using System.Text.Json.Serialization;
using SudokuCollective.Core.Interfaces.DataModels;

namespace SudokuCollective.Data.Models.Authentication
{
    public class TokenManagement : ITokenManagement
    {
        [JsonPropertyName("Secret")]
        public string Secret { get; set; }
        [JsonPropertyName("Issuer")]
        public string Issuer { get; set; }
        [JsonPropertyName("Audience")]
        public string Audience { get; set; }
    }
}