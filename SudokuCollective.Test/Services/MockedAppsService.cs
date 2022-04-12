using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Results;
using SudokuCollective.Test.Repositories;

namespace SudokuCollective.Test.Services
{
    public class MockedAppsService
    {
        internal MockedAppsRepository MockedAppsRepository { get; set; }
        internal MockedUsersRepository MockedUsersRepository { get; set; }
        internal MockedAppAdminsRepository MockedAppAdminsRepository { get; set; }

        internal Mock<IAppsService> SuccessfulRequest { get; set; }
        internal Mock<IAppsService> FailedRequest { get; set; }
        internal Mock<IAppsService> InvalidRequest { get; set; }
        internal Mock<IAppsService> PromoteUserFailsRequest { get; set; }

        public MockedAppsService(DatabaseContext context)
        {
            MockedAppsRepository = new MockedAppsRepository(context);
            MockedUsersRepository = new MockedUsersRepository(context);
            MockedAppAdminsRepository = new MockedAppAdminsRepository(context);

            SuccessfulRequest = new Mock<IAppsService>();
            FailedRequest = new Mock<IAppsService>();
            InvalidRequest = new Mock<IAppsService>();
            PromoteUserFailsRequest = new Mock<IAppsService>();

            SuccessfulRequest.Setup(service =>
                service.Get(
                    It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .Get(It.IsAny<int>())
                        .Result
                        .IsSuccess,
                    Message = AppsMessages.AppFoundMessage,
                    Payload = new List<object>
                        {
                            MockedAppsRepository
                                .SuccessfulRequest
                                .Object
                                .Get(It.IsAny<int>())
                                .Result
                                .Object
                        }
                } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            SuccessfulRequest.Setup(service =>
                service.GetByLicense(
                    It.IsAny<string>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .GetByLicense(It.IsAny<string>())
                        .Result
                        .IsSuccess,
                    Message = AppsMessages.AppFoundMessage,
                    Payload = new List<object>
                        {
                            MockedAppsRepository
                                .SuccessfulRequest
                                .Object
                                .Get(It.IsAny<int>())
                                .Result
                                .Object
                        }
                } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            SuccessfulRequest.Setup(service =>
                service.GetApps(
                    It.IsAny<Paginator>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .GetAll()
                        .Result
                        .IsSuccess,
                    Message = AppsMessages.AppsFoundMessage,
                    Payload = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .GetAll()
                        .Result
                        .Objects
                        .ConvertAll(a => (object)a)
                } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            SuccessfulRequest.Setup(service =>
                service.GetMyApps(
                    It.IsAny<int>(),
                    It.IsAny<Paginator>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .GetAll()
                        .Result
                        .IsSuccess,
                    Message = AppsMessages.AppsFoundMessage,
                    Payload = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .GetAll()
                        .Result
                        .Objects
                        .ConvertAll(a => (object)a)
                } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            SuccessfulRequest.Setup(service =>
                service.GetRegisteredApps(
                    It.IsAny<int>(),
                    It.IsAny<Paginator>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .GetAll()
                        .Result
                        .IsSuccess,
                    Message = AppsMessages.AppsFoundMessage,
                    Payload = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .GetAll()
                        .Result
                        .Objects
                        .ConvertAll(a => (object)a)
                } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            SuccessfulRequest.Setup(appsService =>
                appsService.Create(It.IsAny<ILicenseRequest>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .Add(It.IsAny<App>())
                        .Result
                        .IsSuccess,
                    Message = AppsMessages.AppCreatedMessage,
                    Payload = new List<object>
                        {
                            MockedAppsRepository
                                .SuccessfulRequest
                                .Object
                                .Get(It.IsAny<int>())
                                .Result
                                .Object
                        }
                } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            SuccessfulRequest.Setup(service =>
                service.IsRequestValidOnThisToken(
                    It.IsAny<IHttpContextAccessor>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(service =>
                service.IsOwnerOfThisLicense(
                    It.IsAny<IHttpContextAccessor>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(service =>
                service.Update(It.IsAny<int>(), It.IsAny<IRequest>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .Update(It.IsAny<App>())
                        .Result
                        .IsSuccess,
                    Message = AppsMessages.AppUpdatedMessage,
                    Payload = new List<object>
                        {
                            MockedAppsRepository
                                .SuccessfulRequest
                                .Object
                                .Get(It.IsAny<int>())
                                .Result
                                .Object
                        }
                } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            SuccessfulRequest.Setup(service =>
                service.GetAppUsers(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Paginator>(),
                    It.IsAny<bool>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .GetAppUsers(It.IsAny<int>())
                        .Result
                        .IsSuccess,
                    Message = UsersMessages.UsersFoundMessage,
                    Payload = new List<object>()
                        { 
                            MockedAppsRepository
                                .SuccessfulRequest
                                .Object
                                .GetAppUsers(It.IsAny<int>())
                                .Result
                                .Objects
                                .ConvertAll(u => (object)u)
                        }
                } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            SuccessfulRequest.Setup(service =>
                service.GetLicense(It.IsAny<int>()))
                .Returns(Task.FromResult(new LicenseResult()
                {
                    IsSuccess = true,
                    Message = AppsMessages.AppFoundMessage,
                    License = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .GetLicense(It.IsAny<int>())
                        .Result
                } as ILicenseResult));

            SuccessfulRequest.Setup(service =>
                service.AddAppUser(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .AddAppUser(It.IsAny<int>(), It.IsAny<string>())
                        .Result
                        .IsSuccess,
                    Message = AppsMessages.UserAddedToAppMessage
                } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            SuccessfulRequest.Setup(service =>
                service.RemoveAppUser(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .RemoveAppUser(It.IsAny<int>(), It.IsAny<string>())
                        .Result
                        .IsSuccess,
                    Message = AppsMessages.UserRemovedFromAppMessage
                } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            SuccessfulRequest.Setup(service =>
                service.Activate(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .Activate(It.IsAny<int>())
                        .Result
                        .IsSuccess,
                    Message = AppsMessages.AppActivatedMessage
                } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            SuccessfulRequest.Setup(service =>
                service.Deactivate(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .Deactivate(It.IsAny<int>())
                        .Result
                        .IsSuccess,
                    Message = AppsMessages.AppDeactivatedMessage
                } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            SuccessfulRequest.Setup(service =>
                service.DeleteOrReset(It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .Delete(It.IsAny<App>())
                        .Result
                        .IsSuccess,
                    Message = AppsMessages.AppDeletedMessage
                } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            SuccessfulRequest.Setup(appsService =>
                appsService.ActivateAdminPrivileges(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppAdminsRepository
                        .SuccessfulRequest
                        .Object
                        .Get(It.IsAny<int>())
                        .Result
                        .IsSuccess,
                    Message = UsersMessages.UserHasBeenPromotedToAdminMessage,
                    Payload = new List<object>()
                        {
                             MockedUsersRepository
                            .SuccessfulRequest
                            .Object
                            .Add(It.IsAny<User>())
                            .Result
                            .Object
                        }
                } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            SuccessfulRequest.Setup(appsService =>
                appsService.DeactivateAdminPrivileges(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppAdminsRepository
                        .SuccessfulRequest
                        .Object
                        .Get(It.IsAny<int>())
                        .Result
                        .IsSuccess,
                    Message = AppsMessages.AdminPrivilegesDeactivatedMessage,
                    Payload = new List<object>()
                        {
                            MockedUsersRepository
                                .SuccessfulRequest
                                .Object
                                .Add(It.IsAny<User>())
                                .Result
                                .Object
                        }
                } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            FailedRequest.Setup(service =>
                service.Get(
                    It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .FailedRequest
                            .Object
                            .Get(It.IsAny<int>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.AppNotFoundMessage,
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            FailedRequest.Setup(service =>
                service.GetByLicense(
                    It.IsAny<string>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .FailedRequest
                            .Object
                            .GetByLicense(It.IsAny<string>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.AppNotFoundMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            FailedRequest.Setup(service =>
                service.GetApps(
                    It.IsAny<Paginator>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .FailedRequest
                            .Object
                            .GetAll()
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.AppsNotFoundMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            FailedRequest.Setup(service =>
                service.GetMyApps(
                    It.IsAny<int>(),
                    It.IsAny<Paginator>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .FailedRequest
                            .Object
                            .GetAll()
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.AppsNotFoundMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            FailedRequest.Setup(service =>
                service.GetRegisteredApps(
                    It.IsAny<int>(),
                    It.IsAny<Paginator>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .FailedRequest
                            .Object
                            .GetAll()
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.AppsNotFoundMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            FailedRequest.Setup(appsService =>
                appsService.Create(It.IsAny<ILicenseRequest>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .FailedRequest
                            .Object
                            .Add(It.IsAny<App>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.AppNotCreatedMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            FailedRequest.Setup(service =>
                service.IsRequestValidOnThisToken(
                    It.IsAny<IHttpContextAccessor>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(true));

            FailedRequest.Setup(service =>
                service.IsOwnerOfThisLicense(
                    It.IsAny<IHttpContextAccessor>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(false));

            FailedRequest.Setup(service =>
                service.Update(It.IsAny<int>(), It.IsAny<Request>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .FailedRequest
                            .Object
                            .Update(It.IsAny<App>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.AppNotUpdatedMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            FailedRequest.Setup(service =>
                service.GetAppUsers(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Paginator>(),
                    It.IsAny<bool>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .FailedRequest
                            .Object
                            .GetAppUsers(It.IsAny<int>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.UsersNotFoundMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            FailedRequest.Setup(service =>
                service.GetLicense(It.IsAny<int>()))
                .Returns(Task.FromResult(new LicenseResult()
                    {
                        IsSuccess = false,
                        Message = AppsMessages.AppNotFoundMessage
                    } as ILicenseResult));

            FailedRequest.Setup(service =>
                service.AddAppUser(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .FailedRequest
                            .Object
                            .AddAppUser(It.IsAny<int>(), It.IsAny<string>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.UserNotAddedToAppMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            FailedRequest.Setup(service =>
                service.RemoveAppUser(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .FailedRequest
                            .Object
                            .RemoveAppUser(It.IsAny<int>(), It.IsAny<string>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.UserNotRemovedFromAppMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            FailedRequest.Setup(service =>
                service.Activate(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .FailedRequest
                            .Object
                            .Activate(It.IsAny<int>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.AppNotActivatedMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            FailedRequest.Setup(service =>
                service.Deactivate(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .FailedRequest
                            .Object
                            .Deactivate(It.IsAny<int>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.AppNotDeactivatedMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            FailedRequest.Setup(service =>
                service.DeleteOrReset(It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .FailedRequest
                            .Object
                            .Delete(It.IsAny<App>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.AppNotDeletedMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            FailedRequest.Setup(appsService =>
                appsService.ActivateAdminPrivileges(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppAdminsRepository
                            .FailedRequest
                            .Object
                            .Get(It.IsAny<int>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.UserHasNotBeenPromotedToAdminMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            FailedRequest.Setup(appsService =>
                appsService.DeactivateAdminPrivileges(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppAdminsRepository
                            .FailedRequest
                            .Object
                            .Get(It.IsAny<int>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.DeactivationOfAdminPrivilegesFailedMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            InvalidRequest.Setup(service =>
                service.Get(
                    It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .Get(It.IsAny<int>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.AppFoundMessage,
                        Payload = new List<object>()
                            {
                                MockedAppsRepository
                                    .SuccessfulRequest
                                    .Object
                                    .Get(It.IsAny<int>())
                                    .Result
                                    .Object
                            }
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            InvalidRequest.Setup(service =>
                service.GetByLicense(
                    It.IsAny<string>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .GetByLicense(It.IsAny<string>())
                        .Result
                        .IsSuccess,
                    Message = AppsMessages.AppFoundMessage,
                    Payload = new List<object>()
                            {
                                MockedAppsRepository
                                    .SuccessfulRequest
                                    .Object
                                    .Get(It.IsAny<int>())
                                    .Result
                                    .Object
                            }
                } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            InvalidRequest.Setup(service =>
                service.GetApps(
                    It.IsAny<Paginator>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .GetAll()
                        .Result
                        .IsSuccess,
                    Message = AppsMessages.AppsFoundMessage,
                    Payload = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .GetAll()
                        .Result
                        .Objects
                        .ConvertAll(a => (object)a)
                } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            InvalidRequest.Setup(service =>
                service.GetMyApps(
                    It.IsAny<int>(),
                    It.IsAny<Paginator>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .GetAll()
                        .Result
                        .IsSuccess,
                    Message = AppsMessages.AppsFoundMessage,
                    Payload = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .GetAll()
                        .Result
                        .Objects
                        .ConvertAll(a => (object)a)
                } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            InvalidRequest.Setup(service =>
                service.GetRegisteredApps(
                    It.IsAny<int>(),
                    It.IsAny<Paginator>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .GetAll()
                        .Result
                        .IsSuccess,
                    Message = AppsMessages.AppsFoundMessage,
                    Payload = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .GetAll()
                        .Result
                        .Objects
                        .ConvertAll(a => (object)a)
                } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            InvalidRequest.Setup(appsService =>
                appsService.Create(It.IsAny<ILicenseRequest>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .Add(It.IsAny<App>())
                        .Result
                        .IsSuccess,
                    Message = AppsMessages.AppCreatedMessage,
                    Payload = new List<object>()
                        {
                            MockedAppsRepository
                                .SuccessfulRequest
                                .Object
                                .Add(It.IsAny<App>())
                                .Result
                                .Object
                        }
                } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            InvalidRequest.Setup(service =>
                service.IsRequestValidOnThisToken(
                    It.IsAny<IHttpContextAccessor>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(false));

            InvalidRequest.Setup(service =>
                service.IsOwnerOfThisLicense(
                    It.IsAny<IHttpContextAccessor>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(true));

            InvalidRequest.Setup(service =>
                service.Update(It.IsAny<int>(), It.IsAny<Request>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .Update(It.IsAny<App>())
                        .Result
                        .IsSuccess,
                    Message = AppsMessages.AppUpdatedMessage,
                    Payload = new List<object>()
                        {
                            MockedAppsRepository
                                .SuccessfulRequest
                                .Object
                                .Update(It.IsAny<App>())
                                .Result
                                .Object
                        }
                } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            InvalidRequest.Setup(service =>
                service.GetAppUsers(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Paginator>(),
                    It.IsAny<bool>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .GetAppUsers(It.IsAny<int>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.UsersFoundMessage,
                        Payload = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .GetAppUsers(It.IsAny<int>())
                            .Result
                            .Objects
                            .ConvertAll(u => (object)u)
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            InvalidRequest.Setup(service =>
                service.GetLicense(It.IsAny<int>()))
                .Returns(Task.FromResult(new LicenseResult()
                    {
                        IsSuccess = true,
                        Message = AppsMessages.AppFoundMessage,
                        License = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .GetLicense(It.IsAny<int>())
                            .Result
                    } as ILicenseResult));

            InvalidRequest.Setup(service =>
                service.AddAppUser(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .AddAppUser(It.IsAny<int>(), It.IsAny<string>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.UserAddedToAppMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            InvalidRequest.Setup(service =>
                service.RemoveAppUser(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .RemoveAppUser(It.IsAny<int>(), It.IsAny<string>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.UserRemovedFromAppMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            InvalidRequest.Setup(service =>
                service.Activate(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .Activate(It.IsAny<int>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.AppActivatedMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            InvalidRequest.Setup(service =>
                service.Deactivate(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .Deactivate(It.IsAny<int>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.AppDeactivatedMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            InvalidRequest.Setup(service =>
                service.DeleteOrReset(It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .Delete(It.IsAny<App>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.AppDeletedMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            InvalidRequest.Setup(appsService =>
                appsService.ActivateAdminPrivileges(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppAdminsRepository
                            .FailedRequest
                            .Object
                            .Get(It.IsAny<int>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.AppNotFoundMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            InvalidRequest.Setup(appsService =>
                appsService.DeactivateAdminPrivileges(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppAdminsRepository
                            .SuccessfulRequest
                            .Object
                            .Get(It.IsAny<int>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.AdminPrivilegesDeactivatedMessage,
                        Payload = new List<object>()
                            {
                                MockedUsersRepository
                                    .SuccessfulRequest
                                    .Object
                                    .Update(It.IsAny<User>())
                                    .Result
                                    .Object
                            }
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            PromoteUserFailsRequest.Setup(service =>
                service.Get(
                    It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .Get(It.IsAny<int>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.AppFoundMessage,
                        Payload = new List<object>()
                            {
                                MockedAppsRepository
                                    .SuccessfulRequest
                                    .Object
                                    .Get(It.IsAny<int>())
                                    .Result
                                    .Object
                            }
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            PromoteUserFailsRequest.Setup(service =>
                service.GetByLicense(
                    It.IsAny<string>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .GetByLicense(It.IsAny<string>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.AppFoundMessage,
                        Payload = new List<object>()
                            {
                                MockedAppsRepository
                                    .SuccessfulRequest
                                    .Object
                                    .Get(It.IsAny<int>())
                                    .Result
                                    .Object
                            }
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            PromoteUserFailsRequest.Setup(service =>
                service.GetApps(
                    It.IsAny<Paginator>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .GetAll()
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.AppsFoundMessage,
                        Payload = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .GetAll()
                            .Result
                            .Objects
                            .ConvertAll(a => (object)a)
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            PromoteUserFailsRequest.Setup(service =>
                service.GetMyApps(
                    It.IsAny<int>(),
                    It.IsAny<Paginator>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .GetAll()
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.AppsFoundMessage,
                        Payload = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .GetAll()
                            .Result
                            .Objects
                            .ConvertAll(a => (object)a)
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            PromoteUserFailsRequest.Setup(service =>
                service.GetRegisteredApps(
                    It.IsAny<int>(),
                    It.IsAny<Paginator>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .GetAll()
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.AppsFoundMessage,
                        Payload = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .GetAll()
                            .Result
                            .Objects
                            .ConvertAll(a => (object)a)
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            PromoteUserFailsRequest.Setup(appsService =>
                appsService.Create(It.IsAny<ILicenseRequest>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .Add(It.IsAny<App>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.AppCreatedMessage,
                        Payload = new List<object>()
                            {
                                MockedAppsRepository
                                    .SuccessfulRequest
                                    .Object
                                    .Get(It.IsAny<int>())
                                    .Result
                                    .Object
                            }
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            PromoteUserFailsRequest.Setup(service =>
                service.IsRequestValidOnThisToken(
                    It.IsAny<IHttpContextAccessor>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(true));

            PromoteUserFailsRequest.Setup(service =>
                service.IsOwnerOfThisLicense(
                    It.IsAny<IHttpContextAccessor>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(true));

            PromoteUserFailsRequest.Setup(service =>
                service.Update(It.IsAny<int>(), It.IsAny<Request>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .Update(It.IsAny<App>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.AppUpdatedMessage,
                        Payload = new List<object>()
                            {
                                MockedAppsRepository
                                    .SuccessfulRequest
                                    .Object
                                    .Get(It.IsAny<int>())
                                    .Result
                                    .Object
                            }
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            PromoteUserFailsRequest.Setup(service =>
                service.GetAppUsers(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Paginator>(),
                    It.IsAny<bool>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .GetAppUsers(It.IsAny<int>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.UsersFoundMessage,
                        Payload = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .GetAppUsers(It.IsAny<int>())
                            .Result
                            .Objects
                            .ConvertAll(u => (object)u)
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            PromoteUserFailsRequest.Setup(service =>
                service.GetLicense(It.IsAny<int>()))
                .Returns(Task.FromResult(new LicenseResult()
                    {
                        IsSuccess = true,
                        Message = AppsMessages.AppFoundMessage,
                        License = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .GetLicense(It.IsAny<int>())
                            .Result
                    } as ILicenseResult));

            PromoteUserFailsRequest.Setup(service =>
                service.AddAppUser(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .AddAppUser(It.IsAny<int>(), It.IsAny<string>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.UserAddedToAppMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            PromoteUserFailsRequest.Setup(service =>
                service.RemoveAppUser(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .RemoveAppUser(It.IsAny<int>(), It.IsAny<string>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.UserRemovedFromAppMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            PromoteUserFailsRequest.Setup(service =>
                service.Activate(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .Activate(It.IsAny<int>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.AppActivatedMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            PromoteUserFailsRequest.Setup(service =>
                service.Deactivate(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .Deactivate(It.IsAny<int>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.AppDeactivatedMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            PromoteUserFailsRequest.Setup(service =>
                service.DeleteOrReset(It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .Delete(It.IsAny<App>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.AppDeletedMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            PromoteUserFailsRequest.Setup(appsService =>
                appsService.ActivateAdminPrivileges(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppAdminsRepository
                            .FailedRequest
                            .Object
                            .Get(It.IsAny<int>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.UserHasNotBeenPromotedToAdminMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            PromoteUserFailsRequest.Setup(appsService =>
                appsService.ActivateAdminPrivileges(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppAdminsRepository
                            .FailedRequest
                            .Object
                            .Get(It.IsAny<int>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.UserHasNotBeenPromotedToAdminMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));

            PromoteUserFailsRequest.Setup(appsService =>
                appsService.DeactivateAdminPrivileges(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppAdminsRepository
                            .FailedRequest
                            .Object
                            .Get(It.IsAny<int>())
                            .Result
                            .IsSuccess,
                        Message = AppsMessages.DeactivationOfAdminPrivilegesFailedMessage
                    } as Core.Interfaces.Models.DomainObjects.Params.IResult));
        }
    }
}
