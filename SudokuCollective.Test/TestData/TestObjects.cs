using System;
using System.Collections.Generic;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Requests;

namespace SudokuCollective.Test.TestData
{
    public static class TestObjects
    {
        public static string GetLicense()
        {
            return "D17F0ED3-BE9A-450A-A146-F6733DB2BBDB";
        }
        
        public static string GetToken()
        {
            return "D17F0ED3-BE9A-450A-A146-F6733DB2BBDB";
        }

        public static Request GetRequest()
        {
            return new Request()
            {
                License = GetLicense(),
                RequestorId = 1,
                AppId = 1
            };
        }

        public static Paginator GetPaginator()
        {
            return new Paginator()
            {
                Page = 1,
                ItemsPerPage = 10,
                SortBy = SortValue.NULL,
                OrderByDescending = false,
                IncludeCompletedGames = false
            };
        }

        public static AnnonymousCheckRequest GetValidAnnonymousCheckRequest()
        {
            return new AnnonymousCheckRequest
            {
                FirstRow = new List<int> { 7, 8, 5, 4, 1, 3, 2, 9, 6 },
                SecondRow = new List<int> { 1, 4, 2, 8, 6, 9, 5, 7, 3 },
                ThirdRow = new List<int> { 6, 9, 3, 2, 7, 5, 4, 1, 8 },
                FourthRow = new List<int> { 5, 1, 4, 3, 8, 2, 7, 6, 9 },
                FifthRow = new List<int> { 2, 6, 7, 9, 4, 1, 3, 8, 5 },
                SixthRow = new List<int> { 8, 3, 9, 7, 5, 6, 1, 2, 4 },
                SeventhRow = new List<int> { 4, 2, 1, 6, 3, 8, 9, 5, 7 },
                EighthRow = new List<int> { 3, 5, 8, 1, 9, 7, 6, 4, 2 },
                NinthRow = new List<int> { 9, 7, 6, 5, 2, 4, 8, 3, 1 }
            };
        }

        public static AnnonymousCheckRequest GetInvalidAnnonymousCheckRequest()
        {
            return new AnnonymousCheckRequest
            {
                FirstRow = new List<int> { 7, 8, 5, 4, 1, 3, 2, 9, 6, 0 },
                SecondRow = new List<int> { 1, 4, 2, 8, 6, 9, 5, 7, 3 },
                ThirdRow = new List<int> { 6, 9, 3, 2, 7, 5, 4, 1, 8 },
                FourthRow = new List<int> { 5, 1, 4, 3, 8, 2, 7, 6, 9 },
                FifthRow = new List<int> { 2, 6, 7, 9, 4, 1, 3, 8, 5 },
                SixthRow = new List<int> { 8, 3, 9, 7, 5, 6, 1, 2, 4 },
                SeventhRow = new List<int> { 4, 2, 1, 6, 3, 8, 9, 5, 7 },
                EighthRow = new List<int> { 3, 5, 8, 1, 9, 7, 6, 4, 2 },
                NinthRow = new List<int> { 9, 7, 6, 5, 2, 4, 8, 3, 1 }
            };
        }

        public static SolutionRequest GetValidSolutionRequest()
        {
            return new SolutionRequest
            {
                FirstRow = new List<int> { 7, 8, 5, 4, 1, 3, 2, 9, 6 },
                SecondRow = new List<int> { 1, 4, 2, 8, 6, 9, 5, 7, 3 },
                ThirdRow = new List<int> { 6, 9, 3, 2, 7, 5, 4, 1, 8 },
                FourthRow = new List<int> { 5, 1, 4, 3, 8, 2, 7, 6, 9 },
                FifthRow = new List<int> { 2, 6, 7, 9, 4, 1, 3, 8, 5 },
                SixthRow = new List<int> { 8, 3, 9, 7, 5, 6, 1, 2, 4 },
                SeventhRow = new List<int> { 4, 2, 1, 6, 3, 8, 9, 5, 7 },
                EighthRow = new List<int> { 3, 5, 8, 1, 9, 7, 6, 4, 2 },
                NinthRow = new List<int> { 9, 7, 6, 5, 2, 4, 8, 3, 1 }
            };
        }

