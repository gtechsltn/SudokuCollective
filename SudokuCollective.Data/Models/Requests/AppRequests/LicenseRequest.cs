using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using System.ComponentModel.DataAnnotations;

namespace SudokuCollective.Data.Models.Requests.AppRequests
{
    public class LicenseRequest : ILicenseRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int OwnerId { get; set; }
        public string LocalUrl { get; set; }
        public string DevUrl { get; set; }
        public string QaUrl { get; set; }
        public string ProdUrl { get; set; }

        public LicenseRequest()
        {
            Name = string.Empty;
            OwnerId = 0;
            LocalUrl = string.Empty;
            DevUrl = string.Empty;
            QaUrl = string.Empty;
            ProdUrl = string.Empty;
        }

        public LicenseRequest(
            string name,
            int ownerId,
            string localUrl,
            string devUrl,
            string qaUrl,
            string prodUrl)
        {
            Name = name;
            OwnerId = ownerId;
            LocalUrl = localUrl;
            DevUrl = devUrl;
            QaUrl = qaUrl;
            ProdUrl = prodUrl;
        }
    }
}
