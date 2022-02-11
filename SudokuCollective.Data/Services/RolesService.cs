using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SudokuCollective.Core.Interfaces.Cache;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Requests;

namespace SudokuCollective.Data.Services
{
    public class RolesService : IRolesService
    {
        #region Fields
        private readonly IRolesRepository<Role> _rolesRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly ICacheService _cacheService;
        private readonly ICacheKeys _cacheKeys;
        private readonly ICachingStrategy _cachingStrategy;
        #endregion

        #region Constructor
        public RolesService(
            IRolesRepository<Role> rolesRepository,
            IDistributedCache distributedCache,
            ICacheService cacheService,
            ICacheKeys cacheKeys,
            ICachingStrategy cachingStrategy)
        {
            _rolesRepository = rolesRepository;
            _distributedCache = distributedCache;
            _cacheService = cacheService;
            _cacheKeys = cacheKeys;
            _cachingStrategy = cachingStrategy;
        }
        #endregion
        
        #region Methods
        public async Task<IResult> Create(IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = new Result();

            CreateRoleRequest createRoleRequest;

            if (request.DataPacket is CreateRoleRequest r)
            {
                createRoleRequest = r;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = ServicesMesages.InvalidRequestMessage;

                return result;
            }

            if (string.IsNullOrEmpty(createRoleRequest.Name)) throw new ArgumentNullException(nameof(createRoleRequest.Name));

            try
            {
                if (!await _cacheService.HasRoleLevelWithCacheAsync(
                    _rolesRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetRole, createRoleRequest.RoleLevel),
                    _cachingStrategy.Heavy,
                    createRoleRequest.RoleLevel))
                {
                    var role = new Role()
                    {
                        Name = createRoleRequest.Name,
                        RoleLevel = createRoleRequest.RoleLevel
                    };

                    var response = await _cacheService.AddWithCacheAsync(
                        _rolesRepository,
                        _distributedCache,
                        _cacheKeys.GetRole,
                        _cachingStrategy.Heavy,
                        _cacheKeys,
                        role);

                    if (response.Success)
                    {
                        result.IsSuccess = response.Success;
                        result.Message = RolesMessages.RoleCreatedMessage;
                        result.DataPacket.Add((IRole)response.Object);

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
                        result.Message = RolesMessages.RoleNotCreatedMessage;

                        return result;
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = RolesMessages.RoleAlreadyExistsMessage;

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
                result.Message = RolesMessages.RoleNotFoundMessage;

                return result;
            }

            try
            {
                var cacheServiceResponse = await _cacheService.GetWithCacheAsync<Role>(
                    _rolesRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetRole, id),
                    _cachingStrategy.Heavy,
                    id,
                    result);

                var response = (RepositoryResponse)cacheServiceResponse.Item1;
                result = (Result)cacheServiceResponse.Item2;

                if (response.Success)
                {
                    var role = (Role)response.Object;

                    result.IsSuccess = response.Success;
                    result.Message = RolesMessages.RoleFoundMessage;
                    result.DataPacket.Add(role);

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
                    result.Message = RolesMessages.RoleNotFoundMessage;

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

        public async Task<IResult> GetRoles()
        {
            var result = new Result();

            try
            {
                var cacheServiceResponse = await _cacheService.GetAllWithCacheAsync<Role>(
                    _rolesRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetRoles),
                    _cachingStrategy.Heavy,
                    result);

                var response = (RepositoryResponse)cacheServiceResponse.Item1;
                result = (Result)cacheServiceResponse.Item2;

                if (response.Success)
                {
                    var roles = response.Objects.ConvertAll(r => (IRole)r);

                    result.IsSuccess = response.Success;
                    result.Message = RolesMessages.RolesFoundMessage;
                    result.DataPacket.AddRange(roles);

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
                    result.Message = RolesMessages.RolesNotFoundMessage;

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
                result.Message = RolesMessages.RoleNotFoundMessage;

                return result;
            }

            UpdateRoleRequest updateRoleRequest;

            if (request.DataPacket is UpdateRoleRequest r)
            {
                updateRoleRequest = r;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = ServicesMesages.InvalidRequestMessage;

                return result;
            }

            try
            {
                var cacheServiceResponse = await _cacheService.GetWithCacheAsync<Role>(
                    _rolesRepository,
                    _distributedCache,
                    string.Format(_cacheKeys.GetRole, id),
                    _cachingStrategy.Heavy,
                    id,
                    result);

                var response = (RepositoryResponse)cacheServiceResponse.Item1;
                result = (Result)cacheServiceResponse.Item2;

                if (response.Success)
                {
                    var role = (Role)response.Object;

                    role.Name = updateRoleRequest.Name;

                    var updateResponse = await _cacheService.UpdateWithCacheAsync(
                        _rolesRepository,
                        _distributedCache,
                        _cacheKeys,
                        role);

                    if (updateResponse.Success)
                    {
                        result.IsSuccess = updateResponse.Success;
                        result.Message = RolesMessages.RoleUpdatedMessage;

                        return result;
                    }
                    else if (!updateResponse.Success && updateResponse.Exception != null)
                    {
                        result.IsSuccess = updateResponse.Success;
                        result.Message = updateResponse.Exception.Message;

                        return result;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = RolesMessages.RoleNotUpdatedMessage;

                        return result;
                    }

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
                    result.Message = RolesMessages.RoleNotFoundMessage;

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

        public async Task<IResult> Delete(int id)
        {
            var result = new Result();

            if (id == 0)
            {
                result.IsSuccess = false;
                result.Message = RolesMessages.RoleNotFoundMessage;

                return result;
            }

            try
            {
                var response = await _rolesRepository.Get(id);

                if (response.Success)
                {
                    var deleteResponse = await _cacheService.DeleteWithCacheAsync(
                        _rolesRepository,
                        _distributedCache,
                        _cacheKeys,
                        (Role)response.Object);

                    if (deleteResponse.Success)
                    {
                        result.IsSuccess = deleteResponse.Success;
                        result.Message = RolesMessages.RoleDeletedMessage;

                        return result;
                    }
                    else if (!deleteResponse.Success && deleteResponse.Exception != null)
                    {
                        result.IsSuccess = deleteResponse.Success;
                        result.Message = deleteResponse.Exception.Message;

                        return result;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = RolesMessages.RoleNotDeletedMessage;

                        return result;
                    }

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
                    result.Message = RolesMessages.RoleNotFoundMessage;

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
        #endregion
    }
}
