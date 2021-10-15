using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SudokuCollective.Core.Interfaces.DataModels;
using SudokuCollective.Core.Interfaces.Models;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models;

namespace SudokuCollective.Data.Repositories
{
    public class GamesRepository<TEntity> : IGamesRepository<TEntity> where TEntity : Game
    {
        #region Fields
        private readonly DatabaseContext _context;
        #endregion

        #region Constructor
        public GamesRepository(DatabaseContext context)
        {
            _context = context;
        }
        #endregion

        #region Methods    
        public async Task<IRepositoryResponse> Add(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var result = new RepositoryResponse();

            if (entity.Id != 0)
            {
                result.Success = false;

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

                result.Success = true;
                result.Object = entity;

                return result;
            }
            catch (Exception exp)
            {
                result.Success = false;
                result.Exception = exp;

                return result;
            }
        }

        public async Task<IRepositoryResponse> Get(int id)
        {
            var result = new RepositoryResponse();

            if (id == 0)
            {
                result.Success = false;

                return result;
            }

            try
            {
                var query = new Game();

                query = await _context
                    .Games
                    .Include(g => g.SudokuMatrix)
                        .ThenInclude(g => g.Difficulty)
                    .Include(g => g.SudokuMatrix)
                        .ThenInclude(g => g.SudokuCells)
                    .Include(g => g.SudokuSolution)
                    .FirstOrDefaultAsync(g => g.Id == id);

                if (query != null)
                {
                    query.SudokuMatrix.SudokuCells = query
                        .SudokuMatrix
                        .SudokuCells
                        .OrderBy(c => c.Index)
                        .ToList();

                    result.Success = true;
                    result.Object = query;
                }
                else
                {
                    result.Success = false;
                }

                return result;
            }
            catch (Exception exp)
            {
                result.Success = false;
                result.Exception = exp;

                return result;
            }
        }

        public async Task<IRepositoryResponse> GetAll()
        {
            var result = new RepositoryResponse();
            var query = new List<Game>();

            try
            {
                query = await _context
                    .Games
                    .Include(g => g.SudokuMatrix)
                        .ThenInclude(g => g.Difficulty)
                    .Include(g => g.SudokuMatrix)
                        .ThenInclude(g => g.SudokuCells)
                    .Include(g => g.SudokuSolution)
                    .ToListAsync();

                if (query.Count > 0)
                {
                    foreach (var game in query)
                    {
                        game.SudokuMatrix.SudokuCells = game
                            .SudokuMatrix
                            .SudokuCells
                            .OrderBy(c => c.Index)
                            .ToList();
                    }

                    result.Success = true;
                    result.Objects = query
                        .ConvertAll(g => (IDomainEntity)g)
                        .ToList();
                }
                else
                {
                    result.Success = false;
                }

                return result;
            }
            catch (Exception exp)
            {
                result.Success = false;
                result.Exception = exp;

                return result;
            }
        }
        
        public async Task<IRepositoryResponse> GetAppGame(int id, int appid)
        {
            var result = new RepositoryResponse();

            if (id == 0 || appid == 0)
            {
                result.Success = false;

                return result;
            }

            try
            {
                var query = new Game();

                query = await _context
                    .Games
                    .Include(g => g.SudokuMatrix)
                        .ThenInclude(g => g.Difficulty)
                    .Include(g => g.SudokuMatrix)
                        .ThenInclude(g => g.SudokuCells)
                    .Include(g => g.SudokuSolution)
                    .FirstOrDefaultAsync(g => g.Id == id && g.AppId == appid);

                if (query != null)
                {
                    query.SudokuMatrix.SudokuCells = query
                        .SudokuMatrix
                        .SudokuCells
                        .Where(c => c.Id != 0)
                        .OrderBy(c => c.Index)
                        .ToList();

                    result.Success = true;
                    result.Object = query;
                }
                else
                {
                    result.Success = false;
                }

                return result;
            }
            catch (Exception exp)
            {
                result.Success = false;
                result.Exception = exp;

                return result;
            }
        }

