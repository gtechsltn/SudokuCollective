namespace SudokuCollective.Data.Resiliency
{
    internal static class CacheKeys
    {
        internal const string GetAppCacheKey = ":GetApp:{0}";
        internal const string GetAppByLicenseCacheKey = ":GetAppByLicense:{0}";
        internal const string GetAppsCacheKey = ":GetApps";
        internal const string GetMyAppsCacheKey = ":GetMyApps:{0}";
        internal const string GetMyRegisteredCacheKey = ":GetMyRegistered:{0}";
        internal const string GetAppUsersCacheKey = ":GetAppUsers:{0}";
        internal const string GetNonAppUsersCacheKey = ":GetNonAppUsers:{0}";
        internal const string GetAppLicenseCacheKey = ":GetAppLicense:{0}";
        internal const string HasAppCacheKey = ":HasApp:{0}";
        internal const string IsAppLicenseValidCacheKey = ":IsAppLicenseValid:{0}";
        internal const string GetDifficulty = ":GetDifficulty:{0}";
        internal const string GetDifficulties = ":GetDifficutlies";
        internal const string GetUserCacheKey = ":GetUser:{0}-{1}";
        internal const string GetUserByUsernameCacheKey = ":GetUserByUsername:{0}-{1}";
        internal const string GetUserByEmailCacheKey = ":GetUserByEmail:{0}-{1}";
        internal const string GetUsersCacheKey = ":GetUsers";
        internal const string HasUserCacheKey = ":HasUser:{0}";
        internal const string IsUserRegisteredCacheKey = ":IsUserRegistered:{0}-{1}";
        internal const string GetSolutionsCacheKey = ":GetSolutions";
        internal const string GetRole = ":GetRole:{0}";
        internal const string GetRoles = ":GetRoles";
    }
}