        public static SolutionRequest GetInvalidSolutionRequest()
        {
            return new SolutionRequest
            {
                FirstRow = new List<int> { 7, 8, 5, 4, 1, 3, 2, 9, 6, 0 },
                SecondRow = new List<int> { 1, 4, 2, 8, 6, 9, 5, 7, 3 },
                ThirdRow = new List<int> { 6, 9, 3, 2, 7, 5, 4, 1, 8 },
                FourthRow = new List<int> { 5, 1, 4, 3, 8, 2, 7, 6, 9 },
                FifthRow = new List<int> { 2, 6, 7, 9, 4, 1, 3, 8, 5 },
                SixthRow = new List<int> { 8, 3, 9, 7, 5, 6, 1, 2, 4 },
                SeventhRow = new List<int> { 4, 2, 1, 6, 3, 8, 9, 5, 7 },
                EighthRow = new List<int> { 3, 5, 8, 1, 9, 7, 6, 4, 2 },
                NinthRow = new List<int> { 9, 7, 6, 5, 2, 4, 8, 3, 1 }
            };
        }

        public static List<SudokuCell> GetUpdateSudokuCells(int updatedValue)
        {
            var cells = new List<SudokuCell>()
            {
                new SudokuCell(81,1,1,1,1,4,4,false,1),
                new SudokuCell(58,2,2,1,1,2,updatedValue,true,1),
                new SudokuCell(57,3,3,1,1,8,8,false,1),
                new SudokuCell(56,4,4,2,1,6,0,true,1),
                new SudokuCell(55,5,5,2,1,1,0,true,1),
                new SudokuCell(54,6,6,2,1,5,0,true,1),
                new SudokuCell(53,7,7,3,1,9,0,true,1),
                new SudokuCell(52,8,8,3,1,3,0,true,1),
                new SudokuCell(51,9,9,3,1,7,0,true,1),
                new SudokuCell(50,10,1,1,2,3,3,false,1),
                new SudokuCell(49,11,2,1,2,9,0,true,1),
                new SudokuCell(48,12,3,1,2,6,6,false,1),
                new SudokuCell(47,13,4,2,2,2,0,true,1),
                new SudokuCell(46,14,5,2,2,8,0,true,1),
                new SudokuCell(45,15,6,2,2,7,0,true,1),
                new SudokuCell(44,16,7,3,2,1,1,false,1),
                new SudokuCell(43,17,8,3,2,4,4,false,1),
                new SudokuCell(42,18,9,3,2,5,0,false,1),
                new SudokuCell(59,19,1,1,3,1,0,true,1),
                new SudokuCell(61,20,2,1,3,5,0,true,1),
                new SudokuCell(80,21,3,1,3,7,7,false,1),
                new SudokuCell(62,22,4,2,3,3,0,true,1),
                new SudokuCell(79,23,5,2,3,4,0,true,1),
                new SudokuCell(78,24,6,2,3,9,9,false,1),
                new SudokuCell(77,25,7,3,3,6,0,true,1),
                new SudokuCell(76,26,8,3,3,8,0,true,1),
                new SudokuCell(75,27,9,3,3,2,2,false,1),
                new SudokuCell(74,28,1,4,4,5,0,true,1),
                new SudokuCell(73,29,2,4,4,8,8,false,1),
                new SudokuCell(72,30,3,4,4,4,4,false,1),
                new SudokuCell(71,31,4,5,4,9,0,true,1),
                new SudokuCell(70,32,5,5,4,3,0,true,1),
                new SudokuCell(69,33,6,5,4,6,0,true,1),
                new SudokuCell(68,34,7,6,4,2,0,true,1),
                new SudokuCell(67,35,8,6,4,7,0,true,1),
                new SudokuCell(66,36,9,6,4,1,0,true,1),
                new SudokuCell(65,37,1,4,5,7,7,false,1),
                new SudokuCell(64,38,2,4,5,6,0,true,1),
                new SudokuCell(63,39,3,4,5,1,0,true,1),
                new SudokuCell(41,40,4,5,5,8,8,false,1),
                new SudokuCell(40,41,5,5,5,2,2,false,1),
                new SudokuCell(39,42,6,5,5,4,4,false,1),
                new SudokuCell(38,43,7,6,5,5,5,false,1),
                new SudokuCell(16,44,8,6,5,9,0,true,1),
                new SudokuCell(15,45,9,6,5,3,0,true,1),
                new SudokuCell(14,46,1,4,6,2,2,false,1),
                new SudokuCell(13,47,2,4,6,3,0,true,1),
                new SudokuCell(12,48,3,4,6,9,9,false,1),
                new SudokuCell(11,49,4,5,6,7,7,false,1),
                new SudokuCell(10,50,5,5,6,5,5,false,1),
                new SudokuCell(17,51,6,5,6,1,0,true,1),
                new SudokuCell(9,52,7,6,6,4,4,false,1),
                new SudokuCell(7,53,8,6,6,6,6,false,1),
                new SudokuCell(6,54,9,6,6,8,0,true,1),
                new SudokuCell(5,55,1,7,7,9,0,true,1),
                new SudokuCell(4,56,2,7,7,7,0,true,1),
                new SudokuCell(3,57,3,7,7,5,0,true,1),
                new SudokuCell(2,58,4,8,7,1,0,true,1),
                new SudokuCell(1,59,5,8,7,6,0,true,1),
                new SudokuCell(8,60,6,8,7,8,8,false,1),
                new SudokuCell(18,61,7,9,7,3,3,false,1),
                new SudokuCell(19,62,8,9,7,2,2,false,1),
                new SudokuCell(20,63,9,9,7,4,0,true,1),
                new SudokuCell(37,64,1,7,8,6,0,true,1),
                new SudokuCell(36,65,2,7,8,1,0,true,1),
                new SudokuCell(35,66,3,7,8,2,2,false,1),
                new SudokuCell(34,67,4,8,8,4,0,true,1),
                new SudokuCell(33,68,5,8,8,7,7,false,1),
                new SudokuCell(32,69,6,8,8,3,0,true,1),
                new SudokuCell(31,70,7,9,8,8,0,true,1),
                new SudokuCell(30,71,8,9,8,5,0,true,1),
                new SudokuCell(29,72,9,9,8,9,0,true,1),
                new SudokuCell(28,73,1,7,9,8,0,true,1),
                new SudokuCell(27,74,2,7,9,4,0,true,1),
                new SudokuCell(26,75,3,7,9,3,0,true,1),
                new SudokuCell(25,76,4,8,9,5,0,true,1),
                new SudokuCell(24,77,5,8,9,9,0,true,1),
                new SudokuCell(23,78,6,8,9,2,0,true,1),
                new SudokuCell(22,79,7,9,9,7,7,false,1),
                new SudokuCell(21,80,8,9,9,1,0,true,1),
                new SudokuCell(60,81,9,9,9,6,6,false,1)
            };

            return cells;
        }

