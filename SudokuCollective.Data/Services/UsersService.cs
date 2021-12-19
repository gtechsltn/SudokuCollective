using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.DataModels;
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
using SudokuCollective.Data.Models.Results;
using SudokuCollective.Data.Resiliency;
using SudokuCollective.Data.Utilities;

namespace SudokuCollective.Data.Services
{
    public class UsersService : IUsersService
    {
        #region Fields
        private readonly IUsersRepository<User> _usersRepository;
        private readonly IAppsRepository<App> _appsRepository;
        private readonly IRolesRepository<Role> _rolesRepository;
        private readonly IAppAdminsRepository<AppAdmin> _appAdminsRepository;
        private readonly IEmailConfirmationsRepository<EmailConfirmation> _emailConfirmationsRepository;
        private readonly IPasswordResetsRepository<PasswordReset> _passwordResetsRepository;
        private readonly IEmailService _emailService;
        private readonly IDistributedCache _distributedCache;
        #endregion

        #region Constructor
        public UsersService(
            IUsersRepository<User> usersRepository,
            IAppsRepository<App> appsRepository,
            IRolesRepository<Role> rolesRepository,
            IAppAdminsRepository<AppAdmin> appAdminsRepository,
            IEmailConfirmationsRepository<EmailConfirmation> emailConfirmationsRepository,
            IPasswordResetsRepository<PasswordReset> passwordResetsRepository,
            IEmailService emailService,
            IDistributedCache distributedCache)
        {
            _usersRepository = usersRepository;
            _appsRepository = appsRepository;
            _rolesRepository = rolesRepository;
            _appAdminsRepository = appAdminsRepository;
            _emailConfirmationsRepository = emailConfirmationsRepository;
            _passwordResetsRepository = passwordResetsRepository;
            _emailService = emailService;
            _distributedCache = distributedCache;
            
            throw new System.NotImplementedException();
        }
        public async Task<IResult> Create(
            IRequest request, 
            string baseUrl, 
            string emailTemplatePath)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrEmpty(baseUrl)) throw new ArgumentNullException(nameof(baseUrl));

            if (string.IsNullOrEmpty(emailTemplatePath)) throw new ArgumentNullException(nameof(emailTemplatePath));

            var result = new Result();

            RegisterRequest registerRequest;

            if (request.DataPacket is RegisterRequest r)
            {
                registerRequest = r;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = ServicesMesages.InvalidRequestMessage;

                return result;
            }

            var isUserNameUnique = false;
            var isEmailUnique = false;

            // User name accepsts alphanumeric and special characters except double and single quotes
            var regex = new Regex("^[^-]{1}?[^\"\']*$");

            if (!string.IsNullOrEmpty(registerRequest.UserName))
            {
                isUserNameUnique = await _usersRepository.IsUserNameUnique(registerRequest.UserName);
            }

            if (!string.IsNullOrEmpty(registerRequest.Email))
            {
                isEmailUnique = await _usersRepository.IsEmailUnique(registerRequest.Email);
            }

