using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;

namespace SudokuCollective.Core.Models
{
    public class Game : IGame
    {
        #region Properties
        [Required]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [JsonIgnore]
        IUser IGame.User
        { 
            get
            {
                return User;
            }
            set
            {
                User = (User)value;
            }
        }
        [JsonIgnore]
        public User User { get; set; }
        [Required]
        public int SudokuMatrixId { get; set; }
        [JsonIgnore]
        ISudokuMatrix IGame.SudokuMatrix
        {
            get
            {
                return SudokuMatrix;
            }
            set
            {
                SudokuMatrix = (SudokuMatrix)value;
            }
        }
        [Required]
        public virtual SudokuMatrix SudokuMatrix { get; set; }
        [Required]
        public int SudokuSolutionId { get; set; }
        [JsonIgnore]
        ISudokuSolution IGame.SudokuSolution
        {
            get
            {
                return SudokuSolution;
            }
            set
            {
                SudokuSolution = (SudokuSolution)value;
            }
        }
        [Required]
        public virtual SudokuSolution SudokuSolution { get; set; }
        [Required]
        public int AppId { get; set; }
        [Required]
        public bool ContinueGame { get; set; }
        [Required]
        public int Score { get; set; }
        [Required]
        public bool KeepScore { get; set; }
        [Required]
        public TimeSpan TimeToSolve { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }
        [Required]
        public DateTime DateUpdated { get; set; }
        [Required]
        public DateTime DateCompleted { get; set; }
        #endregion

        #region Constructors
        public Game()
        {
            Id = 0;
            DateCreated = DateTime.UtcNow;
            DateUpdated = DateTime.MinValue;
            DateCompleted = DateTime.MinValue;
            ContinueGame = true;
            Score = 0;
            KeepScore = false;
            SudokuMatrix = new SudokuMatrix();
            SudokuSolution = new SudokuSolution();
            AppId = 0;
            TimeToSolve = new TimeSpan();
        }

        public Game(
            User user,
            SudokuMatrix matrix,
            IDifficulty difficulty,
            int appId = 0) : this()
        {
            User = user;
            SudokuMatrix = matrix;
            SudokuMatrix.Difficulty = (Difficulty)difficulty;
            SudokuMatrix.SetDifficulty(SudokuMatrix.Difficulty);
            AppId = appId;

            User.Games.Add(this);
        }

        public Game(IDifficulty difficulty, List<int> intList = null) : this()
        {
            if (intList != null)
            {
                SudokuMatrix = new SudokuMatrix(difficulty, intList);
            }
            else
            {
                SudokuMatrix.Difficulty = (Difficulty)difficulty;
            }

            SudokuMatrix.SetDifficulty();
        }

        [JsonConstructor]
        public Game(
            int id,
            int userId,
            int sudokuMatrixId,
            int sudokuSolutionId,
            int appId,
            bool continueGame,
            int score,
            bool keepScore,
            long timeToSolve,
            DateTime dateCreated,
            DateTime dateUpdated,
            DateTime dateCompleted)
        {
            Id = id;
            UserId = userId;
            SudokuMatrixId = sudokuMatrixId;
            SudokuSolutionId = sudokuSolutionId;
            AppId = appId;
            ContinueGame = continueGame;
            Score = score;
            KeepScore = keepScore;
            TimeToSolve = new TimeSpan(timeToSolve);
            DateCreated = dateCreated;
            DateUpdated = dateUpdated;
            DateCompleted = dateCompleted;
        }
        #endregion

        #region Methods
        public bool IsSolved()
        {
            if (ContinueGame)
            {
                if (SudokuMatrix.IsSolved())
                {
                    if (SudokuMatrix.Stopwatch.IsRunning)
                    {
                        SudokuMatrix.Stopwatch.Stop();
                    }

                    foreach (var sudokuCell in SudokuMatrix.SudokuCells)
                    {
                        if (sudokuCell.DisplayedValue > 0)
                        {
                            sudokuCell.Value = sudokuCell.DisplayedValue;
                        }
                    }

                    if (KeepScore)
                    {
                        TimeToSolve = DateTime.UtcNow - DateCreated;

                        if (SudokuMatrix.Difficulty.DifficultyLevel == DifficultyLevel.EASY)
                        {
                            Score = Convert.ToInt32((TimeToSolve.Ticks * .80) / 1000000000);
                        }
                        else if (SudokuMatrix.Difficulty.DifficultyLevel == DifficultyLevel.MEDIUM)
                        {
                            Score = Convert.ToInt32((TimeToSolve.Ticks * .60) / 1000000000);
                        }
                        else if (SudokuMatrix.Difficulty.DifficultyLevel == DifficultyLevel.HARD)
                        {
                            Score = Convert.ToInt32((TimeToSolve.Ticks * .40) / 1000000000);
                        }
                        else if (SudokuMatrix.Difficulty.DifficultyLevel == DifficultyLevel.EVIL)
                        {
                            Score = Convert.ToInt32((TimeToSolve.Ticks * 20) / 1000000000);
                        }
                        else
                        {
                            Score = int.MaxValue;
                        }
                    }

                    var solvedDate = DateTime.UtcNow;

                    ContinueGame = false;
                    DateUpdated = solvedDate;
                    DateCompleted = solvedDate;
                    SudokuSolution.SolutionList = SudokuMatrix.ToIntList();
                    SudokuSolution.DateSolved = solvedDate;
                }
            }

            return !ContinueGame;
        }
        #endregion
    }
}
