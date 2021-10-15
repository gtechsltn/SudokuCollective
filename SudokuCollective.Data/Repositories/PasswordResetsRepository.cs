using System;
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
    public class PasswordResetsRepository<TEntity> : IPasswordResetsRepository<TEntity> where TEntity : PasswordReset
    {
        #region Fields
        private readonly DatabaseContext _context;
        #endregion

        #region Constructor
        public PasswordResetsRepository(DatabaseContext context)
        {
            _context = context;
        }
        #endregion

        #region Methods
        public async Task<IRepositoryResponse> Create(TEntity entity)
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
                if (await _context.PasswordResets
                        .AnyAsync(pu => pu.Token.ToLower().Equals(entity.Token.ToLower())))
                {
                    result.Success = false;

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

        public async Task<IRepositoryResponse> Get(string token)
        {
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token));

            var result = new RepositoryResponse();

            try
            {
                var query = await _context
                    .PasswordResets
                    .FirstOrDefaultAsync(pu => pu.Token.ToLower().Equals(token.ToLower()));

                if (query == null)
                {
                    result.Success = false;
                }
                else
                {
                    result.Success = true;
                    result.Object = query;
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

            try
            {
                var query = await _context
                    .PasswordResets
                    .OrderBy(ec => ec.Id)
                    .ToListAsync();

                if (query.Count == 0)
                {
                    result.Success = false;
                }
                else
                {
                    result.Success = true;
                    result.Objects = query
                        .ConvertAll(pu => (IDomainEntity)pu)
                        .ToList();
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
                if (await _context.PasswordResets.AnyAsync(a => a.Id == entity.Id))
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
                if (await _context.PasswordResets.AnyAsync(pu => pu.Id == entity.Id))
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

        public async Task<bool> HasEntity(int id) => 
            await _context.PasswordResets.AnyAsync(ec => ec.Id == id);

        public async Task<bool> HasOutstandingPasswordReset(int userId, int appid) => 
            await _context
                .PasswordResets
                .AnyAsync(pw => pw.UserId == userId && pw.AppId == appid);

        public async Task<IRepositoryResponse> RetrievePasswordReset(int userId, int appid)
        {
            var result = new RepositoryResponse();

            if (userId == 0 || appid == 0)
            {
                result.Success = false;

                return result;
            }

            try
            {
                var query = await _context
                    .PasswordResets
                    .FirstOrDefaultAsync(pw =>
                        pw.UserId == userId &&
                        pw.AppId == appid);

                if (query == null)
                {
                    result.Success = false;
                }
                else
                {
                    result.Success = true;
                    result.Object = query;
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
        #endregion
    }
}
