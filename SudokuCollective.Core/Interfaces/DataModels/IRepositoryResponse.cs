﻿using System;
using System.Collections.Generic;
using SudokuCollective.Core.Interfaces.Models;

namespace SudokuCollective.Core.Interfaces.DataModels
{
    public interface IRepositoryResponse
    {
        bool Success { get; set; }
        Exception Exception { get; set; }
        IDomainEntity Object { get; set; }
        List<IDomainEntity> Objects { get; set; }
    }
}
