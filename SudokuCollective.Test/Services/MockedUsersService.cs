using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using SudokuCollective.Core.Models;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Messages;
using SudokuCollective.Test.TestData;
using SudokuCollective.Test.Repositories;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Results;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;

namespace SudokuCollective.Test.Services
{
    public class MockedUsersService
    {
        private MockedUsersRepository MockedUsersRepository { get; set; }
        private MockedPasswordResetsRepository MockedPasswordResetsRepository { get; set; }
        private MockedEmailConfirmationsRepository MockedEmailConfirmationsRepository { get; set; }

        internal Mock<IUsersService> SuccessfulRequest { get; set; }
        internal Mock<IUsersService> FailedRequest { get; set; }
        internal Mock<IUsersService> FailedResetPasswordRequest { get; set; }

        public MockedUsersService(DatabaseContext context)
        {
            MockedUsersRepository = new MockedUsersRepository(context);
            MockedPasswordResetsRepository = new MockedPasswordResetsRepository(context);
            MockedEmailConfirmationsRepository = new MockedEmailConfirmationsRepository(context);

            SuccessfulRequest = new Mock<IUsersService>();
            FailedRequest = new Mock<IUsersService>();
            FailedResetPasswordRequest = new Mock<IUsersService>();

            SuccessfulRequest.Setup(service =>
                service.Create(It.IsAny<ISignupRequest>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .SuccessfulRequest
                            .Object
                            .Add(It.IsAny<User>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.UserCreatedMessage,
                        Payload = new List<object>()
                            {
                                new EmailConfirmationSentResult 
                                { 
                                    EmailConfirmationSent = true 
                                }
                            }
                    } as IResult));

            SuccessfulRequest.Setup(service =>
                service.Get(
                    It.IsAny<int>(),
                    It.IsAny<Request>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .SuccessfulRequest
                            .Object
                            .Get(It.IsAny<int>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.UserFoundMessage,
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

            SuccessfulRequest.Setup(service =>
                service.GetUsers(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<Paginator>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .SuccessfulRequest
                            .Object
                            .GetAll()
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.UsersFoundMessage,
                        Payload = MockedUsersRepository
                            .SuccessfulRequest
                            .Object
                            .GetAll()
                            .Result
                            .Objects
                            .ConvertAll(u => (object)u)
                    } as IResult));

            SuccessfulRequest.Setup(service =>
                service.Update(
                    It.IsAny<int>(),
                    It.IsAny<IRequest>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .SuccessfulRequest
                            .Object
                            .Update(It.IsAny<User>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.UserUpdatedMessage,
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

            SuccessfulRequest.Setup(service =>
                service.RequestPasswordReset(
                    It.IsAny<IRequestPasswordResetRequest>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = true,
                        Message = UsersMessages.ProcessedPasswordResetRequestMessage
                    } as IResult));

            SuccessfulRequest.Setup(service =>
                service.ResendPasswordReset(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = true,
                        Message = UsersMessages.PasswordResetEmailResentMessage
                    } as IResult));

            SuccessfulRequest.Setup(service =>
                service.GetUserByPasswordToken(It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = true,
                        Message = UsersMessages.UserFoundMessage,
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

            SuccessfulRequest.Setup(service =>
                service.GetAppLicenseByPasswordToken(It.IsAny<string>()))
                .Returns(Task.FromResult(new LicenseResult()
                    {
                        IsSuccess = true,
                        Message = AppsMessages.AppsFoundMessage,
                        License = TestObjects.GetLicense()
                    } as ILicenseResult));

            SuccessfulRequest.Setup(service =>
                service.UpdatePassword(
                    It.IsAny<IUpdatePasswordRequest>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .SuccessfulRequest
                            .Object
                            .Update(It.IsAny<User>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.PasswordResetMessage
                    } as IResult));

            SuccessfulRequest.Setup(service =>
                service.Delete(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .SuccessfulRequest
                            .Object
                            .Delete(It.IsAny<User>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.UserDeletedMessage
                    } as IResult));

            SuccessfulRequest.Setup(service =>
                service.AddUserRoles(
                    It.IsAny<int>(),
                    It.IsAny<IRequest>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .SuccessfulRequest
                            .Object
                            .AddRoles(It.IsAny<int>(), It.IsAny<List<int>>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.RolesAddedMessage
                    } as IResult));

            SuccessfulRequest.Setup(service =>
                service.RemoveUserRoles(
                    It.IsAny<int>(),
                    It.IsAny<IRequest>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .SuccessfulRequest
                            .Object
                            .RemoveRoles(It.IsAny<int>(), It.IsAny<List<int>>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.RolesRemovedMessage
                    } as IResult));

            SuccessfulRequest.Setup(service =>
                service.Activate(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .SuccessfulRequest
                            .Object
                            .Activate(It.IsAny<int>())
                            .Result,
                        Message = UsersMessages.UserActivatedMessage
                    } as IResult));

            SuccessfulRequest.Setup(service =>
                service.Deactivate(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .SuccessfulRequest
                            .Object
                            .Deactivate(It.IsAny<int>())
                            .Result,
                        Message = UsersMessages.UserDeactivatedMessage
                    } as IResult));

            SuccessfulRequest.Setup(service =>
                service.ConfirmEmail(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedUsersRepository
                        .SuccessfulRequest
                        .Object
                        .ConfirmEmail(It.IsAny<IEmailConfirmation>())
                        .Result
                        .IsSuccess,
                    Message = UsersMessages.EmailConfirmedMessage,
                    Payload = new List<object>()
                    {
                        TestObjects.GetConfirmEmailResult()
                    }
                } as IResult));

            SuccessfulRequest.Setup(service =>
                service.InitiatePasswordReset(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedPasswordResetsRepository
                            .SuccessfulRequest
                            .Object
                            .Create(It.IsAny<PasswordReset>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.UserFoundMessage,
                        Payload = new List<object>()
                        {
                            TestObjects.GetInitiatePasswordResetResult()
                        }
                    } as IResult));

            SuccessfulRequest.Setup(service =>
                service.ResendEmailConfirmation(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedEmailConfirmationsRepository
                            .SuccessfulRequest
                            .Object
                            .HasOutstandingEmailConfirmation(It.IsAny<int>(), It.IsAny<int>())
                            .Result,
                        Message = UsersMessages.EmailConfirmationEmailResentMessage,
                        Payload = new List<object>()
                        {
                            TestObjects.GetUserResult()
                        }
                    } as IResult));

            SuccessfulRequest.Setup(service =>
                service.CancelEmailConfirmationRequest(
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedEmailConfirmationsRepository
                            .SuccessfulRequest
                            .Object
                            .Delete(It.IsAny<EmailConfirmation>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.EmailConfirmationRequestCancelledMessage,
                        Payload = new List<object>()
                            {
                                TestObjects.GetUserResult()
                            }
                    } as IResult));

            SuccessfulRequest.Setup(service =>
                service.CancelPasswordResetRequest(
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedPasswordResetsRepository
                            .SuccessfulRequest
                            .Object
                            .Delete(It.IsAny<PasswordReset>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.PasswordResetRequestCancelledMessage,
                        Payload = new List<object>()
                            {
                                TestObjects.GetUserResult()
                            }
                    } as IResult));

            SuccessfulRequest.Setup(service =>
                service.CancelAllEmailRequests(
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = true,
                        Message = string.Format("{0} and {1}",
                            UsersMessages.EmailConfirmationRequestCancelledMessage,
                            UsersMessages.PasswordResetRequestCancelledMessage),
                        Payload = new List<object>()
                            {
                                TestObjects.GetUserResult()
                            }
                } as IResult));

            FailedRequest.Setup(service =>
                service.Create(It.IsAny<ISignupRequest>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .FailedRequest
                            .Object
                            .Add(It.IsAny<User>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.UserNotCreatedMessage
                    } as IResult));

            FailedRequest.Setup(service =>
                service.Get(
                    It.IsAny<int>(),
                    It.IsAny<Request>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .FailedRequest
                            .Object
                            .Get(It.IsAny<int>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.UserNotFoundMessage
                    } as IResult));

            FailedRequest.Setup(service =>
                service.GetUsers(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<Paginator>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .FailedRequest
                            .Object
                            .GetAll()
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.UsersNotFoundMessage
                    } as IResult));

