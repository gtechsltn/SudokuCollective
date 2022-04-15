﻿using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;

namespace SudokuCollective.Data.Models.Params
{
    public class Paginator : IPaginator
    {
        public int Page { get; set; }
        public int ItemsPerPage { get; set; }
        public SortValue SortBy { get; set; }
        public bool OrderByDescending { get; set; }
        public bool IncludeCompletedGames { get; set; }

        public Paginator()
        {
            Page = 0;
            ItemsPerPage = 0;
            SortBy = SortValue.NULL;
            OrderByDescending = false;
            IncludeCompletedGames = false;
        }

        public Paginator(
            int page,
            int itemsPerPage,
            int sortValue,
            bool orderByDescending,
            bool includeCompletedGames)
        {
            Page = page;
            ItemsPerPage = itemsPerPage;
            SortBy = (SortValue)sortValue;
            OrderByDescending = orderByDescending;
            IncludeCompletedGames = includeCompletedGames;
        }

        public Paginator(
            int page,
            int itemsPerPage,
            SortValue sortValue,
            bool orderByDescending,
            bool includeCompletedGames)
        {
            Page = page;
            ItemsPerPage = itemsPerPage;
            SortBy = sortValue;
            OrderByDescending = orderByDescending;
            IncludeCompletedGames = includeCompletedGames;
        }

        public bool IsNull()
        {
            var result = false;

            if (Page == 0
                && ItemsPerPage == 0
                && SortBy == SortValue.NULL
                && OrderByDescending == false
                && IncludeCompletedGames == false)
            {
                result = true;
            }

            return result;
        }

        public static explicit operator SudokuCollective.Logs.Models.Paginator(Paginator paginator)
        {
            var result = new SudokuCollective.Logs.Models.Paginator();

            result.Page = paginator.Page;
            result.ItemsPerPage = paginator.ItemsPerPage;
            result.SortBy = paginator.SortBy;
            result.OrderByDescending = paginator.OrderByDescending;
            result.IncludeCompletedGames = paginator.IncludeCompletedGames;

            return result;
        }
    }
}
