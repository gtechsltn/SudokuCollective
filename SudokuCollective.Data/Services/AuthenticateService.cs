using System;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SudokuCollective.Core.Interfaces.DataModels;
using SudokuCollective.Core.Interfaces.Models.TokenModels;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models.Authentication;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Data.Resiliency;
using SudokuCollective.Data.Models;
using System.Linq;
using SudokuCollective.Core.Enums;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
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

        public AuthenticateService(
            IUsersRepository<User> usersRepository,
            IRolesRepository<Role> rolesRepository,
            IAppsRepository<App> appsRepository,
            IAppAdminsRepository<AppAdmin> appsAdminRepository,
            IUserManagementService userManagementService,
            IOptions<TokenManagement> tokenManagement,
            IDistributedCache distributedCache)
        {
            _usersRepository = usersRepository;
            _rolesRepository = rolesRepository;
            _appsRepository = appsRepository;
            _appAdminsRepository = appsAdminRepository;
            _userManagementService = userManagementService;
            _tokenManagement = tokenManagement.Value;
            _distributedCache = distributedCache;
        }
        
        public async Task<IResult> IsAuthenticated(ITokenRequest request)
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

                var userResponse = await CacheFactory.GetByUserNameWithCacheAsync(
                    _usersRepository,
                    _distributedCache,
                    string.Format(CacheKeys.GetUserByUsernameCacheKey, request.UserName, request.License),
                    CachingStrategy.Medium,
                    request.UserName,
                    request.License,
                    result);

                var user = (User)((RepositoryResponse)userResponse.Item1).Object;

                var appResponse = await CacheFactory.GetAppByLicenseWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(CacheKeys.GetAppByLicenseCacheKey, request.License),
                    CachingStrategy.Medium,
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

                var authenticationResult = new AuthenticationResult();

                authenticationResult.User = authenticatedUser;

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
                result.DataPacket.Add(authenticationResult);

                return result;
            }
            catch
            {
                throw;
            }
        }
    }
}
