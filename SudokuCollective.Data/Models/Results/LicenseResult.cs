using System.Collections.Generic;
using System.Linq;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;

namespace SudokuCollective.Data.Models.Results
{
    public class LicenseResult : ILicenseResult
    {
        public bool IsSuccess { get; set; }
        public bool IsFromCache { get; set; }
        public string Message { get; set; }
        public string License { get; set; }
        public List<object> DataPacket { get; set; }

        public LicenseResult()
        {
            IsSuccess = false;
            IsFromCache = false;
            Message = string.Empty;
            License = string.Empty;
            DataPacket = new List<object>();
        }

        public LicenseResult(
            bool isSuccess,
            bool isFromCache,
            string message,
            string license,
            object[] dataPacket)
        {
            IsSuccess = isSuccess;
            IsFromCache = isFromCache;
            Message = message;
            License = license;
            DataPacket = dataPacket.ToList();
        }

        public LicenseResult(
            bool isSuccess,
            bool isFromCache,
            string message,
            string license,
            List<object> dataPacket)
        {
            IsSuccess = isSuccess;
            IsFromCache = isFromCache;
            Message = message;
            License = license;
            DataPacket = dataPacket;
        }
    }
}