            if (string.IsNullOrEmpty(registerRequest.UserName)
                || string.IsNullOrEmpty(registerRequest.Email)
                || !isUserNameUnique
                || !isEmailUnique
                || !regex.IsMatch(registerRequest.UserName))
            {
                if (string.IsNullOrEmpty(registerRequest.UserName))
                {
                    result.IsSuccess = false;
                    result.Message = UsersMessages.UserNameRequiredMessage;

                    return result;
                }
                else if (string.IsNullOrEmpty(registerRequest.Email))
                {
                    result.IsSuccess = false;
                    result.Message = UsersMessages.EmailRequiredMessage;

                    return result;
                }
                else if (!regex.IsMatch(registerRequest.UserName))
                {
                    result.IsSuccess = false;
                    result.Message = UsersMessages.UserNameInvalidMessage;

                    return result;
                }
                else if (!isUserNameUnique)
                {
                    result.IsSuccess = isUserNameUnique;
                    result.Message = UsersMessages.UserNameUniqueMessage;

                    return result;
                }
                else
                {
                    result.IsSuccess = isEmailUnique;
                    result.Message = UsersMessages.EmailUniqueMessage;

                    return result;
                }
            }
            else
            {
                try
                {
                    var cacheFactoryResponse = await CacheFactory.GetAppByLicenseWithCacheAsync(
                        _appsRepository,
                        _distributedCache,
                        string.Format(CacheKeys.GetAppByLicenseCacheKey, request.License),
                        CachingStrategy.Medium,
                        request.License,
                        result);

                    var appResponse = (RepositoryResponse)cacheFactoryResponse.Item1;

                    if (appResponse.Success)
                    {
                        var app = (App)appResponse.Object;

                        if (!app.IsActive)
                        {
                            result.IsSuccess = false;
                            result.Message = AppsMessages.AppDeactivatedMessage;

                            return result;
                        }

                        var salt = BCrypt.Net.BCrypt.GenerateSalt();

                        var user = new User(
                            0,
                            registerRequest.UserName,
                            registerRequest.FirstName,
                            registerRequest.LastName,
                            registerRequest.NickName,
                            registerRequest.Email,
                            false,
                            false,
                            BCrypt.Net.BCrypt.HashPassword(registerRequest.Password, salt),
                            false,
                            true,
                            DateTime.UtcNow,
                            DateTime.MinValue);

                        user.Apps.Add(
                            new UserApp()
                            {
                                User = user,
                                App = app,
                                AppId = app.Id
                            });

                        var userResponse = await CacheFactory.AddWithCacheAsync<User>(
                            _usersRepository,
                            _distributedCache,
                            CacheKeys.GetUserCacheKey,
                            CachingStrategy.Medium,
                            user);

                        if (userResponse.Success)
                        {
                            user = (User)userResponse.Object;

                            if (user.Roles.Any(ur => ur.Role.RoleLevel == RoleLevel.ADMIN))
                            {
                                var appAdmin = new AppAdmin(app.Id, user.Id);

                                _ = await _appAdminsRepository.Add(appAdmin);
                            }

                            var emailConfirmation = new EmailConfirmation(
                                user.Id,
                                app.Id);

                            emailConfirmation = await EnsureEmailConfirmationTokenIsUnique(emailConfirmation);

                            emailConfirmation = (EmailConfirmation)(await _emailConfirmationsRepository.Create(emailConfirmation))
                                .Object;

                            string EmailConfirmationAction;

                            if (app.UseCustomEmailConfirmationAction)
                            {
                                if (app.Environment == ReleaseEnvironment.LOCAL)
                                {
                                    EmailConfirmationAction = string.Format("{0}/{1}/{2}",
                                        app.LocalUrl,
                                        app.CustomEmailConfirmationAction,
                                        emailConfirmation.Token);
                                }
                                else if (app.Environment == ReleaseEnvironment.DEV)
                                {
                                    EmailConfirmationAction = string.Format("{0}/{1}/{2}",
                                        app.DevUrl,
                                        app.CustomEmailConfirmationAction,
                                        emailConfirmation.Token);
                                }
                                else if (app.Environment == ReleaseEnvironment.QA)
                                {
                                    EmailConfirmationAction = string.Format("{0}/{1}/{2}",
                                        app.QaUrl,
                                        app.CustomEmailConfirmationAction,
                                        emailConfirmation.Token);
                                }
                                else
                                {
                                    EmailConfirmationAction = string.Format("{0}/{1}/{2}",
                                        app.ProdUrl,
                                        app.CustomEmailConfirmationAction,
                                        emailConfirmation.Token);
                                }
                            }
                            else
                            {
                                EmailConfirmationAction = string.Format("https://{0}/confirmEmail/{1}",
                                    baseUrl,
                                    emailConfirmation.Token);
                            }

                            var html = File.ReadAllText(emailTemplatePath);
                            var appTitle = app.Name;
                            var url = string.Empty;

                            if (app.Environment == ReleaseEnvironment.LOCAL)
                            {
                                url = app.LocalUrl;
                            }
                            else if (app.Environment == ReleaseEnvironment.DEV)
                            {
                                url = app.DevUrl;
                            }
                            else if (app.Environment == ReleaseEnvironment.QA)
                            {
                                url = app.QaUrl;
                            }
                            else
                            {
                                url = app.ProdUrl;
                            }

                            html = html.Replace("{{USER_NAME}}", user.UserName);
                            html = html.Replace("{{CONFIRM_EMAIL_URL}}", EmailConfirmationAction);
                            html = html.Replace("{{APP_TITLE}}", appTitle);
                            html = html.Replace("{{URL}}", url);

                            var emailSubject = string.Format("Greetings from {0}: Please Confirm Email", appTitle);

                            IUserResult userResult = new UserResult();

                            userResult.ConfirmationEmailSuccessfullySent = _emailService
                                .Send(user.Email, emailSubject, html);

                            foreach (var userRole in user.Roles)
                            {
                                userRole.Role.Users = new List<UserRole>();
                            }

                            foreach (var userApp in user.Apps)
                            {
                                userApp.App.Users = new List<UserApp>();
                            }

                            userResult.User = user;

                            result.IsSuccess = userResponse.Success;
                            result.Message = UsersMessages.UserCreatedMessage;
                            result.DataPacket.Add(userResult);

                            return result;
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
                            result.Message = UsersMessages.UserNotCreatedMessage;

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
        }

        public async Task<IResult> Get(int id, string license)
        {
            if (string.IsNullOrEmpty(license)) throw new ArgumentNullException(nameof(license));

            var result = new Result();

            if (id == 0)
            {
                result.IsSuccess = false;
                result.Message = UsersMessages.UserNotFoundMessage;

                return result;
            }

            try
            {
                var cacheFactoryResponse = await CacheFactory.GetWithCacheAsync<User>(
                    _usersRepository,
                    _distributedCache,
                    string.Format(CacheKeys.GetUserCacheKey, id, license),
                    CachingStrategy.Medium,
                    id,
                    result);

                var response = (RepositoryResponse)cacheFactoryResponse.Item1;
                result = (Result)cacheFactoryResponse.Item2;

                if (response.Success)
                {
                    var user = (User)response.Object;

                    result.IsSuccess = response.Success;
                    result.Message = UsersMessages.UserFoundMessage;
                    result.DataPacket.Add(user);

                    cacheFactoryResponse = await CacheFactory.GetAppByLicenseWithCacheAsync(
                        _appsRepository,
                        _distributedCache,
                        string.Format(CacheKeys.GetAppByLicenseCacheKey, license),
                        CachingStrategy.Medium,
                        license);

                    var app = (App)((RepositoryResponse)cacheFactoryResponse.Item1).Object;

                    var appAdmins = (await _appAdminsRepository.GetAll())
                        .Objects
                        .ConvertAll(aa => (AppAdmin)aa)
                        .ToList();

                    if (((User)(result
                        .DataPacket[0]))
                        .Roles
                        .Any(ur => ur.Role.RoleLevel == RoleLevel.ADMIN))
                    {
                        if (!user.IsSuperUser)
                        {
                            if (!appAdmins.Any(aa =>
                                aa.AppId == app.Id &&
                                aa.UserId == ((User)(result.DataPacket[0])).Id &&
                                aa.IsActive))
                            {
                                var adminRole = ((User)(result.DataPacket[0]))
                                    .Roles
                                    .FirstOrDefault(ur =>
                                        ur.Role.RoleLevel == RoleLevel.ADMIN);

                                ((User)(result.DataPacket[0])).Roles.Remove(adminRole);
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
                    }

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
                    result.Message = UsersMessages.UserNotFoundMessage;

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

        public async Task<IResult> Update(
            int id, 
            IRequest request, 
            string baseUrl, 
            string emailTemplatePath)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrEmpty(baseUrl)) throw new ArgumentNullException(nameof(baseUrl));

            if (string.IsNullOrEmpty(emailTemplatePath)) throw new ArgumentNullException(nameof(emailTemplatePath));

            var result = new Result();
            
            var userResult = new UserResult();

            UpdateUserRequest updateUserRequest;

            if (request.DataPacket is UpdateUserRequest r)
            {
                updateUserRequest = r;
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
                result.Message = UsersMessages.UserNotFoundMessage;

                return result;
            }

            // User name accepsts alphanumeric and special characters except double and single quotes
            var regex = new Regex("^[^-]{1}?[^\"\']*$");

            var isUserNameUnique = await _usersRepository.IsUpdatedUserNameUnique(id, updateUserRequest.UserName);
            var isEmailUnique = await _usersRepository.IsUpdatedEmailUnique(id, updateUserRequest.Email);

            if (string.IsNullOrEmpty(updateUserRequest.UserName)
                || string.IsNullOrEmpty(updateUserRequest.Email)
                || !isUserNameUnique
                || !isEmailUnique
                || !regex.IsMatch(updateUserRequest.UserName))
            {
                if (string.IsNullOrEmpty(updateUserRequest.UserName))
                {
                    result.IsSuccess = false;
                    result.Message = UsersMessages.UserNameRequiredMessage;

                    return result;
                }
                else if (string.IsNullOrEmpty(updateUserRequest.Email))
                {
                    result.IsSuccess = false;
                    result.Message = UsersMessages.EmailRequiredMessage;

                    return result;
                }
                else if (!regex.IsMatch(updateUserRequest.UserName))
                {
                    result.IsSuccess = false;
                    result.Message = UsersMessages.UserNameInvalidMessage;

                    return result;
                }
                else if (!isUserNameUnique)
                {
                    result.IsSuccess = isUserNameUnique;
                    result.Message = UsersMessages.UserNameUniqueMessage;

                    return result;
                }
                else
                {
                    result.IsSuccess = isEmailUnique;
                    result.Message = UsersMessages.EmailUniqueMessage;

                    return result;
                }
            }
            else
            {
                try
                {
                    var cacheFactoryResponse = await CacheFactory.GetWithCacheAsync<User>(
                        _usersRepository,
                        _distributedCache,
                        string.Format(CacheKeys.GetUserCacheKey, id, request.License),
                        CachingStrategy.Medium,
                        id);

                    var userResponse = (RepositoryResponse)cacheFactoryResponse.Item1;

                    if (userResponse.Success)
                    {
                        var user = (User)userResponse.Object;

                        cacheFactoryResponse = await CacheFactory.GetWithCacheAsync<App>(
                            _appsRepository,
                            _distributedCache,
                            string.Format(CacheKeys.GetAppCacheKey, request.AppId),
                            CachingStrategy.Medium,
                            request.AppId);

                        var app = (App)((RepositoryResponse)cacheFactoryResponse.Item1).Object;

                        user.UserName = updateUserRequest.UserName;
                        user.FirstName = updateUserRequest.FirstName;
                        user.LastName = updateUserRequest.LastName;
                        user.NickName = updateUserRequest.NickName;
                        user.DateUpdated = DateTime.UtcNow;

                        if (!user.Email.ToLower().Equals(updateUserRequest.Email.ToLower()))
                        {
                            if (!user.ReceivedRequestToUpdateEmail)
                            {
                                user.ReceivedRequestToUpdateEmail = true;
                            }

                            EmailConfirmation emailConfirmation;

                            if (await _emailConfirmationsRepository.HasOutstandingEmailConfirmation(user.Id, app.Id))
                            {
                                emailConfirmation = (EmailConfirmation)(await _emailConfirmationsRepository
                                    .RetrieveEmailConfirmation(user.Id, app.Id)).Object;

                                if (!user.IsEmailConfirmed)
                                {
                                    user.Email = emailConfirmation.OldEmailAddress;
                                }

                                emailConfirmation.OldEmailAddress = user.Email;
                                emailConfirmation.NewEmailAddress = updateUserRequest.Email;
                            }
                            else
                            {
                                emailConfirmation = new EmailConfirmation(
                                    user.Id,
                                    request.AppId,
                                    user.Email,
                                    updateUserRequest.Email);
                            }

                            emailConfirmation = await EnsureEmailConfirmationTokenIsUnique(emailConfirmation);

                            IRepositoryResponse emailConfirmationResponse;

                            if (emailConfirmation.Id == 0)
                            {
                                emailConfirmationResponse = await _emailConfirmationsRepository
                                    .Create(emailConfirmation);
                            }
                            else
                            {
                                emailConfirmationResponse = await _emailConfirmationsRepository
                                    .Update(emailConfirmation);
                            }


                            string EmailConfirmationAction;

                            if (app.UseCustomEmailConfirmationAction)
                            {
                                if (app.Environment == ReleaseEnvironment.LOCAL)
                                {
                                    EmailConfirmationAction = string.Format("{0}/{1}/{2}",
                                        app.LocalUrl,
                                        app.CustomEmailConfirmationAction,
                                        emailConfirmation.Token);
                                }
                                else if (app.Environment == ReleaseEnvironment.DEV)
                                {
                                    EmailConfirmationAction = string.Format("{0}/{1}/{2}",
                                        app.DevUrl,
                                        app.CustomEmailConfirmationAction,
                                        emailConfirmation.Token);
                                }
                                else if (app.Environment == ReleaseEnvironment.QA)
                                {
                                    EmailConfirmationAction = string.Format("{0}/{1}/{2}",
                                        app.QaUrl,
                                        app.CustomEmailConfirmationAction,
                                        emailConfirmation.Token);
                                }
                                else
                                {
                                    EmailConfirmationAction = string.Format("{0}/{1}/{2}",
                                        app.ProdUrl,
                                        app.CustomEmailConfirmationAction,
                                        emailConfirmation.Token);
                                }
                            }
                            else
                            {
                                EmailConfirmationAction = string.Format("https://{0}/confirmEmail/{1}",
                                    baseUrl,
                                    ((EmailConfirmation)emailConfirmationResponse.Object).Token);
                            }

                            var html = File.ReadAllText(emailTemplatePath);
                            var appTitle = app.Name;
                            var url = string.Empty;

                            if (app.Environment == ReleaseEnvironment.LOCAL)
                            {
                                url = app.LocalUrl;
                            }
                            else if (app.Environment == ReleaseEnvironment.DEV)
                            {
                                url = app.DevUrl;
                            }
                            else if (app.Environment == ReleaseEnvironment.QA)
                            {
                                url = app.QaUrl;
                            }
                            else
                            {
                                url = app.ProdUrl;
                            }

                            html = html.Replace("{{USER_NAME}}", user.UserName);
                            html = html.Replace("{{CONFIRM_EMAIL_URL}}", EmailConfirmationAction);
                            html = html.Replace("{{APP_TITLE}}", appTitle);
                            html = html.Replace("{{URL}}", url);

                            var emailSubject = string.Format("Greetings from {0}: Please Confirm Old Email", appTitle);

                            userResult.ConfirmationEmailSuccessfullySent = _emailService
                                .Send(user.Email, emailSubject, html);
                        }

                        var updateUserResponse = await CacheFactory.UpdateWithCacheAsync(
                            _usersRepository,
                            _distributedCache,
                            user,
                            request.License);

                        if (updateUserResponse.Success)
                        {
                            userResult.User = (User)updateUserResponse.Object;

                            result.IsSuccess = userResponse.Success;
                            result.Message = UsersMessages.UserUpdatedMessage;
                            result.DataPacket.Add(userResult);

                            return result;
                        }
                        else if (!updateUserResponse.Success && updateUserResponse.Exception != null)
                        {
                            result.IsSuccess = updateUserResponse.Success;
                            result.Message = updateUserResponse.Exception.Message;

                            return result;
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Message = UsersMessages.UserNotUpdatedMessage;

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
                        result.IsSuccess = false;
                        result.Message = UsersMessages.UserNotFoundMessage;

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
        }

        public async Task<IResult> GetUsers(
            int requestorId, 
            string license, 
            IPaginator paginator)
        {
            if (paginator == null) throw new ArgumentNullException(nameof(paginator));

            if (string.IsNullOrEmpty(license)) throw new ArgumentNullException(nameof(license));

            var result = new Result();

            if (requestorId == 0)
            {
                result.IsSuccess = false;
                result.Message = UsersMessages.UserNotFoundMessage;

                return result;
            }

            try
            {
                var response = await _usersRepository.GetAll();

                var cacheFactoryResponse = await CacheFactory.GetAllWithCacheAsync<User>(
                    _usersRepository,
                    _distributedCache,
                    CacheKeys.GetUsersCacheKey,
                    CachingStrategy.Medium,
                    result);

                var userResponse = (RepositoryResponse)cacheFactoryResponse.Item1;
                result = (Result)cacheFactoryResponse.Item2;

                if (response.Success)
                {
                    if (DataUtilities.IsPageValid(paginator, response.Objects))
                    {
                        result = PaginatorUtilities.PaginateUsers(paginator, response, result);
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = ServicesMesages.PageNotFoundMessage;

                        return result;
                    }

                    cacheFactoryResponse = await CacheFactory.GetAppByLicenseWithCacheAsync(
                        _appsRepository,
                        _distributedCache,
                        string.Format(CacheKeys.GetAppByLicenseCacheKey, license),
                        CachingStrategy.Medium,
                        license);

                    var app = (App)((RepositoryResponse)cacheFactoryResponse.Item1).Object;

                    var appAdmins = (await _appAdminsRepository.GetAll())
                        .Objects
                        .ConvertAll(aa => (AppAdmin)aa)
                        .ToList();

                    foreach (var user in result.DataPacket.ConvertAll(u => (IUser)u))
                    {
                        if (user
                            .Roles
                            .Any(ur => ur.Role.RoleLevel == RoleLevel.ADMIN))
                        {
                            if (!user.IsSuperUser)
                            {
                                if (!appAdmins.Any(aa =>
                                    aa.AppId == app.Id &&
                                    aa.UserId == user.Id &&
                                    aa.IsActive))
                                {
                                    var adminRole = user
                                        .Roles
                                        .FirstOrDefault(ur =>
                                            ur.Role.RoleLevel == RoleLevel.ADMIN);

                                    user.Roles.Remove(adminRole);
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
                        }
                    }

                    cacheFactoryResponse = await CacheFactory.GetWithCacheAsync<User>(
                        _usersRepository,
                        _distributedCache,
                        string.Format(CacheKeys.GetUserCacheKey, requestorId, license),
                        CachingStrategy.Medium,
                        requestorId);

                    var requestor = (User)((RepositoryResponse)cacheFactoryResponse.Item1).Object;

                    if (!requestor.IsSuperUser)
                    {
                        // Filter out user emails from the frontend...
                        foreach (var user in result.DataPacket.ConvertAll(u => (IUser)u))
                        {
                            var emailConfirmed = user.IsEmailConfirmed;
                            user.Email = null;
                            user.IsEmailConfirmed = emailConfirmed;
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
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> Delete(int id, string license)
        {
            var result = new Result();

            if (id == 0)
            {
                result.IsSuccess = false;
                result.Message = UsersMessages.UserNotFoundMessage;
            }

            try
            {
                var cacheFactoryResponse = await CacheFactory.GetWithCacheAsync<User>(
                    _usersRepository,
                    _distributedCache,
                    string.Format(CacheKeys.GetUserCacheKey, id, license),
                    CachingStrategy.Medium,
                    id);

                var userResponse = (RepositoryResponse)cacheFactoryResponse.Item1;

                if (userResponse.Success)
                {
                    if (((User)userResponse.Object).Id == 1 && ((User)userResponse.Object).IsSuperUser)
                    {
                        result.IsSuccess = false;
                        result.Message = UsersMessages.SuperUserCannotBeDeletedMessage;

                        return result;
                    }

                    var deletionResponse = await CacheFactory.DeleteWithCacheAsync<User>(
                        _usersRepository,
                        _distributedCache,
                        (User)userResponse.Object);

                    if (deletionResponse.Success)
                    {
                        var admins = (await _appAdminsRepository.GetAll())
                            .Objects
                            .ConvertAll(aa => (AppAdmin)aa)
                            .Where(aa => aa.UserId == id)
                            .ToList();

                        _ = await _appAdminsRepository.DeleteRange(admins);

                        result.IsSuccess = true;
                        result.Message = UsersMessages.UserDeletedMessage;

                        return result;
                    }
                    else if (!deletionResponse.Success && deletionResponse.Exception != null)
                    {
                        result.IsSuccess = deletionResponse.Success;
                        result.Message = deletionResponse.Exception.Message;

                        return result;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = UsersMessages.UserNotDeletedMessage;

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
                    result.IsSuccess = false;
                    result.Message = UsersMessages.UserNotFoundMessage;

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

        public async Task<IResult> GetUserByPasswordToken(string token)
        {
            var result = new Result();

            try
            {
                var response = await _passwordResetsRepository.Get(token);

                if (response.Success)
                {
                    var license = (await CacheFactory.GetLicenseWithCacheAsync(
                        _appsRepository,
                        _distributedCache,
                        string.Format(CacheKeys.GetAppLicenseCacheKey, ((PasswordReset)response.Object).AppId),
                        CachingStrategy.Heavy,
                        ((PasswordReset)response.Object).AppId)).Item1;

                    result.DataPacket.Add((User)((await CacheFactory.GetWithCacheAsync<User>(
                        _usersRepository,
                        _distributedCache,
                        string.Format(CacheKeys.GetUserCacheKey, ((PasswordReset)response.Object).UserId, license),
                        CachingStrategy.Medium,
                        ((PasswordReset)response.Object).UserId,
                        result)).Item1).Object);

                    result.IsSuccess = response.Success;
                    result.Message = UsersMessages.UserFoundMessage;

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
                    result.IsSuccess = response.Success;
                    result.Message = UsersMessages.UserNotFoundMessage;

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

        public async Task<ILicenseResult> GetAppLicenseByPasswordToken(string token)
        {
            var result = new LicenseResult();

            try
            {
                var response = await _passwordResetsRepository.Get(token);

                if (response.Success)
                {
                    result.License = await _appsRepository.GetLicense(((PasswordReset)response.Object).AppId);
                    result.IsSuccess = response.Success;
                    result.Message = AppsMessages.AppFoundMessage;

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
                    result.IsSuccess = response.Success;
                    result.Message = UsersMessages.NoOutstandingRequestToResetPassworMessage;

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

        public async Task<IResult> AddUserRoles(
            int userid, 
            List<int> roleIds, 
            string license)
        {
            if (roleIds == null) throw new ArgumentNullException(nameof(roleIds));

            if (string.IsNullOrEmpty(license)) throw new ArgumentNullException(nameof(license));

            var result = new Result();

            if (userid == 0)
            {
                result.IsSuccess = false;
                result.Message = UsersMessages.UserNotFoundMessage;

                return result;
            }

            try
            {
                if (await _rolesRepository.IsListValid(roleIds))
                {
                    var response = await _usersRepository.AddRoles(userid, roleIds);

                    var cacheFactoryResponse = new Tuple<IRepositoryResponse, IResult>(
                        new RepositoryResponse(), 
                        new Result());

                    if (response.Success)
                    {
                        var roles = response
                            .Objects
                            .ConvertAll(ur => (UserRole)ur)
                            .ToList();

                        foreach (var role in roles)
                        {
                            if (role.Role.RoleLevel == RoleLevel.ADMIN)
                            {
                                cacheFactoryResponse = await CacheFactory.GetAppByLicenseWithCacheAsync(
                                    _appsRepository,
                                    _distributedCache,
                                    string.Format(CacheKeys.GetAppByLicenseCacheKey, license),
                                    CachingStrategy.Medium,
                                    license);

                                var app = (App)((RepositoryResponse)cacheFactoryResponse.Item1).Object;

                                var appAdmin = (AppAdmin)(await _appAdminsRepository.Add(new AppAdmin(app.Id, userid))).Object;
                            }

                            result.DataPacket.ConvertAll(r => (Role)r).Add(role.Role);
                        }

                        cacheFactoryResponse = await CacheFactory.GetWithCacheAsync<User>(
                            _usersRepository,
                            _distributedCache,
                            string.Format(CacheKeys.GetUserCacheKey, userid, license),
                            CachingStrategy.Medium,
                            userid);

                        var user = (User)((RepositoryResponse)cacheFactoryResponse.Item1).Object;

                        // Remove any user cache items which may exist
                        var cacheKeys = new List<string> {
                                string.Format(CacheKeys.GetUserCacheKey, user.Id, license),
                                string.Format(CacheKeys.GetUserByUsernameCacheKey, user.UserName, license),
                                string.Format(CacheKeys.GetUserByEmailCacheKey, user.Email, license)
                            };

                        await CacheFactory.RemoveKeysAsync(_distributedCache, cacheKeys);

                        result.IsSuccess = response.Success;
                        result.Message = UsersMessages.RolesAddedMessage;

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
                        result.Message = UsersMessages.RolesNotAddedMessage;

                        return result;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = UsersMessages.RolesInvalidMessage;

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

        public async Task<IResult> RemoveUserRoles(
            int userid, 
            List<int> roleIds, 
            string license)
        {
            if (roleIds == null) throw new ArgumentNullException(nameof(roleIds));

            if (string.IsNullOrEmpty(license)) throw new ArgumentNullException(nameof(license));

            var result = new Result();

            if (userid == 0)
            {
                result.IsSuccess = false;
                result.Message = UsersMessages.UserNotFoundMessage;

                return result;
            }

            try
            {
                if (await _rolesRepository.IsListValid(roleIds))
                {
                    var response = await _usersRepository.RemoveRoles(userid, roleIds);

                    var cacheFactoryResponse = new Tuple<IRepositoryResponse, IResult>(
                        new RepositoryResponse(),
                        new Result());

                    if (response.Success)
                    {
                        var roles = response
                            .Objects
                            .ConvertAll(ur => (UserRole)ur)
                            .ToList();

                        foreach (var role in roles)
                        {
                            if (role.Role.RoleLevel == RoleLevel.ADMIN)
                            {
                                cacheFactoryResponse = await CacheFactory.GetAppByLicenseWithCacheAsync(
                                    _appsRepository,
                                    _distributedCache,
                                    string.Format(CacheKeys.GetAppByLicenseCacheKey, license),
                                    CachingStrategy.Medium,
                                    license);

                                var app = (App)((RepositoryResponse)cacheFactoryResponse.Item1).Object;

                                var appAdmin = (AppAdmin)
                                    (await _appAdminsRepository.GetAdminRecord(app.Id, userid))
                                    .Object;

                                _ = await _appAdminsRepository.Delete(appAdmin);
                            }
                        }

                        cacheFactoryResponse = await CacheFactory.GetWithCacheAsync<User>(
                            _usersRepository,
                            _distributedCache,
                            string.Format(CacheKeys.GetUserCacheKey, userid, license),
                            CachingStrategy.Medium,
                            userid);

                        var user = (User)((RepositoryResponse)cacheFactoryResponse.Item1).Object;

                        // Remove any user cache items which may exist
                        var cacheKeys = new List<string> {
                                string.Format(CacheKeys.GetUserCacheKey, user.Id, license),
                                string.Format(CacheKeys.GetUserByUsernameCacheKey, user.UserName, license),
                                string.Format(CacheKeys.GetUserByEmailCacheKey, user.Email, license)
                            };

                        await CacheFactory.RemoveKeysAsync(_distributedCache, cacheKeys);

                        result.IsSuccess = response.Success;
                        result.Message = UsersMessages.RolesRemovedMessage;

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
                        result.Message = UsersMessages.RolesNotRemovedMessage;

                        return result;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = UsersMessages.RolesInvalidMessage;

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
                result.Message = UsersMessages.UserNotFoundMessage;

                return result;
            }

            try
            {
                if (await _usersRepository.Activate(id))
                {
                    var license = (await CacheFactory.GetLicenseWithCacheAsync(
                        _appsRepository,
                        _distributedCache,
                        string.Format(CacheKeys.GetAppLicenseCacheKey, 1),
                        CachingStrategy.Medium,
                        1)).Item1;

                    var cacheFactoryResponse = await CacheFactory.GetWithCacheAsync(
                        _usersRepository,
                        _distributedCache,
                        string.Format(CacheKeys.GetUserCacheKey, id, license),
                        CachingStrategy.Medium,
                        id);

                    var user = (User)((RepositoryResponse)cacheFactoryResponse.Item1).Object;

                    // Remove any user cache items which may exist
                    var cacheKeys = new List<string> {
                                string.Format(CacheKeys.GetUserCacheKey, user.Id, license),
                                string.Format(CacheKeys.GetUserByUsernameCacheKey, user.UserName, license),
                                string.Format(CacheKeys.GetUserByEmailCacheKey, user.Email, license)
                            };

                    await CacheFactory.RemoveKeysAsync(_distributedCache, cacheKeys);

                    result.IsSuccess = true;
                    result.Message = UsersMessages.UserActivatedMessage;

                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = UsersMessages.UserNotActivatedMessage;

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
                result.Message = UsersMessages.UserNotFoundMessage;

                return result;
            }

            try
            {
                if (await _usersRepository.Deactivate(id))
                {
                    var license = (await CacheFactory.GetLicenseWithCacheAsync(
                        _appsRepository,
                        _distributedCache,
                        string.Format(CacheKeys.GetAppLicenseCacheKey, 1),
                        CachingStrategy.Medium,
                        1)).Item1;

                    var cacheFactoryResponse = await CacheFactory.GetWithCacheAsync(
                        _usersRepository,
                        _distributedCache,
                        string.Format(CacheKeys.GetUserCacheKey, id, license),
                        CachingStrategy.Medium,
                        id);

                    var user = (User)((RepositoryResponse)cacheFactoryResponse.Item1).Object;

                    // Remove any user cache items which may exist
                    var cacheKeys = new List<string> {
                                string.Format(CacheKeys.GetUserCacheKey, user.Id, license),
                                string.Format(CacheKeys.GetUserByUsernameCacheKey, user.UserName, license),
                                string.Format(CacheKeys.GetUserByEmailCacheKey, user.Email, license)
                            };

                    await CacheFactory.RemoveKeysAsync(_distributedCache, cacheKeys);

                    result.IsSuccess = true;
                    result.Message = UsersMessages.UserDeactivatedMessage;

                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = UsersMessages.UserNotDeactivatedMessage;

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

        public async Task<IResult> ResendEmailConfirmation(
            int userId, 
            int appId, 
            string baseUrl, 
            string emailTemplatePath, 
            string license)
        {
            if (string.IsNullOrEmpty(baseUrl)) throw new ArgumentException(nameof(baseUrl));

            if (string.IsNullOrEmpty(emailTemplatePath)) throw new ArgumentException(nameof(emailTemplatePath));

            var result = new Result();

            var userResult = new UserResult();

            if (userId == 0 || appId == 0)
            {
                result.IsSuccess = false;
                result.Message = UsersMessages.EmailConfirmationEmailNotResentMessage;

                return result;
            }

            try
            {
                var cacheFactoryResponse = await CacheFactory.GetWithCacheAsync<User>(
                    _usersRepository,
                    _distributedCache,
                    string.Format(CacheKeys.GetUserCacheKey, userId, license),
                    CachingStrategy.Medium,
                    userId);

                var userResponse = (RepositoryResponse)cacheFactoryResponse.Item1;

                if (userResponse.Success)
                {
                    var user = (User)userResponse.Object;

                    if (!user.IsEmailConfirmed)
                    {
                        cacheFactoryResponse = await CacheFactory.GetWithCacheAsync<App>(
                            _appsRepository,
                            _distributedCache,
                            string.Format(CacheKeys.GetAppCacheKey, appId),
                            CachingStrategy.Medium,
                            appId);

                        var appResponse = (RepositoryResponse)cacheFactoryResponse.Item1;

                        if (appResponse.Success)
                        {
                            var app = (App)appResponse.Object;

                            if (await _emailConfirmationsRepository.HasOutstandingEmailConfirmation(userId, appId))
                            {
                                var emailConfirmationResponse = await _emailConfirmationsRepository.RetrieveEmailConfirmation(userId, appId);

                                if (emailConfirmationResponse.Success)
                                {
                                    var emailConfirmation = (EmailConfirmation)emailConfirmationResponse.Object;

                                    string EmailConfirmationAction;

                                    if (app.UseCustomEmailConfirmationAction)
                                    {
                                        if (app.Environment == ReleaseEnvironment.LOCAL)
                                        {
                                            EmailConfirmationAction = string.Format("{0}/{1}/{2}",
                                                app.LocalUrl,
                                                app.CustomEmailConfirmationAction,
                                                emailConfirmation.Token);
                                        }
                                        else if (app.Environment == ReleaseEnvironment.DEV)
                                        {
                                            EmailConfirmationAction = string.Format("{0}/{1}/{2}",
                                                app.DevUrl,
                                                app.CustomEmailConfirmationAction,
                                                emailConfirmation.Token);
                                        }
                                        else if (app.Environment == ReleaseEnvironment.QA)
                                        {
                                            EmailConfirmationAction = string.Format("{0}/{1}/{2}",
                                                app.QaUrl,
                                                app.CustomEmailConfirmationAction,
                                                emailConfirmation.Token);
                                        }
                                        else
                                        {
                                            EmailConfirmationAction = string.Format("{0}/{1}/{2}",
                                                app.ProdUrl,
                                                app.CustomEmailConfirmationAction,
                                                emailConfirmation.Token);
                                        }
                                    }
                                    else
                                    {
                                        EmailConfirmationAction = string.Format("https://{0}/confirmEmail/{1}",
                                            baseUrl,
                                            ((EmailConfirmation)emailConfirmationResponse.Object).Token);
                                    }

                                    var html = File.ReadAllText(emailTemplatePath);
                                    var appTitle = app.Name;
                                    var url = string.Empty;

                                    if (app.Environment == ReleaseEnvironment.LOCAL)
                                    {
                                        url = app.LocalUrl;
                                    }
                                    else if (app.Environment == ReleaseEnvironment.DEV)
                                    {
                                        url = app.DevUrl;
                                    }
                                    else if (app.Environment == ReleaseEnvironment.QA)
                                    {
                                        url = app.QaUrl;
                                    }
                                    else
                                    {
                                        url = app.ProdUrl;
                                    }

                                    html = html.Replace("{{USER_NAME}}", user.UserName);
                                    html = html.Replace("{{CONFIRM_EMAIL_URL}}", EmailConfirmationAction);
                                    html = html.Replace("{{APP_TITLE}}", appTitle);
                                    html = html.Replace("{{URL}}", url);

                                    var emailSubject = string.Format("Greetings from {0}: Please Confirm Email", appTitle);

                                    userResult.ConfirmationEmailSuccessfullySent = _emailService
                                        .Send(user.Email, emailSubject, html);

                                    if ((bool)userResult.ConfirmationEmailSuccessfullySent)
                                    {
                                        userResult.User = user;
                                        result.IsSuccess = true;
                                        result.Message = UsersMessages.EmailConfirmationEmailResentMessage;
                                        result.DataPacket.Add(userResult);

                                        return result;
                                    }
                                    else
                                    {
                                        result.IsSuccess = false;
                                        result.Message = UsersMessages.EmailConfirmationEmailNotResentMessage;

                                        return result;
                                    }

                                }
                                else if (!emailConfirmationResponse.Success && emailConfirmationResponse.Exception != null)
                                {
                                    result.IsSuccess = emailConfirmationResponse.Success;
                                    result.Message = emailConfirmationResponse.Exception.Message;

                                    return result;
                                }
                                else
                                {
                                    result.IsSuccess = false;
                                    result.Message = UsersMessages.EmailConfirmationRequestNotFoundMessage;

                                    return result;
                                }

                            }
                            else
                            {
                                result.IsSuccess = false;
                                result.Message = UsersMessages.EmailConfirmationRequestNotFoundMessage;

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
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = UsersMessages.EmailConfirmedMessage;

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
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> ConfirmEmail(
            string token, 
            string baseUrl, 
            string emailTemplatePath)
        {
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token));

            if (string.IsNullOrEmpty(baseUrl)) throw new ArgumentNullException(nameof(baseUrl));

            if (string.IsNullOrEmpty(emailTemplatePath)) throw new ArgumentNullException(nameof(emailTemplatePath));

            var result = new Result();

            var confirmEmailResult = new ConfirmEmailResult();

            try
            {
                var emailConfirmationResponse = await _emailConfirmationsRepository.Get(token);

                if (emailConfirmationResponse.Success)
                {
                    var emailConfirmation = (EmailConfirmation)emailConfirmationResponse.Object;

                    var license = (await CacheFactory.GetLicenseWithCacheAsync(
                        _appsRepository,
                        _distributedCache,
                        string.Format(CacheKeys.GetAppLicenseCacheKey, emailConfirmation.AppId),
                        CachingStrategy.Medium,
                        emailConfirmation.AppId)).Item1;

                    if (!emailConfirmation.IsUpdate)
                    {
                        var response = await CacheFactory.ConfirmEmailWithCacheAsync(
                            _usersRepository,
                            _distributedCache,
                            emailConfirmation,
                            license);

                        if (response.Success)
                        {
                            var removeEmailConfirmationResponse = await _emailConfirmationsRepository
                                .Delete(emailConfirmation);

                            var user = (User)response.Object;

                            result.IsSuccess = response.Success;
                            confirmEmailResult.UserName = user.UserName;
                            confirmEmailResult.Email = user.Email;
                            confirmEmailResult.DateUpdated = user.DateUpdated;
                            confirmEmailResult.NewEmailAddressConfirmed = true;
                            confirmEmailResult.IsUpdate = emailConfirmation.IsUpdate;

                            confirmEmailResult.AppTitle = user
                                .Apps
                                .Where(ua => ua.AppId == emailConfirmation.AppId)
                                .Select(ua => ua.App.Name)
                                .FirstOrDefault();

                            if (user
                                .Apps
                                .Where(ua => ua.AppId == emailConfirmation.AppId)
                                .Select(ua => ua.App.Environment == ReleaseEnvironment.LOCAL)
                                .FirstOrDefault())
                            {
                                confirmEmailResult.Url = user
                                    .Apps
                                    .Where(ua => ua.AppId == emailConfirmation.AppId)
                                    .Select(ua => ua.App.LocalUrl)
                                    .FirstOrDefault();
                            }
                            else if (user
                                .Apps
                                .Where(ua => ua.AppId == emailConfirmation.AppId)
                                .Select(ua => ua.App.Environment == ReleaseEnvironment.DEV)
                                .FirstOrDefault())
                            {
                                confirmEmailResult.Url = user
                                    .Apps
                                    .Where(ua => ua.AppId == emailConfirmation.AppId)
                                    .Select(ua => ua.App.DevUrl)
                                    .FirstOrDefault();
                            }
                            else if (user
                                .Apps
                                .Where(ua => ua.AppId == emailConfirmation.AppId)
                                .Select(ua => ua.App.Environment == ReleaseEnvironment.QA)
                                .FirstOrDefault())
                            {
                                confirmEmailResult.Url = user
                                    .Apps
                                    .Where(ua => ua.AppId == emailConfirmation.AppId)
                                    .Select(ua => ua.App.QaUrl)
                                    .FirstOrDefault();
                            }
                            else
                            {
                                confirmEmailResult.Url = user
                                    .Apps
                                    .Where(ua => ua.AppId == emailConfirmation.AppId)
                                    .Select(ua => ua.App.ProdUrl)
                                    .FirstOrDefault();
                            }

                            result.Message = UsersMessages.EmailConfirmedMessage;

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
                            result.Message = UsersMessages.EmailNotConfirmedMessage;

                            return result;
                        }
                    }
                    else if (emailConfirmation.IsUpdate && !(bool)emailConfirmation.OldEmailAddressConfirmed)
                    {
                        var response = await CacheFactory.UpdateEmailWithCacheAsync(
                            _usersRepository,
                            _distributedCache,
                            emailConfirmation,
                            license);

                        var user = (User)response.Object;
                        var app = (App)(await _appsRepository.Get(emailConfirmation.AppId)).Object;

                        if (response.Success)
                        {
                            var html = File.ReadAllText(emailTemplatePath);

                            var url = string.Empty;

                            if (app.Environment == ReleaseEnvironment.LOCAL)
                            {
                                url = app.LocalUrl;
                            }
                            else if (app.Environment == ReleaseEnvironment.DEV)
                            {
                                url = app.DevUrl;
                            }
                            else if (app.Environment == ReleaseEnvironment.QA)
                            {
                                url = app.QaUrl;
                            }
                            else
                            {
                                url = app.ProdUrl;
                            }

                            string EmailConfirmationAction;

                            if (!app.DisableCustomUrls && !string.IsNullOrEmpty(app.CustomEmailConfirmationAction))
                            {
                                EmailConfirmationAction = string.Format("{0}/{1}/{2}",
                                    url,
                                    app.CustomEmailConfirmationAction,
                                    emailConfirmation.Token);
                            }
                            else
                            {
                                EmailConfirmationAction = string.Format("https://{0}/confirmEmail/{1}",
                                    baseUrl,
                                    emailConfirmation.Token);
                            }

                            var appTitle = app.Name;

                            html = html.Replace("{{USER_NAME}}", user.UserName);
                            html = html.Replace("{{CONFIRM_EMAIL_URL}}", EmailConfirmationAction);
                            html = html.Replace("{{APP_TITLE}}", appTitle);
                            html = html.Replace("{{URL}}", url);

                            var emailSubject = string.Format("Greetings from {0}: Please Confirm New Email", appTitle);

                            confirmEmailResult.ConfirmationEmailSuccessfullySent = _emailService
                                .Send(user.Email, emailSubject, html);

                            emailConfirmation.OldEmailAddressConfirmed = true;

                            emailConfirmation = (EmailConfirmation)(await _emailConfirmationsRepository.Update(emailConfirmation)).Object;

                            result.IsSuccess = response.Success;
                            result.Message = UsersMessages.OldEmailConfirmedMessage;
                            confirmEmailResult.UserName = user.UserName;
                            confirmEmailResult.Email = user.Email;
                            confirmEmailResult.DateUpdated = user.DateUpdated;
                            confirmEmailResult.IsUpdate = emailConfirmation.IsUpdate;
                            confirmEmailResult.AppTitle = appTitle;
                            confirmEmailResult.Url = url;
                            result.DataPacket.Add(confirmEmailResult);

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
                            result.Message = UsersMessages.OldEmailNotConfirmedMessage;

                            return result;
                        }
                    }
                    else
                    {
                        var response = await CacheFactory.ConfirmEmailWithCacheAsync(
                            _usersRepository,
                            _distributedCache,
                            emailConfirmation,
                            license);

                        if (response.Success)
                        {
                            var removeEmailConfirmationResponse = await _emailConfirmationsRepository.Delete(emailConfirmation);

                            var user = (User)response.Object;

                            result.IsSuccess = response.Success;
                            confirmEmailResult.Email = user.Email;
                            confirmEmailResult.UserName = user.UserName;
                            confirmEmailResult.DateUpdated = user.DateUpdated;
                            confirmEmailResult.IsUpdate = emailConfirmation.IsUpdate;
                            confirmEmailResult.NewEmailAddressConfirmed = true;
                            confirmEmailResult.AppTitle = user
                                .Apps
                                .Where(ua => ua.AppId == emailConfirmation.AppId)
                                .Select(ua => ua.App.Name)
                                .FirstOrDefault();


                            if (user
                                .Apps
                                .Where(ua => ua.AppId == emailConfirmation.AppId)
                                .Select(ua => ua.App.Environment == ReleaseEnvironment.LOCAL)
                                .FirstOrDefault())
                            {
                                confirmEmailResult.Url = user
                                    .Apps
                                    .Where(ua => ua.AppId == emailConfirmation.AppId)
                                    .Select(ua => ua.App.LocalUrl)
                                    .FirstOrDefault();
                            }
                            else if (user
                                .Apps
                                .Where(ua => ua.AppId == emailConfirmation.AppId)
                                .Select(ua => ua.App.Environment == ReleaseEnvironment.DEV)
                                .FirstOrDefault())
                            {
                                confirmEmailResult.Url = user
                                    .Apps
                                    .Where(ua => ua.AppId == emailConfirmation.AppId)
                                    .Select(ua => ua.App.DevUrl)
                                    .FirstOrDefault();
                            }
                            else if (user
                                .Apps
                                .Where(ua => ua.AppId == emailConfirmation.AppId)
                                .Select(ua => ua.App.Environment == ReleaseEnvironment.QA)
                                .FirstOrDefault())
                            {
                                confirmEmailResult.Url = user
                                    .Apps
                                    .Where(ua => ua.AppId == emailConfirmation.AppId)
                                    .Select(ua => ua.App.QaUrl)
                                    .FirstOrDefault();
                            }
                            else
                            {
                                confirmEmailResult.Url = user
                                    .Apps
                                    .Where(ua => ua.AppId == emailConfirmation.AppId)
                                    .Select(ua => ua.App.ProdUrl)
                                    .FirstOrDefault();
                            }

                            result.Message = UsersMessages.EmailConfirmedMessage;

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
                            result.Message = UsersMessages.EmailNotConfirmedMessage;

                            return result;
                        }
                    }
                }
                else if (!emailConfirmationResponse.Success && emailConfirmationResponse.Exception != null)
                {
                    result.IsSuccess = emailConfirmationResponse.Success;
                    result.Message = emailConfirmationResponse.Exception.Message;

                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = UsersMessages.EmailNotConfirmedMessage;

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

        public async Task<IResult> CancelEmailConfirmationRequest(int id, int appId)
        {
            var result = new Result();

            var userResult = new UserResult();

            if (id == 0 || appId == 0)
            {
                result.IsSuccess = false;
                result.Message = UsersMessages.EmailConfirmationRequestNotCancelledMessage;

                return result;
            }

            try
            {
                var license = await _appsRepository.GetLicense(appId);

                var cacheFactoryResponse = await CacheFactory.GetWithCacheAsync(
                    _usersRepository,
                    _distributedCache,
                    string.Format(CacheKeys.GetUserCacheKey, id, license),
                    CachingStrategy.Medium,
                    id);

                var userResponse = (RepositoryResponse)cacheFactoryResponse.Item1;

                if (userResponse.Success)
                {
                    if (await _emailConfirmationsRepository.HasOutstandingEmailConfirmation(id, appId))
                    {
                        var user = (User)userResponse.Object;

                        var emailConfirmation = (EmailConfirmation)
                            (await _emailConfirmationsRepository.RetrieveEmailConfirmation(id, appId))
                            .Object;

                        var response = await _emailConfirmationsRepository.Delete(emailConfirmation);

                        if (response.Success)
                        {
                            // Role back email request
                            user.Email = emailConfirmation.OldEmailAddress;
                            user.ReceivedRequestToUpdateEmail = false;
                            user.IsEmailConfirmed = true;

                            userResult.User = (User)(await CacheFactory.UpdateWithCacheAsync<User>(
                                _usersRepository,
                                _distributedCache,
                                user,
                                license)).Object;

                            result.IsSuccess = response.Success;
                            result.Message = UsersMessages.EmailConfirmationRequestCancelledMessage;
                            result.DataPacket.Add(userResult);

                            return result;
                        }
                        else if (response.Success == false && response.Exception != null)
                        {
                            userResult.User = (User)(await _usersRepository.Update(user)).Object;
                            result.IsSuccess = response.Success;
                            result.Message = response.Exception.Message;
                            result.DataPacket.Add(userResult);

                            return result;
                        }
                        else
                        {
                            userResult.User = (User)(await _usersRepository.Update(user)).Object;
                            result.IsSuccess = false;
                            result.Message = UsersMessages.EmailConfirmationRequestNotCancelledMessage;
                            result.DataPacket.Add(userResult);

                            return result;
                        }
                    }
                    else
                    {
                        userResult.User = (User)(await _usersRepository.Get(id)).Object;
                        result.IsSuccess = false;
                        result.Message = UsersMessages.EmailConfirmationRequestNotFoundMessage;
                        result.DataPacket.Add(userResult);

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
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> CancelAllEmailRequests(int id, int appId)
        {
            var result = new Result();
            var userResult = new UserResult();

            if (id == 0 || appId == 0)
            {
                result.IsSuccess = false;
                result.Message = UsersMessages.UserNotFoundMessage + " or " + AppsMessages.AppNotFoundMessage;

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
                    string.Format(CacheKeys.GetUserCacheKey, id, app.License),
                    CachingStrategy.Medium,
                    id);

                var userResponse = (RepositoryResponse)cacheFactoryResponse.Item1;

                if (userResponse.Success)
                {
                    if (await _appsRepository.HasEntity(appId))
                    {
                        var emailConfirmationResponse = await _emailConfirmationsRepository.RetrieveEmailConfirmation(id, appId);
                        var passwordResetResponse = await _passwordResetsRepository.RetrievePasswordReset(id, appId);
                        var user = (User)userResponse.Object;

                        if (emailConfirmationResponse.Success || passwordResetResponse.Success)
                        {
                            if (emailConfirmationResponse.Success)
                            {
                                var emailConfirmation = (EmailConfirmation)emailConfirmationResponse.Object;

                                var response = await _emailConfirmationsRepository.Delete(emailConfirmation);

                                if (response.Success)
                                {
                                    // Role back email request
                                    user.Email = emailConfirmation.OldEmailAddress;
                                    user.ReceivedRequestToUpdateEmail = false;
                                    user.IsEmailConfirmed = true;

                                    user = (User)(await CacheFactory.UpdateWithCacheAsync<User>(
                                        _usersRepository,
                                        _distributedCache,
                                        user,
                                        app.License)).Object;

                                    result.IsSuccess = response.Success;
                                    result.Message = UsersMessages.EmailConfirmationRequestCancelledMessage;
                                }
                                else if (response.Success == false && response.Exception != null)
                                {
                                    result.IsSuccess = response.Success;
                                    result.Message = response.Exception.Message;
                                }
                                else
                                {
                                    result.IsSuccess = false;
                                    result.Message = UsersMessages.EmailConfirmationRequestNotCancelledMessage;
                                }
                            }

                            if (passwordResetResponse.Success)
                            {
                                var passwordReset = (PasswordReset)passwordResetResponse.Object;

                                var response = await _passwordResetsRepository.Delete(passwordReset);

                                if (response.Success)
                                {
                                    // Role back password reset
                                    user.ReceivedRequestToUpdatePassword = false;

                                    user = (User)(await CacheFactory.UpdateWithCacheAsync<User>(
                                        _usersRepository,
                                        _distributedCache,
                                        user,
                                        app.License)).Object;

                                    result.IsSuccess = response.Success;
                                    result.Message = string.IsNullOrEmpty(result.Message) ?
                                        UsersMessages.PasswordResetRequestCancelledMessage :
                                        string.Format("{0} and {1}", result.Message, UsersMessages.PasswordResetRequestCancelledMessage);
                                }
                                else if (response.Success == false && response.Exception != null)
                                {
                                    result.IsSuccess = result.IsSuccess ? result.IsSuccess : response.Success;
                                    result.Message = string.IsNullOrEmpty(result.Message) ?
                                        response.Exception.Message :
                                        string.Format("{0} and {1}", result.Message, response.Exception.Message);
                                }
                                else
                                {
                                    result.IsSuccess = false;
                                    result.Message = string.IsNullOrEmpty(result.Message) ?
                                        UsersMessages.PasswordResetRequestNotCancelledMessage :
                                        string.Format("{0} and {1}", result.Message, UsersMessages.PasswordResetRequestNotCancelledMessage);
                                }
                            }

                            userResult.User = user;
                            result.DataPacket.Add(userResult);
                            return result;
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Message = UsersMessages.EmailRequestsNotFoundMessage;

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
                else
                {
                    result.IsSuccess = false;
                    result.Message = UsersMessages.UserNotFoundMessage;

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

        public async Task<IResult> InitiatePasswordReset(string token, string license)
        {
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token));

            var result = new Result();

            var initiatePasswordResetResult = new InitiatePasswordResetResult();

            try
            {
                var passwordResetResponse = await _passwordResetsRepository.Get(token);

                if (passwordResetResponse.Success)
                {
                    var passwordReset = (PasswordReset)passwordResetResponse.Object;

                    var cacheFactoryResponse = await CacheFactory.GetWithCacheAsync<User>(
                        _usersRepository,
                        _distributedCache,
                        string.Format(CacheKeys.GetUserCacheKey, passwordReset.UserId, license),
                        CachingStrategy.Medium,
                        passwordReset.UserId);

                    var userResponse = (RepositoryResponse)cacheFactoryResponse.Item1;

                    if (userResponse.Success)
                    {
                        var user = (User)userResponse.Object;

                        cacheFactoryResponse = await CacheFactory.GetWithCacheAsync<App>(
                            _appsRepository,
                            _distributedCache,
                            string.Format(CacheKeys.GetAppCacheKey, passwordReset.AppId),
                            CachingStrategy.Medium,
                            passwordReset.AppId);

                        var appResponse = (RepositoryResponse)cacheFactoryResponse.Item1;

                        if (appResponse.Success)
                        {
                            var app = (App)appResponse.Object;

                            if (user.Apps.Any(ua => ua.AppId == app.Id))
                            {
                                if (user.ReceivedRequestToUpdatePassword)
                                {
                                    result.IsSuccess = true;
                                    result.Message = UsersMessages.UserFoundMessage;
                                    initiatePasswordResetResult.User = user;
                                    initiatePasswordResetResult.App = app;

                                    return result;
                                }
                                else
                                {
                                    result.IsSuccess = false;
                                    result.Message = UsersMessages.NoOutstandingRequestToResetPassworMessage;

                                    return result;
                                }
                            }
                            else
                            {
                                result.IsSuccess = false;
                                result.Message = AppsMessages.UserNotSignedUpToAppMessage;

                                return result;
                            }
                        }
                        else if (!appResponse.Success && appResponse.Exception != null)
                        {
                            result.IsSuccess = passwordResetResponse.Success;
                            result.Message = passwordResetResponse.Exception.Message;

                            return result;
                        }
                        else
                        {
                            result.IsSuccess = appResponse.Success;
                            result.Message = AppsMessages.AppNotFoundMessage;

                            return result;
                        }
                    }
                    else if (!userResponse.Success && userResponse.Exception != null)
                    {
                        result.IsSuccess = passwordResetResponse.Success;
                        result.Message = passwordResetResponse.Exception.Message;

                        return result;
                    }
                    else
                    {
                        result.IsSuccess = userResponse.Success;
                        result.Message = UsersMessages.UserNotFoundMessage;

                        return result;
                    }
                }
                else if (!passwordResetResponse.Success && passwordResetResponse.Exception != null)
                {
                    result.IsSuccess = passwordResetResponse.Success;
                    result.Message = passwordResetResponse.Exception.Message;

                    return result;
                }
                else
                {
                    result.IsSuccess = passwordResetResponse.Success;
                    result.Message = UsersMessages.PasswordResetRequestNotFoundMessage;

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

        public async Task<IResult> ResendPasswordReset(
            int userId, 
            int appId, 
            string baseUrl, 
            string emailTemplatePath)
        {
            if (string.IsNullOrEmpty(baseUrl)) throw new ArgumentException(nameof(baseUrl));

            if (string.IsNullOrEmpty(emailTemplatePath)) throw new ArgumentException(nameof(emailTemplatePath));

            var result = new Result();

            if (userId == 0 || appId == 0)
            {
                result.IsSuccess = false;
                result.Message = UsersMessages.PasswordResetRequestNotFoundMessage;

                return result;
            }

            try
            {
                var cacheFactoryResponse = await CacheFactory.GetWithCacheAsync<App>(
                    _appsRepository,
                    _distributedCache,
                    string.Format(CacheKeys.GetAppCacheKey, appId),
                    CachingStrategy.Medium,
                    appId);

                var appResponse = (RepositoryResponse)cacheFactoryResponse.Item1;

                if (appResponse.Success)
                {
                    var app = (App)((await _appsRepository.Get(appId)).Object);
                    app.License = await _appsRepository.GetLicense(app.Id);

                    cacheFactoryResponse = await CacheFactory.GetWithCacheAsync(
                        _usersRepository,
                        _distributedCache,
                        string.Format(CacheKeys.GetUserCacheKey, userId, app.License),
                        CachingStrategy.Medium,
                        userId);

                    var userResponse = (RepositoryResponse)cacheFactoryResponse.Item1;

                    if (userResponse.Success)
                    {
                        var user = (User)userResponse.Object;

                        if (user.ReceivedRequestToUpdatePassword)
                        {
                            if (await _passwordResetsRepository.HasOutstandingPasswordReset(userId, appId))
                            {
                                var passwordReset = (PasswordReset)
                                    ((await _passwordResetsRepository.RetrievePasswordReset(userId, appId)).Object);

                                string EmailConfirmationAction;

                                if (app.UseCustomPasswordResetAction)
                                {
                                    if (app.Environment == ReleaseEnvironment.LOCAL)
                                    {
                                        EmailConfirmationAction = string.Format("{0}/{1}/{2}",
                                            app.LocalUrl,
                                            app.CustomPasswordResetAction,
                                            passwordReset.Token);
                                    }
                                    else if (app.Environment == ReleaseEnvironment.DEV)
                                    {
                                        EmailConfirmationAction = string.Format("{0}/{1}/{2}",
                                            app.DevUrl,
                                            app.CustomPasswordResetAction,
                                            passwordReset.Token);
                                    }
                                    else if (app.Environment == ReleaseEnvironment.QA)
                                    {
                                        EmailConfirmationAction = string.Format("{0}/{1}/{2}",
                                            app.QaUrl,
                                            app.CustomPasswordResetAction,
                                            passwordReset.Token);
                                    }
                                    else
                                    {
                                        EmailConfirmationAction = string.Format("{0}/{1}/{2}",
                                            app.ProdUrl,
                                            app.CustomPasswordResetAction,
                                            passwordReset.Token);
                                    }
                                }
                                else
                                {
                                    EmailConfirmationAction = string.Format("https://{0}/passwordReset/{1}",
                                        baseUrl,
                                        passwordReset.Token);
                                }

                                var html = File.ReadAllText(emailTemplatePath);
                                var appTitle = app.Name;
                                var url = string.Empty;

                                if (app.Environment == ReleaseEnvironment.LOCAL)
                                {
                                    url = app.LocalUrl;
                                }
                                else if (app.Environment == ReleaseEnvironment.DEV)
                                {
                                    url = app.DevUrl;
                                }
                                else if (app.Environment == ReleaseEnvironment.QA)
                                {
                                    url = app.QaUrl;
                                }
                                else
                                {
                                    url = app.ProdUrl;
                                }

                                html = html.Replace("{{USER_NAME}}", user.UserName);
                                html = html.Replace("{{CONFIRM_EMAIL_URL}}", EmailConfirmationAction);
                                html = html.Replace("{{APP_TITLE}}", appTitle);
                                html = html.Replace("{{URL}}", url);

                                var emailSubject = string.Format("Greetings from {0}: Password Update Request Received", appTitle);

                                result.IsSuccess = _emailService
                                    .Send(user.Email, emailSubject, html);

                                if (result.IsSuccess)
                                {
                                    result.Message = UsersMessages.PasswordResetEmailResentMessage;

                                    return result;
                                }
                                else
                                {
                                    result.IsSuccess = false;
                                    result.Message = UsersMessages.PasswordResetEmailNotResentMessage;

                                    return result;
                                }
                            }
                            else
                            {
                                result.IsSuccess = false;
                                result.Message = UsersMessages.NoOutstandingRequestToResetPassworMessage;

                                return result;
                            }
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Message = UsersMessages.NoOutstandingRequestToResetPassworMessage;

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
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
            }
        }

        public async Task<IResult> RequestPasswordReset(
            IRequest request, 
            string baseUrl, 
            string emailTemplatePath)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrEmpty(baseUrl)) throw new ArgumentNullException(nameof(baseUrl));

            if (string.IsNullOrEmpty(emailTemplatePath)) throw new ArgumentNullException(nameof(emailTemplatePath));

            var result = new Result();

            RequestPasswordResetRequest requestPasswordResetRequest;

            if (request.DataPacket is RequestPasswordResetRequest r)
            {
                requestPasswordResetRequest = r;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = ServicesMesages.InvalidRequestMessage;

                return result;
            }

            try
            {
                var cacheFactoryResponse = await CacheFactory.GetAppByLicenseWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(CacheKeys.GetAppByLicenseCacheKey, request.License),
                    CachingStrategy.Medium,
                    request.License);

                var appResponse = (RepositoryResponse)cacheFactoryResponse.Item1;
                var app = (App)appResponse.Object;

                app.License = (await CacheFactory.GetLicenseWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(CacheKeys.GetAppLicenseCacheKey, app.Id),
                    CachingStrategy.Heavy,
                    app.Id)).Item1;

                if (appResponse.Success)
                {
                    cacheFactoryResponse = await CacheFactory.GetByEmailWithCacheAsync(
                        _usersRepository,
                        _distributedCache,
                        string.Format(CacheKeys.GetUserByEmailCacheKey, requestPasswordResetRequest.Email, app.License),
                        CachingStrategy.Medium,
                        requestPasswordResetRequest.Email);

                    var userResponse = (RepositoryResponse)cacheFactoryResponse.Item1;

                    if (userResponse.Success)
                    {
                        var user = (User)userResponse.Object;
                        PasswordReset passwordReset;

                        if (user.Apps.Any(ua => ua.AppId == app.Id))
                        {
                            if (!user.IsEmailConfirmed)
                            {
                                result.IsSuccess = false;
                                result.Message = UsersMessages.UserEmailNotConfirmedMessage;

                                return result;
                            }

                            if (await _passwordResetsRepository.HasOutstandingPasswordReset(user.Id, app.Id))
                            {
                                passwordReset = (PasswordReset)(await _passwordResetsRepository.RetrievePasswordReset(
                                    user.Id,
                                    app.Id)).Object;

                                passwordReset = await EnsurePasswordResetTokenIsUnique(passwordReset);

                                passwordReset = (PasswordReset)(await _passwordResetsRepository.Update(passwordReset)).Object;

                                if (!user.ReceivedRequestToUpdatePassword)
                                {
                                    user.ReceivedRequestToUpdatePassword = true;

                                    user = (User)(await CacheFactory.UpdateWithCacheAsync<User>(
                                        _usersRepository,
                                        _distributedCache,
                                        user,
                                        app.License)).Object;
                                }

                                return SendPasswordResetEmail(
                                    user,
                                    app,
                                    passwordReset,
                                    emailTemplatePath,
                                    baseUrl,
                                    result,
                                    false);
                            }
                            else
                            {
                                passwordReset = new PasswordReset(user.Id, app.Id);

                                passwordReset = await EnsurePasswordResetTokenIsUnique(passwordReset);

                                var passwordResetResponse = await _passwordResetsRepository.Create(passwordReset);

                                if (passwordResetResponse.Success)
                                {
                                    user.ReceivedRequestToUpdatePassword = true;

                                    user = (User)(await CacheFactory.UpdateWithCacheAsync<User>(
                                        _usersRepository,
                                        _distributedCache,
                                        user,
                                        app.License)).Object;

                                    return SendPasswordResetEmail(
                                        user,
                                        app,
                                        passwordReset,
                                        emailTemplatePath,
                                        baseUrl,
                                        result,
                                        true);
                                }
                                else if (!passwordResetResponse.Success && passwordResetResponse.Exception != null)
                                {
                                    result.IsSuccess = passwordResetResponse.Success;
                                    result.Message = passwordResetResponse.Exception.Message;

                                    return result;
                                }
                                else
                                {
                                    result.IsSuccess = userResponse.Success;
                                    result.Message = UsersMessages.UnableToProcessPasswordResetRequesMessage;

                                    return result;
                                }
                            }
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Message = AppsMessages.UserNotSignedUpToAppMessage;

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
                        result.Message = UsersMessages.NoUserIsUsingThisEmailMessage;

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

        public async Task<IResult> UpdatePassword(IRequest request, string license)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = new Result();

            PasswordResetRequest passwordResetRequest;

            if (request.DataPacket is PasswordResetRequest r)
            {
                passwordResetRequest = r;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = ServicesMesages.InvalidRequestMessage;

                return result;
            }

            var salt = BCrypt.Net.BCrypt.GenerateSalt();

            try
            {
                var cacheFactoryResponse = await CacheFactory.GetWithCacheAsync(
                    _usersRepository,
                    _distributedCache,
                    string.Format(CacheKeys.GetUserCacheKey, passwordResetRequest.UserId, license),
                    CachingStrategy.Medium,
                    passwordResetRequest.UserId);

                var userResponse = (RepositoryResponse)cacheFactoryResponse.Item1;
                var user = (User)userResponse.Object;

                cacheFactoryResponse = await CacheFactory.GetAppByLicenseWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(CacheKeys.GetAppByLicenseCacheKey, license),
                    CachingStrategy.Medium,
                    license);

                var appResponse = (RepositoryResponse)cacheFactoryResponse.Item1;
                var app = (App)appResponse.Object;

                app.License = (await CacheFactory.GetLicenseWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(CacheKeys.GetAppLicenseCacheKey, app.Id),
                    CachingStrategy.Heavy,
                    app.Id)).Item1;


                if (userResponse.Success)
                {
                    if (user.ReceivedRequestToUpdatePassword)
                    {
                        user.Password = BCrypt.Net.BCrypt
                                .HashPassword(passwordResetRequest.NewPassword, salt);

                        user.DateUpdated = DateTime.UtcNow;

                        user.ReceivedRequestToUpdatePassword = false;

                        var updateUserResponse = await CacheFactory.UpdateWithCacheAsync<User>(
                            _usersRepository,
                            _distributedCache,
                            user,
                            app.License);

                        if (updateUserResponse.Success)
                        {
                            var passwordResetReponse = await _passwordResetsRepository
                                .RetrievePasswordReset(
                                    user.Id, 
                                    app.Id);

                            _ = await _passwordResetsRepository
                                .Delete((PasswordReset)passwordResetReponse.Object);

                            user = (User)updateUserResponse.Object;

                            result.IsSuccess = userResponse.Success;
                            result.Message = UsersMessages.PasswordResetMessage;

                            return result;
                        }
                        else if (!updateUserResponse.Success && updateUserResponse.Exception != null)
                        {
                            result.IsSuccess = userResponse.Success;
                            result.Message = userResponse.Exception.Message;

                            return result;
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Message = UsersMessages.PasswordNotResetMessage;

                            return result;
                        }
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = UsersMessages.NoOutstandingRequestToResetPassworMessage;

                        return result;
                    }
                }
                else if (!userResponse.Success && userResponse.Exception != null)
                {
                    result.IsSuccess = false;
                    result.Message = UsersMessages.UserNotFoundMessage;

                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = UsersMessages.UserNotFoundMessage;

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

        public async Task<IResult> CancelPasswordResetRequest(int id, int appId)
        {
            var result = new Result();

            var userResult = new UserResult();

            if (id == 0 || appId == 0)
            {
                result.IsSuccess = false;
                result.Message = UsersMessages.PasswordResetRequestNotCancelledMessage;

                return result;
            }

            try
            {
                var license = await _appsRepository.GetLicense(appId);

                var cacheFactoryResponse = await CacheFactory.GetWithCacheAsync<User>(
                    _usersRepository,
                    _distributedCache,
                    string.Format(CacheKeys.GetUserCacheKey, id, license),
                    CachingStrategy.Medium,
                    id);

                var userResponse = (RepositoryResponse)cacheFactoryResponse.Item1;

                if (userResponse.Success)
                {
                    if (await _passwordResetsRepository.HasOutstandingPasswordReset(id, appId))
                    {
                        var user = (User)userResponse.Object;

                        var passwordReset = (PasswordReset)
                            (await _passwordResetsRepository.RetrievePasswordReset(id, appId))
                            .Object;

                        var response = await _passwordResetsRepository.Delete(passwordReset);

                        if (response.Success)
                        {
                            // Role back password reset
                            user.ReceivedRequestToUpdatePassword = false;

                            userResult.User = (User)(await CacheFactory.UpdateWithCacheAsync<User>(
                                _usersRepository,
                                _distributedCache,
                                user,
                                license)).Object;

                            result.IsSuccess = response.Success;
                            result.Message = UsersMessages.PasswordResetRequestCancelledMessage;
                            result.DataPacket.Add(userResult);

                            return result;
                        }
                        else if (response.Success == false && response.Exception != null)
                        {
                            userResult.User = (User)(await _usersRepository.Update(user)).Object;
                            result.IsSuccess = response.Success;
                            result.Message = response.Exception.Message;
                            result.DataPacket.Add(userResult);

                            return result;
                        }
                        else
                        {
                            userResult.User = (User)(await _usersRepository.Update(user)).Object;
                            result.IsSuccess = false;
                            result.Message = UsersMessages.PasswordResetRequestNotCancelledMessage;
                            result.DataPacket.Add(userResult);

                            return result;
                        }
                    }
                    else
                    {
                        userResult.User = (User)(await _usersRepository.Get(id)).Object;
                        result.IsSuccess = false;
                        result.Message = UsersMessages.PasswordResetRequestNotFoundMessage;
                            result.DataPacket.Add(userResult);

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
            catch (Exception exp)
            {
                result.IsSuccess = false;
                result.Message = exp.Message;

                return result;
            }
        }
        
        private async Task<EmailConfirmation> EnsureEmailConfirmationTokenIsUnique(EmailConfirmation emailConfirmation)
        {
            try
            {
                var emailConfirmationResponse = await _emailConfirmationsRepository.GetAll();

                if (emailConfirmationResponse.Success)
                {
                    bool tokenNotUnique;

                    var emailConfirmations = emailConfirmationResponse
                        .Objects
                        .ConvertAll(ec => (EmailConfirmation)ec);

                    do
                    {
                        if (emailConfirmations
                            .Any(ec => ec.Token.ToLower()
                            .Equals(emailConfirmation.Token.ToLower()) && ec.Id != emailConfirmation.Id))
                        {
                            tokenNotUnique = true;

                            emailConfirmation.Token = Guid.NewGuid().ToString();
                        }
                        else
                        {
                            tokenNotUnique = false;
                        }

                    } while (tokenNotUnique);
                }

                return emailConfirmation;
            }
            catch
            {
                throw;
            }
        }

        private async Task<PasswordReset> EnsurePasswordResetTokenIsUnique(PasswordReset passwordReset)
        {
            try
            {
                var passwordResetResponse = await _passwordResetsRepository.GetAll();

                if (passwordResetResponse.Success)
                {
                    bool tokenUnique;

                    var passwordResets = passwordResetResponse
                        .Objects
                        .ConvertAll(pu => (PasswordReset)pu);

                    do
                    {
                        if (passwordResets
                            .Where(pw => pw.Id != passwordReset.Id)
                            .ToList()
                            .Count > 0)
                        {
                            if (passwordResets
                                .Where(pw => pw.Id != passwordReset.Id)
                                .Any(pw => pw.Token.ToLower().Equals(passwordReset.Token.ToLower())))
                            {
                                tokenUnique = false;

                                passwordReset.Token = Guid.NewGuid().ToString();
                            }
                            else
                            {
                                tokenUnique = true;
                            }
                        }
                        else
                        {
                            passwordReset.Token = Guid.NewGuid().ToString();

                            tokenUnique = true;
                        }

                    } while (!tokenUnique);
                }
                else
                {
                    if (passwordReset.Id != 0)
                    {
                        passwordReset.Token = Guid.NewGuid().ToString();
                    }
                }

                return passwordReset;
            }
            catch
            {
                throw;
            }
        }

        private Result SendPasswordResetEmail(
            User user,
            App app,
            PasswordReset passwordReset,
            string emailTemplatePath,
            string baseUrl,
            Result result,
            bool newRequest)
        {
            try
            {
                string EmailConfirmationAction;

                if (app.UseCustomPasswordResetAction)
                {
                    string emailUrl;

                    if (app.Environment == ReleaseEnvironment.LOCAL)
                    {
                        emailUrl = app.LocalUrl;
                    }
                    else if (app.Environment == ReleaseEnvironment.DEV)
                    {
                        emailUrl = app.DevUrl;
                    }
                    else if (app.Environment == ReleaseEnvironment.QA)
                    {
                        emailUrl = app.QaUrl;
                    }
                    else
                    {
                        emailUrl = app.DevUrl;
                    }

                    EmailConfirmationAction = string.Format("{0}/{1}/{2}",
                        emailUrl,
                        app.CustomPasswordResetAction,
                        passwordReset.Token);
                }
                else
                {
                    EmailConfirmationAction = string.Format("https://{0}/passwordReset/{1}",
                        baseUrl,
                        passwordReset.Token);
                }

                var html = File.ReadAllText(emailTemplatePath);
                var appTitle = app.Name;
                string url;

                if (app.Environment == ReleaseEnvironment.LOCAL)
                {
                    url = app.LocalUrl;
                }
                else if (app.Environment == ReleaseEnvironment.DEV)
                {
                    url = app.DevUrl;
                }
                else if (app.Environment == ReleaseEnvironment.QA)
                {
                    url = app.QaUrl;
                }
                else
                {
                    url = app.DevUrl;
                }

                html = html.Replace("{{USER_NAME}}", user.UserName);
                html = html.Replace("{{CONFIRM_EMAIL_URL}}", EmailConfirmationAction);
                html = html.Replace("{{APP_TITLE}}", appTitle);
                html = html.Replace("{{URL}}", url);

                var emailSubject = string.Format("Greetings from {0}: Password Update Request Received", appTitle);

                result.IsSuccess = _emailService
                    .Send(user.Email, emailSubject, html);

                if (result.IsSuccess)
                {
                    if (newRequest)
                    {
                        result.Message = UsersMessages.ProcessedPasswordResetRequestMessage;
                    }
                    else
                    {
                        result.Message = UsersMessages.ResentPasswordResetRequestMessage;
                    }

                    return result;
                }
                else
                {
                    result.Message = UsersMessages.UnableToProcessPasswordResetRequesMessage;

                    return result;
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
