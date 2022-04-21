using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Moq;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Payloads;
using SudokuCollective.Data.Models.Results;

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

        public static AnnonymousCheckPayload GetValidAnnonymousCheckPayload()
        {
            return new AnnonymousCheckPayload
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

        public static AnnonymousCheckPayload GetInvalidAnnonymousCheckPayload()
        {
            return new AnnonymousCheckPayload
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

        public static List<List<int>> GetAnnonymousGame() => new List<List<int>>()
            {
                new List<int> { 7, 8, 5, 4, 1, 3, 2, 9, 6 },
                new List<int> { 1, 4, 2, 8, 6, 9, 5, 7, 3 },
                new List<int> { 6, 9, 3, 2, 7, 5, 4, 1, 8 },
                new List<int> { 5, 1, 4, 3, 8, 2, 7, 6, 9 },
                new List<int> { 2, 6, 7, 9, 4, 1, 3, 8, 5 },
                new List<int> { 8, 3, 9, 7, 5, 6, 1, 2, 4 },
                new List<int> { 4, 2, 1, 6, 3, 8, 9, 5, 7 },
                new List<int> { 3, 5, 8, 1, 9, 7, 6, 4, 2 },
                new List<int> { 9, 7, 6, 5, 2, 4, 8, 3, 1 }
            };

        public static SolutionPayload GetValidSolutionPayload()
        {
            return new SolutionPayload
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

        public static SolutionPayload GetInvalidSolutionPayload()
        {
            return new SolutionPayload
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
                SmtpServer = "email-smtp.us-east-1.amazonaws.com",
                Port = 465,
                UserName = "AKIAVPGOYLGBWICBZ4GW",
                Password = "BMvhz4xMUuhlghfzkG1ooc66eqDqcB7aKCgiDvE1PogP",
                FromEmail = "sudokucollective-testing@sudokucollective.com"
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
                Password = "P@s5w1",
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

        public static Request GetRequest()
        {
            return new Request()
            {
                License = GetLicense(),
                RequestorId = 1,
                AppId = 1,
                Paginator = GetPaginator(),
                Payload = new JsonElement()
            };
        }

        public static CreateGamePayload GetCreateGamePayload()
        {
            return new CreateGamePayload()
            {
                UserId = 1,
                DifficultyId = 4
            };
        }

        public static UpdateGamePayload GetUpdateGamePayload(int updatedValue)
        {
            return new UpdateGamePayload()
            {
                GameId = 1,
                SudokuCells = GetUpdateSudokuCells(updatedValue)
            };
        }

        public static UpdateGamePayload GetInvalidUpdateGamePayload(int updatedValue)
        {
            return new UpdateGamePayload()
            {
                GameId = 1,
                SudokuCells = GetUpdateInvalidSudokuCells(updatedValue)
            };
        }

        public static UpdateGamePayload GetSolvedUpdateGamePayload()
        {
            return new UpdateGamePayload()
            {
                GameId = 1,
                SudokuCells = GetValidSudokuCells()
            };
        }

        public static UpdateGamePayload GetGameNotFoundUpdateGamePayload()
        {
            return new UpdateGamePayload()
            {
                GameId = 5,
                SudokuCells = GetValidSudokuCells()
            };
        }

        public static GamesPayload GetGamesPayload()
        {
            return new GamesPayload()
            {
                UserId = 1,
            };
        }

        public static UpdateRolePayload GetUpdateRolePayload()
        {
            return new UpdateRolePayload()
            {
                Id = 1,
                Name = "Null UPDATED!",
                RoleLevel = RoleLevel.NULL
            };
        }

        public static UpdateRolePayload GetInvalidUpdateRolePayload()
        {
            return new UpdateRolePayload()
            {
                Id = 10,
                Name = "Null UPDATED!",
                RoleLevel = RoleLevel.NULL
            };
        }

        public static Result GetResult()
        {
            return new Result()
            {
                IsSuccess = true,
                IsFromCache = true,
            };
        }

        public static ConfirmEmailResult GetConfirmEmailResult()
        {
            return new ConfirmEmailResult()
            {
                UserName = "TestSuperUser",
                Email = "TestSuperUser@example.com",
                DateUpdated = DateTime.Now,
                AppTitle = "Test App 1",
                AppUrl = "https://localhost:4200",
                IsUpdate = true,
                NewEmailAddressConfirmed = false,
                ConfirmationEmailSuccessfullySent = true
            };
        }

        public static InitiatePasswordResetResult GetInitiatePasswordResetResult()
        {
            return new InitiatePasswordResetResult()
            {
                App = new App(),
                User = new User(),
                ConfirmationEmailSuccessfullySent = true,
                Token = GetToken()
            };
        }

        public static UserResult GetUserResult()
        {
            return new UserResult()
            {
                User = new User(),
                ConfirmationEmailSuccessfullySent = true,
                Token = GetToken()
            };
        }

        public static Mock<IHttpContextAccessor> GetHttpContextAccessor(User user, App app)
        {
            var result = new Mock<IHttpContextAccessor>();

            var claim = new List<Claim> {

                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Name, app.Id.ToString()),
            };
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("865542af-e02f-446d-ad34-b121554f37be"));
            
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expirationLimit = DateTime.UtcNow.AddDays(1);

            var jwtToken = new JwtSecurityToken(
                    "test",
                    "test",
                    claim.ToArray(),
                    notBefore: DateTime.UtcNow,
                    expires: expirationLimit,
                    signingCredentials: credentials
                );

            result.Setup(
                mock => mock.HttpContext.Request.Headers["Authorization"])
                .Returns(string.Format("bearer {0}", new JwtSecurityTokenHandler().WriteToken(jwtToken)));

            var request = GetRequest();

            string json;

            try 
            {
                json = JsonSerializer.Serialize<SudokuCollective.Logs.Models.Request>((SudokuCollective.Logs.Models.Request)request);
            }
            catch
            {
                throw;
            }

            byte[] byteArray = Encoding.UTF8.GetBytes(json);
            //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
            MemoryStream stream = new MemoryStream(byteArray);

            result.Setup(
                mock => mock.HttpContext.Request.Body)
                .Returns(stream);

            return result;
        }

        public static Mock<IHttpContextAccessor> GetInvalidHttpContextAccessor(User user)
        {
            var result = new Mock<IHttpContextAccessor>();

            var appId = 1;

            var claim = new List<Claim> {

                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Name, appId.ToString()),
            };
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("865542af-e02f-446d-ad34-b121554f37be"));
            
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expirationLimit = DateTime.UtcNow.AddDays(1);

            var jwtInvalidToken = new JwtSecurityToken(
                    "test",
                    "test",
                    claim.ToArray(),
                    notBefore: DateTime.UtcNow,
                    expires: expirationLimit,
                    signingCredentials: credentials
                );

            result.Setup(
                mock => mock.HttpContext.Request.Headers["Authorization"])
                .Returns(string.Format("bearer {0}", new JwtSecurityTokenHandler().WriteToken(jwtInvalidToken)));

            return result;
        }

        public static SudokuCollective.Api.Models.PasswordReset GetPasswordReset()
        {
            return new SudokuCollective.Api.Models.PasswordReset
            {
                IsSuccess = true,
                UserId = 1,
                UserName = "UserName",
                NewPassword = "N3wP@ssw0rd!",
                ConfirmNewPassword = "N3wP@ssw0rd!",
                AppTitle = "AppTItle",
                AppId = 1,
                AppUrl = "https://localhost:5001",
                ErrorMessage = "Message"
            };
        }
    }
}