        public async Task<IRepositoryResponse> GetAppGames(int appid)
        {
            var result = new RepositoryResponse();

            if (appid == 0)
            {
                result.Success = false;

                return result;
            }

            try
            {
                var query = new List<Game>();

                query = await _context
                    .Games
                    .Include(g => g.SudokuMatrix)
                        .ThenInclude(g => g.Difficulty)
                    .Include(g => g.SudokuMatrix)
                        .ThenInclude(g => g.SudokuCells)
                    .Include(g => g.SudokuSolution)
                    .Where(g => g.AppId == appid)
                    .ToListAsync();

                if (query.Count > 0)
                {
                    foreach (var game in query)
                    {
                        game.SudokuMatrix.SudokuCells = game
                            .SudokuMatrix
                            .SudokuCells
                            .Where(c => c.Id != 0)
                            .OrderBy(c => c.Index)
                            .ToList();
                    }

                    result.Success = true;
                    result.Objects = query.ConvertAll(g => (IDomainEntity)g);
                }
                else
                {
                    result.Success = false;
                }

                return result;
            }
            catch (Exception exp)
            {
                result.Success = false;
                result.Exception = exp;

                return result;
            }
        }

        public async Task<IRepositoryResponse> GetMyGame(int userid, int gameid, int appid)
        {
            var result = new RepositoryResponse();

            if (userid == 0 || gameid == 0 || appid == 0)
            {
                result.Success = false;

                return result;
            }

            try
            {
                var query = new Game();

                query = await _context
                    .Games
                    .Include(g => g.SudokuMatrix)
                        .ThenInclude(g => g.Difficulty)
                    .Include(g => g.SudokuMatrix)
                        .ThenInclude(g => g.SudokuCells)
                    .Include(g => g.SudokuSolution)
                    .Where(g => g.AppId == appid)
                    .FirstOrDefaultAsync(predicate:
                        g => g.Id == gameid
                        && g.AppId == appid
                        && g.UserId == userid);

                if (query != null)
                {
                    query.SudokuMatrix.SudokuCells = query
                        .SudokuMatrix
                        .SudokuCells
                        .Where(c => c.Id != 0)
                        .OrderBy(c => c.Index)
                        .ToList();

                    result.Success = true;
                    result.Object = query;
                }
                else
                {
                    result.Success = false;
                }

                return result;
            }
            catch (Exception exp)
            {
                result.Success = false;
                result.Exception = exp;

                return result;
            }
        }

        public async Task<IRepositoryResponse> GetMyGames(int userid, int appid)
        {
            var result = new RepositoryResponse();

            if (userid == 0 || appid == 0)
            {
                result.Success = false;

                return result;
            }

            try
            {
                if (await _context.Users.AnyAsync(u => u.Id == userid) && await _context.Apps.AnyAsync(a => a.Id == appid))
                {
                    var query = new List<Game>();

                    query = await _context
                        .Games
                        .Include(g => g.SudokuMatrix)
                            .ThenInclude(g => g.Difficulty)
                        .Include(g => g.SudokuMatrix)
                            .ThenInclude(g => g.SudokuCells)
                        .Include(g => g.SudokuSolution)
                        .Where(g => g.AppId == appid && g.UserId == userid)
                        .ToListAsync();

                    if (query.Count > 0)
                    {
                        foreach (var game in query)
                        {
                            game.SudokuMatrix.SudokuCells = game
                                .SudokuMatrix
                                .SudokuCells
                                .Where(c => c.Id != 0)
                                .OrderBy(c => c.Index)
                                .ToList();
                        }

                        result.Success = true;
                        result.Objects = query.ConvertAll(g => (IDomainEntity)g);
                    }
                    else
                    {
                        result.Success = false;
                    }

                    return result;
                }
                else
                {
                    result.Success = false;

                    return result;
                }
            }
            catch (Exception exp)
            {
                result.Success = false;
                result.Exception = exp;

                return result;
            }
        }

