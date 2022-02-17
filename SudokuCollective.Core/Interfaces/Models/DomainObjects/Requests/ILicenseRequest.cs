namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests
{
    public interface ILicenseRequest
    {
        string Name { get; set; }
        int OwnerId { get; set; }
        string LocalUrl { get; set; }
        string DevUrl { get; set; }
        string QaUrl { get; set; }
        string ProdUrl { get; set; }
    }
}
