using System.Collections.Generic;
using System.Threading.Tasks;
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
                        .Success,
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
                } as IResult));

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
                        .Success,
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
                } as IResult));

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
                        .Success,
                    Message = AppsMessages.AppsFoundMessage,
                    Payload = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .GetAll()
                        .Result
                        .Objects
                        .ConvertAll(a => (object)a)
                } as IResult));

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
                        .Success,
                    Message = AppsMessages.AppsFoundMessage,
                    Payload = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .GetAll()
                        .Result
                        .Objects
                        .ConvertAll(a => (object)a)
                } as IResult));

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
                        .Success,
                    Message = AppsMessages.AppsFoundMessage,
                    Payload = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .GetAll()
                        .Result
                        .Objects
                        .ConvertAll(a => (object)a)
                } as IResult));

            SuccessfulRequest.Setup(appsService =>
                appsService.Create(It.IsAny<ILicenseRequest>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .Add(It.IsAny<App>())
                        .Result
                        .Success,
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
                } as IResult));

            SuccessfulRequest.Setup(service =>
                service.IsRequestValidOnThisLicense(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(service =>
                service.IsOwnerOfThisLicense(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
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
                        .Success,
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
                } as IResult));

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
                        .Success,
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
                } as IResult));

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
                        .Success,
                    Message = AppsMessages.UserAddedToAppMessage
                } as IResult));

            SuccessfulRequest.Setup(service =>
                service.RemoveAppUser(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .RemoveAppUser(It.IsAny<int>(), It.IsAny<string>())
                        .Result
                        .Success,
                    Message = AppsMessages.UserRemovedFromAppMessage
                } as IResult));

            SuccessfulRequest.Setup(service =>
                service.Activate(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .Activate(It.IsAny<int>())
                        .Result
                        .Success,
                    Message = AppsMessages.AppActivatedMessage
                } as IResult));

            SuccessfulRequest.Setup(service =>
                service.Deactivate(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .Deactivate(It.IsAny<int>())
                        .Result
                        .Success,
                    Message = AppsMessages.AppDeactivatedMessage
                } as IResult));

            SuccessfulRequest.Setup(service =>
                service.DeleteOrReset(It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .Delete(It.IsAny<App>())
                        .Result
                        .Success,
                    Message = AppsMessages.AppDeletedMessage
                } as IResult));

            SuccessfulRequest.Setup(appsService =>
                appsService.ActivateAdminPrivileges(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppAdminsRepository
                        .SuccessfulRequest
                        .Object
                        .Get(It.IsAny<int>())
                        .Result
                        .Success,
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
                } as IResult));

            SuccessfulRequest.Setup(appsService =>
                appsService.DeactivateAdminPrivileges(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppAdminsRepository
                        .SuccessfulRequest
                        .Object
                        .Get(It.IsAny<int>())
                        .Result
                        .Success,
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
                } as IResult));

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
                            .Success,
                        Message = AppsMessages.AppNotFoundMessage,
                    } as IResult));

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
                            .Success,
                        Message = AppsMessages.AppNotFoundMessage
                    } as IResult));

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
                            .Success,
                        Message = AppsMessages.AppsNotFoundMessage
                    } as IResult));

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
                            .Success,
                        Message = AppsMessages.AppsNotFoundMessage
                    } as IResult));

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
                            .Success,
                        Message = AppsMessages.AppsNotFoundMessage
                    } as IResult));

            FailedRequest.Setup(appsService =>
                appsService.Create(It.IsAny<ILicenseRequest>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .FailedRequest
                            .Object
                            .Add(It.IsAny<App>())
                            .Result
                            .Success,
                        Message = AppsMessages.AppNotCreatedMessage
                    } as IResult));

            FailedRequest.Setup(service =>
                service.IsRequestValidOnThisLicense(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(true));

            FailedRequest.Setup(service =>
                service.IsOwnerOfThisLicense(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
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
                            .Success,
                        Message = AppsMessages.AppNotUpdatedMessage
                    } as IResult));

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
                            .Success,
                        Message = UsersMessages.UsersNotFoundMessage
                    } as IResult));

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
                            .Success,
                        Message = AppsMessages.UserNotAddedToAppMessage
                    } as IResult));

            FailedRequest.Setup(service =>
                service.RemoveAppUser(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .FailedRequest
                            .Object
                            .RemoveAppUser(It.IsAny<int>(), It.IsAny<string>())
                            .Result
                            .Success,
                        Message = AppsMessages.UserNotRemovedFromAppMessage
                    } as IResult));

            FailedRequest.Setup(service =>
                service.Activate(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .FailedRequest
                            .Object
                            .Activate(It.IsAny<int>())
                            .Result
                            .Success,
                        Message = AppsMessages.AppNotActivatedMessage
                    } as IResult));

            FailedRequest.Setup(service =>
                service.Deactivate(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .FailedRequest
                            .Object
                            .Deactivate(It.IsAny<int>())
                            .Result
                            .Success,
                        Message = AppsMessages.AppNotDeactivatedMessage
                    } as IResult));

            FailedRequest.Setup(service =>
                service.DeleteOrReset(It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .FailedRequest
                            .Object
                            .Delete(It.IsAny<App>())
                            .Result
                            .Success,
                        Message = AppsMessages.AppNotDeletedMessage
                    } as IResult));

            FailedRequest.Setup(appsService =>
                appsService.ActivateAdminPrivileges(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppAdminsRepository
                            .FailedRequest
                            .Object
                            .Get(It.IsAny<int>())
                            .Result
                            .Success,
                        Message = UsersMessages.UserHasNotBeenPromotedToAdminMessage
                    } as IResult));

            FailedRequest.Setup(appsService =>
                appsService.DeactivateAdminPrivileges(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppAdminsRepository
                            .FailedRequest
                            .Object
                            .Get(It.IsAny<int>())
                            .Result
                            .Success,
                        Message = AppsMessages.DeactivationOfAdminPrivilegesFailedMessage
                    } as IResult));

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
                            .Success,
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
                    } as IResult));

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
                        .Success,
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
                } as IResult));

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
                        .Success,
                    Message = AppsMessages.AppsFoundMessage,
                    Payload = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .GetAll()
                        .Result
                        .Objects
                        .ConvertAll(a => (object)a)
                } as IResult));

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
                        .Success,
                    Message = AppsMessages.AppsFoundMessage,
                    Payload = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .GetAll()
                        .Result
                        .Objects
                        .ConvertAll(a => (object)a)
                } as IResult));

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
                        .Success,
                    Message = AppsMessages.AppsFoundMessage,
                    Payload = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .GetAll()
                        .Result
                        .Objects
                        .ConvertAll(a => (object)a)
                } as IResult));

            InvalidRequest.Setup(appsService =>
                appsService.Create(It.IsAny<ILicenseRequest>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedAppsRepository
                        .SuccessfulRequest
                        .Object
                        .Add(It.IsAny<App>())
                        .Result
                        .Success,
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
                } as IResult));

            InvalidRequest.Setup(service =>
                service.IsRequestValidOnThisLicense(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(false));

            InvalidRequest.Setup(service =>
                service.IsOwnerOfThisLicense(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
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
                        .Success,
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
                } as IResult));

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
                            .Success,
                        Message = UsersMessages.UsersFoundMessage,
                        Payload = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .GetAppUsers(It.IsAny<int>())
                            .Result
                            .Objects
                            .ConvertAll(u => (object)u)
                    } as IResult));

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
                            .Success,
                        Message = AppsMessages.UserAddedToAppMessage
                    } as IResult));

            InvalidRequest.Setup(service =>
                service.RemoveAppUser(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .RemoveAppUser(It.IsAny<int>(), It.IsAny<string>())
                            .Result
                            .Success,
                        Message = AppsMessages.UserRemovedFromAppMessage
                    } as IResult));

            InvalidRequest.Setup(service =>
                service.Activate(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .Activate(It.IsAny<int>())
                            .Result
                            .Success,
                        Message = AppsMessages.AppActivatedMessage
                    } as IResult));

            InvalidRequest.Setup(service =>
                service.Deactivate(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .Deactivate(It.IsAny<int>())
                            .Result
                            .Success,
                        Message = AppsMessages.AppDeactivatedMessage
                    } as IResult));

            InvalidRequest.Setup(service =>
                service.DeleteOrReset(It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .Delete(It.IsAny<App>())
                            .Result
                            .Success,
                        Message = AppsMessages.AppDeletedMessage
                    } as IResult));

            InvalidRequest.Setup(appsService =>
                appsService.ActivateAdminPrivileges(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppAdminsRepository
                            .FailedRequest
                            .Object
                            .Get(It.IsAny<int>())
                            .Result
                            .Success,
                        Message = AppsMessages.AppNotFoundMessage
                    } as IResult));

            InvalidRequest.Setup(appsService =>
                appsService.DeactivateAdminPrivileges(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppAdminsRepository
                            .SuccessfulRequest
                            .Object
                            .Get(It.IsAny<int>())
                            .Result
                            .Success,
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
                    } as IResult));

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
                            .Success,
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
                    } as IResult));

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
                            .Success,
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
                    } as IResult));

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
                            .Success,
                        Message = AppsMessages.AppsFoundMessage,
                        Payload = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .GetAll()
                            .Result
                            .Objects
                            .ConvertAll(a => (object)a)
                    } as IResult));

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
                            .Success,
                        Message = AppsMessages.AppsFoundMessage,
                        Payload = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .GetAll()
                            .Result
                            .Objects
                            .ConvertAll(a => (object)a)
                    } as IResult));

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
                            .Success,
                        Message = AppsMessages.AppsFoundMessage,
                        Payload = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .GetAll()
                            .Result
                            .Objects
                            .ConvertAll(a => (object)a)
                    } as IResult));

            PromoteUserFailsRequest.Setup(appsService =>
                appsService.Create(It.IsAny<ILicenseRequest>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .Add(It.IsAny<App>())
                            .Result
                            .Success,
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
                    } as IResult));

            PromoteUserFailsRequest.Setup(service =>
                service.IsRequestValidOnThisLicense(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(true));

            PromoteUserFailsRequest.Setup(service =>
                service.IsOwnerOfThisLicense(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
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
                            .Success,
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
                    } as IResult));

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
                            .Success,
                        Message = UsersMessages.UsersFoundMessage,
                        Payload = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .GetAppUsers(It.IsAny<int>())
                            .Result
                            .Objects
                            .ConvertAll(u => (object)u)
                    } as IResult));

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
                            .Success,
                        Message = AppsMessages.UserAddedToAppMessage
                    } as IResult));

            PromoteUserFailsRequest.Setup(service =>
                service.RemoveAppUser(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .RemoveAppUser(It.IsAny<int>(), It.IsAny<string>())
                            .Result
                            .Success,
                        Message = AppsMessages.UserRemovedFromAppMessage
                    } as IResult));

            PromoteUserFailsRequest.Setup(service =>
                service.Activate(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .Activate(It.IsAny<int>())
                            .Result
                            .Success,
                        Message = AppsMessages.AppActivatedMessage
                    } as IResult));

            PromoteUserFailsRequest.Setup(service =>
                service.Deactivate(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .Deactivate(It.IsAny<int>())
                            .Result
                            .Success,
                        Message = AppsMessages.AppDeactivatedMessage
                    } as IResult));

            PromoteUserFailsRequest.Setup(service =>
                service.DeleteOrReset(It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppsRepository
                            .SuccessfulRequest
                            .Object
                            .Delete(It.IsAny<App>())
                            .Result
                            .Success,
                        Message = AppsMessages.AppDeletedMessage
                    } as IResult));

            PromoteUserFailsRequest.Setup(appsService =>
                appsService.ActivateAdminPrivileges(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppAdminsRepository
                            .FailedRequest
                            .Object
                            .Get(It.IsAny<int>())
                            .Result
                            .Success,
                        Message = UsersMessages.UserHasNotBeenPromotedToAdminMessage
                    } as IResult));

            PromoteUserFailsRequest.Setup(appsService =>
                appsService.ActivateAdminPrivileges(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppAdminsRepository
                            .FailedRequest
                            .Object
                            .Get(It.IsAny<int>())
                            .Result
                            .Success,
                        Message = UsersMessages.UserHasNotBeenPromotedToAdminMessage
                    } as IResult));

            PromoteUserFailsRequest.Setup(appsService =>
                appsService.DeactivateAdminPrivileges(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedAppAdminsRepository
                            .FailedRequest
                            .Object
                            .Get(It.IsAny<int>())
                            .Result
                            .Success,
                        Message = AppsMessages.DeactivationOfAdminPrivilegesFailedMessage
                    } as IResult));
        }
    }
}
