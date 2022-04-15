using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.ServiceModels;
using SudokuCollective.Core.Interfaces.Models;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models;
using SudokuCollective.Repos.Utilities;

namespace SudokuCollective.Repos
{
    public class AppsRepository<TEntity> : IAppsRepository<TEntity> where TEntity : App
    {
        #region Fields
        private readonly DatabaseContext _context;
        private readonly IRequestService _requestService;
        private readonly ILogger<AppsRepository<App>> _logger;
        #endregion

        #region Constructor
        public AppsRepository(
            DatabaseContext context,
            IRequestService requestService,
            ILogger<AppsRepository<App>> logger)
        {
            _context = context;
            _requestService = requestService;
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

                var user = await _context
                    .Users
                    .FirstOrDefaultAsync(u => u.Id == entity.OwnerId);

                // Add connection between the app and the user
                var userApp = new UserApp
                {
                    User = user,
                    UserId = user.Id,
                    App = entity,
                    AppId = entity.Id
                };

                entity.Users.Add(userApp);

                _context.Attach(userApp);

                foreach (var entry in _context.ChangeTracker.Entries())
                {
                    if (entry.Entity is App app)
                    {
                        if (app.Id == entity.Id)
                        {
                            entry.State = EntityState.Added;
                        }
                        else
                        {
                            entry.State = EntityState.Modified;
                        }
                    }
                    else if (entry.Entity is UserApp ua)
                    {
                        if (ua.Id == userApp.Id)
                        {
                            entry.State = EntityState.Added;
                        }
                        else
                        {
                            entry.State = EntityState.Modified;
                        }
                    }
                    else
                    {
                        var dbEntry = (IDomainEntity)entry.Entity;

                        if (dbEntry.Id == 0)
                        {
                            entry.State = EntityState.Added;
                        }
                        else
                        {
                            entry.State = EntityState.Modified;
                        }
                    }
                }

                await _context.SaveChangesAsync();

                // Ensure that the owner has admin priviledges, if not they will be promoted
                var addAdminRole = true;
                var newUserAdminRole = new UserRole();

                foreach (var userRole in user.Roles)
                {
                    userRole.Role = await _context
                        .Roles
                        .FirstOrDefaultAsync(roleDbSet => roleDbSet.Id == userRole.RoleId);

                    if (userRole.Role.RoleLevel == RoleLevel.ADMIN)
                    {
                        addAdminRole = false;
                    }
                }

                // Promote user to admin if user is not already
                if (addAdminRole)
                {
                    var adminRole = await _context
                        .Roles
                        .FirstOrDefaultAsync(r => r.RoleLevel == RoleLevel.ADMIN);

                    newUserAdminRole = new UserRole
                    {
                        User = user,
                        UserId = user.Id,
                        Role = adminRole,
                        RoleId = adminRole.Id
                    };

                    var appAdmin = new AppAdmin
                    {
                        AppId = entity.Id,
                        UserId = user.Id
                    };

                    _context.Attach(newUserAdminRole);

                    _context.Attach(appAdmin);

                    foreach (var entry in _context.ChangeTracker.Entries())
                    {
                        if (entry.Entity is UserApp ua)
                        {
                            if (ua.Id == newUserAdminRole.Id)
                            {
                                entry.State = EntityState.Added;
                            }
                            else
                            {
                                entry.State = EntityState.Modified;
                            }
                        }
                        else
                        {
                            var dbEntry = (IDomainEntity)entry.Entity;

                            if (dbEntry.Id == 0)
                            {
                                entry.State = EntityState.Added;
                            }
                            else
                            {
                                entry.State = EntityState.Modified;
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                }

                result.Object = entity;
                result.IsSuccess = true;

                return result;
            }
            catch (Exception e)
            {
                return ReposUtilities.ProcessException<AppsRepository<App>>(
                    _requestService, 
                    _logger, 
                    result, 
                    e);
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
                var query = new App();

                query = await _context
                    .Apps
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (query != null)
                {
                    query.Users = await _context.UsersApps
                        .Include(ua => ua.User)
                            .ThenInclude(u => u.Roles)
                        .Where(ua => ua.AppId == query.Id)
                        .ToListAsync();

                    foreach (var userApp in query.Users)
                    {
                        foreach (var userRole in userApp.User.Roles)
                        {
                            userRole.Role = await _context
                                .Roles
                                .FirstOrDefaultAsync(r => r.Id == userRole.RoleId);
                        }
                    }

                    // Filter games by app
                    foreach (var userApp in query.Users)
                    {
                        userApp.User.Games = new List<Game>();

                        userApp.User.Games = await _context
                            .Games
                            .Include(g => g.SudokuMatrix)
                                .ThenInclude(g => g.Difficulty)
                            .Include(g => g.SudokuMatrix)
                                .ThenInclude(m => m.SudokuCells)
                            .Include(g => g.SudokuSolution)
                            .Where(g => g.AppId == userApp.AppId && g.UserId == userApp.UserId)
                            .ToListAsync();
                    }

                    result.IsSuccess = true;
                    result.Object = query;
                }
                else
                {
                    result.IsSuccess = false;
                }

                return result;
            }
            catch (Exception e)
            {
                return ReposUtilities.ProcessException<AppsRepository<App>>(
                    _requestService, 
                    _logger, 
                    result, 
                    e);
            }
        }

        public async Task<IRepositoryResponse> GetByLicense(string license)
        {
            var result = new RepositoryResponse();

            if (string.IsNullOrEmpty(license))
            {
                result.IsSuccess = false;

                return result;
            }

            try
            {
                var query = new App();

                query = await _context
                    .Apps
                    .FirstOrDefaultAsync(
                        a => a.License.ToLower().Equals(license.ToLower()));

                if (query != null)
                {
                    query.Users = await _context.UsersApps
                        .Include(ua => ua.User)
                            .ThenInclude(u => u.Roles)
                        .Where(ua => ua.AppId == query.Id)
                        .ToListAsync();

                    foreach (var userApp in query.Users)
                    {
                        foreach (var userRole in userApp.User.Roles)
                        {
                            userRole.Role = await _context
                                .Roles
                                .FirstOrDefaultAsync(r => r.Id == userRole.RoleId);
                        }
                    }

                    // Filter games by app
                    foreach (var userApp in query.Users)
                    {
                        userApp.User.Games = new List<Game>();

                        userApp.User.Games = await _context
                            .Games
                            .Include(g => g.SudokuMatrix)
                                .ThenInclude(g => g.Difficulty)
                            .Include(g => g.SudokuMatrix)
                                .ThenInclude(m => m.SudokuCells)
                            .Include(g => g.SudokuSolution)
                            .Where(g => g.AppId == userApp.AppId && g.UserId == userApp.UserId)
                            .ToListAsync();
                    }

                    result.IsSuccess = true;
                    result.Object = query;
                }
                else
                {
                    result.IsSuccess = false;
                }

                return result;
            }
            catch (Exception e)
            {
                return ReposUtilities.ProcessException<AppsRepository<App>>(
                    _requestService, 
                    _logger, 
                    result, 
                    e);
            }
        }

        public async Task<IRepositoryResponse> GetAll()
        {
            var result = new RepositoryResponse();

            try
            {
                var query = new List<App>();

                query = await _context
                    .Apps
                    .Include(a => a.Users)
                        .ThenInclude(ua => ua.User)
                            .ThenInclude(u => u.Roles)
                                .ThenInclude(ur => ur.Role)
                    .OrderBy(a => a.Id)
                    .ToListAsync();

                if (query.Count != 0)
                {
                    // Filter games by app
                    foreach (var app in query)
                    {
                        foreach (var userApp in app.Users)
                        {
                            userApp.User.Games = new List<Game>();

                            userApp.User.Games = await _context
                                .Games
                                .Include(g => g.SudokuMatrix)
                                    .ThenInclude(g => g.Difficulty)
                                .Include(g => g.SudokuMatrix)
                                    .ThenInclude(m => m.SudokuCells)
                                .Include(g => g.SudokuSolution)
                                .Where(g => g.AppId == userApp.AppId && g.UserId == userApp.UserId)
                                .ToListAsync();
                        }
                    }

                    result.IsSuccess = true;
                    result.Objects = query
                        .ConvertAll(a => (IDomainEntity)a)
                        .ToList();
                }
                else
                {
                    result.IsSuccess = false;
                }

                return result;
            }
            catch (Exception e)
            {
                return ReposUtilities.ProcessException<AppsRepository<App>>(
                    _requestService, 
                    _logger, 
                    result, 
                    e);
            }
        }

        public async Task<IRepositoryResponse> GetMyApps(int ownerId)
        {
            var result = new RepositoryResponse();

            if (ownerId == 0)
            {
                result.IsSuccess = false;

                return result;
            }

            try
            {
                var query = new List<App>();

                query = await _context
                    .Apps
                    .Where(a => a.OwnerId == ownerId)
                    .Include(a => a.Users)
                        .ThenInclude(ua => ua.User)
                            .ThenInclude(u => u.Roles)
                                .ThenInclude(ur => ur.Role)
                    .OrderBy(a => a.Id)
                    .ToListAsync();

                if (query.Count != 0)
                {
                    // Filter games by app
                    foreach (var app in query)
                    {
                        foreach (var userApp in app.Users)
                        {
                            userApp.User.Games = new List<Game>();

                            userApp.User.Games = await _context
                                .Games
                                .Include(g => g.SudokuMatrix)
                                    .ThenInclude(g => g.Difficulty)
                                .Include(g => g.SudokuMatrix)
                                    .ThenInclude(m => m.SudokuCells)
                                .Include(g => g.SudokuSolution)
                                .Where(g => g.AppId == userApp.AppId && g.UserId == userApp.UserId)
                                .ToListAsync();
                        }
                    }

                    result.IsSuccess = true;
                    result.Objects = query
                        .ConvertAll(a => (IDomainEntity)a)
                        .ToList();
                }
                else
                {
                    result.IsSuccess = false;
                }

                return result;
            }
            catch (Exception e)
            {
                return ReposUtilities.ProcessException<AppsRepository<App>>(
                    _requestService, 
                    _logger, 
                    result, 
                    e);
            }
        }

        public async Task<IRepositoryResponse> GetMyRegisteredApps(int userId)
        {
            var result = new RepositoryResponse();

            if (userId == 0)
            {
                result.IsSuccess = false;

                return result;
            }

            try
            {
                var query = new List<App>();

                query = await _context.Users
                    .Where(u => u.Id == userId)
                    .SelectMany(u => u.Apps.Where(ua => ua.App.OwnerId != userId))
                    .Select(ua => ua.App)
                    .ToListAsync();

                if (query.Count != 0)
                {
                    // Filter games by app
                    foreach (var app in query)
                    {
                        foreach (var userApp in app.Users)
                        {
                            userApp.User.Games = new List<Game>();

                            userApp.User.Games = await _context
                                .Games
                                .Include(g => g.SudokuMatrix)
                                    .ThenInclude(g => g.Difficulty)
                                .Include(g => g.SudokuMatrix)
                                    .ThenInclude(m => m.SudokuCells)
                                .Include(g => g.SudokuSolution)
                                .Where(g => g.AppId == userApp.AppId && g.UserId == userApp.UserId)
                                .ToListAsync();
                        }
                    }

                    result.IsSuccess = true;
                    result.Objects = query
                        .ConvertAll(a => (IDomainEntity)a)
                        .ToList();
                }
                else
                {
                    result.IsSuccess = false;
                }

                return result;
            }
            catch (Exception e)
            {
                return ReposUtilities.ProcessException<AppsRepository<App>>(
                    _requestService, 
                    _logger, 
                    result, 
                    e);
            }
        }

        public async Task<IRepositoryResponse> GetAppUsers(int id)
        {
            var result = new RepositoryResponse();

            if (id == 0)
            {
                result.IsSuccess = false;

                return result;
            }

            try
            {
                var query = new List<User>();

                query = await _context
                    .Users
                    .Include(u => u.Apps)
                        .ThenInclude(ua => ua.App)
                    .Include(u => u.Roles)
                        .ThenInclude(ur => ur.Role)
                    .Include(u => u.Games)
                        .ThenInclude(g => g.SudokuSolution)
                    .Include(u => u.Games)
                        .ThenInclude(g => g.SudokuMatrix)
                            .ThenInclude(m => m.SudokuCells)
                    .Where(u => u.Apps.Any(ua => ua.AppId == id))
                    .OrderBy(u => u.Id)
                    .ToListAsync();

                if (query.Count != 0)
                {
                    foreach (var user in query)
                    {
                        // Filter games by app
                        user.Games = new List<Game>();

                        user.Games = await _context
                            .Games
                            .Where(g => g.AppId == id && g.UserId == user.Id)
                            .ToListAsync();

                        // Filter roles by app
                        var appAdmins = await _context
                            .AppAdmins
                            .Where(aa => aa.AppId == id && aa.UserId == user.Id)
                            .ToListAsync();

                        var filteredRoles = new List<UserRole>();

                        foreach (var ur in user.Roles)
                        {
                            if (ur.Role.RoleLevel != RoleLevel.ADMIN)
                            {
                                filteredRoles.Add(ur);
                            }
                            else
                            {
                                if (appAdmins.Any(aa => aa.AppId == id && aa.UserId == user.Id && aa.IsActive))
                                {
                                    filteredRoles.Add(ur);
                                }
                            }
                        }

                        user.Roles = filteredRoles;
                    }

                    result.IsSuccess = true;
                    result.Objects = query
                        .ConvertAll(u => (IDomainEntity)u)
                        .ToList();
                }
                else
                {
                    result.IsSuccess = false;
                }

                return result;
            }
            catch (Exception e)
            {
                return ReposUtilities.ProcessException<AppsRepository<App>>(
                    _requestService, 
                    _logger, 
                    result, 
                    e);
            }
        }

        public async Task<IRepositoryResponse> GetNonAppUsers(int id)
        {
            var result = new RepositoryResponse();

            if (id == 0 || !await HasEntity(id))
            {
                result.IsSuccess = false;

                return result;
            }

            try
            {
                var query = new List<User>();

                query = await _context
                    .Users
                    .Include(u => u.Apps)
                        .ThenInclude(ua => ua.App)
                    .Include(u => u.Roles)
                        .ThenInclude(ur => ur.Role)
                    .Include(u => u.Games)
                        .ThenInclude(g => g.SudokuSolution)
                    .Include(u => u.Games)
                        .ThenInclude(g => g.SudokuMatrix)
                            .ThenInclude(m => m.SudokuCells)
                    .Where(u => !u.Apps.Any(ua => ua.AppId == id))
                    .OrderBy(u => u.Id)
                    .ToListAsync();

                if (query.Count != 0)
                {
                    foreach (var user in query)
                    {
                        // Filter games by app
                        user.Games = new List<Game>();

                        user.Games = await _context
                            .Games
                            .Where(g => g.AppId == id && g.UserId != user.Id)
                            .ToListAsync();
                    }

                    result.IsSuccess = true;
                    result.Objects = query
                        .ConvertAll(u => (IDomainEntity)u)
                        .ToList();
                }
                else
                {
                    result.IsSuccess = false;
                }

                return result;
            }
            catch (Exception e)
            {
                return ReposUtilities.ProcessException<AppsRepository<App>>(
                    _requestService, 
                    _logger, 
                    result, 
                    e);
            }
        }

        public async Task<IRepositoryResponse> Update(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var result = new RepositoryResponse();

            try
            {
                if (entity.Id == 0)
                {
                    result.IsSuccess = false;

                    return result;
                }

                if (await _context.Apps.AnyAsync(a => a.Id == entity.Id))
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
                return ReposUtilities.ProcessException<AppsRepository<App>>(
                    _requestService, 
                    _logger, 
                    result, 
                    e);
            }
        }

        public async Task<IRepositoryResponse> UpdateRange(List<TEntity> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            var result = new RepositoryResponse();

            try
            {
                var dateUpdated = DateTime.UtcNow;

                foreach (var entity in entities)
                {
                    if (entity.Id == 0)
                    {
                        result.IsSuccess = false;

                        return result;
                    }

                    if (await _context.Apps.AnyAsync(a => a.Id == entity.Id))
                    {
                        entity.DateUpdated = dateUpdated;
                    }
                    else
                    {
                        result.IsSuccess = false;

                        return result;
                    }
                }

                _context.Apps.UpdateRange(entities);

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
                return ReposUtilities.ProcessException<AppsRepository<App>>(
                    _requestService, 
                    _logger, 
                    result, 
                    e);
            }
        }

        public async Task<IRepositoryResponse> Delete(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var result = new RepositoryResponse();

            try
            {
                if (await _context.Apps.AnyAsync(a => a.Id == entity.Id))
                {
                    var games = await _context
                        .Games
                        .Include(g => g.SudokuMatrix)
                            .ThenInclude(g => g.Difficulty)
                        .Include(g => g.SudokuMatrix)
                            .ThenInclude(m => m.SudokuCells)
                        .Where(g => g.AppId == entity.Id)
                        .ToListAsync();

                    _context.RemoveRange(games);

                    _context.Remove(entity);

                    foreach (var entry in _context.ChangeTracker.Entries())
                    {
                        var dbEntry = (IDomainEntity)entry.Entity;

                        if (dbEntry is UserApp userApp)
                        {
                            if (userApp.AppId == entity.Id)
                            {
                                entry.State = EntityState.Deleted;
                            }
                            else
                            {
                                entry.State = EntityState.Modified;
                            }
                        }
                        else if (dbEntry is UserRole)
                        {
                            entry.State = EntityState.Modified;
                        }
                        else if (dbEntry is SudokuSolution)
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
                else
                {
                    result.IsSuccess = false;

                    return result;
                }
            }
            catch (Exception e)
            {
                return ReposUtilities.ProcessException<AppsRepository<App>>(
                    _requestService, 
                    _logger, 
                    result, 
                    e);
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
                        result.IsSuccess = false;

                        return result;
                    }

                    if (await _context.Apps.AnyAsync(a => a.Id == entity.Id))
                    {
                        _context.Remove(entity);

                        var games = await _context
                            .Games
                            .Include(g => g.SudokuMatrix)
                                .ThenInclude(g => g.Difficulty)
                            .Include(g => g.SudokuMatrix)
                                .ThenInclude(m => m.SudokuCells)
                            .Where(g => g.AppId == entity.Id)
                            .ToListAsync();

                        _context.RemoveRange(games);
                    }
                    else
                    {
                        result.IsSuccess = false;

                        return result;
                    }
                }

                foreach (var entity in entities)
                {
                    foreach (var entry in _context.ChangeTracker.Entries())
                    {
                        var dbEntry = (IDomainEntity)entry.Entity;

                        if (dbEntry is UserApp userApp)
                        {
                            if (userApp.AppId == entity.Id)
                            {
                                entry.State = EntityState.Deleted;
                            }
                            else
                            {
                                entry.State = EntityState.Modified;
                            }
                        }
                        else if (dbEntry is UserRole)
                        {
                            entry.State = EntityState.Modified;
                        }
                        else if (dbEntry is SudokuSolution)
                        {
                            entry.State = EntityState.Modified;
                        }
                        else
                        {
                            // Otherwise do nothing...
                        }
                    }

                    await _context.SaveChangesAsync();
                }

                result.IsSuccess = true;

                return result;
            }
            catch (Exception e)
            {
                return ReposUtilities.ProcessException<AppsRepository<App>>(
                    _requestService, 
                    _logger, 
                    result, 
                    e);
            }
        }

        public async Task<IRepositoryResponse> Reset(TEntity entity)
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
                var games = await _context
                    .Games
                    .Include(g => g.SudokuMatrix)
                        .ThenInclude(g => g.Difficulty)
                    .Include(g => g.SudokuMatrix)
                        .ThenInclude(m => m.SudokuCells)
                    .Where(g => g.AppId == entity.Id)
                    .ToListAsync();

                if (games.Count > 0)
                {
                    _context.RemoveRange(games);

                    foreach (var entry in _context.ChangeTracker.Entries())
                    {
                        var dbEntry = (IDomainEntity)entry.Entity;

                        if (dbEntry is UserApp userApp)
                        {
                            entry.State = EntityState.Modified;
                        }
                        else if (dbEntry is UserRole)
                        {
                            entry.State = EntityState.Modified;
                        }
                        else if (dbEntry is SudokuSolution)
                        {
                            entry.State = EntityState.Modified;
                        }
                        else
                        {
                            // Otherwise do nothing...
                        }
                    }

                    await _context.SaveChangesAsync();
                }

                result.IsSuccess = true;
                result.Object = await _context
                    .Apps
                    .FirstOrDefaultAsync(a => a.Id == entity.Id);

                return result;
            }
            catch (Exception e)
            {
                return ReposUtilities.ProcessException<AppsRepository<App>>(
                    _requestService, 
                    _logger, 
                    result, 
                    e);
            }
        }

