using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Cache;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Payloads;
using SudokuCollective.Data.Models.Results;
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
        private readonly ICacheService _cacheService;
        private readonly ICacheKeys _cacheKeys;
        private readonly ICachingStrategy _cachingStrategy;
        #endregion

        #region Constructor
        public AppsService(
            IAppsRepository<App> appRepository, 
            IUsersRepository<User> userRepository,
            IAppAdminsRepository<AppAdmin> appAdminsRepository,
            IRolesRepository<Role> rolesRepository,
            IDistributedCache distributedCache,
            ICacheService cacheService,
            ICacheKeys cacheKeys,
            ICachingStrategy cachingStrategy)
        {
            _appsRepository = appRepository;
            _usersRepository = userRepository;
            _appAdminsRepository = appAdminsRepository;
            _rolesRepository = rolesRepository;
            _distributedCache = distributedCache;
            _cacheService = cacheService;
            _cacheKeys = cacheKeys;
            _cachingStrategy = cachingStrategy;
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
                    var cacheServiceResponse = await _cacheService.GetAllWithCacheAsync(
                        _appsRepository,
                        _distributedCache,
                        string.Format(_cacheKeys.GetAppsCacheKey),
                        _cachingStrategy.Medium);

                    var checkAppsResponse = (RepositoryResponse)cacheServiceResponse.Item1;

                    foreach (var a in checkAppsResponse.Objects.ConvertAll(a => (App)a))
                    {
                        a.License = (await _cacheService.GetLicenseWithCacheAsync(
                            _appsRepository,
                            _distributedCache,
                            string.Format(_cacheKeys.GetAppLicenseCacheKey, a.Id),
                            _cachingStrategy.Heavy,
                            _cacheKeys,
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

                    var addAppResponse = await _cacheService.AddWithCacheAsync(
                        _appsRepository,
                        _distributedCache,
                        _cacheKeys.GetAppCacheKey,
                        _cachingStrategy.Medium,
                        _cacheKeys,
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
                        result.Payload.Add((App)addAppResponse.Object);

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
                var cacheServiceResponse = await _cacheService.GetWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetAppCacheKey, id),
                    _cachingStrategy.Medium,
                    id,
                    result);

                var response = (RepositoryResponse)cacheServiceResponse.Item1;
                result = (Result)cacheServiceResponse.Item2;

                if (response.Success)
                {
                    var app = (App)response.Object;

                    result.IsSuccess = response.Success;
                    result.Message = AppsMessages.AppFoundMessage;
                    result.Payload.Add(app);

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
                var cacheServiceResponse = await _cacheService.GetAppByLicenseWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetAppByLicenseCacheKey, license),
                    _cachingStrategy.Medium,
                    license,
                    result);

                var response = (RepositoryResponse)cacheServiceResponse.Item1;
                result = (Result)cacheServiceResponse.Item2;

                if (response.Success)
                {
                    var app = (IApp)response.Object;

                    result.IsSuccess = response.Success;
                    result.Message = AppsMessages.AppFoundMessage;
                    result.Payload.Add(app);

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
                var cacheServiceResponse = await _cacheService.GetAllWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetAppsCacheKey),
                    _cachingStrategy.Medium,
                    result);

                var response = (RepositoryResponse)cacheServiceResponse.Item1;
                result = (Result)cacheServiceResponse.Item2;

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
                var cacheServiceResponse = await _cacheService.GetMyAppsWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(string.Format(_cacheKeys.GetMyAppsCacheKey, ownerId)),
                    _cachingStrategy.Medium,
                    ownerId,
                    result);

                var response = (RepositoryResponse)cacheServiceResponse.Item1;
                result = (Result)cacheServiceResponse.Item2;

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
                var cacheServiceResponse = await _cacheService.GetMyRegisteredAppsWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(string.Format(_cacheKeys.GetMyRegisteredCacheKey, userId)),
                    _cachingStrategy.Medium,
                    userId,
                    result);

                var response = (RepositoryResponse)cacheServiceResponse.Item1;
                result = (Result)cacheServiceResponse.Item2;

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

            AppPayload payload;

            if (request.Payload is AppPayload r)
            {
                payload = r;
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

                        app.Name = payload.Name;
                        app.DevUrl = payload.DevUrl;
                        app.ProdUrl = payload.ProdUrl;
                        app.IsActive = payload.IsActive;
                        app.Environment = payload.Environment;
                        app.PermitSuperUserAccess = payload.PermitSuperUserAccess;
                        app.PermitCollectiveLogins = payload.PermitCollectiveLogins;
                        app.DisableCustomUrls = payload.DisableCustomUrls;
                        app.CustomEmailConfirmationAction = payload.CustomEmailConfirmationAction;
                        app.CustomPasswordResetAction = payload.CustomPasswordResetAction;
                        app.TimeFrame = payload.TimeFrame;
                        app.AccessDuration = payload.AccessDuration;
                        app.DateUpdated = DateTime.UtcNow;

                        var updateAppResponse = await _cacheService.UpdateWithCacheAsync<App>(
                            _appsRepository,
                            _distributedCache,
                            _cacheKeys,
                            app);

                        if (updateAppResponse.Success)
                        {
                            result.IsSuccess = true;
                            result.Message = AppsMessages.AppUpdatedMessage;
                            result.Payload.Add(app);

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
                            var resetAppResponse = await _cacheService.ResetWithCacheAsync(
                                _appsRepository,
                                _distributedCache,
                                _cacheKeys,
                                (App)getAppResponse.Object);

                            if (resetAppResponse.Success)
                            {
                                result.IsSuccess = resetAppResponse.Success;
                                result.Message = AppsMessages.AppResetMessage;
                                result.Payload.Add(resetAppResponse.Object);

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
                            var deleteAppResponse = await _cacheService.DeleteWithCacheAsync(
                                _appsRepository,
                                _distributedCache,
                                _cacheKeys,
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
                var cacheServiceResponse = await _cacheService.GetWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetAppCacheKey, id),
                    _cachingStrategy.Medium,
                    id);

                var app = (App)((RepositoryResponse)cacheServiceResponse.Item1).Object;

                if (app != null)
                {
                    RepositoryResponse response;

                    if (appUsers)
                    {
                        cacheServiceResponse = await _cacheService.GetAppUsersWithCacheAsync(
                            _appsRepository,
                            _distributedCache,
                            string.Format(string.Format(_cacheKeys.GetAppUsersCacheKey, id)),
                            _cachingStrategy.Light,
                            id,
                            result);
                    }
                    else
                    {
                        cacheServiceResponse = await _cacheService.GetNonAppUsersWithCacheAsync(
                            _appsRepository,
                            _distributedCache,
                            string.Format(string.Format(_cacheKeys.GetNonAppUsersCacheKey, id)),
                            _cachingStrategy.Light,
                            id,
                            result);
                    }

                    response = (RepositoryResponse)cacheServiceResponse.Item1;
                    result = (Result)cacheServiceResponse.Item2;

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
                            foreach (var user in result.Payload)
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
                var cacheServiceResponse = await _cacheService.GetWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetAppCacheKey, appId),
                    _cachingStrategy.Medium,
                    appId);

                var appResponse = (RepositoryResponse)cacheServiceResponse.Item1;

                if (appResponse.Success)
                {
                    var app = (App)appResponse.Object;

                    app.License = (await _cacheService.GetLicenseWithCacheAsync(
                        _appsRepository,
                        _distributedCache,
                        string.Format(_cacheKeys.GetAppLicenseCacheKey, app.Id),
                        _cachingStrategy.Heavy,
                        _cacheKeys,
                        app.Id)).Item1;

                    cacheServiceResponse = await _cacheService.GetWithCacheAsync(
                        _usersRepository,
                        _distributedCache,
                        string.Format(_cacheKeys.GetUserCacheKey, userId, app.License),
                        _cachingStrategy.Medium,
                        userId);

                    var userResponse = (RepositoryResponse)cacheServiceResponse.Item1;

                    if (userResponse.Success)
                    {
                        var addUserToAppResponse = await _appsRepository.AddAppUser(
                            userId,
                            app.License);

                        if (addUserToAppResponse.Success)
                        {
                            // Remove any cache items which may exist
                            var removeKeys = new List<string> {
                                string.Format(_cacheKeys.GetAppCacheKey, app.Id),
                                string.Format(_cacheKeys.GetAppByLicenseCacheKey, app.License),
                                string.Format(_cacheKeys.GetAppUsersCacheKey, app.Id),
                                string.Format(_cacheKeys.GetNonAppUsersCacheKey, app.Id),
                                string.Format(_cacheKeys.GetMyAppsCacheKey, userId),
                                string.Format(_cacheKeys.GetMyRegisteredCacheKey, userId)
                            };

                            await _cacheService.RemoveKeysAsync(_distributedCache, removeKeys);

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
                var cacheServiceResponse = await _cacheService.GetWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetAppCacheKey, appId),
                    _cachingStrategy.Medium,
                    appId);

                var appResponse = (RepositoryResponse)cacheServiceResponse.Item1;

                if (appResponse.Success)
                {
                    if (await _cacheService.HasEntityWithCacheAsync(
                        _usersRepository,
                        _distributedCache,
                        string.Format(_cacheKeys.HasUserCacheKey, userId),
                        _cachingStrategy.Heavy,
                        userId))
                    {
                        var app = (App)appResponse.Object;

                        app.License = (await _cacheService.GetLicenseWithCacheAsync(
                            _appsRepository,
                            _distributedCache,
                            string.Format(_cacheKeys.GetAppLicenseCacheKey, app.Id),
                            _cachingStrategy.Heavy,
                            _cacheKeys,
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
                            var removeKeys = new List<string> {
                                string.Format(_cacheKeys.GetAppCacheKey, app.Id),
                                string.Format(_cacheKeys.GetAppByLicenseCacheKey, app.License),
                                string.Format(_cacheKeys.GetAppUsersCacheKey, app.Id),
                                string.Format(_cacheKeys.GetNonAppUsersCacheKey, app.Id),
                                string.Format(_cacheKeys.GetMyAppsCacheKey, userId),
                                string.Format(_cacheKeys.GetMyRegisteredCacheKey, userId)
                            };

                            await _cacheService.RemoveKeysAsync(_distributedCache, removeKeys);

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
                var cacheServiceResponse = await _cacheService.GetWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetAppCacheKey, appId),
                    _cachingStrategy.Medium,
                    appId);

                var appResponse = (RepositoryResponse)cacheServiceResponse.Item1;

                if (appResponse.Success)
                {
                    var app = (App)appResponse.Object;
                    app.License = (await _cacheService.GetLicenseWithCacheAsync(
                        _appsRepository,
                        _distributedCache,
                        string.Format(_cacheKeys.GetAppLicenseCacheKey, appId),
                        _cachingStrategy.Medium,
                        _cacheKeys,
                        appId)).Item1;

                    cacheServiceResponse = await _cacheService.GetWithCacheAsync(
                        _usersRepository,
                        _distributedCache,
                        string.Format(_cacheKeys.GetUserCacheKey, userId, app.License),
                        _cachingStrategy.Medium,
                        userId);

                    var userReponse = (RepositoryResponse)cacheServiceResponse.Item1;

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

                                    var removeKeys = new List<string> {
                                        string.Format(_cacheKeys.GetAppCacheKey, app.Id),
                                        string.Format(_cacheKeys.GetAppByLicenseCacheKey, app.License),
                                        string.Format(_cacheKeys.GetAppUsersCacheKey, app.Id),
                                        string.Format(_cacheKeys.GetNonAppUsersCacheKey, app.Id),
                                        string.Format(_cacheKeys.GetAppCacheKey, user.Apps.ToList()[0].AppId),
                                        _cacheKeys.GetUsersCacheKey
                                    };

                                    foreach (var key in removeKeys)
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
                            result.Payload.Add(
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
                var cacheServiceResponse = await _cacheService.GetWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetAppCacheKey, appId),
                    _cachingStrategy.Medium,
                    appId);

                var appResponse = (RepositoryResponse)cacheServiceResponse.Item1;

                if (appResponse.Success)
                {
                    var app = (App)appResponse.Object;
                    app.License = (await _cacheService.GetLicenseWithCacheAsync(
                        _appsRepository,
                        _distributedCache,
                        string.Format(_cacheKeys.GetAppLicenseCacheKey, appId),
                        _cachingStrategy.Medium,
                        _cacheKeys,
                        appId)).Item1;

                    cacheServiceResponse = await _cacheService.GetWithCacheAsync(
                        _usersRepository,
                        _distributedCache,
                        string.Format(_cacheKeys.GetAppCacheKey, userId, app.License),
                        _cachingStrategy.Medium,
                        userId);

                    var userResponse = (RepositoryResponse)cacheServiceResponse.Item1;

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
                            var removeKeys = new List<string> {
                                    string.Format(_cacheKeys.GetAppCacheKey, app.Id),
                                    string.Format(_cacheKeys.GetAppByLicenseCacheKey, app.License),
                                    string.Format(_cacheKeys.GetAppUsersCacheKey, app.Id),
                                    string.Format(_cacheKeys.GetNonAppUsersCacheKey, app.Id),
                                    string.Format(_cacheKeys.GetAppCacheKey, user.Apps.ToList()[0].AppId),
                                    _cacheKeys.GetUsersCacheKey
                                };

                            foreach (var key in removeKeys)
                            {
                                if (await _distributedCache.GetAsync(key) != null)
                                {
                                    await _distributedCache.RemoveAsync(string.Format(key));
                                }
                            }

                            result.IsSuccess = appAdminResult.Success;
                            result.Message = AppsMessages.AdminPrivilegesDeactivatedMessage;
                            result.Payload.Add(
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
                var activateAppResponse = await _cacheService.ActivatetWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    _cacheKeys,
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
                var activateAppResponse = await _cacheService.DeactivatetWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    _cacheKeys,
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
                if (await _cacheService.HasEntityWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.HasAppCacheKey, id),
                    _cachingStrategy.Heavy,
                    id))
                {
                    var response = await _cacheService.GetLicenseWithCacheAsync(
                        _appsRepository,
                        _distributedCache,
                        string.Format(_cacheKeys.GetAppLicenseCacheKey, id),
                        _cachingStrategy.Heavy,
                        _cacheKeys,
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
                var cacheServiceResponse = await _cacheService.GetWithCacheAsync(
                    _usersRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetUserCacheKey, userId, license),
                    _cachingStrategy.Medium,
                    userId);

                var userResponse = (RepositoryResponse)cacheServiceResponse.Item1;

                var validLicense = await _cacheService.IsAppLicenseValidWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.IsAppLicenseValidCacheKey, license),
                    _cachingStrategy.Heavy,
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
                var cacheServiceResponse = await _cacheService.GetWithCacheAsync(
                    _usersRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetUserCacheKey, userId, license),
                    _cachingStrategy.Medium,
                    userId);

                var userResponse = (RepositoryResponse)cacheServiceResponse.Item1;

                cacheServiceResponse = await _cacheService.GetWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetAppCacheKey, id),
                    _cachingStrategy.Medium,
                    id);

                var appResponse = (RepositoryResponse)cacheServiceResponse.Item1;

                var validLicense = await _cacheService.IsAppLicenseValidWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.IsAppLicenseValidCacheKey, license),
                    _cachingStrategy.Heavy,
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