        public static List<SudokuCell> GetUpdateInvalidSudokuCells(int updatedValue)
        {
            var cells = new List<SudokuCell>()
            {
                new SudokuCell(81,1,1,1,1,4,4,false,2),
                new SudokuCell(58,2,2,1,1,2,updatedValue,true,1),
                new SudokuCell(57,3,3,1,1,8,8,false,1),
                new SudokuCell(56,4,4,2,1,6,0,true,1),
                new SudokuCell(55,5,5,2,1,1,0,true,1),
                new SudokuCell(54,6,6,2,1,5,0,true,1),
                new SudokuCell(53,7,7,3,1,9,0,true,1),
                new SudokuCell(52,8,8,3,1,3,0,true,1),
                new SudokuCell(51,9,9,3,1,7,0,true,1),
                new SudokuCell(50,10,1,1,2,3,3,false,1),
                new SudokuCell(49,11,2,1,2,9,0,true,1),
                new SudokuCell(48,12,3,1,2,6,6,false,1),
                new SudokuCell(47,13,4,2,2,2,0,true,1),
                new SudokuCell(46,14,5,2,2,8,0,true,1),
                new SudokuCell(45,15,6,2,2,7,0,true,1),
                new SudokuCell(44,16,7,3,2,1,1,false,1),
                new SudokuCell(43,17,8,3,2,4,4,false,1),
                new SudokuCell(42,18,9,3,2,5,0,false,1),
                new SudokuCell(59,19,1,1,3,1,0,true,1),
                new SudokuCell(61,20,2,1,3,5,0,true,1),
                new SudokuCell(80,21,3,1,3,7,7,false,1),
                new SudokuCell(62,22,4,2,3,3,0,true,1),
                new SudokuCell(79,23,5,2,3,4,0,true,1),
                new SudokuCell(78,24,6,2,3,9,9,false,1),
                new SudokuCell(77,25,7,3,3,6,0,true,1),
                new SudokuCell(76,26,8,3,3,8,0,true,1),
                new SudokuCell(75,27,9,3,3,2,2,false,1),
                new SudokuCell(74,28,1,4,4,5,0,true,1),
                new SudokuCell(73,29,2,4,4,8,8,false,1),
                new SudokuCell(72,30,3,4,4,4,4,false,1),
                new SudokuCell(71,31,4,5,4,9,0,true,1),
                new SudokuCell(70,32,5,5,4,3,0,true,1),
                new SudokuCell(69,33,6,5,4,6,0,true,1),
                new SudokuCell(68,34,7,6,4,2,0,true,1),
                new SudokuCell(67,35,8,6,4,7,0,true,1),
                new SudokuCell(66,36,9,6,4,1,0,true,1),
                new SudokuCell(65,37,1,4,5,7,7,false,1),
                new SudokuCell(64,38,2,4,5,6,0,true,1),
                new SudokuCell(63,39,3,4,5,1,0,true,1),
                new SudokuCell(41,40,4,5,5,8,8,false,1),
                new SudokuCell(40,41,5,5,5,2,2,false,1),
                new SudokuCell(39,42,6,5,5,4,4,false,1),
                new SudokuCell(38,43,7,6,5,5,5,false,1),
                new SudokuCell(16,44,8,6,5,9,0,true,1),
                new SudokuCell(15,45,9,6,5,3,0,true,1),
                new SudokuCell(14,46,1,4,6,2,2,false,1),
                new SudokuCell(13,47,2,4,6,3,0,true,1),
                new SudokuCell(12,48,3,4,6,9,9,false,1),
                new SudokuCell(11,49,4,5,6,7,7,false,1),
                new SudokuCell(10,50,5,5,6,5,5,false,1),
                new SudokuCell(17,51,6,5,6,1,0,true,1),
                new SudokuCell(9,52,7,6,6,4,4,false,1),
                new SudokuCell(7,53,8,6,6,6,6,false,1),
                new SudokuCell(6,54,9,6,6,8,0,true,1),
                new SudokuCell(5,55,1,7,7,9,0,true,1),
                new SudokuCell(4,56,2,7,7,7,0,true,1),
                new SudokuCell(3,57,3,7,7,5,0,true,1),
                new SudokuCell(2,58,4,8,7,1,0,true,1),
                new SudokuCell(1,59,5,8,7,6,0,true,1),
                new SudokuCell(8,60,6,8,7,8,8,false,1),
                new SudokuCell(18,61,7,9,7,3,3,false,1),
                new SudokuCell(19,62,8,9,7,2,2,false,1),
                new SudokuCell(20,63,9,9,7,4,0,true,1),
                new SudokuCell(37,64,1,7,8,6,0,true,1),
                new SudokuCell(36,65,2,7,8,1,0,true,1),
                new SudokuCell(35,66,3,7,8,2,2,false,1),
                new SudokuCell(34,67,4,8,8,4,0,true,1),
                new SudokuCell(33,68,5,8,8,7,7,false,1),
                new SudokuCell(32,69,6,8,8,3,0,true,1),
                new SudokuCell(31,70,7,9,8,8,0,true,1),
                new SudokuCell(30,71,8,9,8,5,0,true,1),
                new SudokuCell(29,72,9,9,8,9,0,true,1),
                new SudokuCell(28,73,1,7,9,8,0,true,1),
                new SudokuCell(27,74,2,7,9,4,0,true,1),
                new SudokuCell(26,75,3,7,9,3,0,true,1),
                new SudokuCell(25,76,4,8,9,5,0,true,1),
                new SudokuCell(24,77,5,8,9,9,0,true,1),
                new SudokuCell(23,78,6,8,9,2,0,true,1),
                new SudokuCell(22,79,7,9,9,7,7,false,1),
                new SudokuCell(21,80,8,9,9,1,0,true,1),
                new SudokuCell(60,81,9,9,9,6,6,false,1)
            };

            return cells;
        }