            FailedRequest.Setup(service =>
                service.Update(
                    It.IsAny<int>(),
                    It.IsAny<IRequest>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .FailedRequest
                            .Object
                            .Update(It.IsAny<User>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.UserNotUpdatedMessage
                    } as IResult));

            FailedRequest.Setup(service =>
                service.RequestPasswordReset(
                    It.IsAny<IRequestPasswordResetRequest>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = false,
                        Message = UsersMessages.UnableToProcessPasswordResetRequesMessage
                    } as IResult));

            FailedRequest.Setup(service =>
                service.ResendPasswordReset(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = false,
                        Message = UsersMessages.UserNotFoundMessage
                    } as IResult));

            FailedRequest.Setup(service =>
                service.GetUserByPasswordToken(It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = true,
                        Message = UsersMessages.UserFoundMessage
                    } as IResult));

            FailedRequest.Setup(service =>
                service.GetAppLicenseByPasswordToken(It.IsAny<string>()))
                .Returns(Task.FromResult(new LicenseResult()
                    {
                        IsSuccess = false,
                        Message = AppsMessages.AppNotFoundMessage,
                        License = string.Empty
                    } as ILicenseResult));

            FailedRequest.Setup(service =>
                service.UpdatePassword(
                    It.IsAny<IUpdatePasswordRequest>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .FailedRequest
                            .Object
                            .Update(It.IsAny<User>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.PasswordNotResetMessage
                    } as IResult));

            FailedRequest.Setup(service =>
                service.Delete(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .FailedRequest
                            .Object
                            .Delete(It.IsAny<User>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.UserNotDeletedMessage
                    } as IResult));

            FailedRequest.Setup(service =>
                service.AddUserRoles(
                    It.IsAny<int>(),
                    It.IsAny<IRequest>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .FailedRequest
                            .Object
                            .AddRoles(It.IsAny<int>(), It.IsAny<List<int>>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.RolesNotAddedMessage
                    } as IResult));

            FailedRequest.Setup(service =>
                service.RemoveUserRoles(
                    It.IsAny<int>(),
                    It.IsAny<IRequest>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .FailedRequest
                            .Object
                            .RemoveRoles(It.IsAny<int>(), It.IsAny<List<int>>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.RolesNotRemovedMessage
                    } as IResult));

            FailedRequest.Setup(service =>
                service.Activate(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .FailedRequest
                            .Object
                            .Activate(It.IsAny<int>())
                            .Result,
                        Message = UsersMessages.UserNotActivatedMessage
                    } as IResult));

            FailedRequest.Setup(service =>
                service.Deactivate(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .FailedRequest
                            .Object
                            .Deactivate(It.IsAny<int>())
                            .Result,
                        Message = UsersMessages.UserNotDeactivatedMessage
                    } as IResult));

            FailedRequest.Setup(service =>
                service.ConfirmEmail(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .FailedRequest
                            .Object
                            .ConfirmEmail(It.IsAny<IEmailConfirmation>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.EmailNotConfirmedMessage
                    } as IResult));

            FailedRequest.Setup(service =>
                service.InitiatePasswordReset(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedPasswordResetsRepository
                        .FailedRequest
                        .Object
                        .Create(It.IsAny<PasswordReset>())
                        .Result
                        .IsSuccess,
                    Message = UsersMessages.UserNotFoundMessage
                } as IResult));

            FailedRequest.Setup(service =>
                service.ResendEmailConfirmation(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedEmailConfirmationsRepository
                            .FailedRequest
                            .Object
                            .HasOutstandingEmailConfirmation(It.IsAny<int>(), It.IsAny<int>())
                            .Result,
                        Message = UsersMessages.EmailConfirmationEmailNotResentMessage
                    } as IResult));

            FailedRequest.Setup(service =>
                service.CancelEmailConfirmationRequest(
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedEmailConfirmationsRepository
                            .FailedRequest
                            .Object
                            .Delete(It.IsAny<EmailConfirmation>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.EmailConfirmationRequestNotCancelledMessage
                    } as IResult));

            FailedRequest.Setup(service =>
                service.CancelPasswordResetRequest(
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedPasswordResetsRepository
                            .FailedRequest
                            .Object
                            .Delete(It.IsAny<PasswordReset>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.PasswordResetRequestNotCancelledMessage
                    } as IResult));

            FailedRequest.Setup(service =>
                service.CancelAllEmailRequests(
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = false,
                        Message = UsersMessages.EmailRequestsNotFoundMessage
                    } as IResult));
            
            FailedResetPasswordRequest.Setup(service =>
                service.Create(It.IsAny<ISignupRequest>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .SuccessfulRequest
                            .Object
                            .Add(It.IsAny<User>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.UserCreatedMessage,
                        Payload = new List<object>()
                            {
                                new EmailConfirmationSentResult 
                                { 
                                    EmailConfirmationSent = true 
                                }
                            }
                    } as IResult));

            FailedResetPasswordRequest.Setup(service =>
                service.Get(
                    It.IsAny<int>(),
                    It.IsAny<Request>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .SuccessfulRequest
                            .Object
                            .Get(It.IsAny<int>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.UserFoundMessage,
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

            FailedResetPasswordRequest.Setup(service =>
                service.GetUsers(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<Paginator>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .SuccessfulRequest
                            .Object
                            .GetAll()
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.UsersFoundMessage,
                        Payload = MockedUsersRepository
                            .SuccessfulRequest
                            .Object
                            .GetAll()
                            .Result
                            .Objects
                            .ConvertAll(u => (object)u)
                    } as IResult));

            FailedResetPasswordRequest.Setup(service =>
                service.Update(
                    It.IsAny<int>(),
                    It.IsAny<IRequest>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .SuccessfulRequest
                            .Object
                            .Update(It.IsAny<User>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.UserUpdatedMessage,
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

            FailedResetPasswordRequest.Setup(service =>
                service.RequestPasswordReset(
                    It.IsAny<IRequestPasswordResetRequest>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = true,
                        Message = UsersMessages.ProcessedPasswordResetRequestMessage
                    } as IResult));

            FailedResetPasswordRequest.Setup(service =>
                service.ResendPasswordReset(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = true,
                        Message = UsersMessages.PasswordResetEmailResentMessage
                    } as IResult));

            FailedResetPasswordRequest.Setup(service =>
                service.GetUserByPasswordToken(It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = true,
                        Message = UsersMessages.UserFoundMessage,
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

            FailedResetPasswordRequest.Setup(service =>
                service.GetAppLicenseByPasswordToken(It.IsAny<string>()))
                .Returns(Task.FromResult(new LicenseResult()
                    {
                        IsSuccess = true,
                        Message = AppsMessages.AppsFoundMessage,
                        License = TestObjects.GetLicense()
                    } as ILicenseResult));

            FailedResetPasswordRequest.Setup(service =>
                service.UpdatePassword(
                    It.IsAny<IUpdatePasswordRequest>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .FailedRequest
                            .Object
                            .Update(It.IsAny<User>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.PasswordNotResetMessage
                    } as IResult));

            FailedResetPasswordRequest.Setup(service =>
                service.Delete(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .SuccessfulRequest
                            .Object
                            .Delete(It.IsAny<User>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.UserDeletedMessage
                    } as IResult));

            FailedResetPasswordRequest.Setup(service =>
                service.AddUserRoles(
                    It.IsAny<int>(),
                    It.IsAny<IRequest>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .SuccessfulRequest
                            .Object
                            .AddRoles(It.IsAny<int>(), It.IsAny<List<int>>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.RolesAddedMessage
                    } as IResult));

            FailedResetPasswordRequest.Setup(service =>
                service.RemoveUserRoles(
                    It.IsAny<int>(),
                    It.IsAny<IRequest>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .SuccessfulRequest
                            .Object
                            .RemoveRoles(It.IsAny<int>(), It.IsAny<List<int>>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.RolesRemovedMessage
                    } as IResult));

            FailedResetPasswordRequest.Setup(service =>
                service.Activate(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .SuccessfulRequest
                            .Object
                            .Activate(It.IsAny<int>())
                            .Result,
                        Message = UsersMessages.UserActivatedMessage
                    } as IResult));

            FailedResetPasswordRequest.Setup(service =>
                service.Deactivate(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedUsersRepository
                            .SuccessfulRequest
                            .Object
                            .Deactivate(It.IsAny<int>())
                            .Result,
                        Message = UsersMessages.UserDeactivatedMessage
                    } as IResult));

            FailedResetPasswordRequest.Setup(service =>
                service.ConfirmEmail(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedUsersRepository
                        .SuccessfulRequest
                        .Object
                        .ConfirmEmail(It.IsAny<IEmailConfirmation>())
                        .Result
                        .IsSuccess,
                    Message = UsersMessages.EmailConfirmedMessage,
                    Payload = new List<object>()
                    {
                        TestObjects.GetConfirmEmailResult()
                    }
                } as IResult));

            FailedResetPasswordRequest.Setup(service =>
                service.InitiatePasswordReset(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedPasswordResetsRepository
                            .SuccessfulRequest
                            .Object
                            .Create(It.IsAny<PasswordReset>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.UserFoundMessage,
                        Payload = new List<object>()
                        {
                            TestObjects.GetInitiatePasswordResetResult()
                        }
                    } as IResult));

            FailedResetPasswordRequest.Setup(service =>
                service.ResendEmailConfirmation(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedEmailConfirmationsRepository
                            .SuccessfulRequest
                            .Object
                            .HasOutstandingEmailConfirmation(It.IsAny<int>(), It.IsAny<int>())
                            .Result,
                        Message = UsersMessages.EmailConfirmationEmailResentMessage,
                        Payload = new List<object>()
                        {
                            TestObjects.GetUserResult()
                        }
                    } as IResult));

            FailedResetPasswordRequest.Setup(service =>
                service.CancelEmailConfirmationRequest(
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedEmailConfirmationsRepository
                            .SuccessfulRequest
                            .Object
                            .Delete(It.IsAny<EmailConfirmation>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.EmailConfirmationRequestCancelledMessage,
                        Payload = new List<object>()
                            {
                                TestObjects.GetUserResult()
                            }
                    } as IResult));

            FailedResetPasswordRequest.Setup(service =>
                service.CancelPasswordResetRequest(
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedPasswordResetsRepository
                            .SuccessfulRequest
                            .Object
                            .Delete(It.IsAny<PasswordReset>())
                            .Result
                            .IsSuccess,
                        Message = UsersMessages.PasswordResetRequestCancelledMessage,
                        Payload = new List<object>()
                            {
                                TestObjects.GetUserResult()
                            }
                    } as IResult));

            FailedResetPasswordRequest.Setup(service =>
                service.CancelAllEmailRequests(
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = true,
                        Message = string.Format("{0} and {1}",
                            UsersMessages.EmailConfirmationRequestCancelledMessage,
                            UsersMessages.PasswordResetRequestCancelledMessage),
                        Payload = new List<object>()
                            {
                                TestObjects.GetUserResult()
                            }
                } as IResult));
        }
    }
}
