﻿using SudokuCollective.Core.Enums;

namespace SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests
{
    public interface IAnnonymousGameRequest : IDomainObject
    {
        DifficultyLevel DifficultyLevel { get; set; }
    }
}
