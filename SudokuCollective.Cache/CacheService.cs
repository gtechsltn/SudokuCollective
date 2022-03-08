using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Cache;
using SudokuCollective.Core.Interfaces.ServiceModels;
using SudokuCollective.Core.Interfaces.Models;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Models;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Data.Models;

namespace SudokuCollective.Cache
{
    public class CacheService : ICacheService
    {
        public async Task<IRepositoryResponse> AddWithCacheAsync<T>(
            IRepository<T> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            ICacheKeys keys,
            T entity) where T : IDomainEntity
        {
            try
            {
                var response = await repo.Add(entity);

                if (response.IsSuccess && response.Object.Id > 0)
                {
                    var serializedItem = JsonSerializer.Serialize<T>(
                        (T)response.Object, 
                        new JsonSerializerOptions 
                        { 
                            ReferenceHandler = ReferenceHandler.IgnoreCycles 
                        });
                    var encodedItem = Encoding.UTF8.GetBytes(serializedItem);
                    var options = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(expiration);

                    List<string> cacheKeys;

                    if (response.Object is User user)
                    {
                        var appLicense = await ((IUsersRepository<User>)repo)
                            .GetAppLicense(user.Apps.ToList()[0].AppId);

                        await cache.SetAsync(
                            string.Format(cacheKey, response.Object.Id, appLicense),
                            encodedItem,
                            options);

                        cacheKeys = new List<string> {
                            string.Format(keys.GetAppCacheKey, user.Apps.ToList()[0].AppId),
                            string.Format(keys.GetAppUsersCacheKey, user.Apps.ToList()[0].AppId),
                            string.Format(keys.GetNonAppUsersCacheKey, user.Apps.ToList()[0].AppId),
                            string.Format(keys.GetAppByLicenseCacheKey, appLicense),
                            string.Format(keys.GetAppUsersCacheKey, 1),
                            string.Format(keys.GetNonAppUsersCacheKey, 1),
                            keys.GetUsersCacheKey
                        };
                    }
                    else if (response.Object is App app)
                    {

                        await cache.SetAsync(
                            string.Format(cacheKey, response.Object.Id),
                            encodedItem,
                            options);

                        // Remove any app list cache items which may exist
                        cacheKeys = new List<string> {
                                string.Format(keys.GetMyAppsCacheKey, app.OwnerId),
                                keys.GetAppsCacheKey
                            };
                    }
                    else if (response.Object is Difficulty)
                    {

                        await cache.SetAsync(
                            string.Format(cacheKey, response.Object.Id),
                            encodedItem,
                            options);

                        // Remove any difficutly list cache items which may exist
                        cacheKeys = new List<string> {
                                keys.GetDifficultiesCacheKey
                            };
                    }
                    else
                    {

                        await cache.SetAsync(
                            string.Format(cacheKey, response.Object.Id),
                            encodedItem,
                            options);

                        // Remove any role list cache items which may exist
                        cacheKeys = new List<string> {
                                keys.GetRolesCacheKey
                            };
                    }

                    if (cacheKeys != null)
                    {
                        await RemoveKeysAsync(cache, cacheKeys);
                    }
                }

                return response;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Tuple<IRepositoryResponse, IResult>> GetWithCacheAsync<T>(
            IRepository<T> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            int id,
            IResult result = null) where T : IDomainEntity
        {
            try
            {
                IRepositoryResponse response;

                var cachedItem = await cache.GetAsync(cacheKey);

                if (cachedItem != null)
                {
                    var serializedItem = Encoding.UTF8.GetString(cachedItem);

                    response = new RepositoryResponse
                    {
                        IsSuccess = true,
                        Object = JsonSerializer.Deserialize<T>(serializedItem)
                    };

                    if (result != null)
                    {
                        result.IsFromCache = true;
                    }
                }
                else
                {
                    response = await repo.Get(id);

                    if (response.IsSuccess && response.Object != null)
                    {
                        var serializedItem = JsonSerializer.Serialize<T>(
                            (T)response.Object, 
                            new JsonSerializerOptions 
                            {
                                ReferenceHandler = ReferenceHandler.IgnoreCycles
                            });
                        var encodedItem = Encoding.UTF8.GetBytes(serializedItem);
                        var options = new DistributedCacheEntryOptions()
                            .SetAbsoluteExpiration(expiration);

                        await cache.SetAsync(
                            cacheKey,
                            encodedItem,
                            options);
                    }
                }

                return new Tuple<IRepositoryResponse, IResult>(response, result);
            }
            catch
            {
                throw;
            }
        }

        public async Task<Tuple<IRepositoryResponse, IResult>> GetAllWithCacheAsync<T>(
            IRepository<T> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            IResult result = null) where T : IDomainEntity
        {
            try
            {
                IRepositoryResponse response;

                var cachedItems = await cache.GetAsync(cacheKey);

                if (cachedItems != null)
                {
                    var serializedItems = Encoding.UTF8.GetString(cachedItems);

                    response = new RepositoryResponse
                    {
                        IsSuccess = true,
                        Objects = JsonSerializer.Deserialize<List<T>>(serializedItems)
                        .ConvertAll(x => (IDomainEntity)x)
                    };

                    if (result != null)
                    {
                        result.IsFromCache = true;
                    }
                }
                else
                {
                    response = await repo.GetAll();

                    if (response.IsSuccess && response.Objects.Count > 0)
                    {
                        var serializedItems = JsonSerializer.Serialize<List<T>>(
                            response.Objects.ConvertAll(x => (T)x), 
                            new JsonSerializerOptions 
                            { 
                                ReferenceHandler = ReferenceHandler.IgnoreCycles 
                            });
                        var encodedItems = Encoding.UTF8.GetBytes(serializedItems);
                        var options = new DistributedCacheEntryOptions()
                            .SetAbsoluteExpiration(expiration);

                        await cache.SetAsync(
                            cacheKey,
                            encodedItems,
                            options);
                    }
                }

                return new Tuple<IRepositoryResponse, IResult>(response, result);
            }
            catch
            {
                throw;
            }
        }

        public async Task<IRepositoryResponse> UpdateWithCacheAsync<T>(
            IRepository<T> repo,
            IDistributedCache cache,
            ICacheKeys keys,
            T entity,
            string license = null) where T : IDomainEntity
        {
            try
            {
                var response = await repo.Update(entity);

                if (response.IsSuccess && response.Object.Id > 0)
                {
                    List<string> cacheKeys;

                    if (response.Object is User user)
                    {
                        // Remove any user cache items which may exist
                        cacheKeys = new List<string> {
                                string.Format(keys.GetUserCacheKey, user.Id, license),
                                string.Format(keys.GetUserByUsernameCacheKey, user.UserName, license),
                                string.Format(keys.GetUserByEmailCacheKey, user.Email, license),
                                string.Format(keys.GetUsersCacheKey),
                            };
                    }
                    else if (response.Object is App app)
                    {
                        // Remove any app cache items which may exist
                        cacheKeys = new List<string> {
                                string.Format(keys.GetAppCacheKey, app.Id),
                                string.Format(keys.GetAppByLicenseCacheKey, app.License),
                                string.Format(keys.GetMyAppsCacheKey, app.OwnerId),
                                string.Format(keys.GetAppsCacheKey),
                            };
                    }
                    else if (response.Object is Difficulty difficulty)
                    {
                        // Remove any difficutly cache items which may exist
                        cacheKeys = new List<string> {
                                string.Format(keys.GetDifficultyCacheKey, difficulty.Id),
                                keys.GetDifficultiesCacheKey
                            };
                    }
                    else
                    {
                        var role = response.Object as Role;

                        // Remove any role cache items which may exist
                        cacheKeys = new List<string> {
                                string.Format(keys.GetRoleCacheKey, role.Id),
                                keys.GetRolesCacheKey
                            };
                    }

                    await RemoveKeysAsync(cache, cacheKeys);
                }

                return response;
            }
            catch
            {
                throw;
            }
        }

        public async Task<IRepositoryResponse> DeleteWithCacheAsync<T>(
            IRepository<T> repo,
            IDistributedCache cache,
            ICacheKeys keys,
            T entity,
            string license = null) where T : IDomainEntity
        {
            try
            {
                /* If deleting a user this list will be need to get the associated
                 * user apps in order to clear the cache */
                List<App> apps = null;

                if (entity is User)
                {
                    /* Since we're deleting a user we get the associated apps
                     * to clear the cache */
                    apps = new List<App>();

                    var userRepo = (IUsersRepository<User>)repo;
                    var appsResponse = await userRepo.GetMyApps(entity.Id);

                    if (appsResponse.IsSuccess && appsResponse.Objects.Count > 0)
                    {
                        apps = appsResponse
                            .Objects
                            .ConvertAll(a => (App)a)
                            .ToList();
                    }

                    // Finally, attach the license to each app...
                    foreach (var app in apps)
                    {
                        app.License = await userRepo.GetAppLicense(app.Id);
                    }
                }

                var response = await repo.Delete(entity);

                if (response.IsSuccess)
                {
                    List<string> cacheKeys;

                    if (entity is User user)
                    {
                        // Remove any user cache items which may exist
                        cacheKeys = new List<string> {
                            string.Format(keys.GetUserCacheKey, user.Id, license),
                            string.Format(keys.GetUserByUsernameCacheKey, user.UserName, license),
                            string.Format(keys.GetUserByEmailCacheKey, user.Email, license),
                            string.Format(keys.GetMyAppsCacheKey, user.Id),
                            string.Format(keys.GetMyRegisteredCacheKey, user.Id)
                        };

                        if (user.Apps.Count > 0 && apps != null)
                        {
                            foreach (var userApp in user.Apps)
                            {
                                var app = apps.Find(a => a.Id == userApp.AppId);

                                if (app != null)
                                {
                                    cacheKeys.Add(string.Format(keys.GetAppCacheKey, userApp.AppId));
                                    cacheKeys.Add(string.Format(keys.HasAppCacheKey, userApp.AppId));
                                    cacheKeys.Add(string.Format(keys.GetAppLicenseCacheKey, userApp.AppId));
                                    cacheKeys.Add(string.Format(keys.GetAppByLicenseCacheKey, app.License));
                                    cacheKeys.Add(string.Format(keys.IsAppLicenseValidCacheKey, app.License));
                                    cacheKeys.Add(string.Format(keys.GetAppUsersCacheKey, userApp.AppId));
                                    cacheKeys.Add(string.Format(keys.GetNonAppUsersCacheKey, userApp.AppId));
                                    cacheKeys.Add(keys.GetAppsCacheKey);
                                }
                            }
                        }
                    }
                    else if (entity is App app)
                    {
                        // Remove any user cache items which may exist
                        cacheKeys = new List<string> {
                        string.Format(keys.GetAppCacheKey, app.Id),
                        string.Format(keys.HasAppCacheKey, app.Id),
                        string.Format(keys.GetAppLicenseCacheKey, app.Id),
                        string.Format(keys.GetAppByLicenseCacheKey, app.License),
                        string.Format(keys.IsAppLicenseValidCacheKey, app.License),
                        string.Format(keys.GetMyAppsCacheKey, app.OwnerId),
                        string.Format(keys.HasAppCacheKey, app.Id),
                        keys.GetAppsCacheKey
                    };
                    }
                    else if (entity is Difficulty difficulty)
                    {
                        // Remove any difficulty cache items which may exist
                        cacheKeys = new List<string> {
                            string.Format(keys.GetDifficultyCacheKey, difficulty.Id),
                            keys.GetDifficultiesCacheKey
                        };
                    }
                    else
                    {
                        var role = entity as Role;

                        // Remove any role cache items which may exist
                        cacheKeys = new List<string> {
                            string.Format(keys.GetRoleCacheKey, role.Id),
                            keys.GetRolesCacheKey
                        };
                    }

                    await RemoveKeysAsync(cache, cacheKeys);
                }

                return response;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> HasEntityWithCacheAsync<T>(
            IRepository<T> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            int id) where T : IDomainEntity
        {
            try
            {
                bool result;

                var cachedItem = await cache.GetAsync(cacheKey);

                if (cachedItem != null)
                {
                    var serializedItem = Encoding.UTF8.GetString(cachedItem);
                    result = JsonSerializer.Deserialize<bool>(serializedItem);
                }
                else
                {
                    var response = await repo.HasEntity(id);

                    var serializedItem = JsonSerializer.Serialize<bool>(
                        response, 
                        new JsonSerializerOptions 
                        { 
                            ReferenceHandler = ReferenceHandler.IgnoreCycles
                        });
                    var encodedItem = Encoding.UTF8.GetBytes(serializedItem);
                    var options = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(expiration);

                    await cache.SetAsync(
                        cacheKey,
                        encodedItem,
                        options);

                    result = response;
                }

                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task RemoveKeysAsync(
            IDistributedCache cache,
            List<string> keys)
        {
            foreach (var key in keys)
            {
                if (await cache.GetAsync(key) != null)
                {
                    await cache.RemoveAsync(string.Format(key));
                }
            }
        }

        #region App Repository Cache Methods
        public async Task<Tuple<IRepositoryResponse, IResult>> GetAppByLicenseWithCacheAsync(
            IAppsRepository<App> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            string license,
            IResult result = null)
        {
            try
            {
                IRepositoryResponse response;

                var cachedItem = await cache.GetAsync(cacheKey);

                if (cachedItem != null)
                {
                    var serializedItem = Encoding.UTF8.GetString(cachedItem);

                    response = new RepositoryResponse
                    {
                        IsSuccess = true,
                        Object = JsonSerializer.Deserialize<App>(serializedItem)
                    };

                    if (result != null)
                    {
                        result.IsFromCache = true;
                    }
                }
                else
                {
                    response = await repo.GetByLicense(license);

                    if (response.IsSuccess && response.Object != null)
                    {
                        var serializedItem = JsonSerializer.Serialize<App>(
                            (App)response.Object, 
                            new JsonSerializerOptions 
                            { 
                                ReferenceHandler = ReferenceHandler.IgnoreCycles 
                            });
                        var encodedItem = Encoding.UTF8.GetBytes(serializedItem);
                        var options = new DistributedCacheEntryOptions()
                            .SetAbsoluteExpiration(expiration);

                        await cache.SetAsync(
                            cacheKey,
                            encodedItem,
                            options);
                    }
                }

                return new Tuple<IRepositoryResponse, IResult>(response, result);
            }
            catch
            {
                throw;
            }
        }

        public async Task<Tuple<IRepositoryResponse, IResult>> GetAppUsersWithCacheAsync(
            IAppsRepository<App> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            int id,
            IResult result = null)
        {
            try
            {
                IRepositoryResponse response;

                var cachedItems = await cache.GetAsync(cacheKey);

                if (cachedItems != null)
                {
                    var serializedItems = Encoding.UTF8.GetString(cachedItems);

                    response = new RepositoryResponse
                    {
                        IsSuccess = true,
                        Objects = JsonSerializer.Deserialize<List<User>>(serializedItems)
                        .ConvertAll(u => (IDomainEntity)u)
                    };

                    if (result != null)
                    {
                        result.IsFromCache = true;
                    }
                }
                else
                {
                    response = await repo.GetAppUsers(id);

                    if (response.IsSuccess && response.Objects.Count > 0)
                    {
                        var serializedItems = JsonSerializer.Serialize<List<User>>(
                            response.Objects.ConvertAll(u => (User)u), 
                            new JsonSerializerOptions 
                            { 
                                ReferenceHandler = ReferenceHandler.IgnoreCycles 
                            });
                        var encodedItems = Encoding.UTF8.GetBytes(serializedItems);
                        var options = new DistributedCacheEntryOptions()
                            .SetAbsoluteExpiration(expiration);

                        await cache.SetAsync(
                            cacheKey,
                            encodedItems,
                            options);
                    }
                }

                return new Tuple<IRepositoryResponse, IResult>(response, result);
            }
            catch
            {
                throw;
            }
        }

        public async Task<Tuple<IRepositoryResponse, IResult>> GetNonAppUsersWithCacheAsync(
            IAppsRepository<App> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            int id,
            IResult result = null)
        {
            try
            {
                IRepositoryResponse response;

                var cachedItems = await cache.GetAsync(cacheKey);

                if (cachedItems != null)
                {
                    var serializedItems = Encoding.UTF8.GetString(cachedItems);

                    response = new RepositoryResponse
                    {
                        IsSuccess = true,
                        Objects = JsonSerializer.Deserialize<List<User>>(serializedItems)
                        .ConvertAll(s => (IDomainEntity)s)
                    };

                    if (result != null)
                    {
                        result.IsFromCache = true;
                    }
                }
                else
                {
                    response = await repo.GetNonAppUsers(id);

                    if (response.IsSuccess && response.Objects.Count > 0)
                    {
                        var serializedItems = JsonSerializer.Serialize<List<User>>(
                            response.Objects.ConvertAll(u => (User)u), 
                            new JsonSerializerOptions 
                            { 
                                ReferenceHandler = ReferenceHandler.IgnoreCycles 
                            });
                        var encodedItems = Encoding.UTF8.GetBytes(serializedItems);
                        var options = new DistributedCacheEntryOptions()
                            .SetAbsoluteExpiration(expiration);

                        await cache.SetAsync(
                            cacheKey,
                            encodedItems,
                            options);
                    }
                }

                return new Tuple<IRepositoryResponse, IResult>(response, result);
            }
            catch
            {
                throw;
            }
        }

        public async Task<Tuple<IRepositoryResponse, IResult>> GetMyAppsWithCacheAsync(
            IAppsRepository<App> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            int ownerId,
            IResult result = null)
        {
            try
            {
                IRepositoryResponse response;

                var cachedItems = await cache.GetAsync(cacheKey);

                if (cachedItems != null)
                {
                    var serializedItems = Encoding.UTF8.GetString(cachedItems);

                    response = new RepositoryResponse
                    {
                        IsSuccess = true,
                        Objects = JsonSerializer.Deserialize<List<App>>(serializedItems)
                        .ConvertAll(a => (IDomainEntity)a)
                    };

                    if (result != null)
                    {
                        result.IsFromCache = true;
                    }
                }
                else
                {
                    response = await repo.GetMyApps(ownerId);

                    if (response.IsSuccess && response.Objects.Count > 0)
                    {
                        var serializedItems = JsonSerializer.Serialize<List<App>>(
                            response.Objects.ConvertAll(a => (App)a), 
                            new JsonSerializerOptions 
                            { 
                                ReferenceHandler = ReferenceHandler.IgnoreCycles
                            });
                        var encodedItems = Encoding.UTF8.GetBytes(serializedItems);
                        var options = new DistributedCacheEntryOptions()
                            .SetAbsoluteExpiration(expiration);

                        await cache.SetAsync(
                            cacheKey,
                            encodedItems,
                            options);
                    }
                }

                return new Tuple<IRepositoryResponse, IResult>(response, result);
            }
            catch
            {
                throw;
            }
        }

        public async Task<Tuple<IRepositoryResponse, IResult>> GetMyRegisteredAppsWithCacheAsync(
            IAppsRepository<App> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            int userId,
            IResult result = null)
        {
            try
            {
                IRepositoryResponse response;

                var cachedItems = await cache.GetAsync(cacheKey);

                if (cachedItems != null)
                {
                    var serializedItems = Encoding.UTF8.GetString(cachedItems);

                    response = new RepositoryResponse
                    {
                        IsSuccess = true,
                        Objects = JsonSerializer.Deserialize<List<App>>(serializedItems)
                        .ConvertAll(a => (IDomainEntity)a)
                    };

                    if (result != null)
                    {
                        result.IsFromCache = true;
                    }
                }
                else
                {
                    response = await repo.GetMyRegisteredApps(userId);

                    if (response.IsSuccess && response.Objects.Count > 0)
                    {
                        var serializedItems = JsonSerializer.Serialize<List<App>>(
                            response.Objects.ConvertAll(a => (App)a), 
                            new JsonSerializerOptions 
                            { 
                                ReferenceHandler = ReferenceHandler.IgnoreCycles 
                            });
                        var encodedItems = Encoding.UTF8.GetBytes(serializedItems);
                        var options = new DistributedCacheEntryOptions()
                            .SetAbsoluteExpiration(expiration);

                        await cache.SetAsync(
                            cacheKey,
                            encodedItems,
                            options);
                    }
                }

                return new Tuple<IRepositoryResponse, IResult>(response, result);
            }
            catch
            {
                throw;
            }
        }

        public async Task<Tuple<string, IResult>> GetLicenseWithCacheAsync(
            IAppsRepository<App> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            ICacheKeys keys,
            int id,
            IResult result = null)
        {
            try
            {
                string license;

                var cachedItem = await cache.GetAsync(cacheKey);

                if (cachedItem != null)
                {
                    var serializedItem = Encoding.UTF8.GetString(cachedItem);

                    license = JsonSerializer.Deserialize<string>(serializedItem);

                    if (result != null)
                    {
                        result.IsFromCache = true;
                    }
                }
                else
                {
                    license = await repo.GetLicense(id);

                    if (!string.IsNullOrEmpty(license))
                    {
                        var serializedItem = JsonSerializer.Serialize<string>(
                            license, 
                            new JsonSerializerOptions 
                            { 
                                ReferenceHandler = ReferenceHandler.IgnoreCycles 
                            });
                        var encodedItem = Encoding.UTF8.GetBytes(serializedItem);
                        var options = new DistributedCacheEntryOptions()
                            .SetAbsoluteExpiration(expiration);

                        await cache.SetAsync(
                            string.Format(keys.GetAppLicenseCacheKey, id),
                            encodedItem,
                            options);
                    }
                }

                return new Tuple<string, IResult>(license, result);
            }
            catch
            {
                throw;
            }
        }

        public async Task<IRepositoryResponse> ResetWithCacheAsync(
            IAppsRepository<App> repo,
            IDistributedCache cache,
            ICacheKeys keys,
            App app)
        {
            try
            {
                var response = await repo.Reset(app);

                if (response.IsSuccess)
                {
                    List<string> cacheKeys;

                    // Remove any user cache items which may exist
                    cacheKeys = new List<string> {
                            string.Format(keys.GetAppCacheKey, app.Id),
                            string.Format(keys.GetAppByLicenseCacheKey, app.License)
                        };

                    await RemoveKeysAsync(cache, cacheKeys);
                }

                return response;
            }
            catch
            {
                throw;
            }
        }

        public async Task<IRepositoryResponse> ActivatetWithCacheAsync(
            IAppsRepository<App> repo,
            IDistributedCache cache,
            ICacheKeys keys,
            int id)
        {
            try
            {
                IRepositoryResponse response;

                var app = (App)(await repo.Get(id)).Object;

                if (app != null)
                {
                    response = await repo.Activate(app.Id);
                }
                else
                {
                    response = new RepositoryResponse();
                }

                if (response.IsSuccess)
                {
                    List<string> cacheKeys;

                    // Remove any user cache items which may exist
                    cacheKeys = new List<string> {
                            string.Format(keys.GetAppCacheKey, app.Id),
                            string.Format(keys.GetAppByLicenseCacheKey, app.License)
                        };

                    await RemoveKeysAsync(cache, cacheKeys);
                }

                return response;
            }
            catch
            {
                throw;
            }
        }

        public async Task<IRepositoryResponse> DeactivatetWithCacheAsync(
            IAppsRepository<App> repo,
            IDistributedCache cache,
            ICacheKeys keys,
            int id)
        {
            try
            {
                IRepositoryResponse response;

                var app = (App)(await repo.Get(id)).Object;

                if (app != null)
                {
                    response = await repo.Deactivate(app.Id);
                }
                else
                {
                    response = new RepositoryResponse();
                }

                if (response.IsSuccess)
                {
                    List<string> cacheKeys;

                    // Remove any user cache items which may exist
                    cacheKeys = new List<string> {
                            string.Format(keys.GetAppCacheKey, app.Id),
                            string.Format(keys.GetAppByLicenseCacheKey, app.License)
                        };

                    await RemoveKeysAsync(cache, cacheKeys);
                }

                return response;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> IsAppLicenseValidWithCacheAsync(
            IAppsRepository<App> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            string license)
        {
            try
            {
                bool result;

                var cachedItem = await cache.GetAsync(cacheKey);

                if (cachedItem != null)
                {
                    var serializedItem = Encoding.UTF8.GetString(cachedItem);
                    result = JsonSerializer.Deserialize<bool>(serializedItem);
                }
                else
                {
                    var response = await repo.IsAppLicenseValid(license);

                    var serializedItem = JsonSerializer.Serialize<bool>(
                        response, 
                        new JsonSerializerOptions 
                        { 
                            ReferenceHandler = ReferenceHandler.IgnoreCycles 
                        });
                    var encodedItem = Encoding.UTF8.GetBytes(serializedItem);
                    var options = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(expiration);

                    await cache.SetAsync(
                        cacheKey,
                        encodedItem,
                        options);

                    result = response;
                }

                return result;
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region User Repository Cache Methods
        public async Task<Tuple<IRepositoryResponse, IResult>> GetByUserNameWithCacheAsync(
            IUsersRepository<User> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            ICacheKeys keys,
            string username,
            string license = null,
            IResult result = null)
        {
            try
            {
                IRepositoryResponse response;

                var cachedItem = await cache.GetAsync(cacheKey);


                if (cachedItem != null)
                {
                    var serializedItem = Encoding.UTF8.GetString(cachedItem);

                    response = new RepositoryResponse
                    {
                        IsSuccess = true,
                        Object = JsonSerializer.Deserialize<User>(serializedItem)
                    };

                    if (result != null)
                    {
                        result.IsFromCache = true;
                    }
                }
                else
                {
                    response = await repo.GetByUserName(username);

                    if (response.IsSuccess && response.Object != null)
                    {
                        var serializedItem = JsonSerializer.Serialize<User>(
                            (User)response.Object, 
                            new JsonSerializerOptions 
                            { 
                                ReferenceHandler = ReferenceHandler.IgnoreCycles 
                            });
                        var encodedItem = Encoding.UTF8.GetBytes(serializedItem);
                        var options = new DistributedCacheEntryOptions()
                            .SetAbsoluteExpiration(expiration);

                        // Add user cache key by username
                        await cache.SetAsync(
                            cacheKey,
                            encodedItem,
                            options);

                        // Add user cache key by id
                        await cache.SetAsync(
                            string.Format(keys.GetUserCacheKey, response.Object.Id, license),
                            encodedItem,
                            options);
                    }
                }

                return new Tuple<IRepositoryResponse, IResult>(response, result);
            }
            catch
            {
                throw;
            }
        }

        public async Task<Tuple<IRepositoryResponse, IResult>> GetByEmailWithCacheAsync(
            IUsersRepository<User> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            string email,
            IResult result = null)
        {
            try
            {
                IRepositoryResponse response;

                var cachedItem = await cache.GetAsync(cacheKey);

                if (cachedItem != null)
                {
                    var serializedItem = Encoding.UTF8.GetString(cachedItem);

                    response = new RepositoryResponse
                    {
                        IsSuccess = true,
                        Object = JsonSerializer.Deserialize<User>(serializedItem)
                    };

                    if (result != null)
                    {
                        result.IsFromCache = true;
                    }
                }
                else
                {
                    response = await repo.GetByEmail(email);

                    if (response.IsSuccess && response.Object != null)
                    {
                        var serializedItem = JsonSerializer.Serialize<User>(
                            (User)response.Object, 
                            new JsonSerializerOptions 
                            { 
                                ReferenceHandler = ReferenceHandler.IgnoreCycles
                            });
                        var encodedItem = Encoding.UTF8.GetBytes(serializedItem);
                        var options = new DistributedCacheEntryOptions()
                            .SetAbsoluteExpiration(expiration);

                        await cache.SetAsync(
                            cacheKey,
                            encodedItem,
                            options);
                    }
                }

                return new Tuple<IRepositoryResponse, IResult>(response, result);
            }
            catch
            {
                throw;
            }
        }

        public async Task<IRepositoryResponse> ConfirmEmailWithCacheAsync(
            IUsersRepository<User> repo,
            IDistributedCache cache,
            ICacheKeys keys,
            EmailConfirmation emailConfirmation,
            string license)
        {
            try
            {
                var response = await repo.ConfirmEmail(emailConfirmation);

                if (response.IsSuccess)
                {
                    var user = (User)response.Object;
                    List<string> cacheKeys;

                    // Remove any user cache items which may exist
                    cacheKeys = new List<string> {
                        string.Format(keys.GetUserCacheKey, user.Id, license),
                        string.Format(keys.GetUserByUsernameCacheKey, user.UserName, license),
                        string.Format(keys.GetUserByEmailCacheKey, user.Email, license)
                    };

                    await RemoveKeysAsync(cache, cacheKeys);
                }

                return response;
            }
            catch
            {
                throw;
            }
        }

        public async Task<IRepositoryResponse> UpdateEmailWithCacheAsync(
            IUsersRepository<User> repo,
            IDistributedCache cache,
            ICacheKeys keys,
            EmailConfirmation emailConfirmation,
            string license)
        {
            try
            {
                var response = await repo.UpdateEmail(emailConfirmation);

                if (response.IsSuccess)
                {
                    var user = (User)response.Object;
                    List<string> cacheKeys;

                    // Remove any user cache items which may exist
                    cacheKeys = new List<string> {
                        string.Format(keys.GetUserCacheKey, user.Id, license),
                        string.Format(keys.GetUserByUsernameCacheKey, user.UserName, license),
                        string.Format(keys.GetUserByEmailCacheKey, user.Email, license)
                    };

                    await RemoveKeysAsync(cache, cacheKeys);
                }

                return response;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> IsUserRegisteredWithCacheAsync(
            IUsersRepository<User> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            int id)
        {
            try
            {
                bool result;

                var cachedItem = await cache.GetAsync(cacheKey);

                if (cachedItem != null)
                {
                    var serializedItem = Encoding.UTF8.GetString(cachedItem);
                    result = JsonSerializer.Deserialize<bool>(serializedItem);
                }
                else
                {
                    var response = await repo.IsUserRegistered(id);

                    var serializedItem = JsonSerializer.Serialize<bool>(
                        response, 
                        new JsonSerializerOptions 
                        { 
                            ReferenceHandler = ReferenceHandler.IgnoreCycles 
                        });
                    var encodedItem = Encoding.UTF8.GetBytes(serializedItem);
                    var options = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(expiration);

                    await cache.SetAsync(
                        cacheKey,
                        encodedItem,
                        options);

                    result = response;
                }

                return result;
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region Difficulty Repository Cache Methods
        public async Task<bool> HasDifficultyLevelWithCacheAsync(
            IDifficultiesRepository<Difficulty> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            DifficultyLevel difficultyLevel)
        {
            try
            {
                bool result;

                var cachedItem = await cache.GetAsync(cacheKey);

                if (cachedItem != null)
                {
                    var serializedItem = Encoding.UTF8.GetString(cachedItem);
                    result = JsonSerializer.Deserialize<bool>(serializedItem);
                }
                else
                {
                    var response = await repo.HasDifficultyLevel(difficultyLevel);

                    var serializedItem = JsonSerializer.Serialize<bool>(
                        response, 
                        new JsonSerializerOptions 
                        { 
                            ReferenceHandler = ReferenceHandler.IgnoreCycles 
                        });
                    var encodedItem = Encoding.UTF8.GetBytes(serializedItem);
                    var options = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(expiration);

                    await cache.SetAsync(
                        cacheKey,
                        encodedItem,
                        options);

                    result = response;
                }

                return result;
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region Roles Repository Cache Methods
        public async Task<bool> HasRoleLevelWithCacheAsync(
            IRolesRepository<Role> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            RoleLevel roleLevel)
        {
            try
            {
                bool result;

                var cachedItem = await cache.GetAsync(cacheKey);

                if (cachedItem != null)
                {
                    var serializedItem = Encoding.UTF8.GetString(cachedItem);
                    result = JsonSerializer.Deserialize<bool>(serializedItem);
                }
                else
                {
                    var response = await repo.HasRoleLevel(roleLevel);

                    var serializedItem = JsonSerializer.Serialize<bool>(
                        response, 
                        new JsonSerializerOptions 
                        { 
                            ReferenceHandler = ReferenceHandler.IgnoreCycles 
                        });
                    var encodedItem = Encoding.UTF8.GetBytes(serializedItem);
                    var options = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(expiration);

                    await cache.SetAsync(
                        cacheKey,
                        encodedItem,
                        options);

                    result = response;
                }

                return result;
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
