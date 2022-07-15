﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SudokuCollective.Core.Interfaces.ServiceModels;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Interfaces.Models;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Core.Models;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Settings;

namespace SudokuCollective.Core.Interfaces.Cache
{
    public interface ICacheService
    {
        Task<IRepositoryResponse> AddWithCacheAsync<T>(
            IRepository<T> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            ICacheKeys keys,
            T entity) where T : IDomainEntity;
        Task<Tuple<IRepositoryResponse, IResult>> GetWithCacheAsync<T>(
            IRepository<T> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            int id,
            IResult result = null) where T : IDomainEntity;
        Task<Tuple<IRepositoryResponse, IResult>> GetAllWithCacheAsync<T>(
            IRepository<T> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            IResult result = null) where T : IDomainEntity;
        Task<IRepositoryResponse> UpdateWithCacheAsync<T>(
            IRepository<T> repo,
            IDistributedCache cache,
            ICacheKeys keys,
            T entity,
            string license = null) where T : IDomainEntity;
        Task<IRepositoryResponse> DeleteWithCacheAsync<T>(
            IRepository<T> repo,
            IDistributedCache cache,
            ICacheKeys keys,
            T entity,
            string license = null) where T : IDomainEntity;
        Task<bool> HasEntityWithCacheAsync<T>(
            IRepository<T> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            int id) where T : IDomainEntity;
        Task RemoveKeysAsync(
            IDistributedCache cache,
            List<string> keys);
        Task<Tuple<IRepositoryResponse, IResult>> GetAppByLicenseWithCacheAsync(
            IAppsRepository<App> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            string license,
            IResult result = null);
        Task<Tuple<IRepositoryResponse, IResult>> GetAppUsersWithCacheAsync(
            IAppsRepository<App> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            int id,
            IResult result = null);
        Task<Tuple<IRepositoryResponse, IResult>> GetNonAppUsersWithCacheAsync(
            IAppsRepository<App> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            int id,
            IResult result = null);
        Task<Tuple<IRepositoryResponse, IResult>> GetMyAppsWithCacheAsync(
            IAppsRepository<App> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            int ownerId,
            IResult result = null);
        Task<Tuple<IRepositoryResponse, IResult>> GetMyRegisteredAppsWithCacheAsync(
            IAppsRepository<App> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            int userId,
            IResult result = null);
        Task<Tuple<string, IResult>> GetLicenseWithCacheAsync(
            IAppsRepository<App> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            ICacheKeys keys,
            int id,
            IResult result = null);
        Task<IRepositoryResponse> ResetWithCacheAsync(
            IAppsRepository<App> repo,
            IDistributedCache cache,
            ICacheKeys keys,
            App app);
        Task<IRepositoryResponse> ActivatetWithCacheAsync(
            IAppsRepository<App> repo,
            IDistributedCache cache,
            ICacheKeys keys,
            int id);
        Task<IRepositoryResponse> DeactivatetWithCacheAsync(
            IAppsRepository<App> repo,
            IDistributedCache cache,
            ICacheKeys keys,
            int id);
        Task<bool> IsAppLicenseValidWithCacheAsync(
            IAppsRepository<App> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            string license);
        Task<Tuple<IRepositoryResponse, IResult>> GetByUserNameWithCacheAsync(
            IUsersRepository<User> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            ICacheKeys keys,
            string username,
            string license = null,
            IResult result = null);
        Task<Tuple<IRepositoryResponse, IResult>> GetByEmailWithCacheAsync(
            IUsersRepository<User> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            string email,
            IResult result = null);
        Task<IRepositoryResponse> ConfirmEmailWithCacheAsync(
            IUsersRepository<User> repo,
            IDistributedCache cache,
            ICacheKeys keys,
            EmailConfirmation emailConfirmation,
            string license);
        Task<IRepositoryResponse> UpdateEmailWithCacheAsync(
            IUsersRepository<User> repo,
            IDistributedCache cache,
            ICacheKeys keys,
            EmailConfirmation emailConfirmation,
            string license);
        Task<bool> IsUserRegisteredWithCacheAsync(
            IUsersRepository<User> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            int id);
        Task<bool> HasDifficultyLevelWithCacheAsync(
            IDifficultiesRepository<Difficulty> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            DifficultyLevel difficultyLevel);
        Task<bool> HasRoleLevelWithCacheAsync(
            IRolesRepository<Role> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            RoleLevel roleLevel);
        Task<Tuple<ISettings, IResult>> GetSettingsAsync(
            IDifficultiesRepository<Difficulty> repo,
            IDistributedCache cache,
            string cacheKey,
            DateTime expiration,
            List<IEnumListItem> releaseEnvironments,
            List<IEnumListItem> sortValues,
            List<IEnumListItem> timeFrames,
            IResult result = null);
    }
}