        public async Task<IRepositoryResponse> Update(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var result = new RepositoryResponse();

            if (entity.Id == 0)
            {
                result.Success = false;

                return result;
            }

            try
            {
                if (await _context.Games.AnyAsync(g => g.Id == entity.Id))
                {
                    entity.DateUpdated = DateTime.UtcNow;

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

                    result.Success = true;
                    result.Object = entity;

                    return result;
                }
                else
                {
                    result.Success = false;

                    return result;
                }
            }
            catch (Exception exp)
            {
                result.Success = false;
                result.Exception = exp;

                return result;
            }
        }

        public async Task<IRepositoryResponse> UpdateRange(List<TEntity> entities)
        {
            var result = new RepositoryResponse();

            try
            {
                var dateUpdated = DateTime.UtcNow;

                foreach (var entity in entities)
                {
                    if (entity.Id == 0)
                    {
                        result.Success = false;

                        return result;
                    }

                    if (await _context.Games.AnyAsync(g => g.Id == entity.Id))
                    {
                        entity.DateUpdated = dateUpdated;

                        _context.Attach(entity);
                    }
                    else
                    {
                        result.Success = false;

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

                result.Success = true;

                return result;
            }
            catch (Exception exp)
            {
                result.Success = false;
                result.Exception = exp;

                return result;
            }
        }

        public async Task<IRepositoryResponse> Delete(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var result = new RepositoryResponse();

            if (entity.Id == 0)
            {
                result.Success = false;

                return result;
            }

            try
            {
                if (await _context.Games.AnyAsync(g => g.Id == entity.Id))
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

                    result.Success = true;

                    return result;
                }
                else
                {
                    result.Success = false;

                    return result;
                }
            }
            catch (Exception exp)
            {
                result.Success = false;
                result.Exception = exp;

                return result;
            }
        }

        public async Task<IRepositoryResponse> DeleteRange(List<TEntity> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            var result = new RepositoryResponse();

            try
            {
                foreach (var entity in entities)
                {
                    if (entity.Id == 0)
                    {
                        result.Success = false;

                        return result;
                    }

                    if (await _context.Games.AnyAsync(g => g.Id == entity.Id))
                    {
                        _context.Remove(entity);
                    }
                    else
                    {
                        result.Success = false;

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

                result.Success = true;

                return result;
            }
            catch (Exception exp)
            {
                result.Success = false;
                result.Exception = exp;

                return result;
            }
        }

        public async Task<IRepositoryResponse> DeleteMyGame(int userid, int gameid, int appid)
        {
            var result = new RepositoryResponse();

            if (userid == 0 || gameid == 0 || appid == 0)
            {
                result.Success = false;

                return result;
            }

            try
            {
                if (await _context.Users.AnyAsync(u => u.Id == userid) && 
                    await _context.Games.AnyAsync(g => g.Id == gameid) &&
                    await _context.Apps.AnyAsync(a => a.Id == appid))
                {
                    var query = new Game();

                    query = await _context
                        .Games
                        .Include(g => g.SudokuMatrix)
                            .ThenInclude(g => g.Difficulty)
                        .Include(g => g.SudokuMatrix)
                            .ThenInclude(g => g.SudokuCells)
                        .FirstOrDefaultAsync(predicate:
                            g => g.Id == gameid
                            && g.AppId == appid
                            && g.UserId == userid);

                    _context.Remove(query);

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

                    result.Success = true;

                    return result;
                }
                else
                {
                    result.Success = false;

                    return result;
                }
            }
            catch (Exception exp)
            {
                result.Success = false;
                result.Exception = exp;

                return result;
            }
        }

        public async Task<bool> HasEntity(int id) => await _context.Games.AnyAsync(g => g.Id == id);
        #endregion
    }
}