using Microsoft.EntityFrameworkCore;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.ServiceModels;
using SudokuCollective.Core.Interfaces.Models;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models;

namespace SudokuCollective.Repos
{
    public class RolesRepository<TEntity> : IRolesRepository<TEntity> where TEntity : Role
    {
        #region Fields
        private readonly DatabaseContext _context;
        #endregion

        #region Constructor
        public RolesRepository(DatabaseContext context)
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
                result.IsSuccess = false;

                return result;
            }

            try
            {
                if (await _context.Roles.AnyAsync(r => r.RoleLevel == entity.RoleLevel))
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
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Exception = exp;

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
                var query = new Role();

                query = await _context
                    .Roles
                    .FirstOrDefaultAsync(r => r.Id == id);

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
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Exception = exp;

                return result;
            }
        }

        public async Task<IRepositoryResponse> GetAll()
        {
            var result = new RepositoryResponse();

            try
            {
                var query = new List<Role>();

                query = await _context
                    .Roles
                    .Where(r =>
                        r.RoleLevel != RoleLevel.NULL)
                    .OrderBy(r => r.RoleLevel)
                    .ToListAsync();

                if (query.Count == 0)
                {
                    result.IsSuccess = false;
                }
                else
                {
                    result.IsSuccess = true;
                    result.Objects = query
                        .ConvertAll(r => (IDomainEntity)r)
                        .ToList();
                }

                return result;
            }
            catch (Exception exp)
            {
                result.IsSuccess = false;
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
                result.IsSuccess = false;

                return result;
            }

            try
            {
                if (await _context.Roles.AnyAsync(r => r.Id == entity.Id))
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
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Exception = exp;

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

                    if (await _context.Roles.AnyAsync(d => d.Id == entity.Id))
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
            catch (Exception exp)
            {
                result.IsSuccess = false;
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
                result.IsSuccess = false;

                return result;
            }

            try
            {
                if (await _context.Roles.AnyAsync(d => d.Id == entity.Id))
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
            catch (Exception exp)
            {
                result.IsSuccess = false;
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
                var roleIds = new List<int>();

                foreach (var entity in entities)
                {
                    if (entity.Id == 0)
                    {
                        result.IsSuccess = false;

                        return result;
                    }

                    if (await _context.Roles.AnyAsync(d => d.Id == entity.Id))
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
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Exception = exp;

                return result;
            }
        }

        public async Task<bool> HasEntity(int id) => 
            await _context.Roles.AnyAsync(r => r.Id == id);

        public async Task<bool> HasRoleLevel(RoleLevel level) => 
            await _context.Roles.AnyAsync(r => r.RoleLevel == level);

        public async Task<bool> IsListValid(List<int> ids)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));

            var result = true;

            foreach (var id in ids)
            {
                var isIdValid = await _context.Roles.AnyAsync(r => r.Id == id);

                if (!isIdValid)
                {
                    result = false;
                }
            }

            return result;
        }
        #endregion
    }
}
