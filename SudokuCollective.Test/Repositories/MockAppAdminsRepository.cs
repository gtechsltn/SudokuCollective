using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using SudokuCollective.Core.Interfaces.DataModels;
using SudokuCollective.Core.Interfaces.Models;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models;

namespace SudokuCollective.Test.Repositories
{
    public class MockAppAdminsRepository
    {
        private readonly DatabaseContext context;
        internal Mock<IAppAdminsRepository<AppAdmin>> SuccessfulRequest { get; set; }
        internal Mock<IAppAdminsRepository<AppAdmin>> FailedRequest { get; set; }
        internal Mock<IAppAdminsRepository<AppAdmin>> PromoteUserRequest { get; set; }

        public MockAppAdminsRepository(DatabaseContext ctxt)
        {
            context = ctxt;

            SuccessfulRequest = new Mock<IAppAdminsRepository<AppAdmin>>();
            FailedRequest = new Mock<IAppAdminsRepository<AppAdmin>>();
            PromoteUserRequest = new Mock<IAppAdminsRepository<AppAdmin>>();

            SuccessfulRequest.Setup(appAdminsRepo =>
                appAdminsRepo.Add(It.IsAny<AppAdmin>()))
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    {
                        Success = true,
                        Object = new AppAdmin() { 
                            Id = 2,
                            AppId = 1,
                            UserId = 2,
                            IsActive = true }
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(appAdminsRepo =>
                appAdminsRepo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.AppAdmins.FirstOrDefault(aa => aa.Id == 1)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(appAdminsRepo =>
                appAdminsRepo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    {
                        Success = true,
                        Objects = context.AppAdmins
                            .ToList()
                            .ConvertAll(aa => (IDomainEntity)aa)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(appAdminsRepo =>
                appAdminsRepo.Update(It.IsAny<AppAdmin>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Difficulties.FirstOrDefault(d => d.Id == 1)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(appAdminsRepo =>
                appAdminsRepo.UpdateRange(It.IsAny<List<AppAdmin>>()))
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    { 
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(appAdminsRepo =>
                appAdminsRepo.Delete(It.IsAny<AppAdmin>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(appAdminsRepo =>
                appAdminsRepo.DeleteRange(It.IsAny<List<AppAdmin>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(appAdminsRepo =>
                appAdminsRepo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(appAdminsRepo =>
                appAdminsRepo.HasAdminRecord(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(appAdminsRepo =>
                appAdminsRepo.GetAdminRecord(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.AppAdmins.FirstOrDefault(aa => aa.Id == 1)
                    } as IRepositoryResponse));

            FailedRequest.Setup(appAdminsRepo =>
                appAdminsRepo.Add(It.IsAny<AppAdmin>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(appAdminsRepo =>
                appAdminsRepo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(appAdminsRepo =>
                appAdminsRepo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(appAdminsRepo =>
                appAdminsRepo.Update(It.IsAny<AppAdmin>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(appAdminsRepo =>
                appAdminsRepo.UpdateRange(It.IsAny<List<AppAdmin>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(appAdminsRepo =>
                appAdminsRepo.Delete(It.IsAny<AppAdmin>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(appAdminsRepo =>
                appAdminsRepo.DeleteRange(It.IsAny<List<AppAdmin>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(appAdminsRepo =>
                appAdminsRepo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(false));

            FailedRequest.Setup(appAdminsRepo =>
                appAdminsRepo.HasAdminRecord(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(false));

            FailedRequest.Setup(appAdminsRepo =>
                appAdminsRepo.GetAdminRecord(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            PromoteUserRequest.Setup(appAdminsRepo =>
                appAdminsRepo.Add(It.IsAny<AppAdmin>()))
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

            PromoteUserRequest.Setup(appAdminsRepo =>
                appAdminsRepo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.AppAdmins.FirstOrDefault(aa => aa.Id == 1)
                    } as IRepositoryResponse));

            PromoteUserRequest.Setup(appAdminsRepo =>
                appAdminsRepo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context.AppAdmins
                            .ToList()
                            .ConvertAll(aa => (IDomainEntity)aa)
                    } as IRepositoryResponse));

            PromoteUserRequest.Setup(appAdminsRepo =>
                appAdminsRepo.Update(It.IsAny<AppAdmin>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Difficulties.FirstOrDefault(d => d.Id == 1)
                    } as IRepositoryResponse));

            PromoteUserRequest.Setup(appAdminsRepo =>
                appAdminsRepo.UpdateRange(It.IsAny<List<AppAdmin>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            PromoteUserRequest.Setup(appAdminsRepo =>
                appAdminsRepo.Delete(It.IsAny<AppAdmin>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            PromoteUserRequest.Setup(appAdminsRepo =>
                appAdminsRepo.DeleteRange(It.IsAny<List<AppAdmin>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            PromoteUserRequest.Setup(appAdminsRepo =>
                appAdminsRepo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            PromoteUserRequest.Setup(appAdminsRepo =>
                appAdminsRepo.HasAdminRecord(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(false));

            PromoteUserRequest.Setup(appAdminsRepo =>
                appAdminsRepo.GetAdminRecord(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.AppAdmins.FirstOrDefault(aa => aa.Id == 1)
                    } as IRepositoryResponse));
        }
    }
}
