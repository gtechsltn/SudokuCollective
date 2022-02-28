using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Caching.Distributed;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Cache;
using SudokuCollective.Core.Interfaces.ServiceModels;
using SudokuCollective.Core.Interfaces.Models.LoginModels;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Results;

namespace SudokuCollective.Data.Services
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly IUsersRepository<User> _usersRepository;
        private readonly IRolesRepository<Role> _rolesRepository;
        private readonly IAppsRepository<App> _appsRepository;
        private readonly IAppAdminsRepository<AppAdmin> _appAdminsRepository;
        private readonly IUserManagementService _userManagementService;
        private readonly ITokenManagement _tokenManagement;
        private readonly IDistributedCache _distributedCache;
        private readonly ICacheService _cacheService;
        private readonly ICacheKeys _cacheKeys;
        private readonly ICachingStrategy _cachingStrategy;

        public AuthenticateService(
            IUsersRepository<User> usersRepository,
            IRolesRepository<Role> rolesRepository,
            IAppsRepository<App> appsRepository,
            IAppAdminsRepository<AppAdmin> appsAdminRepository,
            IUserManagementService userManagementService,
            ITokenManagement tokenManagement,
            IDistributedCache distributedCache,
            ICacheService cacheService,
            ICacheKeys cacheKeys,
            ICachingStrategy cachingStrategy)
        {
            _usersRepository = usersRepository;
            _rolesRepository = rolesRepository;
            _appsRepository = appsRepository;
            _appAdminsRepository = appsAdminRepository;
            _userManagementService = userManagementService;
            _tokenManagement = tokenManagement;
            _distributedCache = distributedCache;
            _cacheService = cacheService;
            _cacheKeys = cacheKeys;
            _cachingStrategy = cachingStrategy;
        }
        
        public async Task<IResult> IsAuthenticated(ILoginRequest request)
        {
            try
            {
                if (request == null) throw new ArgumentNullException(nameof(request));

                var result = new Result();

                var validateUserTask = _userManagementService.IsValidUser(request.UserName, request.Password);

                validateUserTask.Wait();

                if (!validateUserTask.Result)
                {
                    result.IsSuccess = false;
                    result.Message = UsersMessages.UserNotFoundMessage;

                    return result;
                }

                var userResponse = await _cacheService.GetByUserNameWithCacheAsync(
                    _usersRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetUserByUsernameCacheKey, request.UserName, request.License),
                    _cachingStrategy.Medium,
                    _cacheKeys,
                    request.UserName,
                    request.License,
                    result);

                var user = (User)((RepositoryResponse)userResponse.Item1).Object;
                result = (Result)userResponse.Item2;

                var appResponse = await _cacheService.GetAppByLicenseWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetAppByLicenseCacheKey, request.License),
                    _cachingStrategy.Medium,
                    request.License);

                var app = (App)((RepositoryResponse)appResponse.Item1).Object;

                if (!app.IsActive)
                {
                    result.IsSuccess = false;
                    result.Message = AppsMessages.AppDeactivatedMessage;

                    return result;
                }

                if (!app.PermitCollectiveLogins && !app.Users.Any(ua => ua.UserId == user.Id))
                {
                    result.IsSuccess = false;
                    result.Message = AppsMessages.UserIsNotARegisteredUserOfThisAppMessage;

                    return result;
                }

                var appAdmins = (await _appAdminsRepository.GetAll()).Objects.ConvertAll(aa => (AppAdmin)aa);

                if (!user.IsSuperUser)
                {
                    if (user.Roles.Any(ur => ur.Role.RoleLevel == RoleLevel.ADMIN))
                    {
                        if (!appAdmins.Any(aa => aa.AppId == app.Id && aa.UserId == user.Id && aa.IsActive))
                        {
                            var adminRole = user
                                .Roles
                                .FirstOrDefault(ur => ur.Role.RoleLevel == RoleLevel.ADMIN);

                            user.Roles.Remove(adminRole);
                        }
                    }
                }
                else
                {
                    if (!app.PermitSuperUserAccess)
                    {
                        if (user.Roles.Any(ur => ur.Role.RoleLevel == RoleLevel.SUPERUSER))
                        {
                            var superUserRole = user
                                .Roles
                                .FirstOrDefault(ur => ur.Role.RoleLevel == RoleLevel.SUPERUSER);

                            user.Roles.Remove(superUserRole);
                        }

                        if (user.Roles.Any(ur => ur.Role.RoleLevel == RoleLevel.ADMIN))
                        {
                            var adminRole = user
                                .Roles
                                .FirstOrDefault(ur => ur.Role.RoleLevel == RoleLevel.ADMIN);

                            user.Roles.Remove(adminRole);
                        }
                    }
                }

                var authenticatedUser = new AuthenticatedUser();

                authenticatedUser.UpdateWithUserInfo(user);

                var authenticationResult = new AuthenticationResult
                {
                    User = authenticatedUser
                };

                var claim = new List<Claim> {

                    new Claim(ClaimTypes.Name, request.UserName)
                };

                foreach (var role in user.Roles)
                {
                    var r = (Role)(await _rolesRepository.Get(role.Role.Id)).Object;

                    claim.Add(new Claim(ClaimTypes.Role, r.RoleLevel.ToString()));
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenManagement.Secret));

                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                DateTime expirationLimit;

                if (app.TimeFrame == TimeFrame.SECONDS)
                {
                    expirationLimit = DateTime.UtcNow.AddSeconds(app.AccessDuration);
                }
                else if (app.TimeFrame == TimeFrame.MINUTES)
                {
                    expirationLimit = DateTime.UtcNow.AddMinutes(app.AccessDuration);
                }
                else if (app.TimeFrame == TimeFrame.HOURS)
                {
                    expirationLimit = DateTime.UtcNow.AddHours(app.AccessDuration);
                }
                else if (app.TimeFrame == TimeFrame.DAYS)
                {
                    expirationLimit = DateTime.UtcNow.AddDays(app.AccessDuration);
                }
                else
                {
                    expirationLimit = DateTime.UtcNow.AddMonths(app.AccessDuration);
                }

                var jwtToken = new JwtSecurityToken(
                        _tokenManagement.Issuer,
                        _tokenManagement.Audience,
                        claim.ToArray(),
                        notBefore: DateTime.UtcNow,
                        expires: expirationLimit,
                        signingCredentials: credentials
                    );

                authenticationResult.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

                result.IsSuccess = true;
                result.Message = UsersMessages.UserFoundMessage;
                result.Payload.Add(authenticationResult);

                return result;
            }
            catch
            {
                throw;
            }
        }
    }
}