        public static List<SudokuCell> GetValidSudokuCells()
        {
            var cells = new List<SudokuCell>()
            {
                new SudokuCell(81,1,1,1,1,4,4,false,1),
                new SudokuCell(58,2,2,1,1,2,0,true,1),
                new SudokuCell(57,3,3,1,1,8,8,false,1),
                new SudokuCell(56,4,4,2,1,6,0,true,1),
                new SudokuCell(55,5,5,2,1,1,0,true,1),
                new SudokuCell(54,6,6,2,1,5,0,true,1),
                new SudokuCell(53,7,7,3,1,9,0,true,1),
                new SudokuCell(52,8,8,3,1,3,0,true,1),
                new SudokuCell(51,9,9,3,1,7,0,true,1),
                new SudokuCell(50,10,1,1,2,3,3,false,1),
                new SudokuCell(49,11,2,1,2,9,0,true,1),
                new SudokuCell(48,12,3,1,2,6,6,false,1),
                new SudokuCell(47,13,4,2,2,2,0,true,1),
                new SudokuCell(46,14,5,2,2,8,0,true,1),
                new SudokuCell(45,15,6,2,2,7,0,true,1),
                new SudokuCell(44,16,7,3,2,1,1,false,1),
                new SudokuCell(43,17,8,3,2,4,4,false,1),
                new SudokuCell(42,18,9,3,2,5,0,false,1),
                new SudokuCell(59,19,1,1,3,1,0,true,1),
                new SudokuCell(61,20,2,1,3,5,0,true,1),
                new SudokuCell(80,21,3,1,3,7,7,false,1),
                new SudokuCell(62,22,4,2,3,3,0,true,1),
                new SudokuCell(79,23,5,2,3,4,0,true,1),
                new SudokuCell(78,24,6,2,3,9,9,false,1),
                new SudokuCell(77,25,7,3,3,6,0,true,1),
                new SudokuCell(76,26,8,3,3,8,0,true,1),
                new SudokuCell(75,27,9,3,3,2,2,false,1),
                new SudokuCell(74,28,1,4,4,5,0,true,1),
                new SudokuCell(73,29,2,4,4,8,8,false,1),
                new SudokuCell(72,30,3,4,4,4,4,false,1),
                new SudokuCell(71,31,4,5,4,9,0,true,1),
                new SudokuCell(70,32,5,5,4,3,0,true,1),
                new SudokuCell(69,33,6,5,4,6,0,true,1),
                new SudokuCell(68,34,7,6,4,2,0,true,1),
                new SudokuCell(67,35,8,6,4,7,0,true,1),
                new SudokuCell(66,36,9,6,4,1,0,true,1),
                new SudokuCell(65,37,1,4,5,7,7,false,1),
                new SudokuCell(64,38,2,4,5,6,0,true,1),
                new SudokuCell(63,39,3,4,5,1,0,true,1),
                new SudokuCell(41,40,4,5,5,8,8,false,1),
                new SudokuCell(40,41,5,5,5,2,2,false,1),
                new SudokuCell(39,42,6,5,5,4,4,false,1),
                new SudokuCell(38,43,7,6,5,5,5,false,1),
                new SudokuCell(16,44,8,6,5,9,0,true,1),
                new SudokuCell(15,45,9,6,5,3,0,true,1),
                new SudokuCell(14,46,1,4,6,2,2,false,1),
                new SudokuCell(13,47,2,4,6,3,0,true,1),
                new SudokuCell(12,48,3,4,6,9,9,false,1),
                new SudokuCell(11,49,4,5,6,7,7,false,1),
                new SudokuCell(10,50,5,5,6,5,5,false,1),
                new SudokuCell(17,51,6,5,6,1,0,true,1),
                new SudokuCell(9,52,7,6,6,4,4,false,1),
                new SudokuCell(7,53,8,6,6,6,6,false,1),
                new SudokuCell(6,54,9,6,6,8,0,true,1),
                new SudokuCell(5,55,1,7,7,9,0,true,1),
                new SudokuCell(4,56,2,7,7,7,0,true,1),
                new SudokuCell(3,57,3,7,7,5,0,true,1),
                new SudokuCell(2,58,4,8,7,1,0,true,1),
                new SudokuCell(1,59,5,8,7,6,0,true,1),
                new SudokuCell(8,60,6,8,7,8,8,false,1),
                new SudokuCell(18,61,7,9,7,3,3,false,1),
                new SudokuCell(19,62,8,9,7,2,2,false,1),
                new SudokuCell(20,63,9,9,7,4,0,true,1),
                new SudokuCell(37,64,1,7,8,6,0,true,1),
                new SudokuCell(36,65,2,7,8,1,0,true,1),
                new SudokuCell(35,66,3,7,8,2,2,false,1),
                new SudokuCell(34,67,4,8,8,4,0,true,1),
                new SudokuCell(33,68,5,8,8,7,7,false,1),
                new SudokuCell(32,69,6,8,8,3,0,true,1),
                new SudokuCell(31,70,7,9,8,8,0,true,1),
                new SudokuCell(30,71,8,9,8,5,0,true,1),
                new SudokuCell(29,72,9,9,8,9,0,true,1),
                new SudokuCell(28,73,1,7,9,8,0,true,1),
                new SudokuCell(27,74,2,7,9,4,0,true,1),
                new SudokuCell(26,75,3,7,9,3,0,true,1),
                new SudokuCell(25,76,4,8,9,5,0,true,1),
                new SudokuCell(24,77,5,8,9,9,0,true,1),
                new SudokuCell(23,78,6,8,9,2,0,true,1),
                new SudokuCell(22,79,7,9,9,7,7,false,1),
                new SudokuCell(21,80,8,9,9,1,0,true,1),
                new SudokuCell(60,81,9,9,9,6,6,false,1)
            };

            return cells;
        }

