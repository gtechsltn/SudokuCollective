using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Results;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Requests;
using SudokuCollective.Data.Resiliency;

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
                        request.LiveUrl);

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

        public async Task<IResult> Update(int id, IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

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
                    if (getAppResponse.Success)
                    {
                        ((IApp)getAppResponse.Object).Name = ((AppRequest)request.DataPacket).Name;
                        ((IApp)getAppResponse.Object).DevUrl = ((AppRequest)request.DataPacket).DevUrl;
                        ((IApp)getAppResponse.Object).LiveUrl = ((AppRequest)request.DataPacket).LiveUrl;
                        ((IApp)getAppResponse.Object).IsActive = ((AppRequest)request.DataPacket).IsActive;
                        ((IApp)getAppResponse.Object).Environment = ((AppRequest)request.DataPacket).Environment;
                        ((IApp)getAppResponse.Object).PermitSuperUserAccess = ((AppRequest)request.DataPacket).PermitSuperUserAccess;
                        ((IApp)getAppResponse.Object).PermitCollectiveLogins = ((AppRequest)request.DataPacket).PermitCollectiveLogins;
                        ((IApp)getAppResponse.Object).DisableCustomUrls = ((AppRequest)request.DataPacket).DisableCustomUrls;
                        ((IApp)getAppResponse.Object).CustomEmailConfirmationAction = ((AppRequest)request.DataPacket).CustomEmailConfirmationAction;
                        ((IApp)getAppResponse.Object).CustomPasswordResetAction = ((AppRequest)request.DataPacket).CustomPasswordResetAction;
                        ((IApp)getAppResponse.Object).TimeFrame = ((AppRequest)request.DataPacket).TimeFrame;
                        ((IApp)getAppResponse.Object).AccessDuration = ((AppRequest)request.DataPacket).AccessDuration;
                        ((IApp)getAppResponse.Object).DateUpdated = DateTime.UtcNow;

                        var updateAppResponse = await CacheFactory.UpdateWithCacheAsync(
                            _appsRepository,
                            _distributedCache,
                            (App)getAppResponse.Object);

                        if (updateAppResponse.Success)
                        {
                            result.IsSuccess = true;
                            result.Message = AppsMessages.AppUpdatedMessage;
                            result.DataPacket.Add((App)updateAppResponse.Object);

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

        public Task<IResult> Activate(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<IResult> ActivateAdminPrivileges(int appId, int userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IResult> AddAppUser(int appId, int userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IResult> Deactivate(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<IResult> DeactivateAdminPrivileges(int appId, int userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IResult> DeleteOrReset(int id, bool isReset = false)
        {
            throw new System.NotImplementedException();
        }

        public Task<IResult> GetAppByLicense(string license, int requestorId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IResult> GetApps(IPaginator paginator, int requestorId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IResult> GetAppUsers(int id, int requestorId, IPaginator paginator, bool appUsers = true)
        {
            throw new System.NotImplementedException();
        }

        public Task<ILicenseResult> GetLicense(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<IResult> GetMyApps(int ownerId, IPaginator paginator)
        {
            throw new System.NotImplementedException();
        }

        public Task<IResult> GetRegisteredApps(int userId, IPaginator paginator)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> IsOwnerOfThisLicense(int id, string license, int userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> IsRequestValidOnThisLicense(int id, string license, int userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IResult> RemoveAppUser(int appId, int userId)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
