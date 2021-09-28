﻿namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Params
{
    public interface IRequest
    {
        string License { get; set; }
        int RequestorId { get; set; }
        int AppId { get; set; }
        IPaginator Paginator { get; set; }
        object DataPacket { get; set; }
    }
}