        public static List<SudokuCell> GetSolvedSudokuCells()
        {
            var cells = new List<SudokuCell>()
            {
                new SudokuCell(81,1,1,1,1,4,4,false,1),
                new SudokuCell(58,2,2,1,1,2,2,true,1),
                new SudokuCell(57,3,3,1,1,8,8,false,1),
                new SudokuCell(56,4,4,2,1,6,6,true,1),
                new SudokuCell(55,5,5,2,1,1,1,true,1),
                new SudokuCell(54,6,6,2,1,5,5,true,1),
                new SudokuCell(53,7,7,3,1,9,9,true,1),
                new SudokuCell(52,8,8,3,1,3,3,true,1),
                new SudokuCell(51,9,9,3,1,7,7,true,1),
                new SudokuCell(50,10,1,1,2,3,3,false,1),
                new SudokuCell(49,11,2,1,2,9,9,true,1),
                new SudokuCell(48,12,3,1,2,6,6,false,1),
                new SudokuCell(47,13,4,2,2,2,2,true,1),
                new SudokuCell(46,14,5,2,2,8,8,true,1),
                new SudokuCell(45,15,6,2,2,7,7,true,1),
                new SudokuCell(44,16,7,3,2,1,1,false,1),
                new SudokuCell(43,17,8,3,2,4,4,false,1),
                new SudokuCell(42,18,9,3,2,5,5,false,1),
                new SudokuCell(59,19,1,1,3,1,1,true,1),
                new SudokuCell(61,20,2,1,3,5,5,true,1),
                new SudokuCell(80,21,3,1,3,7,7,false,1),
                new SudokuCell(62,22,4,2,3,3,3,true,1),
                new SudokuCell(79,23,5,2,3,4,4,true,1),
                new SudokuCell(78,24,6,2,3,9,9,false,1),
                new SudokuCell(77,25,7,3,3,6,6,true,1),
                new SudokuCell(76,26,8,3,3,8,8,true,1),
                new SudokuCell(75,27,9,3,3,2,2,false,1),
                new SudokuCell(74,28,1,4,4,5,5,true,1),
                new SudokuCell(73,29,2,4,4,8,8,false,1),
                new SudokuCell(72,30,3,4,4,4,4,false,1),
                new SudokuCell(71,31,4,5,4,9,9,true,1),
                new SudokuCell(70,32,5,5,4,3,3,true,1),
                new SudokuCell(69,33,6,5,4,6,6,true,1),
                new SudokuCell(68,34,7,6,4,2,2,true,1),
                new SudokuCell(67,35,8,6,4,7,7,true,1),
                new SudokuCell(66,36,9,6,4,1,1,true,1),
                new SudokuCell(65,37,1,4,5,7,7,false,1),
                new SudokuCell(64,38,2,4,5,6,6,true,1),
                new SudokuCell(63,39,3,4,5,1,1,true,1),
                new SudokuCell(41,40,4,5,5,8,8,false,1),
                new SudokuCell(40,41,5,5,5,2,2,false,1),
                new SudokuCell(39,42,6,5,5,4,4,false,1),
                new SudokuCell(38,43,7,6,5,5,5,false,1),
                new SudokuCell(16,44,8,6,5,9,9,true,1),
                new SudokuCell(15,45,9,6,5,3,3,true,1),
                new SudokuCell(14,46,1,4,6,2,2,false,1),
                new SudokuCell(13,47,2,4,6,3,3,true,1),
                new SudokuCell(12,48,3,4,6,9,9,false,1),
                new SudokuCell(11,49,4,5,6,7,7,false,1),
                new SudokuCell(10,50,5,5,6,5,5,false,1),
                new SudokuCell(17,51,6,5,6,1,1,true,1),
                new SudokuCell(9,52,7,6,6,4,4,false,1),
                new SudokuCell(7,53,8,6,6,6,6,false,1),
                new SudokuCell(6,54,9,6,6,8,8,true,1),
                new SudokuCell(5,55,1,7,7,9,9,true,1),
                new SudokuCell(4,56,2,7,7,7,7,true,1),
                new SudokuCell(3,57,3,7,7,5,5,true,1),
                new SudokuCell(2,58,4,8,7,1,1,true,1),
                new SudokuCell(1,59,5,8,7,6,6,true,1),
                new SudokuCell(8,60,6,8,7,8,8,false,1),
                new SudokuCell(18,61,7,9,7,3,3,false,1),
                new SudokuCell(19,62,8,9,7,2,2,false,1),
                new SudokuCell(20,63,9,9,7,4,4,true,1),
                new SudokuCell(37,64,1,7,8,6,6,true,1),
                new SudokuCell(36,65,2,7,8,1,1,true,1),
                new SudokuCell(35,66,3,7,8,2,2,false,1),
                new SudokuCell(34,67,4,8,8,4,4,true,1),
                new SudokuCell(33,68,5,8,8,7,7,false,1),
                new SudokuCell(32,69,6,8,8,3,3,true,1),
                new SudokuCell(31,70,7,9,8,8,8,true,1),
                new SudokuCell(30,71,8,9,8,5,5,true,1),
                new SudokuCell(29,72,9,9,8,9,9,true,1),
                new SudokuCell(28,73,1,7,9,8,8,true,1),
                new SudokuCell(27,74,2,7,9,4,4,true,1),
                new SudokuCell(26,75,3,7,9,3,3,true,1),
                new SudokuCell(25,76,4,8,9,5,5,true,1),
                new SudokuCell(24,77,5,8,9,9,9,true,1),
                new SudokuCell(23,78,6,8,9,2,2,true,1),
                new SudokuCell(22,79,7,9,9,7,7,false,1),
                new SudokuCell(21,80,8,9,9,1,1,true,1),
                new SudokuCell(60,81,9,9,9,6,6,false,1)
            };

            return cells;
        }

