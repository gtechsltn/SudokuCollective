using System.Text.Json;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;

namespace SudokuCollective.Data.Models.Requests
{
    public class ResendRequestPasswordRequest : IResendRequestPasswordRequest
    {
        public int UserId { get; set; }
        public int AppId { get; set; }

        public ResendRequestPasswordRequest()
        {
            UserId = 0;
            AppId = 0;
        }

        public ResendRequestPasswordRequest(int userId, int appId)
        {
            UserId = userId;
            AppId = appId;
        }

        public static implicit operator JsonElement(ResendRequestPasswordRequest v)
        {
            return JsonSerializer.SerializeToElement(v, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }
}
