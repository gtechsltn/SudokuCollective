﻿namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads
{
    public interface IAddSolutionPayload : IPayload
    {
        int Limit { get; set; }
    }
}