        public static EmailMetaData GetEmailMetaData()
        {
            return new EmailMetaData()
            {
                SmtpServer = "smtp.gmail.com",
                Port = 465,
                UserName = "sudokucollectivetesting@gmail.com",
                Password = "testing@123",
                FromEmail = "SudokuCollectivetesting@gmail.com"
            };
        }

        public static EmailMetaData GetIncorrectEmailMetaData()
        {
            return new EmailMetaData()
            {
                SmtpServer = "smtp.mail.yahoo.com",
                Port = 465,
                UserName = "sudokucollectivetesting@yahoo.com",
                Password = "P@ssw0rd1",
                FromEmail = "SudokuCollectivetesting@yahoo.com"
            };
        }

        public static User GetNewUser()
        {
            return new User
            {
                FirstName = "John",
                LastName = "Doe",
                UserName = "Johnny",
                Email = "john.doe@example.com",
                Password = "password1",
                DateCreated = DateTime.UtcNow
            };
        }

        public static EmailConfirmation GetNewEmailConfirmation()
        {
            return new EmailConfirmation
            {
                UserId = 1,
                AppId = 1,
                Token = "CC924471-AAB6-4809-8E6D-723AE422CB33"
            };
        }

        public static EmailConfirmation GetUpdateEmailConfirmation()
        {
            return new EmailConfirmation
            {
                UserId = 1,
                AppId = 1,
                Token = "CC924471-AAB6-4809-8E6D-723AE422CB33",
                OldEmailAddress = "TestSuperUser@example.com",
                NewEmailAddress = "UPDATEDTestSuperUser@example.com"
            };
        }
    }
}
