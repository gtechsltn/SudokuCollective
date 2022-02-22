using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using SudokuCollective.Core.Interfaces.ServiceModels;
using SudokuCollective.Core.Interfaces.Models;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models;

namespace SudokuCollective.Test.Repositories
{
    public class MockedAppAdminsRepository
    {
        private readonly DatabaseContext context;
        internal Mock<IAppAdminsRepository<AppAdmin>> SuccessfulRequest { get; set; }
        internal Mock<IAppAdminsRepository<AppAdmin>> FailedRequest { get; set; }
        internal Mock<IAppAdminsRepository<AppAdmin>> PromoteUserRequest { get; set; }

        public MockedAppAdminsRepository(DatabaseContext ctxt)
        {
            context = ctxt;

            SuccessfulRequest = new Mock<IAppAdminsRepository<AppAdmin>>();
            FailedRequest = new Mock<IAppAdminsRepository<AppAdmin>>();
            PromoteUserRequest = new Mock<IAppAdminsRepository<AppAdmin>>();

            SuccessfulRequest.Setup(repo =>
                repo.Add(It.IsAny<AppAdmin>()))
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    {
                        Success = true,
                        Object = new AppAdmin() { 
                            Id = 2,
                            AppId = 1,
                            UserId = 2,
                            IsActive = true }
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.AppAdmins.FirstOrDefault(aa => aa.Id == 1)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    {
                        Success = true,
                        Objects = context.AppAdmins
                            .ToList()
                            .ConvertAll(aa => (IDomainEntity)aa)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.Update(It.IsAny<AppAdmin>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Difficulties.FirstOrDefault(d => d.Id == 1)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.UpdateRange(It.IsAny<List<AppAdmin>>()))
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    { 
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.Delete(It.IsAny<AppAdmin>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.DeleteRange(It.IsAny<List<AppAdmin>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(repo =>
                repo.HasAdminRecord(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(repo =>
                repo.GetAdminRecord(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.AppAdmins.FirstOrDefault(aa => aa.Id == 1)
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.Add(It.IsAny<AppAdmin>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.Update(It.IsAny<AppAdmin>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.UpdateRange(It.IsAny<List<AppAdmin>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.Delete(It.IsAny<AppAdmin>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.DeleteRange(It.IsAny<List<AppAdmin>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(false));

            FailedRequest.Setup(repo =>
                repo.HasAdminRecord(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(false));

            FailedRequest.Setup(repo =>
                repo.GetAdminRecord(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            PromoteUserRequest.Setup(repo =>
                repo.Add(It.IsAny<AppAdmin>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = new AppAdmin()
                        {
                            Id = 2,
                            AppId = 1,
                            UserId = 2,
                            IsActive = true
                        }
                    } as IRepositoryResponse));

            PromoteUserRequest.Setup(repo =>
                repo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.AppAdmins.FirstOrDefault(aa => aa.Id == 1)
                    } as IRepositoryResponse));

            PromoteUserRequest.Setup(repo =>
                repo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context.AppAdmins
                            .ToList()
                            .ConvertAll(aa => (IDomainEntity)aa)
                    } as IRepositoryResponse));

            PromoteUserRequest.Setup(repo =>
                repo.Update(It.IsAny<AppAdmin>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Difficulties.FirstOrDefault(d => d.Id == 1)
                    } as IRepositoryResponse));

            PromoteUserRequest.Setup(repo =>
                repo.UpdateRange(It.IsAny<List<AppAdmin>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            PromoteUserRequest.Setup(repo =>
                repo.Delete(It.IsAny<AppAdmin>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            PromoteUserRequest.Setup(repo =>
                repo.DeleteRange(It.IsAny<List<AppAdmin>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            PromoteUserRequest.Setup(repo =>
                repo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            PromoteUserRequest.Setup(repo =>
                repo.HasAdminRecord(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(false));

            PromoteUserRequest.Setup(repo =>
                repo.GetAdminRecord(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.AppAdmins.FirstOrDefault(aa => aa.Id == 1)
                    } as IRepositoryResponse));
        }
    }
}
