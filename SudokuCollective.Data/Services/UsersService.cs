using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Cache;
using SudokuCollective.Core.Interfaces.ServiceModels;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Extensions;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Payloads;
using SudokuCollective.Data.Models.Results;
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
        private readonly ICacheService _cacheService;
        private readonly ICacheKeys _cacheKeys;
        private readonly ICachingStrategy _cachingStrategy;
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
            IDistributedCache distributedCache,
            ICacheService cacheService,
            ICacheKeys cacheKeys,
            ICachingStrategy cachingStrategy)
        {
            _usersRepository = usersRepository;
            _appsRepository = appsRepository;
            _rolesRepository = rolesRepository;
            _appAdminsRepository = appAdminsRepository;
            _emailConfirmationsRepository = emailConfirmationsRepository;
            _passwordResetsRepository = passwordResetsRepository;
            _emailService = emailService;
            _distributedCache = distributedCache;
            _cacheService = cacheService;
            _cacheKeys = cacheKeys;
            _cachingStrategy = cachingStrategy;
        }
        #endregion

        #region Methods
        public async Task<IResult> Create(
            ISignupRequest request, 
            string baseUrl, 
            string emailTemplatePath)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrEmpty(baseUrl)) throw new ArgumentNullException(nameof(baseUrl));

            if (string.IsNullOrEmpty(emailTemplatePath)) throw new ArgumentNullException(nameof(emailTemplatePath));

            var result = new Result();

            var isUserNameUnique = false;
            var isEmailUnique = false;

            // User name accepsts alphanumeric and special characters except double and single quotes
            var regex = new Regex("^[^-]{1}?[^\"\']*$");

            if (!string.IsNullOrEmpty(request.UserName))
            {
                isUserNameUnique = await _usersRepository.IsUserNameUnique(request.UserName);
            }

            if (!string.IsNullOrEmpty(request.Email))
            {
                isEmailUnique = await _usersRepository.IsEmailUnique(request.Email);
            }

            if (string.IsNullOrEmpty(request.UserName)
                || string.IsNullOrEmpty(request.Email)
                || !isUserNameUnique
                || !isEmailUnique
                || !regex.IsMatch(request.UserName))
            {
                if (string.IsNullOrEmpty(request.UserName))
                {
                    result.IsSuccess = false;
                    result.Message = UsersMessages.UserNameRequiredMessage;

                    return result;
                }
                else if (string.IsNullOrEmpty(request.Email))
                {
                    result.IsSuccess = false;
                    result.Message = UsersMessages.EmailRequiredMessage;

                    return result;
                }
                else if (!regex.IsMatch(request.UserName))
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
                    var cacheServiceResponse = await _cacheService.GetAppByLicenseWithCacheAsync(
                        _appsRepository,
                        _distributedCache,
                        string.Format(_cacheKeys.GetAppByLicenseCacheKey, request.License),
                        _cachingStrategy.Medium,
                        request.License,
                        result);

                    var appResponse = (RepositoryResponse)cacheServiceResponse.Item1;

                    if (appResponse.IsSuccess)
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
                            request.UserName,
                            request.FirstName,
                            request.LastName,
                            request.NickName,
                            request.Email,
                            false,
                            false,
                            BCrypt.Net.BCrypt.HashPassword(request.Password, salt),
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

                        var userResponse = await _cacheService.AddWithCacheAsync<User>(
                            _usersRepository,
                            _distributedCache,
                            _cacheKeys.GetUserCacheKey,
                            _cachingStrategy.Medium,
                            _cacheKeys,
                            user);

                        if (userResponse.IsSuccess)
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
                                else if (app.Environment == ReleaseEnvironment.STAGING)
                                {
                                    EmailConfirmationAction = string.Format("{0}/{1}/{2}",
                                        app.StagingUrl,
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
                            else if (app.Environment == ReleaseEnvironment.STAGING)
                            {
                                url = app.StagingUrl;
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

                            result.IsSuccess = userResponse.IsSuccess;
                            result.Message = UsersMessages.UserCreatedMessage;

                            result.Payload.Add(
                                new EmailConfirmationSentResult() 
                                {
                                    EmailConfirmationSent = _emailService
                                        .Send(user.Email, emailSubject, html)
                                });

                            return result;
                        }
                        else if (!userResponse.IsSuccess && userResponse.Exception != null)
                        {
                            result.IsSuccess = userResponse.IsSuccess;
                            result.Message = userResponse.Exception.Message;

                            return result;
                        }
                        else
                        {
                            result.IsSuccess = userResponse.IsSuccess;
                            result.Message = UsersMessages.UserNotCreatedMessage;

                            return result;
                        }
                    }
                    else if (!appResponse.IsSuccess && appResponse.Exception != null)
                    {
                        result.IsSuccess = appResponse.IsSuccess;
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
                var cacheServiceResponse = await _cacheService.GetWithCacheAsync<User>(
                    _usersRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetUserCacheKey, id, license),
                    _cachingStrategy.Medium,
                    id,
                    result);

                var response = (RepositoryResponse)cacheServiceResponse.Item1;
                result = (Result)cacheServiceResponse.Item2;

                if (response.IsSuccess)
                {
                    var user = (User)response.Object;

                    result.IsSuccess = response.IsSuccess;
                    result.Message = UsersMessages.UserFoundMessage;
                    result.Payload.Add(user);

                    cacheServiceResponse = await _cacheService.GetAppByLicenseWithCacheAsync(
                        _appsRepository,
                        _distributedCache,
                        string.Format(_cacheKeys.GetAppByLicenseCacheKey, license),
                        _cachingStrategy.Medium,
                        license);

                    var app = (App)((RepositoryResponse)cacheServiceResponse.Item1).Object;

                    var appAdmins = (await _appAdminsRepository.GetAll())
                        .Objects
                        .ConvertAll(aa => (AppAdmin)aa)
                        .ToList();

                    if (((User)(result
                        .Payload[0]))
                        .Roles
                        .Any(ur => ur.Role.RoleLevel == RoleLevel.ADMIN))
                    {
                        if (!user.IsSuperUser)
                        {
                            if (!appAdmins.Any(aa =>
                                aa.AppId == app.Id &&
                                aa.UserId == ((User)(result.Payload[0])).Id &&
                                aa.IsActive))
                            {
                                var adminRole = ((User)(result.Payload[0]))
                                    .Roles
                                    .FirstOrDefault(ur =>
                                        ur.Role.RoleLevel == RoleLevel.ADMIN);

                                ((User)(result.Payload[0])).Roles.Remove(adminRole);
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
                else if (!response.IsSuccess && response.Exception != null)
                {
                    result.IsSuccess = response.IsSuccess;
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

            UpdateUserPayload payload;

            try
            {
                if (request.Payload.ConvertToPayloadSuccessful(typeof(UpdateUserPayload), out IPayload conversionResult))
                {
                    payload = (UpdateUserPayload)conversionResult;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = ServicesMesages.InvalidRequestMessage;

                    return result;
                }
            }
            catch (ArgumentException ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;

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

            var isUserNameUnique = await _usersRepository.IsUpdatedUserNameUnique(id, payload.UserName);
            var isEmailUnique = await _usersRepository.IsUpdatedEmailUnique(id, payload.Email);

            if (string.IsNullOrEmpty(payload.UserName)
                || string.IsNullOrEmpty(payload.Email)
                || !isUserNameUnique
                || !isEmailUnique
                || !regex.IsMatch(payload.UserName))
            {
                if (string.IsNullOrEmpty(payload.UserName))
                {
                    result.IsSuccess = false;
                    result.Message = UsersMessages.UserNameRequiredMessage;

                    return result;
                }
                else if (string.IsNullOrEmpty(payload.Email))
                {
                    result.IsSuccess = false;
                    result.Message = UsersMessages.EmailRequiredMessage;

                    return result;
                }
                else if (!regex.IsMatch(payload.UserName))
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
                    var cacheServiceResponse = await _cacheService.GetWithCacheAsync<User>(
                        _usersRepository,
                        _distributedCache,
                        string.Format(_cacheKeys.GetUserCacheKey, id, request.License),
                        _cachingStrategy.Medium,
                        id);

                    var userResponse = (RepositoryResponse)cacheServiceResponse.Item1;

                    if (userResponse.IsSuccess)
                    {
                        var user = (User)userResponse.Object;

                        cacheServiceResponse = await _cacheService.GetWithCacheAsync<App>(
                            _appsRepository,
                            _distributedCache,
                            string.Format(_cacheKeys.GetAppCacheKey, request.AppId),
                            _cachingStrategy.Medium,
                            request.AppId);

                        var app = (App)((RepositoryResponse)cacheServiceResponse.Item1).Object;

                        user.UserName = payload.UserName;
                        user.FirstName = payload.FirstName;
                        user.LastName = payload.LastName;
                        user.NickName = payload.NickName;
                        user.DateUpdated = DateTime.UtcNow;

                        if (!user.Email.ToLower().Equals(payload.Email.ToLower()))
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
                                emailConfirmation.NewEmailAddress = payload.Email;
                            }
                            else
                            {
                                emailConfirmation = new EmailConfirmation(
                                    user.Id,
                                    request.AppId,
                                    user.Email,
                                    payload.Email);
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
                                else if (app.Environment == ReleaseEnvironment.STAGING)
                                {
                                    EmailConfirmationAction = string.Format("{0}/{1}/{2}",
                                        app.StagingUrl,
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
                            else if (app.Environment == ReleaseEnvironment.STAGING)
                            {
                                url = app.StagingUrl;
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

                        var updateUserResponse = await _cacheService.UpdateWithCacheAsync(
                            _usersRepository,
                            _distributedCache,
                            _cacheKeys,
                            user,
                            request.License);

                        if (updateUserResponse.IsSuccess)
                        {
                            userResult.User = (User)updateUserResponse.Object;

                            result.IsSuccess = userResponse.IsSuccess;
                            result.Message = UsersMessages.UserUpdatedMessage;
                            result.Payload.Add(userResult);

                            return result;
                        }
                        else if (!updateUserResponse.IsSuccess && updateUserResponse.Exception != null)
                        {
                            result.IsSuccess = updateUserResponse.IsSuccess;
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
                    else if (!userResponse.IsSuccess && userResponse.Exception != null)
                    {
                        result.IsSuccess = userResponse.IsSuccess;
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

                var cacheServiceResponse = await _cacheService.GetAllWithCacheAsync<User>(
                    _usersRepository,
                    _distributedCache,
                    _cacheKeys.GetUsersCacheKey,
                    _cachingStrategy.Medium,
                    result);

                var userResponse = (RepositoryResponse)cacheServiceResponse.Item1;
                result = (Result)cacheServiceResponse.Item2;

                if (response.IsSuccess)
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

                    cacheServiceResponse = await _cacheService.GetAppByLicenseWithCacheAsync(
                        _appsRepository,
                        _distributedCache,
                        string.Format(_cacheKeys.GetAppByLicenseCacheKey, license),
                        _cachingStrategy.Medium,
                        license);

                    var app = (App)((RepositoryResponse)cacheServiceResponse.Item1).Object;

                    var appAdmins = (await _appAdminsRepository.GetAll())
                        .Objects
                        .ConvertAll(aa => (AppAdmin)aa)
                        .ToList();

                    foreach (var user in result.Payload.ConvertAll(u => (IUser)u))
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

                    cacheServiceResponse = await _cacheService.GetWithCacheAsync<User>(
                        _usersRepository,
                        _distributedCache,
                        string.Format(_cacheKeys.GetUserCacheKey, requestorId, license),
                        _cachingStrategy.Medium,
                        requestorId);

                    var requestor = (User)((RepositoryResponse)cacheServiceResponse.Item1).Object;

                    if (!requestor.IsSuperUser)
                    {
                        // Filter out user emails from the frontend...
                        foreach (var user in result.Payload.ConvertAll(u => (IUser)u))
                        {
                            var emailConfirmed = user.IsEmailConfirmed;
                            user.HideEmail();
                            user.IsEmailConfirmed = emailConfirmed;
                        }
                    }

                    result.IsSuccess = response.IsSuccess;
                    result.Message = UsersMessages.UsersFoundMessage;

                    return result;
                }
                else if (!response.IsSuccess && response.Exception != null)
                {
                    result.IsSuccess = response.IsSuccess;
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
                var cacheServiceResponse = await _cacheService.GetWithCacheAsync<User>(
                    _usersRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetUserCacheKey, id, license),
                    _cachingStrategy.Medium,
                    id);

                var userResponse = (RepositoryResponse)cacheServiceResponse.Item1;

                if (userResponse.IsSuccess)
                {
                    if (((User)userResponse.Object).Id == 1 && ((User)userResponse.Object).IsSuperUser)
                    {
                        result.IsSuccess = false;
                        result.Message = UsersMessages.SuperUserCannotBeDeletedMessage;

                        return result;
                    }

                    var deletionResponse = await _cacheService.DeleteWithCacheAsync<User>(
                        _usersRepository,
                        _distributedCache,
                        _cacheKeys,
                        (User)userResponse.Object);

                    if (deletionResponse.IsSuccess)
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
                    else if (!deletionResponse.IsSuccess && deletionResponse.Exception != null)
                    {
                        result.IsSuccess = deletionResponse.IsSuccess;
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
                else if (!userResponse.IsSuccess && userResponse.Exception != null)
                {
                    result.IsSuccess = userResponse.IsSuccess;
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

                if (response.IsSuccess)
                {
                    var license = (await _cacheService.GetLicenseWithCacheAsync(
                        _appsRepository,
                        _distributedCache,
                        string.Format(_cacheKeys.GetAppLicenseCacheKey, ((PasswordReset)response.Object).AppId),
                        _cachingStrategy.Heavy,
                        _cacheKeys,
                        ((PasswordReset)response.Object).AppId)).Item1;

                    result.Payload.Add((User)((await _cacheService.GetWithCacheAsync<User>(
                        _usersRepository,
                        _distributedCache,
                        string.Format(_cacheKeys.GetUserCacheKey, ((PasswordReset)response.Object).UserId, license),
                        _cachingStrategy.Medium,
                        ((PasswordReset)response.Object).UserId,
                        result)).Item1).Object);

                    result.IsSuccess = response.IsSuccess;
                    result.Message = UsersMessages.UserFoundMessage;

                    return result;
                }
                else if (!response.IsSuccess && response.Exception != null)
                {
                    result.IsSuccess = response.IsSuccess;
                    result.Message = response.Exception.Message;

                    return result;
                }
                else
                {
                    result.IsSuccess = response.IsSuccess;
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

                if (response.IsSuccess)
                {
                    result.License = await _appsRepository.GetLicense(((PasswordReset)response.Object).AppId);
                    result.IsSuccess = response.IsSuccess;
                    result.Message = AppsMessages.AppFoundMessage;

                    return result;
                }
                else if (!response.IsSuccess && response.Exception != null)
                {
                    result.IsSuccess = response.IsSuccess;
                    result.Message = response.Exception.Message;

                    return result;
                }
                else
                {
                    result.IsSuccess = response.IsSuccess;
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

                    var cacheServiceResponse = new Tuple<IRepositoryResponse, IResult>(
                        new RepositoryResponse(), 
                        new Result());

                    if (response.IsSuccess)
                    {
                        var roles = response
                            .Objects
                            .ConvertAll(ur => (UserRole)ur)
                            .ToList();

                        foreach (var role in roles)
                        {
                            if (role.Role.RoleLevel == RoleLevel.ADMIN)
                            {
                                cacheServiceResponse = await _cacheService.GetAppByLicenseWithCacheAsync(
                                    _appsRepository,
                                    _distributedCache,
                                    string.Format(_cacheKeys.GetAppByLicenseCacheKey, license),
                                    _cachingStrategy.Medium,
                                    license);

                                var app = (App)((RepositoryResponse)cacheServiceResponse.Item1).Object;

                                var appAdmin = (AppAdmin)(await _appAdminsRepository.Add(new AppAdmin(app.Id, userid))).Object;
                            }

                            result.Payload.ConvertAll(r => (Role)r).Add(role.Role);
                        }

                        cacheServiceResponse = await _cacheService.GetWithCacheAsync<User>(
                            _usersRepository,
                            _distributedCache,
                            string.Format(_cacheKeys.GetUserCacheKey, userid, license),
                            _cachingStrategy.Medium,
                            userid);

                        var user = (User)((RepositoryResponse)cacheServiceResponse.Item1).Object;

                        // Remove any user cache items which may exist
                        var removeKeys = new List<string> {
                                string.Format(_cacheKeys.GetUserCacheKey, user.Id, license),
                                string.Format(_cacheKeys.GetUserByUsernameCacheKey, user.UserName, license),
                                string.Format(_cacheKeys.GetUserByEmailCacheKey, user.Email, license)
                            };

                        await _cacheService.RemoveKeysAsync(_distributedCache, removeKeys);

                        result.IsSuccess = response.IsSuccess;
                        result.Message = UsersMessages.RolesAddedMessage;

                        return result;
                    }
                    else if (!response.IsSuccess && response.Exception != null)
                    {
                        result.IsSuccess = response.IsSuccess;
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

                    var cacheServiceResponse = new Tuple<IRepositoryResponse, IResult>(
                        new RepositoryResponse(),
                        new Result());

                    if (response.IsSuccess)
                    {
                        var roles = response
                            .Objects
                            .ConvertAll(ur => (UserRole)ur)
                            .ToList();

                        foreach (var role in roles)
                        {
                            if (role.Role.RoleLevel == RoleLevel.ADMIN)
                            {
                                cacheServiceResponse = await _cacheService.GetAppByLicenseWithCacheAsync(
                                    _appsRepository,
                                    _distributedCache,
                                    string.Format(_cacheKeys.GetAppByLicenseCacheKey, license),
                                    _cachingStrategy.Medium,
                                    license);

                                var app = (App)((RepositoryResponse)cacheServiceResponse.Item1).Object;

                                var appAdmin = (AppAdmin)
                                    (await _appAdminsRepository.GetAdminRecord(app.Id, userid))
                                    .Object;

                                _ = await _appAdminsRepository.Delete(appAdmin);
                            }
                        }

                        cacheServiceResponse = await _cacheService.GetWithCacheAsync<User>(
                            _usersRepository,
                            _distributedCache,
                            string.Format(_cacheKeys.GetUserCacheKey, userid, license),
                            _cachingStrategy.Medium,
                            userid);

                        var user = (User)((RepositoryResponse)cacheServiceResponse.Item1).Object;

                        // Remove any user cache items which may exist
                        var removeKeys = new List<string> {
                                string.Format(_cacheKeys.GetUserCacheKey, user.Id, license),
                                string.Format(_cacheKeys.GetUserByUsernameCacheKey, user.UserName, license),
                                string.Format(_cacheKeys.GetUserByEmailCacheKey, user.Email, license)
                            };

                        await _cacheService.RemoveKeysAsync(_distributedCache, removeKeys);

                        result.IsSuccess = response.IsSuccess;
                        result.Message = UsersMessages.RolesRemovedMessage;

                        return result;
                    }
                    else if (!response.IsSuccess && response.Exception != null)
                    {
                        result.IsSuccess = response.IsSuccess;
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
                    var license = (await _cacheService.GetLicenseWithCacheAsync(
                        _appsRepository,
                        _distributedCache,
                        string.Format(_cacheKeys.GetAppLicenseCacheKey, 1),
                        _cachingStrategy.Medium,
                        _cacheKeys,
                        1)).Item1;

                    var cacheServiceResponse = await _cacheService.GetWithCacheAsync(
                        _usersRepository,
                        _distributedCache,
                        string.Format(_cacheKeys.GetUserCacheKey, id, license),
                        _cachingStrategy.Medium,
                        id);

                    var user = (User)((RepositoryResponse)cacheServiceResponse.Item1).Object;

                    // Remove any user cache items which may exist
                    var removeKeys = new List<string> {
                                string.Format(_cacheKeys.GetUserCacheKey, user.Id, license),
                                string.Format(_cacheKeys.GetUserByUsernameCacheKey, user.UserName, license),
                                string.Format(_cacheKeys.GetUserByEmailCacheKey, user.Email, license)
                            };

                    await _cacheService.RemoveKeysAsync(_distributedCache, removeKeys);

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
                    var license = (await _cacheService.GetLicenseWithCacheAsync(
                        _appsRepository,
                        _distributedCache,
                        string.Format(_cacheKeys.GetAppLicenseCacheKey, 1),
                        _cachingStrategy.Medium,
                        _cacheKeys,
                        1)).Item1;

                    var cacheServiceResponse = await _cacheService.GetWithCacheAsync(
                        _usersRepository,
                        _distributedCache,
                        string.Format(_cacheKeys.GetUserCacheKey, id, license),
                        _cachingStrategy.Medium,
                        id);

                    var user = (User)((RepositoryResponse)cacheServiceResponse.Item1).Object;

                    // Remove any user cache items which may exist
                    var removeKeys = new List<string> {
                                string.Format(_cacheKeys.GetUserCacheKey, user.Id, license),
                                string.Format(_cacheKeys.GetUserByUsernameCacheKey, user.UserName, license),
                                string.Format(_cacheKeys.GetUserByEmailCacheKey, user.Email, license)
                            };

                    await _cacheService.RemoveKeysAsync(_distributedCache, removeKeys);

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
                var cacheServiceResponse = await _cacheService.GetWithCacheAsync<User>(
                    _usersRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetUserCacheKey, userId, license),
                    _cachingStrategy.Medium,
                    userId);

                var userResponse = (RepositoryResponse)cacheServiceResponse.Item1;

                if (userResponse.IsSuccess)
                {
                    var user = (User)userResponse.Object;

                    if (!user.IsEmailConfirmed)
                    {
                        cacheServiceResponse = await _cacheService.GetWithCacheAsync<App>(
                            _appsRepository,
                            _distributedCache,
                            string.Format(_cacheKeys.GetAppCacheKey, appId),
                            _cachingStrategy.Medium,
                            appId);

                        var appResponse = (RepositoryResponse)cacheServiceResponse.Item1;

                        if (appResponse.IsSuccess)
                        {
                            var app = (App)appResponse.Object;

                            if (await _emailConfirmationsRepository.HasOutstandingEmailConfirmation(userId, appId))
                            {
                                var emailConfirmationResponse = await _emailConfirmationsRepository.RetrieveEmailConfirmation(userId, appId);

                                if (emailConfirmationResponse.IsSuccess)
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
                                        else if (app.Environment == ReleaseEnvironment.STAGING)
                                        {
                                            EmailConfirmationAction = string.Format("{0}/{1}/{2}",
                                                app.StagingUrl,
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
                                    else if (app.Environment == ReleaseEnvironment.STAGING)
                                    {
                                        url = app.StagingUrl;
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
                                        result.Payload.Add(userResult);

                                        return result;
                                    }
                                    else
                                    {
                                        result.IsSuccess = false;
                                        result.Message = UsersMessages.EmailConfirmationEmailNotResentMessage;

                                        return result;
                                    }

                                }
                                else if (!emailConfirmationResponse.IsSuccess && emailConfirmationResponse.Exception != null)
                                {
                                    result.IsSuccess = emailConfirmationResponse.IsSuccess;
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

                if (emailConfirmationResponse.IsSuccess)
                {
                    var emailConfirmation = (EmailConfirmation)emailConfirmationResponse.Object;

                    var license = (await _cacheService.GetLicenseWithCacheAsync(
                        _appsRepository,
                        _distributedCache,
                        string.Format(_cacheKeys.GetAppLicenseCacheKey, emailConfirmation.AppId),
                        _cachingStrategy.Medium,
                        _cacheKeys,
                        emailConfirmation.AppId)).Item1;

                    if (!emailConfirmation.IsUpdate)
                    {
                        var response = await _cacheService.ConfirmEmailWithCacheAsync(
                            _usersRepository,
                            _distributedCache,
                            _cacheKeys,
                            emailConfirmation,
                            license);

                        if (response.IsSuccess)
                        {
                            var removeEmailConfirmationResponse = await _emailConfirmationsRepository
                                .Delete(emailConfirmation);

                            var user = (User)response.Object;

                            result.IsSuccess = response.IsSuccess;
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
                                .Select(ua => ua.App.Environment == ReleaseEnvironment.STAGING)
                                .FirstOrDefault())
                            {
                                confirmEmailResult.Url = user
                                    .Apps
                                    .Where(ua => ua.AppId == emailConfirmation.AppId)
                                    .Select(ua => ua.App.StagingUrl)
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
                            result.Payload.Add(confirmEmailResult);

                            return result;
                        }
                        else if (!response.IsSuccess && response.Exception != null)
                        {
                            result.IsSuccess = response.IsSuccess;
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
                        var response = await _cacheService.UpdateEmailWithCacheAsync(
                            _usersRepository,
                            _distributedCache,
                            _cacheKeys,
                            emailConfirmation,
                            license);

                        var user = (User)response.Object;
                        var app = (App)(await _appsRepository.Get(emailConfirmation.AppId)).Object;

                        if (response.IsSuccess)
                        {
                            var html = File.ReadAllText(emailTemplatePath);

                            var url = string.Empty;

                            if (app.Environment == ReleaseEnvironment.LOCAL)
                            {
                                url = app.LocalUrl;
                            }
                            else if (app.Environment == ReleaseEnvironment.STAGING)
                            {
                                url = app.StagingUrl;
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

                            result.IsSuccess = response.IsSuccess;
                            result.Message = UsersMessages.OldEmailConfirmedMessage;
                            confirmEmailResult.UserName = user.UserName;
                            confirmEmailResult.Email = user.Email;
                            confirmEmailResult.DateUpdated = user.DateUpdated;
                            confirmEmailResult.IsUpdate = emailConfirmation.IsUpdate;
                            confirmEmailResult.AppTitle = appTitle;
                            confirmEmailResult.Url = url;
                            result.Payload.Add(confirmEmailResult);

                            return result;
                        }
                        else if (!response.IsSuccess && response.Exception != null)
                        {
                            result.IsSuccess = response.IsSuccess;
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
                        var response = await _cacheService.ConfirmEmailWithCacheAsync(
                            _usersRepository,
                            _distributedCache,
                            _cacheKeys,
                            emailConfirmation,
                            license);

                        if (response.IsSuccess)
                        {
                            var removeEmailConfirmationResponse = await _emailConfirmationsRepository.Delete(emailConfirmation);

                            var user = (User)response.Object;

                            result.IsSuccess = response.IsSuccess;
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
                                .Select(ua => ua.App.Environment == ReleaseEnvironment.STAGING)
                                .FirstOrDefault())
                            {
                                confirmEmailResult.Url = user
                                    .Apps
                                    .Where(ua => ua.AppId == emailConfirmation.AppId)
                                    .Select(ua => ua.App.StagingUrl)
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
                        else if (!response.IsSuccess && response.Exception != null)
                        {
                            result.IsSuccess = response.IsSuccess;
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
                else if (!emailConfirmationResponse.IsSuccess && emailConfirmationResponse.Exception != null)
                {
                    result.IsSuccess = emailConfirmationResponse.IsSuccess;
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

                var cacheServiceResponse = await _cacheService.GetWithCacheAsync(
                    _usersRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetUserCacheKey, id, license),
                    _cachingStrategy.Medium,
                    id);

                var userResponse = (RepositoryResponse)cacheServiceResponse.Item1;

                if (userResponse.IsSuccess)
                {
                    if (await _emailConfirmationsRepository.HasOutstandingEmailConfirmation(id, appId))
                    {
                        var user = (User)userResponse.Object;

                        var emailConfirmation = (EmailConfirmation)
                            (await _emailConfirmationsRepository.RetrieveEmailConfirmation(id, appId))
                            .Object;

                        var response = await _emailConfirmationsRepository.Delete(emailConfirmation);

                        if (response.IsSuccess)
                        {
                            // Role back email request
                            user.Email = emailConfirmation.OldEmailAddress;
                            user.ReceivedRequestToUpdateEmail = false;
                            user.IsEmailConfirmed = true;

                            userResult.User = (User)(await _cacheService.UpdateWithCacheAsync<User>(
                                _usersRepository,
                                _distributedCache,
                                _cacheKeys,
                                user,
                                license)).Object;

                            result.IsSuccess = response.IsSuccess;
                            result.Message = UsersMessages.EmailConfirmationRequestCancelledMessage;
                            result.Payload.Add(userResult);

                            return result;
                        }
                        else if (response.IsSuccess == false && response.Exception != null)
                        {
                            userResult.User = (User)(await _usersRepository.Update(user)).Object;
                            result.IsSuccess = response.IsSuccess;
                            result.Message = response.Exception.Message;
                            result.Payload.Add(userResult);

                            return result;
                        }
                        else
                        {
                            userResult.User = (User)(await _usersRepository.Update(user)).Object;
                            result.IsSuccess = false;
                            result.Message = UsersMessages.EmailConfirmationRequestNotCancelledMessage;
                            result.Payload.Add(userResult);

                            return result;
                        }
                    }
                    else
                    {
                        userResult.User = (User)(await _usersRepository.Get(id)).Object;
                        result.IsSuccess = false;
                        result.Message = UsersMessages.EmailConfirmationRequestNotFoundMessage;
                        result.Payload.Add(userResult);

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

                var cacheServiceResponse = await _cacheService.GetWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetAppCacheKey, appId),
                    _cachingStrategy.Medium,
                    appId);

                var appResponse = (RepositoryResponse)cacheServiceResponse.Item1;
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
                    string.Format(_cacheKeys.GetUserCacheKey, id, app.License),
                    _cachingStrategy.Medium,
                    id);

                var userResponse = (RepositoryResponse)cacheServiceResponse.Item1;

                if (userResponse.IsSuccess)
                {
                    if (await _appsRepository.HasEntity(appId))
                    {
                        var emailConfirmationResponse = await _emailConfirmationsRepository.RetrieveEmailConfirmation(id, appId);
                        var passwordResetResponse = await _passwordResetsRepository.RetrievePasswordReset(id, appId);
                        var user = (User)userResponse.Object;

                        if (emailConfirmationResponse.IsSuccess || passwordResetResponse.IsSuccess)
                        {
                            if (emailConfirmationResponse.IsSuccess)
                            {
                                var emailConfirmation = (EmailConfirmation)emailConfirmationResponse.Object;

                                var response = await _emailConfirmationsRepository.Delete(emailConfirmation);

                                if (response.IsSuccess)
                                {
                                    // Role back email request
                                    user.Email = emailConfirmation.OldEmailAddress;
                                    user.ReceivedRequestToUpdateEmail = false;
                                    user.IsEmailConfirmed = true;

                                    user = (User)(await _cacheService.UpdateWithCacheAsync<User>(
                                        _usersRepository,
                                        _distributedCache,
                                        _cacheKeys,
                                        user,
                                        app.License)).Object;

                                    result.IsSuccess = response.IsSuccess;
                                    result.Message = UsersMessages.EmailConfirmationRequestCancelledMessage;
                                }
                                else if (response.IsSuccess == false && response.Exception != null)
                                {
                                    result.IsSuccess = response.IsSuccess;
                                    result.Message = response.Exception.Message;
                                }
                                else
                                {
                                    result.IsSuccess = false;
                                    result.Message = UsersMessages.EmailConfirmationRequestNotCancelledMessage;
                                }
                            }

                            if (passwordResetResponse.IsSuccess)
                            {
                                var passwordReset = (PasswordReset)passwordResetResponse.Object;

                                var response = await _passwordResetsRepository.Delete(passwordReset);

                                if (response.IsSuccess)
                                {
                                    // Role back password reset
                                    user.ReceivedRequestToUpdatePassword = false;

                                    user = (User)(await _cacheService.UpdateWithCacheAsync<User>(
                                        _usersRepository,
                                        _distributedCache,
                                        _cacheKeys,
                                        user,
                                        app.License)).Object;

                                    result.IsSuccess = response.IsSuccess;
                                    result.Message = string.IsNullOrEmpty(result.Message) ?
                                        UsersMessages.PasswordResetRequestCancelledMessage :
                                        string.Format("{0} and {1}", result.Message, UsersMessages.PasswordResetRequestCancelledMessage);
                                }
                                else if (response.IsSuccess == false && response.Exception != null)
                                {
                                    result.IsSuccess = result.IsSuccess ? result.IsSuccess : response.IsSuccess;
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
                            result.Payload.Add(userResult);
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

                if (passwordResetResponse.IsSuccess)
                {
                    var passwordReset = (PasswordReset)passwordResetResponse.Object;

                    var cacheServiceResponse = await _cacheService.GetWithCacheAsync<User>(
                        _usersRepository,
                        _distributedCache,
                        string.Format(_cacheKeys.GetUserCacheKey, passwordReset.UserId, license),
                        _cachingStrategy.Medium,
                        passwordReset.UserId);

                    var userResponse = (RepositoryResponse)cacheServiceResponse.Item1;

                    if (userResponse.IsSuccess)
                    {
                        var user = (User)userResponse.Object;

                        cacheServiceResponse = await _cacheService.GetWithCacheAsync<App>(
                            _appsRepository,
                            _distributedCache,
                            string.Format(_cacheKeys.GetAppCacheKey, passwordReset.AppId),
                            _cachingStrategy.Medium,
                            passwordReset.AppId);

                        var appResponse = (RepositoryResponse)cacheServiceResponse.Item1;

                        if (appResponse.IsSuccess)
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
                                    result.Payload.Add(initiatePasswordResetResult);

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
                        else if (!appResponse.IsSuccess && appResponse.Exception != null)
                        {
                            result.IsSuccess = passwordResetResponse.IsSuccess;
                            result.Message = passwordResetResponse.Exception.Message;

                            return result;
                        }
                        else
                        {
                            result.IsSuccess = appResponse.IsSuccess;
                            result.Message = AppsMessages.AppNotFoundMessage;

                            return result;
                        }
                    }
                    else if (!userResponse.IsSuccess && userResponse.Exception != null)
                    {
                        result.IsSuccess = passwordResetResponse.IsSuccess;
                        result.Message = passwordResetResponse.Exception.Message;

                        return result;
                    }
                    else
                    {
                        result.IsSuccess = userResponse.IsSuccess;
                        result.Message = UsersMessages.UserNotFoundMessage;

                        return result;
                    }
                }
                else if (!passwordResetResponse.IsSuccess && passwordResetResponse.Exception != null)
                {
                    result.IsSuccess = passwordResetResponse.IsSuccess;
                    result.Message = passwordResetResponse.Exception.Message;

                    return result;
                }
                else
                {
                    result.IsSuccess = passwordResetResponse.IsSuccess;
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
                var cacheServiceResponse = await _cacheService.GetWithCacheAsync<App>(
                    _appsRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetAppCacheKey, appId),
                    _cachingStrategy.Medium,
                    appId);

                var appResponse = (RepositoryResponse)cacheServiceResponse.Item1;

                if (appResponse.IsSuccess)
                {
                    var app = (App)((await _appsRepository.Get(appId)).Object);
                    app.License = await _appsRepository.GetLicense(app.Id);

                    cacheServiceResponse = await _cacheService.GetWithCacheAsync(
                        _usersRepository,
                        _distributedCache,
                        string.Format(_cacheKeys.GetUserCacheKey, userId, app.License),
                        _cachingStrategy.Medium,
                        userId);

                    var userResponse = (RepositoryResponse)cacheServiceResponse.Item1;

                    if (userResponse.IsSuccess)
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
                                    else if (app.Environment == ReleaseEnvironment.STAGING)
                                    {
                                        EmailConfirmationAction = string.Format("{0}/{1}/{2}",
                                            app.StagingUrl,
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
                                else if (app.Environment == ReleaseEnvironment.STAGING)
                                {
                                    url = app.StagingUrl;
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

            RequestPasswordResetPayload payload;

            if (request.Payload.ConvertToPayloadSuccessful(typeof(RequestPasswordResetPayload), out IPayload conversionResult))
            {
                payload = (RequestPasswordResetPayload)conversionResult;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = ServicesMesages.InvalidRequestMessage;

                return result;
            }

            try
            {
                var cacheServiceResponse = await _cacheService.GetAppByLicenseWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetAppByLicenseCacheKey, request.License),
                    _cachingStrategy.Medium,
                    request.License);

                var appResponse = (RepositoryResponse)cacheServiceResponse.Item1;
                var app = (App)appResponse.Object;

                app.License = (await _cacheService.GetLicenseWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetAppLicenseCacheKey, app.Id),
                    _cachingStrategy.Heavy,
                    _cacheKeys,
                    app.Id)).Item1;

                if (appResponse.IsSuccess)
                {
                    cacheServiceResponse = await _cacheService.GetByEmailWithCacheAsync(
                        _usersRepository,
                        _distributedCache,
                        string.Format(_cacheKeys.GetUserByEmailCacheKey, payload.Email, app.License),
                        _cachingStrategy.Medium,
                        payload.Email);

                    var userResponse = (RepositoryResponse)cacheServiceResponse.Item1;

                    if (userResponse.IsSuccess)
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

                                    user = (User)(await _cacheService.UpdateWithCacheAsync<User>(
                                        _usersRepository,
                                        _distributedCache,
                                        _cacheKeys,
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

                                if (passwordResetResponse.IsSuccess)
                                {
                                    user.ReceivedRequestToUpdatePassword = true;

                                    user = (User)(await _cacheService.UpdateWithCacheAsync<User>(
                                        _usersRepository,
                                        _distributedCache,
                                        _cacheKeys,
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
                                else if (!passwordResetResponse.IsSuccess && passwordResetResponse.Exception != null)
                                {
                                    result.IsSuccess = passwordResetResponse.IsSuccess;
                                    result.Message = passwordResetResponse.Exception.Message;

                                    return result;
                                }
                                else
                                {
                                    result.IsSuccess = userResponse.IsSuccess;
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
                    else if (!userResponse.IsSuccess && userResponse.Exception != null)
                    {
                        result.IsSuccess = userResponse.IsSuccess;
                        result.Message = userResponse.Exception.Message;

                        return result;
                    }
                    else
                    {
                        result.IsSuccess = userResponse.IsSuccess;
                        result.Message = UsersMessages.NoUserIsUsingThisEmailMessage;

                        return result;
                    }
                }
                else if (!appResponse.IsSuccess && appResponse.Exception != null)
                {
                    result.IsSuccess = appResponse.IsSuccess;
                    result.Message = appResponse.Exception.Message;

                    return result;
                }
                else
                {
                    result.IsSuccess = appResponse.IsSuccess;
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

            PasswordResetPayload payload;

            if (request.Payload.ConvertToPayloadSuccessful(typeof(PasswordResetPayload), out IPayload conversionResult))
            {
                payload = (PasswordResetPayload)conversionResult;
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
                var cacheServiceResponse = await _cacheService.GetWithCacheAsync(
                    _usersRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetUserCacheKey, payload.UserId, license),
                    _cachingStrategy.Medium,
                    payload.UserId);

                var userResponse = (RepositoryResponse)cacheServiceResponse.Item1;
                var user = (User)userResponse.Object;

                cacheServiceResponse = await _cacheService.GetAppByLicenseWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetAppByLicenseCacheKey, license),
                    _cachingStrategy.Medium,
                    license);

                var appResponse = (RepositoryResponse)cacheServiceResponse.Item1;
                var app = (App)appResponse.Object;

                app.License = (await _cacheService.GetLicenseWithCacheAsync(
                    _appsRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetAppLicenseCacheKey, app.Id),
                    _cachingStrategy.Heavy,
                    _cacheKeys,
                    app.Id)).Item1;


                if (userResponse.IsSuccess)
                {
                    if (user.ReceivedRequestToUpdatePassword)
                    {
                        user.Password = BCrypt.Net.BCrypt
                                .HashPassword(payload.NewPassword, salt);

                        user.DateUpdated = DateTime.UtcNow;

                        user.ReceivedRequestToUpdatePassword = false;

                        var updateUserResponse = await _cacheService.UpdateWithCacheAsync<User>(
                            _usersRepository,
                            _distributedCache,
                            _cacheKeys,
                            user,
                            app.License);

                        if (updateUserResponse.IsSuccess)
                        {
                            var passwordResetReponse = await _passwordResetsRepository
                                .RetrievePasswordReset(
                                    user.Id, 
                                    app.Id);

                            _ = await _passwordResetsRepository
                                .Delete((PasswordReset)passwordResetReponse.Object);

                            user = (User)updateUserResponse.Object;

                            result.IsSuccess = userResponse.IsSuccess;
                            result.Message = UsersMessages.PasswordResetMessage;

                            return result;
                        }
                        else if (!updateUserResponse.IsSuccess && updateUserResponse.Exception != null)
                        {
                            result.IsSuccess = userResponse.IsSuccess;
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
                else if (!userResponse.IsSuccess && userResponse.Exception != null)
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

                var cacheServiceResponse = await _cacheService.GetWithCacheAsync<User>(
                    _usersRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetUserCacheKey, id, license),
                    _cachingStrategy.Medium,
                    id);

                var userResponse = (RepositoryResponse)cacheServiceResponse.Item1;

                if (userResponse.IsSuccess)
                {
                    if (await _passwordResetsRepository.HasOutstandingPasswordReset(id, appId))
                    {
                        var user = (User)userResponse.Object;

                        var passwordReset = (PasswordReset)
                            (await _passwordResetsRepository.RetrievePasswordReset(id, appId))
                            .Object;

                        var response = await _passwordResetsRepository.Delete(passwordReset);

                        if (response.IsSuccess)
                        {
                            // Role back password reset
                            user.ReceivedRequestToUpdatePassword = false;

                            userResult.User = (User)(await _cacheService.UpdateWithCacheAsync<User>(
                                _usersRepository,
                                _distributedCache,
                                _cacheKeys,
                                user,
                                license)).Object;

                            result.IsSuccess = response.IsSuccess;
                            result.Message = UsersMessages.PasswordResetRequestCancelledMessage;
                            result.Payload.Add(userResult);

                            return result;
                        }
                        else if (response.IsSuccess == false && response.Exception != null)
                        {
                            userResult.User = (User)(await _usersRepository.Update(user)).Object;
                            result.IsSuccess = response.IsSuccess;
                            result.Message = response.Exception.Message;
                            result.Payload.Add(userResult);

                            return result;
                        }
                        else
                        {
                            userResult.User = (User)(await _usersRepository.Update(user)).Object;
                            result.IsSuccess = false;
                            result.Message = UsersMessages.PasswordResetRequestNotCancelledMessage;
                            result.Payload.Add(userResult);

                            return result;
                        }
                    }
                    else
                    {
                        userResult.User = (User)(await _usersRepository.Get(id)).Object;
                        result.IsSuccess = false;
                        result.Message = UsersMessages.PasswordResetRequestNotFoundMessage;
                            result.Payload.Add(userResult);

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

                if (emailConfirmationResponse.IsSuccess)
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

                if (passwordResetResponse.IsSuccess)
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
                    else if (app.Environment == ReleaseEnvironment.STAGING)
                    {
                        emailUrl = app.StagingUrl;
                    }
                    else if (app.Environment == ReleaseEnvironment.QA)
                    {
                        emailUrl = app.QaUrl;
                    }
                    else
                    {
                        emailUrl = app.StagingUrl;
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
                else if (app.Environment == ReleaseEnvironment.STAGING)
                {
                    url = app.StagingUrl;
                }
                else if (app.Environment == ReleaseEnvironment.QA)
                {
                    url = app.QaUrl;
                }
                else
                {
                    url = app.StagingUrl;
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
