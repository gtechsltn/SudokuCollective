using System;
using System.Collections.Generic;
using SudokuCollective.Core.Interfaces.DataModels;
using SudokuCollective.Core.Interfaces.Models;

namespace SudokuCollective.Data.Models
{
    public class RepositoryResponse : IRepositoryResponse
    {
        public bool Success { get; set; }
        public Exception Exception { get; set; }
        public IDomainEntity Object { get; set; }
        public List<IDomainEntity> Objects { get; set; }

        public RepositoryResponse()
        {
            Success = false;
            Exception = null;
            Object = null;
            Objects = new List<IDomainEntity>();
        }
    }
}