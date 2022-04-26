﻿namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads
{
    public interface ILicensePayload : IPayload
    {
        string Name { get; set; }
        int OwnerId { get; set; }
        string LocalUrl { get; set; }
        string StagingUrl { get; set; }
        string QaUrl { get; set; }
        string ProdUrl { get; set; }
    }
}
