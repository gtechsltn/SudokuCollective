using System.Linq;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.ServiceModels;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Core.Models;

namespace SudokuCollective.Data.Utilities
{
    internal static class PaginatorUtilities
    {
        internal static Result PaginateApps(
            IPaginator paginator, 
            IRepositoryResponse response, 
            Result result)
        {
            if (paginator.SortBy == SortValue.NULL)
            {
                result.Payload.AddRange(response
                    .Objects
                    .OrderBy(a => ((IApp)a).Id)
                    .ToList()
                    .ConvertAll(a => (object)a));
            }
            else if (paginator.SortBy == SortValue.ID)
            {
                result.Payload.AddRange(response
                    .Objects
                    .ToList()
                    .ConvertAll(a => (object)a));

                if (!paginator.OrderByDescending)
                {
                    result.Payload = result.Payload
                        .OrderBy(a => ((IApp)a).Id)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
                else
                {
                    result.Payload = result.Payload
                        .OrderByDescending(a => ((IApp)a).Id)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
            }
            else if (paginator.SortBy == SortValue.USERCOUNT)
            {
                result.Payload.AddRange(response
                    .Objects
                    .ToList()
                    .ConvertAll(a => (object)a));

                if (!paginator.OrderByDescending)
                {
                    result.Payload = result.Payload
                        .OrderBy(a => ((IApp)a).UserCount)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
                else
                {
                    result.Payload = result.Payload
                        .OrderByDescending(a => ((IApp)a).UserCount)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
            }
            else if (paginator.SortBy == SortValue.NAME)
            {
                result.Payload.AddRange(response
                    .Objects
                    .ToList()
                    .ConvertAll(a => (object)a));

                if (!paginator.OrderByDescending)
                {
                    result.Payload = result.Payload
                        .OrderBy(a => ((IApp)a).Name)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
                else
                {
                    result.Payload = result.Payload
                        .OrderByDescending(a => ((IApp)a).Name)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
            }
            else if (paginator.SortBy == SortValue.DATECREATED)
            {
                result.Payload.AddRange(response
                    .Objects
                    .ToList()
                    .ConvertAll(a => (object)a));

                if (!paginator.OrderByDescending)
                {
                    result.Payload = result.Payload
                        .OrderBy(a => ((IApp)a).DateCreated)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
                else
                {
                    result.Payload = result.Payload
                        .OrderByDescending(a => ((IApp)a).DateCreated)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
            }
            else if (paginator.SortBy == SortValue.DATEUPDATED)
            {
                result.Payload.AddRange(response
                    .Objects
                    .ToList()
                    .ConvertAll(a => (object)a));

                if (!paginator.OrderByDescending)
                {
                    result.Payload = result.Payload
                        .OrderBy(a => ((IApp)a).DateUpdated)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
                else
                {
                    result.Payload = result.Payload
                        .OrderByDescending(a => ((IApp)a).DateUpdated)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Message = ServicesMesages.SortValueNotImplementedMessage;
            }

            return result;
        }

        internal static Result PaginateGames(
            IPaginator paginator, 
            IRepositoryResponse response, 
            Result result)
        {
            if (paginator.SortBy == SortValue.NULL)
            {
                result.Payload.AddRange(response
                    .Objects
                    .OrderBy(g => ((IGame)g).Id)
                    .ToList()
                    .ConvertAll(g => (object)g));
            }
            else if (paginator.SortBy == SortValue.ID)
            {
                result.Payload.AddRange(response
                    .Objects
                    .OrderBy(g => ((IGame)g).Id)
                    .ToList()
                    .ConvertAll(g => (object)g));

                if (!paginator.OrderByDescending)
                {
                    result.Payload = result.Payload
                        .OrderBy(g => ((IGame)g).Id)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
                else
                {
                    result.Payload = result.Payload
                        .OrderByDescending(g => ((IGame)g).Id)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
            }
            else if (paginator.SortBy == SortValue.SCORE)
            {
                result.Payload.AddRange(response
                    .Objects
                    .OrderBy(g => ((IGame)g).Id)
                    .ToList()
                    .ConvertAll(g => (object)g));

                if (!paginator.OrderByDescending)
                {
                    result.Payload = result.Payload
                        .Where(g => ((IGame)g).Score != 0 && 
                            ((IGame)g).Score != int.MaxValue && 
                            ((IGame)g).Score != 0 &&
                            !((IGame)g).ContinueGame)
                        .OrderByDescending(g => ((IGame)g).Score)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
                else
                {
                    result.Payload = result.Payload
                        .Where(g => ((IGame)g).Score != 0 && 
                            ((IGame)g).Score != int.MaxValue && 
                            ((IGame)g).Score != 0 &&
                            !((IGame)g).ContinueGame)
                        .OrderBy(g => ((IGame)g).Score)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
            }
            else if (paginator.SortBy == SortValue.DATECREATED)
            {
                result.Payload.AddRange(response
                    .Objects
                    .OrderBy(g => ((IGame)g).Id)
                    .ToList()
                    .ConvertAll(g => (object)g));

                if (!paginator.OrderByDescending)
                {
                    result.Payload = result.Payload
                        .OrderBy(g => ((IGame)g).DateCreated)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
                else
                {
                    result.Payload = result.Payload
                        .OrderByDescending(g => ((IGame)g).DateCreated)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
            }
            else if (paginator.SortBy == SortValue.DATEUPDATED)
            {
                result.Payload.AddRange(response
                    .Objects
                    .OrderBy(g => ((IGame)g).Id)
                    .ToList()
                    .ConvertAll(g => (object)g));

                if (!paginator.OrderByDescending)
                {
                    result.Payload = result.Payload
                        .OrderBy(g => ((IGame)g).DateUpdated)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
                else
                {
                    result.Payload = result.Payload
                        .OrderByDescending(g => ((IGame)g).DateUpdated)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Message = ServicesMesages.SortValueNotImplementedMessage;
            }
            
            return result;
        }
        
        internal static Result PaginateSolutions(
            IPaginator paginator,
            IRepositoryResponse response,
            Result result)
        {
            if (paginator.SortBy == SortValue.NULL)
            {
                result.Payload.AddRange(response
                    .Objects
                    .OrderBy(s => ((ISudokuSolution)s).Id)
                    .ToList()
                    .ConvertAll(s => (object)s));
            }
            else if (paginator.SortBy == SortValue.ID)
            {
                result.Payload.AddRange(response
                    .Objects
                    .ToList()
                    .ConvertAll(s => (object)s));

                if (!paginator.OrderByDescending)
                {
                    result.Payload = result.Payload
                        .OrderBy(s => ((ISudokuSolution)s).Id)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
                else
                {
                    result.Payload = result.Payload
                        .OrderByDescending(s => ((ISudokuSolution)s).Id)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
            }
            else if (paginator.SortBy == SortValue.DATECREATED)
            {
                result.Payload.AddRange(response
                    .Objects
                    .ToList()
                    .ConvertAll(s => (object)s));

                if (!paginator.OrderByDescending)
                {
                    result.Payload = result.Payload
                        .OrderBy(s => ((ISudokuSolution)s).DateCreated)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
                else
                {
                    result.Payload = result.Payload
                        .OrderByDescending(s => ((ISudokuSolution)s).DateCreated)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
            }
            else if (paginator.SortBy == SortValue.DATEUPDATED)
            {
                result.Payload.AddRange(response
                    .Objects
                    .ToList()
                    .ConvertAll(s => (object)s));

                if (!paginator.OrderByDescending)
                {
                    result.Payload = result.Payload
                        .OrderBy(s => ((ISudokuSolution)s).DateSolved)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
                else
                {
                    result.Payload = result.Payload
                        .OrderByDescending(s => ((ISudokuSolution)s).DateSolved)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Message = ServicesMesages.SortValueNotImplementedMessage;
            }
            
            return result;
        }

        internal static Result PaginateUsers(
            IPaginator paginator, 
            IRepositoryResponse response, 
            Result result)
        {
            foreach (var user in response.Objects.ConvertAll(u => (User)u))
            {
                user.NullifyProperties();
            }

            if (paginator.SortBy == SortValue.NULL)
            {
                result.Payload.AddRange(response
                    .Objects
                    .OrderBy(u => ((IUser)u).Id)
                    .ToList()
                    .ConvertAll(u => (object)u));
            }
            else if (paginator.SortBy == SortValue.ID)
            {
                result.Payload.AddRange(response
                    .Objects
                    .ToList()
                    .ConvertAll(u => (object)u));

                if (!paginator.OrderByDescending)
                {
                    result.Payload = result.Payload
                        .OrderBy(u => ((IUser)u).Id)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
                else
                {
                    result.Payload = result.Payload
                        .OrderByDescending(u => ((IUser)u).Id)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
            }
            else if (paginator.SortBy == SortValue.USERNAME)
            {
                result.Payload.AddRange(response
                    .Objects
                    .ToList()
                    .ConvertAll(u => (object)u));

                if (!paginator.OrderByDescending)
                {
                    result.Payload = result.Payload
                        .OrderBy(u => ((IUser)u).UserName)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
                else
                {
                    result.Payload = result.Payload
                        .OrderByDescending(u => ((IUser)u).UserName)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
            }
            else if (paginator.SortBy == SortValue.FIRSTNAME)
            {
                result.Payload.AddRange(response
                    .Objects
                    .ToList()
                    .ConvertAll(u => (object)u));

                if (!paginator.OrderByDescending)
                {
                    result.Payload = result.Payload
                        .OrderBy(u => ((IUser)u).FirstName)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
                else
                {
                    result.Payload = result.Payload
                        .OrderByDescending(u => ((IUser)u).FirstName)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
            }
            else if (paginator.SortBy == SortValue.LASTNAME)
            {
                result.Payload.AddRange(response
                    .Objects
                    .ToList()
                    .ConvertAll(u => (object)u));

                if (!paginator.OrderByDescending)
                {
                    result.Payload = result.Payload
                        .OrderBy(u => ((IUser)u).LastName)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
                else
                {
                    result.Payload = result.Payload
                        .OrderByDescending(u => ((IUser)u).LastName)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
            }
            else if (paginator.SortBy == SortValue.FULLNAME)
            {
                result.Payload.AddRange(response
                    .Objects
                    .ToList()
                    .ConvertAll(u => (object)u));

                if (!paginator.OrderByDescending)
                {
                    result.Payload = result.Payload
                        .OrderBy(u => ((IUser)u).FullName)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
                else
                {
                    result.Payload = result.Payload
                        .OrderByDescending(u => ((IUser)u).FullName)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
            }
            else if (paginator.SortBy == SortValue.NICKNAME)
            {
                result.Payload.AddRange(response
                    .Objects
                    .ToList()
                    .ConvertAll(u => (object)u));

                if (!paginator.OrderByDescending)
                {
                    result.Payload = result.Payload
                        .OrderBy(u => ((IUser)u).NickName)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
                else
                {
                    result.Payload = result.Payload
                        .OrderByDescending(u => ((IUser)u).NickName)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
            }
            else if (paginator.SortBy == SortValue.GAMECOUNT)
            {
                result.Payload.AddRange(response
                    .Objects
                    .ToList()
                    .ConvertAll(u => (object)u));

                if (!paginator.OrderByDescending)
                {
                    result.Payload = result.Payload
                        .OrderBy(u => ((IUser)u).Games.Count)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
                else
                {
                    result.Payload = result.Payload
                        .OrderByDescending(u => ((IUser)u).Games.Count)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
            }
            else if (paginator.SortBy == SortValue.DATECREATED)
            {
                result.Payload.AddRange(response
                    .Objects
                    .ToList()
                    .ConvertAll(u => (object)u));

                if (!paginator.OrderByDescending)
                {
                    result.Payload = result.Payload
                        .OrderBy(u => ((IUser)u).DateCreated)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
                else
                {
                    result.Payload = result.Payload
                        .OrderByDescending(u => ((IUser)u).DateCreated)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
            }
            else if (paginator.SortBy == SortValue.DATEUPDATED)
            {
                result.Payload.AddRange(response
                    .Objects
                    .ToList()
                    .ConvertAll(u => (object)u));

                if (!paginator.OrderByDescending)
                {
                    result.Payload = result.Payload
                        .OrderBy(u => ((IUser)u).DateUpdated)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
                else
                {
                    result.Payload = result.Payload
                        .OrderByDescending(u => ((IUser)u).DateUpdated)
                        .Skip((paginator.Page - 1) * paginator.ItemsPerPage)
                        .Take(paginator.ItemsPerPage)
                        .ToList();
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Message = ServicesMesages.SortValueNotImplementedMessage;
            }

            return result;
        }
    }
}
