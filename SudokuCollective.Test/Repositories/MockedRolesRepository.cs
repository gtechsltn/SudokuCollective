using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.ServiceModels;
using SudokuCollective.Core.Interfaces.Models;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models;

namespace SudokuCollective.Test.Repositories
{
    public class MockedRolesRepository
    {
        private readonly DatabaseContext context;
        internal Mock<IRolesRepository<Role>> SuccessfulRequest { get; set; }
        internal Mock<IRolesRepository<Role>> FailedRequest { get; set; }

        public MockedRolesRepository(DatabaseContext ctxt)
        {
            context = ctxt;

            SuccessfulRequest = new Mock<IRolesRepository<Role>>();
            FailedRequest = new Mock<IRolesRepository<Role>>();

            SuccessfulRequest.Setup(rolesRepo =>
                rolesRepo.Add(It.IsAny<Role>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = new Role() { RoleLevel = RoleLevel.NULL }
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(rolesRepo =>
                rolesRepo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Roles.FirstOrDefault(r => r.Id == 3)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(rolesRepo =>
                rolesRepo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context.Roles
                            .Where(r => r.RoleLevel != RoleLevel.NULL && r.RoleLevel != RoleLevel.SUPERUSER)
                            .ToList()
                            .ConvertAll(r => (IDomainEntity)r)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(rolesRepo =>
                rolesRepo.Update(It.IsAny<Role>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Roles.FirstOrDefault(r => r.Id == 3)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(rolesRepo =>
                rolesRepo.UpdateRange(It.IsAny<List<Role>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(rolesRepo =>
                rolesRepo.Delete(It.IsAny<Role>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(rolesRepo =>
                rolesRepo.DeleteRange(It.IsAny<List<Role>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(rolesRepo =>
                rolesRepo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(rolesRepo =>
                rolesRepo.HasRoleLevel(It.IsAny<RoleLevel>()))
                    .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(rolesRepo =>
                rolesRepo.IsListValid(It.IsAny<List<int>>()))
                    .Returns(Task.FromResult(true));

            FailedRequest.Setup(rolesRepo =>
                rolesRepo.Add(It.IsAny<Role>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(rolesRepo =>
                rolesRepo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(rolesRepo =>
                rolesRepo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(rolesRepo =>
                rolesRepo.Update(It.IsAny<Role>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(rolesRepo =>
                rolesRepo.UpdateRange(It.IsAny<List<Role>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(rolesRepo =>
                rolesRepo.Delete(It.IsAny<Role>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(rolesRepo =>
                rolesRepo.DeleteRange(It.IsAny<List<Role>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(rolesRepo =>
                rolesRepo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(false));

            FailedRequest.Setup(rolesRepo =>
                rolesRepo.HasRoleLevel(It.IsAny<RoleLevel>()))
                    .Returns(Task.FromResult(false));

            FailedRequest.Setup(rolesRepo =>
                rolesRepo.IsListValid(It.IsAny<List<int>>()))
                    .Returns(Task.FromResult(false));
        }
    }
}
