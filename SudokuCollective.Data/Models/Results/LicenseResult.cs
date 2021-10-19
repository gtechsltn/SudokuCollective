using SudokuCollective.Core.Interfaces.Models.DomainObjects.Results;

namespace SudokuCollective.Data.Models.Results
{
    public class LicenseResult : ILicenseResult
    {
        public string License { get; set; }
        public LicenseResult()
        {
            License = string.Empty;
        }

        public LicenseResult(string license)
        {
            License = license;
        }

    }
}
