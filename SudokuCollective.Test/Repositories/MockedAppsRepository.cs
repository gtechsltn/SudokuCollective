using System;
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
using SudokuCollective.Test.TestData;

namespace SudokuCollective.Test.Repositories
{
    public class MockedAppsRepository
    {
        private readonly DatabaseContext context;
        internal Mock<IAppsRepository<App>> SuccessfulRequest { get; set; }
        internal Mock<IAppsRepository<App>> FailedRequest { get; set; }
        internal Mock<IAppsRepository<App>> InitiatePasswordSuccessfulRequest { get; set; }

        public MockedAppsRepository(DatabaseContext ctxt)
        {
            context = ctxt;
            var todaysDate = DateTime.UtcNow;

            SuccessfulRequest = new Mock<IAppsRepository<App>>();
            FailedRequest = new Mock<IAppsRepository<App>>();
            InitiatePasswordSuccessfulRequest = new Mock<IAppsRepository<App>>();

            SuccessfulRequest.Setup(repo =>
                repo.Add(It.IsAny<App>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Object = new App(
                            3,
                            "Test App 3",
                            "2b789e72-1df3-4313-8091-68cfa8a1db60",
                            1,
                            "https://localhost:8080",
                            "https://testapp3-dev.com",
                            "https://testapp3-qa.com",
                            "https://testapp3.com",
                            true,
                            false,
                            true,
                            ReleaseEnvironment.LOCAL,
                            true,
                            string.Empty,
                            string.Empty,
                            TimeFrame.DAYS,
                            1,
                            todaysDate,
                            DateTime.MinValue)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Object = context.Apps.FirstOrDefault(a => a.Id == 1)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.GetByLicense(It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Object = context.Apps.FirstOrDefault(a => a.Id == 1)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    {
                        IsSuccess = true,
                        Objects = context.Apps.ToList().ConvertAll(a => (IDomainEntity)a)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.GetMyApps(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Objects = context
                            .Apps
                            .Where(a => a.OwnerId == 1)
                            .ToList()
                            .ConvertAll(a => (IDomainEntity)a)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.GetMyRegisteredApps(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    {
                        IsSuccess = true,
                        Objects = context
                            .Apps
                            .Where(a => a.OwnerId == 1)
                            .ToList()
                            .ConvertAll(a => (IDomainEntity)a)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.GetAppUsers(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    {
                        IsSuccess = true,
                        Objects = context.Users
                            .Where(user => user.Apps.Any(ua => ua.AppId == 1))
                            .ToList()
                            .ConvertAll(a => (IDomainEntity)a)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.GetNonAppUsers(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Objects = context.Users
                            .Where(user => user.Apps.Any(ua => ua.AppId != 1))
                            .ToList()
                            .ConvertAll(a => (IDomainEntity)a)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.Update(It.IsAny<App>()))
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    {
                        IsSuccess = true,
                        Object = context.Apps.FirstOrDefault(a => a.Id == 1)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo => 
                repo.UpdateRange(It.IsAny<List<App>>()))
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    {
                        IsSuccess = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.AddAppUser(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.RemoveAppUser(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.Delete(It.IsAny<App>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.DeleteRange(It.IsAny<List<App>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.Reset(It.IsAny<App>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Object = context.Apps.FirstOrDefault(a => a.Id == 1)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.Activate(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Object = context.Apps.FirstOrDefault(a => a.Id == 1)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.Deactivate(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Object = context.Apps.FirstOrDefault(a => a.Id == 1)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(repo =>
                repo.IsAppLicenseValid(It.IsAny<string>()))
                    .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(repo =>
                repo.GetLicense(It.IsAny<int>()))
                    .Returns(Task.FromResult(
                        TestObjects.GetLicense()));

            SuccessfulRequest.Setup(repo =>
                repo.IsUserRegisteredToApp(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(repo =>
                repo.IsUserOwnerOfApp(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            FailedRequest.Setup(repo =>
                repo.Add(It.IsAny<App>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false,
                        Object = null
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false,
                        Object = null
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.GetByLicense(It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false,
                        Object = null
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false,
                        Objects = null
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.GetMyApps(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false,
                        Objects = null
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.GetMyRegisteredApps(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false,
                        Objects = null
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.GetAppUsers(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false,
                        Objects = null
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.GetNonAppUsers(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false,
                        Objects = null
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.Update(It.IsAny<App>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false,
                        Object = null
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.UpdateRange(It.IsAny<List<App>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.AddAppUser(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.RemoveAppUser(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.Delete(It.IsAny<App>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.DeleteRange(It.IsAny<List<App>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.Reset(It.IsAny<App>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.Activate(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.Deactivate(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(false));

            FailedRequest.Setup(repo =>
                repo.IsAppLicenseValid(It.IsAny<string>()))
                    .Returns(Task.FromResult(false));

            FailedRequest.Setup(repo =>
                repo.GetLicense(It.IsAny<int>()))
                    .Returns(Task.FromResult(string.Empty));

            FailedRequest.Setup(repo =>
                repo.IsUserRegisteredToApp(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(false));

            FailedRequest.Setup(repo =>
                repo.IsUserOwnerOfApp(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(false));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.Add(It.IsAny<App>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Object = new App(
                            3,
                            "Test App 3",
                            "2b789e72-1df3-4313-8091-68cfa8a1db60",
                            1,
                            "https://localhost:8080",
                            "https://testapp3-dev.com",
                            "https://testapp3-qa.com",
                            "https://testapp3.com",
                            true,
                            false,
                            true,
                            ReleaseEnvironment.LOCAL,
                            true,
                            string.Empty,
                            string.Empty,
                            TimeFrame.DAYS,
                            1,
                            todaysDate,
                            DateTime.MinValue)
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Object = context.Apps.FirstOrDefault(a => a.Id == 2)
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.GetByLicense(It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Object = context.Apps.FirstOrDefault(a => a.Id == 1)
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Objects = context.Apps.ToList().ConvertAll(a => (IDomainEntity)a)
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.GetMyApps(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Objects = context
                            .Apps
                            .Where(a => a.OwnerId == 1)
                            .ToList()
                            .ConvertAll(a => (IDomainEntity)a)
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.GetMyRegisteredApps(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false,
                        Objects = null
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.GetAppUsers(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Objects = context.Users
                            .Where(user => user.Apps.Any(ua => ua.AppId == 1))
                            .ToList()
                            .ConvertAll(a => (IDomainEntity)a)
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.GetNonAppUsers(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Objects = context.Users
                            .Where(user => user.Apps.Any(ua => ua.AppId != 1))
                            .ToList()
                            .ConvertAll(a => (IDomainEntity)a)
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.Update(It.IsAny<App>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Object = context.Apps.FirstOrDefault(a => a.Id == 1)
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.UpdateRange(It.IsAny<List<App>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.AddAppUser(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.RemoveAppUser(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.Delete(It.IsAny<App>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.DeleteRange(It.IsAny<List<App>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.Reset(It.IsAny<App>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.Activate(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.Deactivate(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.IsAppLicenseValid(It.IsAny<string>()))
                    .Returns(Task.FromResult(true));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.GetLicense(It.IsAny<int>()))
                    .Returns(Task.FromResult(
                        TestObjects.GetLicense()));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.IsUserRegisteredToApp(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.IsUserOwnerOfApp(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(true));
        }
    }
}
