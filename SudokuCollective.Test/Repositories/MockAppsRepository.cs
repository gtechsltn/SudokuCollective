using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.DataModels;
using SudokuCollective.Core.Interfaces.Models;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models;
using SudokuCollective.Test.TestData;

namespace SudokuCollective.Test.Repositories
{
    public class MockAppsRepository
    {
        private readonly DatabaseContext context;
        internal Mock<IAppsRepository<App>> SuccessfulRequest { get; set; }
        internal Mock<IAppsRepository<App>> FailedRequest { get; set; }
        internal Mock<IAppsRepository<App>> InitiatePasswordSuccessfulRequest { get; set; }

        public MockAppsRepository(DatabaseContext ctxt)
        {
            context = ctxt;
            var todaysDate = DateTime.UtcNow;

            SuccessfulRequest = new Mock<IAppsRepository<App>>();
            FailedRequest = new Mock<IAppsRepository<App>>();
            InitiatePasswordSuccessfulRequest = new Mock<IAppsRepository<App>>();

            SuccessfulRequest.Setup(appsRepo =>
                appsRepo.Add(It.IsAny<App>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
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

            SuccessfulRequest.Setup(appsRepo =>
                appsRepo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Apps.FirstOrDefault(a => a.Id == 1)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(appsRepo =>
                appsRepo.GetByLicense(It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Apps.FirstOrDefault(a => a.Id == 1)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(appsRepo =>
                appsRepo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    {
                        Success = true,
                        Objects = context.Apps.ToList().ConvertAll(a => (IDomainEntity)a)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(appsRepo =>
                appsRepo.GetMyApps(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context
                            .Apps
                            .Where(a => a.OwnerId == 1)
                            .ToList()
                            .ConvertAll(a => (IDomainEntity)a)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(appsRepo =>
                appsRepo.GetMyRegisteredApps(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    {
                        Success = true,
                        Objects = context
                            .Apps
                            .Where(a => a.OwnerId == 1)
                            .ToList()
                            .ConvertAll(a => (IDomainEntity)a)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(appsRepo =>
                appsRepo.GetAppUsers(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    {
                        Success = true,
                        Objects = context.Users
                            .Where(user => user.Apps.Any(ua => ua.AppId == 1))
                            .ToList()
                            .ConvertAll(a => (IDomainEntity)a)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(appsRepo =>
                appsRepo.GetNonAppUsers(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context.Users
                            .Where(user => user.Apps.Any(ua => ua.AppId != 1))
                            .ToList()
                            .ConvertAll(a => (IDomainEntity)a)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(appsRepo =>
                appsRepo.Update(It.IsAny<App>()))
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    {
                        Success = true,
                        Object = context.Apps.FirstOrDefault(a => a.Id == 1)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(appsRepo => 
                appsRepo.UpdateRange(It.IsAny<List<App>>()))
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    {
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(appsRepo =>
                appsRepo.AddAppUser(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(appsRepo =>
                appsRepo.RemoveAppUser(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(appsRepo =>
                appsRepo.Delete(It.IsAny<App>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(appsRepo =>
                appsRepo.DeleteRange(It.IsAny<List<App>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(appsRepo =>
                appsRepo.Reset(It.IsAny<App>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(appsRepo =>
                appsRepo.Activate(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(appsRepo =>
                appsRepo.Deactivate(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(appsRepo =>
                appsRepo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(appsRepo =>
                appsRepo.IsAppLicenseValid(It.IsAny<string>()))
                    .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(appsRepo =>
                appsRepo.GetLicense(It.IsAny<int>()))
                    .Returns(Task.FromResult(
                        TestObjects.GetLicense()));

            SuccessfulRequest.Setup(appsRepo =>
                appsRepo.IsUserRegisteredToApp(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(appsRepo =>
                appsRepo.IsUserOwnerOfApp(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            FailedRequest.Setup(appsRepo =>
                appsRepo.Add(It.IsAny<App>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false,
                        Object = null
                    } as IRepositoryResponse));

            FailedRequest.Setup(appsRepo =>
                appsRepo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false,
                        Object = null
                    } as IRepositoryResponse));

            FailedRequest.Setup(appsRepo =>
                appsRepo.GetByLicense(It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false,
                        Object = null
                    } as IRepositoryResponse));

            FailedRequest.Setup(appsRepo =>
                appsRepo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false,
                        Objects = null
                    } as IRepositoryResponse));

            FailedRequest.Setup(appsRepo =>
                appsRepo.GetMyApps(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false,
                        Objects = null
                    } as IRepositoryResponse));

            FailedRequest.Setup(appsRepo =>
                appsRepo.GetMyRegisteredApps(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false,
                        Objects = null
                    } as IRepositoryResponse));

            FailedRequest.Setup(appsRepo =>
                appsRepo.GetAppUsers(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false,
                        Objects = null
                    } as IRepositoryResponse));

            FailedRequest.Setup(appsRepo =>
                appsRepo.GetNonAppUsers(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false,
                        Objects = null
                    } as IRepositoryResponse));

            FailedRequest.Setup(appsRepo =>
                appsRepo.Update(It.IsAny<App>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false,
                        Object = null
                    } as IRepositoryResponse));

            FailedRequest.Setup(appsRepo =>
                appsRepo.UpdateRange(It.IsAny<List<App>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(appsRepo =>
                appsRepo.AddAppUser(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(appsRepo =>
                appsRepo.RemoveAppUser(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(appsRepo =>
                appsRepo.Delete(It.IsAny<App>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(appsRepo =>
                appsRepo.DeleteRange(It.IsAny<List<App>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(appsRepo =>
                appsRepo.Reset(It.IsAny<App>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(appsRepo =>
                appsRepo.Activate(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(appsRepo =>
                appsRepo.Deactivate(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(appsRepo =>
                appsRepo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(false));

            FailedRequest.Setup(appsRepo =>
                appsRepo.IsAppLicenseValid(It.IsAny<string>()))
                    .Returns(Task.FromResult(false));

            FailedRequest.Setup(appsRepo =>
                appsRepo.GetLicense(It.IsAny<int>()))
                    .Returns(Task.FromResult(string.Empty));

            FailedRequest.Setup(appsRepo =>
                appsRepo.IsUserRegisteredToApp(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(false));

            FailedRequest.Setup(appsRepo =>
                appsRepo.IsUserOwnerOfApp(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(false));

            InitiatePasswordSuccessfulRequest.Setup(appsRepo =>
                appsRepo.Add(It.IsAny<App>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
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

            InitiatePasswordSuccessfulRequest.Setup(appsRepo =>
                appsRepo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Apps.FirstOrDefault(a => a.Id == 2)
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(appsRepo =>
                appsRepo.GetByLicense(It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Apps.FirstOrDefault(a => a.Id == 1)
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(appsRepo =>
                appsRepo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context.Apps.ToList().ConvertAll(a => (IDomainEntity)a)
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(appsRepo =>
                appsRepo.GetMyApps(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context
                            .Apps
                            .Where(a => a.OwnerId == 1)
                            .ToList()
                            .ConvertAll(a => (IDomainEntity)a)
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(appsRepo =>
                appsRepo.GetMyRegisteredApps(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false,
                        Objects = null
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(appsRepo =>
                appsRepo.GetAppUsers(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context.Users
                            .Where(user => user.Apps.Any(ua => ua.AppId == 1))
                            .ToList()
                            .ConvertAll(a => (IDomainEntity)a)
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(appsRepo =>
                appsRepo.GetNonAppUsers(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context.Users
                            .Where(user => user.Apps.Any(ua => ua.AppId != 1))
                            .ToList()
                            .ConvertAll(a => (IDomainEntity)a)
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(appsRepo =>
                appsRepo.Update(It.IsAny<App>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Apps.FirstOrDefault(a => a.Id == 1)
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(appsRepo =>
                appsRepo.UpdateRange(It.IsAny<List<App>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(appsRepo =>
                appsRepo.AddAppUser(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(appsRepo =>
                appsRepo.RemoveAppUser(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(appsRepo =>
                appsRepo.Delete(It.IsAny<App>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(appsRepo =>
                appsRepo.DeleteRange(It.IsAny<List<App>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(appsRepo =>
                appsRepo.Reset(It.IsAny<App>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(appsRepo =>
                appsRepo.Activate(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(appsRepo =>
                appsRepo.Deactivate(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(appsRepo =>
                appsRepo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            InitiatePasswordSuccessfulRequest.Setup(appsRepo =>
                appsRepo.IsAppLicenseValid(It.IsAny<string>()))
                    .Returns(Task.FromResult(true));

            InitiatePasswordSuccessfulRequest.Setup(appsRepo =>
                appsRepo.GetLicense(It.IsAny<int>()))
                    .Returns(Task.FromResult(
                        TestObjects.GetLicense()));

            InitiatePasswordSuccessfulRequest.Setup(appsRepo =>
                appsRepo.IsUserRegisteredToApp(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            InitiatePasswordSuccessfulRequest.Setup(appsRepo =>
                appsRepo.IsUserOwnerOfApp(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(true));
        }
    }
}
