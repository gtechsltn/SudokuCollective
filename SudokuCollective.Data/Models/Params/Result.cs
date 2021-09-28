using System.Collections.Generic;
using SudokuCollective.Core.Interfaces.Models;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;

namespace SudokuCollective.Data.Models.Params
{
    public class Result : IResult
    {
        public bool IsSuccess { get; set; }
        public bool IsFromCache { get; set; }
        public string Message { get; set; }
        public List<object> DataPacket { get; set; }

        public Result()
        {
            IsSuccess = false;
            IsFromCache = false;
            Message = string.Empty;
            DataPacket = new List<object>();
        }
    }
}
