using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SudokuCollective.Core.Interfaces.ServiceModels;
using SudokuCollective.Core.Interfaces.Models;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Messages;
using SudokuCollective.Repos.Utilities;

namespace SudokuCollective.Repos
{
    public class SolutionsRepository<TEntity> : ISolutionsRepository<TEntity> where TEntity : SudokuSolution
    {
        #region Fields
        private readonly DatabaseContext _context;
        private readonly ILogger<SolutionsRepository<SudokuSolution>> _logger;
        #endregion

        #region Constructor
        public SolutionsRepository(
            DatabaseContext context,
            ILogger<SolutionsRepository<SudokuSolution>> logger)
        {
            _context = context;
            _logger = logger;
        }
        #endregion

        #region Methods
        public async Task<IRepositoryResponse> Add(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var result = new RepositoryResponse();

            if (entity.Id != 0)
            {
                result.IsSuccess = false;

                return result;
            }

            try
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

                result.Object = entity;
                result.IsSuccess = true;

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

        public async Task<IRepositoryResponse> Get(int id)
        {
            var result = new RepositoryResponse();

            if (id == 0)
            {
                result.IsSuccess = false;

                return result;
            }

            try
            {
                var query = await _context
                    .SudokuSolutions
                    .FirstOrDefaultAsync(s => s.Id == id);

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
                    .SudokuSolutions
                    .ToListAsync();

                if (query.Count == 0)
                {
                    result.IsSuccess = false;
                }
                else
                {
                    result.IsSuccess = true;
                    result.Objects = query
                        .ConvertAll(s => (IDomainEntity)s)
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

        public async Task<IRepositoryResponse> AddSolutions(List<ISudokuSolution> solutions)
        {
            if (solutions == null) throw new ArgumentNullException(nameof(solutions));

            var result = new RepositoryResponse();

            try
            {
                _context.AddRange(solutions.ConvertAll(s => (SudokuSolution)s));

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

        public async Task<IRepositoryResponse> GetSolvedSolutions()
        {
            var result = new RepositoryResponse();

            try
            {
                var query = await _context
                    .SudokuSolutions
                    .Where(s => s.DateSolved > DateTime.MinValue)
                    .ToListAsync();

                result.IsSuccess = true;
                result.Objects = query
                    .ConvertAll(s => (IDomainEntity)s)
                    .ToList();

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

            if (entity.Id == 0)
            {
                result.IsSuccess = false;

                return result;
            }

            try
            {
                if (await _context.SudokuSolutions.AnyAsync(r => r.Id == entity.Id))
                {
                    try
                    {
                        _context.Update(entity);
                    }
                    catch
                    {
                        _context.Attach(entity);
                    }

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

        public async Task<IRepositoryResponse> UpdateRange(List<TEntity> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            var result = new RepositoryResponse();

            try
            {
                foreach (var entity in entities)
                {
                    if (entity.Id == 0)
                    {
                        result.IsSuccess = false;

                        return result;
                    }

                    if (await _context.SudokuSolutions.AnyAsync(d => d.Id == entity.Id))
                    {
                        _context.Attach(entity);
                    }
                    else
                    {
                        result.IsSuccess = false;

                        return result;
                    }
                }

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

        public async Task<IRepositoryResponse> Delete(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var result = new RepositoryResponse();

            if (entity.Id == 0)
            {
                result.IsSuccess = false;

                return result;
            }

            try
            {
                if (await _context.SudokuSolutions.AnyAsync(d => d.Id == entity.Id))
                {
                    _context.Remove(entity);

                    foreach (var entry in _context.ChangeTracker.Entries())
                    {
                        var dbEntry = (IDomainEntity)entry.Entity;

                        if (dbEntry is UserApp)
                        {
                            entry.State = EntityState.Modified;
                        }
                        else if (dbEntry is UserRole userRole)
                        {
                            if (userRole.RoleId == entity.Id)
                            {
                                entry.State = EntityState.Deleted;
                            }
                            else
                            {
                                entry.State = EntityState.Modified;
                            }
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

        public async Task<IRepositoryResponse> DeleteRange(List<TEntity> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            var result = new RepositoryResponse();

            try
            {
                var roleIds = new List<int>();

                foreach (var entity in entities)
                {
                    if (entity.Id == 0)
                    {
                        result.IsSuccess = false;

                        return result;
                    }

                    if (await _context.SudokuSolutions.AnyAsync(d => d.Id == entity.Id))
                    {
                        _context.Remove(entity);
                        roleIds.Add(entity.Id);
                    }
                    else
                    {
                        result.IsSuccess = false;

                        return result;
                    }
                }

                foreach (var entry in _context.ChangeTracker.Entries())
                {
                    var dbEntry = (IDomainEntity)entry.Entity;

                    if (dbEntry is UserApp)
                    {
                        entry.State = EntityState.Modified;
                    }
                    else if (dbEntry is UserRole userRole)
                    {
                        if (roleIds.Contains(userRole.RoleId))
                        {
                            entry.State = EntityState.Deleted;
                        }
                        else
                        {
                            entry.State = EntityState.Modified;
                        }
                    }
                    else
                    {
                        // Otherwise do nothing...
                    }
                }

                await _context.SaveChangesAsync();

                result.IsSuccess = true;

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

        public async Task<bool> HasEntity(int id) => 
            await _context.SudokuSolutions.AnyAsync(d => d.Id == id);
        #endregion
    }
}
