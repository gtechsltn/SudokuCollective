﻿using System.Collections.Generic;

namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Params
{
    public interface IResult : IDomainObject
    {
        bool IsSuccess { get; set; }
        bool IsFromCache { get; set; }
        string Message { get; set; }
        List<IDomainObject> DataPacket { get; set; }
    }
}