        public async Task<IRepositoryResponse> AddAppUser(int userId, string license)
        {
            var result = new RepositoryResponse();

            if (userId == 0 || string.IsNullOrEmpty(license))
            {
                result.IsSuccess = false;

                return result;
            }

            try
            {
                var user = await _context
                    .Users
                    .FirstOrDefaultAsync(u => u.Id == userId);

                var app = await _context
                    .Apps
                    .FirstOrDefaultAsync(
                        a => a.License.ToLower().Equals(license.ToLower()));

                if (user == null || app == null)
                {
                    result.IsSuccess = false;

                    return result;
                }

                var userApp = new UserApp
                {
                    User = user,
                    UserId = user.Id,
                    App = app,
                    AppId = app.Id
                };

                _context.Attach(userApp);

                foreach (var entry in _context.ChangeTracker.Entries())
                {
                    var dbEntry = (IDomainEntity)entry.Entity;

                    if (dbEntry is UserApp ua)
                    {
                        if (ua.Id == userApp.Id)
                        {
                            entry.State = EntityState.Added;
                        }
                        else
                        {
                            entry.State = EntityState.Modified;
                        }
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
                return ReposUtilities.ProcessException<AppsRepository<App>>(
                    _requestService, 
                    _logger, 
                    result, 
                    e);
            }
        }

        public async Task<IRepositoryResponse> RemoveAppUser(int userId, string license)
        {
            var result = new RepositoryResponse();

            if (userId == 0 || string.IsNullOrEmpty(license))
            {
                result.IsSuccess = false;

                return result;
            }

            try
            {
                var app = await _context
                    .Apps
                    .FirstOrDefaultAsync(
                        a => a.License.ToLower().Equals(license.ToLower()));

                var user = await _context
                    .Users
                    .Include(u => u.Apps)
                    .FirstOrDefaultAsync(
                        u => u.Id == userId && 
                        u.Apps.Any(ua => ua.AppId == app.Id));

                if (user == null || app == null)
                {
                    result.IsSuccess = false;

                    return result;
                }

                if (app.OwnerId == user.Id)
                {
                    result.IsSuccess = false;

                    return result;
                }

                user.Games = new List<Game>();

                user.Games = await _context
                    .Games
                    .Include(g => g.SudokuMatrix)
                        .ThenInclude(g => g.Difficulty)
                    .Include(g => g.SudokuMatrix)
                        .ThenInclude(m => m.SudokuCells)
                    .Where(g => g.AppId == app.Id)
                    .ToListAsync();

                foreach (var game in user.Games)
                {
                    if (game.AppId == app.Id)
                    {
                        _context.Remove(game);
                    }
                }

                foreach (var entry in _context.ChangeTracker.Entries())
                {
                    if (entry.Entity is UserApp userApp)
                    {
                        if (userApp.UserId == user.Id && userApp.AppId == app.Id)
                        {
                            entry.State = EntityState.Deleted;
                        }
                        else
                        {
                            entry.State = EntityState.Modified;
                        }
                    }
                    else if (entry.Entity is UserRole)
                    {
                        entry.State = EntityState.Modified;
                    }
                    else
                    {

                    }
                }

                await _context.SaveChangesAsync();

                result.IsSuccess = true;

                return result;
            }
            catch (Exception e)
            {
                return ReposUtilities.ProcessException<AppsRepository<App>>(
                    _requestService, 
                    _logger, 
                    result, 
                    e);
            }
        }

        public async Task<IRepositoryResponse> Activate(int id)
        {
            var result = new RepositoryResponse();

            if (id == 0)
            {
                result.IsSuccess = false;

                return result;
            }

            try
            {
                var app = await _context.Apps.FindAsync(id);

                if (app != null)
                {
                    if (!app.IsActive)
                    {
                        app.ActivateApp();

                        _context.Attach(app);

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
                    }

                    result.Object = app;
                    result.IsSuccess = true;
                }
                else
                {
                    result.IsSuccess = false;
                }

                return result;
            }
            catch (Exception e)
            {
                return ReposUtilities.ProcessException<AppsRepository<App>>(
                    _requestService, 
                    _logger, 
                    result, 
                    e);
            }
        }

        public async Task<IRepositoryResponse> Deactivate(int id)
        {
            var result = new RepositoryResponse();

            if (id == 0)
            {
                result.IsSuccess = false;

                return result;
            }

            try
            {
                var app = await _context.Apps.FindAsync(id);

                if (app != null)
                {
                    if (app.IsActive)
                    {
                        app.DeactivateApp();

                        _context.Attach(app);

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
                    }

                    result.Object = app;
                    result.IsSuccess = true;
                }
                else
                {
                    result.IsSuccess = false;
                }

                return result;
            }
            catch (Exception e)
            {
                return ReposUtilities.ProcessException<AppsRepository<App>>(
                    _requestService, 
                    _logger, 
                    result, 
                    e);
            }
        }

        public async Task<bool> HasEntity(int id) => 
            await _context.Apps.AnyAsync(a => a.Id == id);

        public async Task<bool> IsAppLicenseValid(string license) => 
            await _context
                .Apps
                .AnyAsync(
                    app => app.License.ToLower().Equals(license.ToLower()));

        public async Task<bool> IsUserRegisteredToApp(
            int id, 
            string license, 
            int userId) => 
            await _context
                .Apps
                .AnyAsync(
                    a => a.Users.Any(ua => ua.UserId == userId)
                    && a.Id == id
                    && a.License.ToLower().Equals(license.ToLower()));

        public async Task<bool> IsUserOwnerOfApp(
            int id, 
            string license, 
            int userId) =>
            await _context
                .Apps
                .AnyAsync(
                    a => a.License.ToLower().Equals(license.ToLower())
                    && a.OwnerId == userId
                    && a.Id == id);

        public async Task<string> GetLicense(int id) => await _context
                .Apps
                .Where(a => a.Id == id)
                .Select(a => a.License)
                .FirstOrDefaultAsync();
        #endregion
    }
}
