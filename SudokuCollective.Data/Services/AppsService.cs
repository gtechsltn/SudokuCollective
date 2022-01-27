using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Requests;
using SudokuCollective.Data.Models.Results;
using SudokuCollective.Data.Resiliency;
using SudokuCollective.Data.Utilities;

namespace SudokuCollective.Data.Services
{
    public class AppsService : IAppsService
    {
        #region Fields
        private readonly IAppsRepository<App> _appsRepository;
        private readonly IUsersRepository<User> _usersRepository;
        private readonly IAppAdminsRepository<AppAdmin> _appAdminsRepository;
        private readonly IRolesRepository<Role> _rolesRepository;
        private readonly IDistributedCache _distributedCache;
        #endregion

        #region Constructor
        public AppsService(
            IAppsRepository<App> appRepository, 
            IUsersRepository<User> userRepository,
            IAppAdminsRepository<AppAdmin> appAdminsRepository,
            IRolesRepository<Role> rolesRepository,
            IDistributedCache distributedCache)
        {
            _appsRepository = appRepository;
            _usersRepository = userRepository;
            _appAdminsRepository = appAdminsRepository;
            _rolesRepository = rolesRepository;
            _distributedCache = distributedCache;
        }
        #endregion

        #region Methods
        public async Task<IResult> Create(ILicenseRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = new Result();

            try
            {
                // Ensure the intended owner exists by pull the records from the repository
                var userResponse = await _usersRepository.Get(request.OwnerId);

                if (userResponse.Success)
                {
                    var user = (User)userResponse.Object;

                    var generatingGuid = true;
                    var license = new Guid();

                    /* Ensure the license is unique by pulling all apps from the repository
                     * and checking that the new license is unique */
                    var cacheFactoryResponse = await CacheFactory.GetAllWithCacheAsync(
                        _appsRepository,
                        _distributedCache,
                        string.Format(CacheKeys.GetAppsCacheKey),
                        CachingStrategy.Medium);

                    var checkAppsResponse = (RepositoryResponse)cacheFactoryResponse.Item1;

                    foreach (var a in checkAppsResponse.Objects.ConvertAll(a => (App)a))
                    {
                        a.License = (await CacheFactory.GetLicenseWithCacheAsync(
                            _appsRepository,
                            _distributedCache,
                            string.Format(CacheKeys.GetAppLicenseCacheKey, a.Id),
                            CachingStrategy.Heavy,
                            a.Id)).Item1;
                    }

                    do
                    {
                        license = Guid.NewGuid();

                        if (!checkAppsResponse
                            .Objects
                            .ConvertAll(a => (App)a)
                            .Any(a => a.License.Equals(license.ToString())))
                        {
                            generatingGuid = false;
                        }
                        else
                        {
                            generatingGuid = true;
                        }

                    } while (generatingGuid);

                    var app = new App(
                        request.Name,
                        license.ToString(),
                        request.OwnerId,
                        request.DevUrl,
                        request.ProdUrl);

                    var addAppResponse = await CacheFactory.AddWithCacheAsync(
                        _appsRepository,
                        _distributedCache,
                        CacheKeys.GetAppCacheKey,
                        CachingStrategy.Medium,
                        app);

                    if (addAppResponse.Success)
                    {
                        if (user.Roles.Any(ur => ur.Role.RoleLevel == RoleLevel.ADMIN))
                        {
                            var appAdmin = new AppAdmin(app.Id, user.Id);

                            _ = await _appAdminsRepository.Add(appAdmin);
                        }

                        result.IsSuccess = addAppResponse.Success;
                        result.Message = AppsMessages.AppCreatedMessage;
                        result.DataPacket.Add((App)addAppResponse.Object);

                        return result;
                    }
                    else if (!addAppResponse.Success && addAppResponse.Exception != null)
                    {
                        result.IsSuccess = addAppResponse.Success;
                        result.Message = addAppResponse.Exception.Message;

                        return result;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = AppsMessages.AppNotCreatedMessage;

                        return result;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = UsersMessages.UserDoesNotExistMessage;

                    return result;
                }
            }
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> Get(int id)
        {
            var result = new Result();

            if (id == 0)
            {
                result.IsSuccess = false;
                result.Message = AppsMessages.AppNotFoundMessage;

                return result;
            }

            try
            {
                var cacheFactoryResponse = await CacheFactory.GetWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(CacheKeys.GetAppCacheKey, id),
                    CachingStrategy.Medium,
                    id,
                    result);

                var response = (RepositoryResponse)cacheFactoryResponse.Item1;
                result = (Result)cacheFactoryResponse.Item2;

                if (response.Success)
                {
                    var app = (App)response.Object;

                    result.IsSuccess = response.Success;
                    result.Message = AppsMessages.AppFoundMessage;
                    result.DataPacket.Add(app);

                    return result;
                }
                else if (!response.Success && response.Exception != null)
                {
                    result.IsSuccess = response.Success;
                    result.Message = response.Exception.Message;

                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = AppsMessages.AppNotFoundMessage;

                    return result;
                }
            }
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> GetByLicense(string license, int requestorId)
        {
            var result = new Result();

            if (string.IsNullOrEmpty(license) || requestorId == 0)
            {
                result.IsSuccess = false;
                result.Message = UsersMessages.UserNotFoundMessage;

                return result;
            }

            try
            {
                var cacheFactoryResponse = await CacheFactory.GetAppByLicenseWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(CacheKeys.GetAppByLicenseCacheKey, license),
                    CachingStrategy.Medium,
                    license,
                    result);

                var response = (RepositoryResponse)cacheFactoryResponse.Item1;
                result = (Result)cacheFactoryResponse.Item2;

                if (response.Success)
                {
                    var app = (IApp)response.Object;

                    result.IsSuccess = response.Success;
                    result.Message = AppsMessages.AppFoundMessage;
                    result.DataPacket.Add(app);

                    return result;
                }
                else if (!response.Success && response.Exception != null)
                {
                    result.IsSuccess = response.Success;
                    result.Message = response.Exception.Message;

                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = AppsMessages.AppNotFoundMessage;

                    return result;
                }
            }
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> GetApps(
            IPaginator paginator, 
            int requestorId)
        {
            if (paginator == null) throw new ArgumentNullException(nameof(paginator));

            var result = new Result();

            if (requestorId == 0)
            {
                result.IsSuccess = false;
                result.Message = UsersMessages.UserNotFoundMessage;

                return result;
            }

            try
            {
                var cacheFactoryResponse = await CacheFactory.GetAllWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(CacheKeys.GetAppsCacheKey),
                    CachingStrategy.Medium,
                    result);

                var response = (RepositoryResponse)cacheFactoryResponse.Item1;
                result = (Result)cacheFactoryResponse.Item2;

                if (response.Success)
                {
                    if (DataUtilities.IsPageValid(paginator, response.Objects))
                    {
                        result = PaginatorUtilities.PaginateApps(paginator, response, result);

                        if (result.Message.Equals(
                            ServicesMesages.SortValueNotImplementedMessage))
                        {
                            return result;
                        }
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = ServicesMesages.PageNotFoundMessage;

                        return result;
                    }

                    result.IsSuccess = response.Success;
                    result.Message = AppsMessages.AppsFoundMessage;

                    return result;
                }
                else if (!response.Success && response.Exception != null)
                {
                    result.IsSuccess = response.Success;
                    result.Message = response.Exception.Message;

                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = AppsMessages.AppsNotFoundMessage;

                    return result;
                }
            }
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> GetMyApps(int ownerId, IPaginator paginator)
        {
            if (paginator == null) throw new ArgumentNullException(nameof(paginator));

            var result = new Result();

            if (ownerId == 0)
            {
                result.IsSuccess = false;
                result.Message = UsersMessages.UserNotFoundMessage;

                return result;
            }

            try
            {
                var cacheFactoryResponse = await CacheFactory.GetMyAppsWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(string.Format(CacheKeys.GetMyAppsCacheKey, ownerId)),
                    CachingStrategy.Medium,
                    ownerId,
                    result);

                var response = (RepositoryResponse)cacheFactoryResponse.Item1;
                result = (Result)cacheFactoryResponse.Item2;

                if (response.Success)
                {
                    if (DataUtilities.IsPageValid(paginator, response.Objects))
                    {
                        result = PaginatorUtilities.PaginateApps(paginator, response, result);

                        if (result.Message.Equals(
                            ServicesMesages.SortValueNotImplementedMessage))
                        {
                            return result;
                        }
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = ServicesMesages.PageNotFoundMessage;

                        return result;
                    }

                    result.IsSuccess = response.Success;
                    result.Message = AppsMessages.AppsFoundMessage;

                    return result;
                }
                else if (!response.Success && response.Exception != null)
                {
                    result.IsSuccess = response.Success;
                    result.Message = response.Exception.Message;

                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = AppsMessages.AppsNotFoundMessage;

                    return result;
                }
            }
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> GetRegisteredApps(int userId, IPaginator paginator)
        {
            if (paginator == null) throw new ArgumentNullException(nameof(paginator));

            var result = new Result();

            if (userId == 0)
            {
                result.IsSuccess = false;
                result.Message = UsersMessages.UserNotFoundMessage;

                return result;
            }

            try
            {
                var cacheFactoryResponse = await CacheFactory.GetMyRegisteredAppsWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(string.Format(CacheKeys.GetMyRegisteredCacheKey, userId)),
                    CachingStrategy.Medium,
                    userId,
                    result);

                var response = (RepositoryResponse)cacheFactoryResponse.Item1;
                result = (Result)cacheFactoryResponse.Item2;

                if (response.Success)
                {
                    if (DataUtilities.IsPageValid(paginator, response.Objects))
                    {
                        result = PaginatorUtilities.PaginateApps(paginator, response, result);

                        if (result.Message.Equals(
                            ServicesMesages.SortValueNotImplementedMessage))
                        {
                            return result;
                        }
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = ServicesMesages.PageNotFoundMessage;

                        return result;
                    }

                    result.IsSuccess = response.Success;
                    result.Message = AppsMessages.AppsFoundMessage;

                    return result;
                }
                else if (!response.Success && response.Exception != null)
                {
                    result.IsSuccess = response.Success;
                    result.Message = response.Exception.Message;

                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = AppsMessages.AppsNotFoundMessage;

                    return result;
                }
            }
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> Update(int id, IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = new Result();

            AppRequest appRequest;

            if (request.DataPacket is AppRequest r)
            {
                appRequest = r;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = ServicesMesages.InvalidRequestMessage;

                return result;
            }

            if (id == 0)
            {
                result.IsSuccess = false;
                result.Message = AppsMessages.AppNotFoundMessage;

                return result;
            }

            try
            {
                var getAppResponse = await _appsRepository.Get(id);

                if (getAppResponse.Success)
                {
                    if (getAppResponse.Success)
                    {
                        var app = (App)getAppResponse.Object;

                        app.Name = appRequest.Name;
                        app.DevUrl = appRequest.DevUrl;
                        app.ProdUrl = appRequest.ProdUrl;
                        app.IsActive = appRequest.IsActive;
                        app.Environment = appRequest.Environment;
                        app.PermitSuperUserAccess = appRequest.PermitSuperUserAccess;
                        app.PermitCollectiveLogins = appRequest.PermitCollectiveLogins;
                        app.DisableCustomUrls = appRequest.DisableCustomUrls;
                        app.CustomEmailConfirmationAction = appRequest.CustomEmailConfirmationAction;
                        app.CustomPasswordResetAction = appRequest.CustomPasswordResetAction;
                        app.TimeFrame = appRequest.TimeFrame;
                        app.AccessDuration = appRequest.AccessDuration;
                        app.DateUpdated = DateTime.UtcNow;

                        var updateAppResponse = await CacheFactory.UpdateWithCacheAsync<App>(
                            _appsRepository,
                            _distributedCache,
                            app);

                        if (updateAppResponse.Success)
                        {
                            result.IsSuccess = true;
                            result.Message = AppsMessages.AppUpdatedMessage;
                            result.DataPacket.Add(app);

                            return result;
                        }
                        else if (!updateAppResponse.Success && updateAppResponse.Exception != null)
                        {
                            result.IsSuccess = updateAppResponse.Success;
                            result.Message = updateAppResponse.Exception.Message;

                            return result;
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Message = AppsMessages.AppNotUpdatedMessage;

                            return result;
                        }
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = getAppResponse.Exception.Message;

                        return result;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = AppsMessages.AppNotFoundMessage;

                    return result;
                }
            }
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> DeleteOrReset(int id, bool isReset = false)
        {
            var result = new Result();

            if (id == 0)
            {
                result.IsSuccess = false;
                result.Message = AppsMessages.AppNotFoundMessage;

                return result;
            }

            try
            {
                var getAppResponse = await _appsRepository.Get(id);

                if (getAppResponse.Success)
                {
                    if (isReset)
                    {
                        if (getAppResponse.Success)
                        {
                            var resetAppResponse = await CacheFactory.ResetWithCacheAsync(
                                _appsRepository,
                                _distributedCache,
                                (App)getAppResponse.Object);

                            if (resetAppResponse.Success)
                            {
                                result.IsSuccess = resetAppResponse.Success;
                                result.Message = AppsMessages.AppResetMessage;
                                result.DataPacket.Add(resetAppResponse.Object);

                                return result;
                            }
                            else if (!resetAppResponse.Success && resetAppResponse.Exception != null)
                            {
                                result.IsSuccess = resetAppResponse.Success;
                                result.Message = resetAppResponse.Exception.Message;

                                return result;
                            }
                            else
                            {
                                result.IsSuccess = false;
                                result.Message = AppsMessages.AppNotFoundMessage;

                                return result;
                            }
                        }
                        else if (!getAppResponse.Success && getAppResponse.Exception != null)
                        {
                            result.IsSuccess = getAppResponse.Success;
                            result.Message = getAppResponse.Exception.Message;

                            return result;
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Message = AppsMessages.AppNotFoundMessage;

                            return result;
                        }
                    }
                    else
                    {
                        if (id == 1)
                        {
                            result.IsSuccess = false;
                            result.Message = AppsMessages.AdminAppCannotBeDeletedMessage;

                            return result;
                        }

                        if (getAppResponse.Success)
                        {
                            var deleteAppResponse = await CacheFactory.DeleteWithCacheAsync(
                                _appsRepository,
                                _distributedCache,
                                (App)getAppResponse.Object);

                            if (deleteAppResponse.Success)
                            {
                                result.IsSuccess = deleteAppResponse.Success;
                                result.Message = AppsMessages.AppDeletedMessage;

                                return result;
                            }
                            else if (!deleteAppResponse.Success && deleteAppResponse.Exception != null)
                            {
                                result.IsSuccess = deleteAppResponse.Success;
                                result.Message = deleteAppResponse.Exception.Message;

                                return result;
                            }
                            else
                            {
                                result.IsSuccess = false;
                                result.Message = AppsMessages.AppNotDeletedMessage;

                                return result;
                            }
                        }
                        else if (!getAppResponse.Success && getAppResponse.Exception != null)
                        {
                            result.IsSuccess = getAppResponse.Success;
                            result.Message = getAppResponse.Exception.Message;

                            return result;
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Message = AppsMessages.AppNotFoundMessage;

                            return result;
                        }
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = AppsMessages.AppNotFoundMessage;

                    return result;
                }
            }
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> GetAppUsers(int id, int requestorId, IPaginator paginator, bool appUsers = true)
        {
            if (paginator == null) throw new ArgumentNullException(nameof(paginator));

            var result = new Result();

            if (id == 0)
            {
                result.IsSuccess = false;
                result.Message = AppsMessages.AppNotFoundMessage;

                return result;
            }

            if (requestorId == 0)
            {
                result.IsSuccess = false;
                result.Message = UsersMessages.UserNotFoundMessage;
            }

            try
            {
                var cacheFactoryResponse = await CacheFactory.GetWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(CacheKeys.GetAppCacheKey, id),
                    CachingStrategy.Medium,
                    id);

                var app = (App)((RepositoryResponse)cacheFactoryResponse.Item1).Object;

                if (app != null)
                {
                    RepositoryResponse response;

                    if (appUsers)
                    {
                        cacheFactoryResponse = await CacheFactory.GetAppUsersWithCacheAsync(
                            _appsRepository,
                            _distributedCache,
                            string.Format(string.Format(CacheKeys.GetAppUsersCacheKey, id)),
                            CachingStrategy.Light,
                            id,
                            result);
                    }
                    else
                    {
                        cacheFactoryResponse = await CacheFactory.GetNonAppUsersWithCacheAsync(
                            _appsRepository,
                            _distributedCache,
                            string.Format(string.Format(CacheKeys.GetNonAppUsersCacheKey, id)),
                            CachingStrategy.Light,
                            id,
                            result);
                    }

                    response = (RepositoryResponse)cacheFactoryResponse.Item1;
                    result = (Result)cacheFactoryResponse.Item2;

                    if (response.Success)
                    {
                        result = PaginatorUtilities.PaginateUsers(paginator, response, result);

                        if (result.Message.Equals(
                            ServicesMesages.SortValueNotImplementedMessage))
                        {
                            return result;
                        }

                        var requestor = (User)(await _usersRepository.Get(requestorId)).Object;

                        if (requestor != null && !requestor.IsSuperUser)
                        {
                            // Filter out user emails from the frontend...
                            foreach (var user in result.DataPacket)
                            {
                                var emailConfirmed = ((IUser)user).IsEmailConfirmed;
                                ((IUser)user).HideEmail();
                                ((IUser)user).IsEmailConfirmed = emailConfirmed;
                            }
                        }

                        result.IsSuccess = response.Success;
                        result.Message = UsersMessages.UsersFoundMessage;

                        return result;
                    }
                    else if (!response.Success && response.Exception != null)
                    {
                        result.IsSuccess = response.Success;
                        result.Message = response.Exception.Message;

                        return result;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = UsersMessages.UsersNotFoundMessage;

                        return result;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = AppsMessages.AppNotFoundMessage;

                    return result;
                }
            }
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> AddAppUser(int appId, int userId)
        {
            var result = new Result();

            if (appId == 0)
            {
                result.IsSuccess = false;
                result.Message = AppsMessages.AppNotFoundMessage;

                return result;
            }

            if (userId == 0)
            {
                result.IsSuccess = false;
                result.Message = UsersMessages.UserNotFoundMessage;

                return result;
            }

            try
            {
                var cacheFactoryResponse = await CacheFactory.GetWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(CacheKeys.GetAppCacheKey, appId),
                    CachingStrategy.Medium,
                    appId);

                var appResponse = (RepositoryResponse)cacheFactoryResponse.Item1;

                if (appResponse.Success)
                {
                    var app = (App)appResponse.Object;

                    app.License = (await CacheFactory.GetLicenseWithCacheAsync(
                        _appsRepository,
                        _distributedCache,
                        string.Format(CacheKeys.GetAppLicenseCacheKey, app.Id),
                        CachingStrategy.Heavy,
                        app.Id)).Item1;

                    cacheFactoryResponse = await CacheFactory.GetWithCacheAsync(
                        _usersRepository,
                        _distributedCache,
                        string.Format(CacheKeys.GetUserCacheKey, userId, app.License),
                        CachingStrategy.Medium,
                        userId);

                    var userResponse = (RepositoryResponse)cacheFactoryResponse.Item1;

                    if (userResponse.Success)
                    {
                        var addUserToAppResponse = await _appsRepository.AddAppUser(
                            userId,
                            app.License);

                        if (addUserToAppResponse.Success)
                        {
                            // Remove any cache items which may exist
                            var cacheKeys = new List<string> {
                                string.Format(CacheKeys.GetAppCacheKey, app.Id),
                                string.Format(CacheKeys.GetAppByLicenseCacheKey, app.License),
                                string.Format(CacheKeys.GetAppUsersCacheKey, app.Id),
                                string.Format(CacheKeys.GetNonAppUsersCacheKey, app.Id),
                                string.Format(CacheKeys.GetMyAppsCacheKey, userId),
                                string.Format(CacheKeys.GetMyRegisteredCacheKey, userId)
                            };

                            await CacheFactory.RemoveKeysAsync(_distributedCache, cacheKeys);

                            result.IsSuccess = addUserToAppResponse.Success;
                            result.Message = AppsMessages.UserAddedToAppMessage;

                            return result;
                        }
                        else if (!addUserToAppResponse.Success && addUserToAppResponse.Exception != null)
                        {
                            result.IsSuccess = addUserToAppResponse.Success;
                            result.Message = addUserToAppResponse.Exception.Message;

                            return result;
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Message = AppsMessages.UserNotAddedToAppMessage;

                            return result;
                        }
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = UsersMessages.UserNotFoundMessage;

                        return result;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = AppsMessages.AppNotFoundMessage;

                    return result;
                }
            }
            catch (Exception exp)
            {
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> RemoveAppUser(int appId, int userId)
        {
            var result = new Result();

            if (appId == 0)
            {
                result.IsSuccess = false;
                result.Message = AppsMessages.AppNotFoundMessage;

                return result;
            }

            if (userId == 0)
            {
                result.IsSuccess = false;
                result.Message = UsersMessages.UserNotFoundMessage;

                return result;
            }

            try
            {
                var cacheFactoryResponse = await CacheFactory.GetWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(CacheKeys.GetAppCacheKey, appId),
                    CachingStrategy.Medium,
                    appId);

                var appResponse = (RepositoryResponse)cacheFactoryResponse.Item1;

                if (appResponse.Success)
                {
                    if (await CacheFactory.HasEntityWithCacheAsync(
                        _usersRepository,
                        _distributedCache,
                        string.Format(CacheKeys.HasUserCacheKey, userId),
                        CachingStrategy.Heavy,
                        userId))
                    {
                        var app = (App)appResponse.Object;

                        app.License = (await CacheFactory.GetLicenseWithCacheAsync(
                            _appsRepository,
                            _distributedCache,
                            string.Format(CacheKeys.GetAppLicenseCacheKey, app.Id),
                            CachingStrategy.Heavy,
                            app.Id)).Item1;

                        if (app.OwnerId == userId)
                        {
                            result.IsSuccess = false;
                            result.Message = AppsMessages.UserIsTheAppOwnerMessage;

                            return result;
                        }

                        var addUserToAppResponse = await _appsRepository.RemoveAppUser(
                            userId,
                            app.License);

                        if (addUserToAppResponse.Success)
                        {
                            // Remove any app cache items which may exist
                            var cacheKeys = new List<string> {
                                string.Format(CacheKeys.GetAppCacheKey, app.Id),
                                string.Format(CacheKeys.GetAppByLicenseCacheKey, app.License),
                                string.Format(CacheKeys.GetAppUsersCacheKey, app.Id),
                                string.Format(CacheKeys.GetNonAppUsersCacheKey, app.Id),
                                string.Format(CacheKeys.GetMyAppsCacheKey, userId),
                                string.Format(CacheKeys.GetMyRegisteredCacheKey, userId)
                            };

                            await CacheFactory.RemoveKeysAsync(_distributedCache, cacheKeys);

                            result.IsSuccess = addUserToAppResponse.Success;
                            result.Message = AppsMessages.UserRemovedFromAppMessage;

                            return result;
                        }
                        else if (!addUserToAppResponse.Success && addUserToAppResponse.Exception != null)
                        {
                            result.IsSuccess = addUserToAppResponse.Success;
                            result.Message = addUserToAppResponse.Exception.Message;

                            return result;
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Message = AppsMessages.UserNotRemovedFromAppMessage;

                            return result;
                        }
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = UsersMessages.UserNotFoundMessage;

                        return result;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = AppsMessages.AppNotFoundMessage;

                    return result;
                }
            }
            catch (Exception exp)
            {
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> ActivateAdminPrivileges(int appId, int userId)
        {
            var result = new Result();

            if (appId == 0)
            {
                result.IsSuccess = false;
                result.Message = AppsMessages.AppNotFoundMessage;

                return result;
            }

            if (userId == 0)
            {
                result.IsSuccess = false;
                result.Message = UsersMessages.UserNotFoundMessage;

                return result;
            }

            try
            {
                var cacheFactoryResponse = await CacheFactory.GetWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(CacheKeys.GetAppCacheKey, appId),
                    CachingStrategy.Medium,
                    appId);

                var appResponse = (RepositoryResponse)cacheFactoryResponse.Item1;

                if (appResponse.Success)
                {
                    var app = (App)appResponse.Object;
                    app.License = (await CacheFactory.GetLicenseWithCacheAsync(
                        _appsRepository,
                        _distributedCache,
                        string.Format(CacheKeys.GetAppLicenseCacheKey, appId),
                        CachingStrategy.Medium,
                        appId)).Item1;

                    cacheFactoryResponse = await CacheFactory.GetWithCacheAsync(
                        _usersRepository,
                        _distributedCache,
                        string.Format(CacheKeys.GetUserCacheKey, userId, app.License),
                        CachingStrategy.Medium,
                        userId);

                    var userReponse = (RepositoryResponse)cacheFactoryResponse.Item1;

                    if (userReponse.Success)
                    {

                        var user = (User)userReponse.Object;

                        if (user.IsSuperUser)
                        {
                            result.IsSuccess = false;
                            result.Message = UsersMessages.SuperUserCannotBePromotedMessage;

                            return result;
                        }

                        if (!await _appsRepository.IsUserRegisteredToApp(
                            app.Id, 
                            app.License, 
                            user.Id))
                        {
                            _ = await _appsRepository.AddAppUser(user.Id, app.License);
                        }
                        else
                        {
                            if (await _appAdminsRepository.HasAdminRecord(app.Id, user.Id))
                            {
                                var adminRecord = (AppAdmin)(await _appAdminsRepository
                                    .GetAdminRecord(app.Id, user.Id)).Object;

                                if (adminRecord.IsActive)
                                {
                                    result.IsSuccess = false;
                                    result.Message = UsersMessages.UserIsAlreadyAnAdminMessage;

                                    return result;
                                }
                                else
                                {
                                    adminRecord.IsActive = true;

                                    var adminRecordUpdateResult = await _appAdminsRepository
                                        .Update(adminRecord);

                                    var cacheKeys = new List<string> {
                                        string.Format(CacheKeys.GetAppCacheKey, app.Id),
                                        string.Format(CacheKeys.GetAppByLicenseCacheKey, app.License),
                                        string.Format(CacheKeys.GetAppUsersCacheKey, app.Id),
                                        string.Format(CacheKeys.GetNonAppUsersCacheKey, app.Id),
                                        string.Format(CacheKeys.GetAppCacheKey, user.Apps.ToList()[0].AppId),
                                        CacheKeys.GetUsersCacheKey
                                    };

                                    foreach (var key in cacheKeys)
                                    {
                                        if (await _distributedCache.GetAsync(key) != null)
                                        {
                                            await _distributedCache.RemoveAsync(string.Format(key));
                                        }
                                    }

                                    result.IsSuccess = adminRecordUpdateResult.Success;
                                    result.Message = UsersMessages.UserHasBeenPromotedToAdminMessage;

                                    return result;
                                }
                            }
                        }


                        if (!user.IsAdmin)
                        {
                            var adminRole = (await _rolesRepository.GetAll())
                                .Objects
                                .ConvertAll(r => (Role)r)
                                .FirstOrDefault(r => r.RoleLevel == RoleLevel.ADMIN);

                            user.Roles.Add(new UserRole {
                                UserId = user.Id,
                                User = user,
                                RoleId = adminRole.Id,
                                Role = adminRole}) ;

                            user = (User)(await _usersRepository.Update(user)).Object;
                        }

                        var appAdmin = new AppAdmin(app.Id, user.Id);

                        var appAdminResult = await _appAdminsRepository.Add(appAdmin);

                        if (appAdminResult.Success)
                        {
                            result.IsSuccess = appAdminResult.Success;
                            result.Message = UsersMessages.UserHasBeenPromotedToAdminMessage;
                            result.DataPacket.Add(
                                (await _usersRepository.Get(userId))
                                    .Object);

                            return result;
                        }
                        else if (!appAdminResult.Success && appAdminResult.Exception != null)
                        {
                            result.IsSuccess = appAdminResult.Success;
                            result.Message = appAdminResult.Exception.Message;

                            return result;
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Message = UsersMessages.UserHasNotBeenPromotedToAdminMessage;

                            return result;
                        }
                    }
                    else if (!userReponse.Success && userReponse.Exception != null)
                    {
                        result.IsSuccess = userReponse.Success;
                        result.Message = userReponse.Exception.Message;

                        return result;
                    }
                    else
                    {
                        result.IsSuccess = userReponse.Success;
                        result.Message = UsersMessages.UserNotFoundMessage;

                        return result;
                    }
                }
                else if (!appResponse.Success && appResponse.Exception != null)
                {
                    result.IsSuccess = appResponse.Success;
                    result.Message = appResponse.Exception.Message;

                    return result;
                }
                else
                {
                    result.IsSuccess = appResponse.Success;
                    result.Message = AppsMessages.AppNotFoundMessage;

                    return result;
                }
            }
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> DeactivateAdminPrivileges(int appId, int userId)
        {
            var result = new Result();

            if (appId == 0)
            {
                result.IsSuccess = false;
                result.Message = AppsMessages.AppNotFoundMessage;

                return result;
            }

            if (userId == 0)
            {
                result.IsSuccess = false;
                result.Message = UsersMessages.UserNotFoundMessage;

                return result;
            }

            try
            {
                var cacheFactoryResponse = await CacheFactory.GetWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(CacheKeys.GetAppCacheKey, appId),
                    CachingStrategy.Medium,
                    appId);

                var appResponse = (RepositoryResponse)cacheFactoryResponse.Item1;

                if (appResponse.Success)
                {
                    var app = (App)appResponse.Object;
                    app.License = (await CacheFactory.GetLicenseWithCacheAsync(
                        _appsRepository,
                        _distributedCache,
                        string.Format(CacheKeys.GetAppLicenseCacheKey, appId),
                        CachingStrategy.Medium,
                        appId)).Item1;

                    cacheFactoryResponse = await CacheFactory.GetWithCacheAsync(
                        _usersRepository,
                        _distributedCache,
                        string.Format(CacheKeys.GetAppCacheKey, userId, app.License),
                        CachingStrategy.Medium,
                        userId);

                    var userResponse = (RepositoryResponse)cacheFactoryResponse.Item1;

                    if (userResponse.Success)
                    {

                        var user = (User)userResponse.Object;

                        if (!user.IsAdmin)
                        {
                            result.IsSuccess = false;
                            result.Message = UsersMessages.UserDoesNotHaveAdminPrivilegesMessage;

                            return result;
                        }

                        if (!await _appAdminsRepository.HasAdminRecord(app.Id, user.Id))
                        {
                            result.IsSuccess = false;
                            result.Message = AppsMessages.UserIsNotAnAssignedAdminMessage;

                            return result;
                        }

                        var appAdmin = (AppAdmin)
                            (await _appAdminsRepository.GetAdminRecord(app.Id, user.Id))
                            .Object;

                        appAdmin.IsActive = false;

                        var appAdminResult = await _appAdminsRepository.Update(appAdmin);

                        if (appAdminResult.Success)
                        {
                            var cacheKeys = new List<string> {
                                    string.Format(CacheKeys.GetAppCacheKey, app.Id),
                                    string.Format(CacheKeys.GetAppByLicenseCacheKey, app.License),
                                    string.Format(CacheKeys.GetAppUsersCacheKey, app.Id),
                                    string.Format(CacheKeys.GetNonAppUsersCacheKey, app.Id),
                                    string.Format(CacheKeys.GetAppCacheKey, user.Apps.ToList()[0].AppId),
                                    CacheKeys.GetUsersCacheKey
                                };

                            foreach (var key in cacheKeys)
                            {
                                if (await _distributedCache.GetAsync(key) != null)
                                {
                                    await _distributedCache.RemoveAsync(string.Format(key));
                                }
                            }

                            result.IsSuccess = appAdminResult.Success;
                            result.Message = AppsMessages.AdminPrivilegesDeactivatedMessage;
                            result.DataPacket.Add(
                                (await _usersRepository.Get(user.Id))
                                .Object);

                            return result;
                        }
                        else if (!appAdminResult.Success && appAdminResult.Exception != null)
                        {
                            result.IsSuccess = appAdminResult.Success;
                            result.Message = appAdminResult.Exception.Message;

                            return result;
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Message = AppsMessages.DeactivationOfAdminPrivilegesFailedMessage;

                            return result;
                        }
                    }
                    else if (!userResponse.Success && userResponse.Exception != null)
                    {
                        result.IsSuccess = userResponse.Success;
                        result.Message = userResponse.Exception.Message;

                        return result;
                    }
                    else
                    {
                        result.IsSuccess = userResponse.Success;
                        result.Message = UsersMessages.UserNotFoundMessage;

                        return result;
                    }
                }
                else if (!appResponse.Success && appResponse.Exception != null)
                {
                    result.IsSuccess = appResponse.Success;
                    result.Message = appResponse.Exception.Message;

                    return result;
                }
                else
                {
                    result.IsSuccess = appResponse.Success;
                    result.Message = AppsMessages.AppNotFoundMessage;

                    return result;
                }
            }
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> Activate(int id)
        {
            var result = new Result();

            if (id == 0)
            {
                result.IsSuccess = false;
                result.Message = AppsMessages.AppNotFoundMessage;

                return result;
            }

            try
            {
                var activateAppResponse = await CacheFactory.ActivatetWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    id);

                if (activateAppResponse.Success)
                {
                    result.IsSuccess = activateAppResponse.Success;
                    result.Message = AppsMessages.AppActivatedMessage;

                    return result;
                }
                else if (!activateAppResponse.Success && activateAppResponse.Exception != null)
                {
                    result.IsSuccess = activateAppResponse.Success;
                    result.Message = activateAppResponse.Exception.Message;

                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = AppsMessages.AppNotActivatedMessage;

                    return result;
                }
            }
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> Deactivate(int id)
        {
            var result = new Result();

            if (id == 0)
            {
                result.IsSuccess = false;
                result.Message = AppsMessages.AppNotFoundMessage;

                return result;
            }

            try
            {
                var activateAppResponse = await CacheFactory.DeactivatetWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    id);

                if (activateAppResponse.Success)
                {
                    result.IsSuccess = activateAppResponse.Success;
                    result.Message = AppsMessages.AppDeactivatedMessage;

                    return result;
                }
                else if (!activateAppResponse.Success && activateAppResponse.Exception != null)
                {
                    result.IsSuccess = activateAppResponse.Success;
                    result.Message = activateAppResponse.Exception.Message;

                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = AppsMessages.AppNotDeactivatedMessage;

                    return result;
                }
            }
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<ILicenseResult> GetLicense(int id)
        {
            var result = new LicenseResult();

            if (id == 0)
            {
                result.IsSuccess = false;
                result.Message = AppsMessages.AppNotFoundMessage;

                return result;
            }

            try
            {
                if (await CacheFactory.HasEntityWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(CacheKeys.HasAppCacheKey, id),
                    CachingStrategy.Heavy,
                    id))
                {
                    var response = await CacheFactory.GetLicenseWithCacheAsync(
                        _appsRepository,
                        _distributedCache,
                        string.Format(CacheKeys.GetAppLicenseCacheKey, id),
                        CachingStrategy.Heavy,
                        id,
                        result);

                    result.IsSuccess = true;
                    result.IsFromCache = response.Item2.IsFromCache;
                    result.Message = AppsMessages.AppFoundMessage;
                    result.License = response.Item1;

                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = AppsMessages.AppNotFoundMessage;

                    return result;
                }
            }
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<bool> IsOwnerOfThisLicense(int id, string license, int userId)
        {
            if (string.IsNullOrEmpty(license)) throw new ArgumentNullException(nameof(license));

            if (id == 0 || userId == 0)
            {
                return false;
            }

            try
            {
                var cacheFactoryResponse = await CacheFactory.GetWithCacheAsync(
                    _usersRepository,
                    _distributedCache,
                    string.Format(CacheKeys.GetUserCacheKey, userId, license),
                    CachingStrategy.Medium,
                    userId);

                var userResponse = (RepositoryResponse)cacheFactoryResponse.Item1;

                var validLicense = await CacheFactory.IsAppLicenseValidWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(CacheKeys.IsAppLicenseValidCacheKey, license),
                    CachingStrategy.Heavy,
                    license);

                if (userResponse.Success && validLicense)
                {
                    var requestorOwnerOfThisApp = await _appsRepository.IsUserOwnerOfApp(id, license, userId);

                    if (requestorOwnerOfThisApp && validLicense)
                    {
                        return true;
                    }
                    else if (((User)userResponse.Object).IsSuperUser && validLicense)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> IsRequestValidOnThisLicense(int id, string license, int userId)
        {
            if (string.IsNullOrEmpty(license)) throw new ArgumentNullException(nameof(license));

            if (id == 0 || userId == 0)
            {
                return false;
            }

            try
            {
                var cacheFactoryResponse = await CacheFactory.GetWithCacheAsync(
                    _usersRepository,
                    _distributedCache,
                    string.Format(CacheKeys.GetUserCacheKey, userId, license),
                    CachingStrategy.Medium,
                    userId);

                var userResponse = (RepositoryResponse)cacheFactoryResponse.Item1;

                cacheFactoryResponse = await CacheFactory.GetWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(CacheKeys.GetAppCacheKey, id),
                    CachingStrategy.Medium,
                    id);

                var appResponse = (RepositoryResponse)cacheFactoryResponse.Item1;

                var validLicense = await CacheFactory.IsAppLicenseValidWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(CacheKeys.IsAppLicenseValidCacheKey, license),
                    CachingStrategy.Heavy,
                    license);

                if (userResponse.Success && appResponse.Success && validLicense)
                {
                    bool userPermittedAccess;

                    if (!((App)appResponse.Object).PermitCollectiveLogins)
                    {
                        userPermittedAccess = await _appsRepository
                            .IsUserRegisteredToApp(id, license, userId);
                    }
                    else
                    {
                        userPermittedAccess = true;
                    }

                    if (userPermittedAccess && validLicense)
                    {
                        if (((App)appResponse.Object).IsActive)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (((User)userResponse.Object).IsSuperUser && validLicense)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
