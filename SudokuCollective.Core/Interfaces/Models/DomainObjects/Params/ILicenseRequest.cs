namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Params
{
    public interface ILicenseRequest : IDomainObject
    {
        string Name { get; set; }
        int OwnerId { get; set; }
        string LocalUrl { get; set; }
        string DevUrl { get; set; }
        string QaUrl { get; set; }
        string LiveUrl { get; set; }
    }
}
