using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SudokuCollective.Core.Interfaces.ServiceModels;
using SudokuCollective.Core.Interfaces.Models;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models;
using SudokuCollective.Repos.Utilities;

namespace SudokuCollective.Repos
{
    public class EmailConfirmationsRepository<TEntity> : IEmailConfirmationsRepository<TEntity> where TEntity : EmailConfirmation
    {
        #region Fields
        private readonly DatabaseContext _context;
        private readonly ILogger<EmailConfirmationsRepository<EmailConfirmation>> _logger;
        #endregion

        #region Constructor
        public EmailConfirmationsRepository(
            DatabaseContext context,
            ILogger<EmailConfirmationsRepository<EmailConfirmation>> logger)
        {
            _context = context;
            _logger = logger;
        }
        #endregion

        #region Methods
        public async Task<IRepositoryResponse> Create(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var result = new RepositoryResponse();

            try
            {
                if (entity.Id != 0)
                {
                    result.IsSuccess = false;

                    return result;
                }

                if (await _context.EmailConfirmations
                        .AnyAsync(pu => pu.Token.ToLower().Equals(entity.Token.ToLower())))
                {
                    result.IsSuccess = false;

                    return result;
                }

                _context.Attach(entity);

                foreach (var entry in _context.ChangeTracker.Entries())
                {
                    var dbEntry = (IDomainEntity)entry.Entity;

                    if (dbEntry is UserApp)
                    {
                        entry.State = EntityState.Modified;
                    }
                    else if (dbEntry is UserRole)
                    {
                        entry.State = EntityState.Modified;
                    }
                    else
                    {
                        // Otherwise do nothing...
                    }
                }

                await _context.SaveChangesAsync();

                result.IsSuccess = true;
                result.Object = entity;

                return result;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Exception = e;

                _logger.LogError(
                    ReposUtilities.GetRepoErrorEventId(),
                    string.Format(LoggerMessages.ErrorThrownMessage, e.Message)
                );

                return result;
            }
        }

        public async Task<IRepositoryResponse> Get(string token)
        {
            var result = new RepositoryResponse();

            if (string.IsNullOrEmpty(token))
            {
                result.IsSuccess = false;

                return result;
            }

            try
            {
                var query = await _context
                    .EmailConfirmations
                    .FirstOrDefaultAsync(ec => ec.Token.ToLower().Equals(token.ToLower()));

                if (query == null)
                {
                    result.IsSuccess = false;
                }
                else
                {
                    result.IsSuccess = true;
                    result.Object = query;
                }

                return result;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Exception = e;

                _logger.LogError(
                    ReposUtilities.GetRepoErrorEventId(),
                    string.Format(LoggerMessages.ErrorThrownMessage, e.Message)
                );

                return result;
            }
        }

        public async Task<IRepositoryResponse> GetAll()
        {
            var result = new RepositoryResponse();

            try
            {
                var query = await _context
                    .EmailConfirmations
                    .OrderBy(ec => ec.Id)
                    .ToListAsync();

                if (query.Count == 0)
                {
                    result.IsSuccess = false;
                }
                else
                {
                    result.IsSuccess = true;
                    result.Objects = query
                        .ConvertAll(ec => (IDomainEntity)ec)
                        .ToList();
                }

                return result;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Exception = e;

                _logger.LogError(
                    ReposUtilities.GetRepoErrorEventId(),
                    string.Format(LoggerMessages.ErrorThrownMessage, e.Message)
                );

                return result;
            }
        }

        public async Task<IRepositoryResponse> Update(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var result = new RepositoryResponse();

            try
            {
                var tokenNotUniqueList = await _context.EmailConfirmations
                    .Where(ec => ec.Token.ToLower().Equals(entity.Token.ToLower()) && ec.Id != entity.Id)
                    .ToListAsync();

                if (await _context.EmailConfirmations
                    .AnyAsync(ec => ec.Id == entity.Id) && tokenNotUniqueList.Count == 0)
                {
                    _context.Attach(entity);

                    foreach (var entry in _context.ChangeTracker.Entries())
                    {
                        var dbEntry = (IDomainEntity)entry.Entity;

                        if (dbEntry is UserApp)
                        {
                            entry.State = EntityState.Modified;
                        }
                        else if (dbEntry is UserRole)
                        {
                            entry.State = EntityState.Modified;
                        }
                        else
                        {
                            // Otherwise do nothing...
                        }
                    }

                    await _context.SaveChangesAsync();

                    result.IsSuccess = true;
                    result.Object = entity;

                    return result;
                }
                else
                {
                    result.IsSuccess = false;

                    return result;
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Exception = e;

                _logger.LogError(
                    ReposUtilities.GetRepoErrorEventId(),
                    string.Format(LoggerMessages.ErrorThrownMessage, e.Message)
                );

                return result;
            }
        }

        public async Task<IRepositoryResponse> Delete(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var result = new RepositoryResponse();

            try
            {
                if (await _context.EmailConfirmations.AnyAsync(ec => ec.Id == entity.Id))
                {
                    _context.Remove(entity);

                    foreach (var entry in _context.ChangeTracker.Entries())
                    {
                        var dbEntry = (IDomainEntity)entry.Entity;

                        if (dbEntry is UserApp)
                        {
                            entry.State = EntityState.Modified;
                        }
                        else if (dbEntry is UserRole)
                        {
                            entry.State = EntityState.Modified;
                        }
                        else
                        {
                            // Otherwise do nothing...
                        }
                    }

                    await _context.SaveChangesAsync();

                    result.IsSuccess = true;
                    result.Object = entity;

                    return result;
                }
                else
                {
                    result.IsSuccess = false;

                    return result;
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Exception = e;

                _logger.LogError(
                    ReposUtilities.GetRepoErrorEventId(),
                    string.Format(LoggerMessages.ErrorThrownMessage, e.Message)
                );

                return result;
            }
        }

        public async Task<bool> HasEntity(int id) => 
            await _context.EmailConfirmations.AnyAsync(ec => ec.Id == id);

        public async Task<bool> HasOutstandingEmailConfirmation(int userId, int appid) => 
            await _context.EmailConfirmations.AnyAsync(ec => ec.UserId == userId && ec.AppId == appid);

        public async Task<IRepositoryResponse> RetrieveEmailConfirmation(int userId, int appid)
        {
            var result = new RepositoryResponse();

            if (userId == 0 || appid == 0)
            {
                result.IsSuccess = false;

                return result;
            }

            try
            {
                var query = await _context
                    .EmailConfirmations
                    .FirstOrDefaultAsync(ec => 
                        ec.UserId == userId && 
                        ec.AppId == appid);

                if (query == null)
                {
                    result.IsSuccess = false;
                }
                else
                {
                    result.IsSuccess = true;
                    result.Object = query;
                }

                return result;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Exception = e;

                _logger.LogError(
                    ReposUtilities.GetRepoErrorEventId(),
                    string.Format(LoggerMessages.ErrorThrownMessage, e.Message)
                );

                return result;
            }
        }
        #endregion
    }
}
