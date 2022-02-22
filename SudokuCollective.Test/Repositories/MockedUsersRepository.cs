using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using SudokuCollective.Core.Interfaces.ServiceModels;
using SudokuCollective.Core.Interfaces.Models;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models;

namespace SudokuCollective.Test.Repositories
{
    public class MockedUsersRepository
    {
        private readonly DatabaseContext context;
        internal Mock<IUsersRepository<User>> SuccessfulRequest { get; set; }
        internal Mock<IUsersRepository<User>> FailedRequest { get; set; }
        internal Mock<IUsersRepository<User>> EmailFailedRequest { get; set; }
        internal Mock<IUsersRepository<User>> InitiatePasswordSuccessfulRequest { get; set; }
        internal Mock<IUsersRepository<User>> ResendEmailConfirmationSuccessfulRequest { get; set; }

        public MockedUsersRepository(DatabaseContext ctxt)
        {
            context = ctxt;
            var todaysDate = DateTime.UtcNow;

            SuccessfulRequest = new Mock<IUsersRepository<User>>();
            FailedRequest = new Mock<IUsersRepository<User>>();
            EmailFailedRequest = new Mock<IUsersRepository<User>>();
            InitiatePasswordSuccessfulRequest = new Mock<IUsersRepository<User>>();
            ResendEmailConfirmationSuccessfulRequest = new Mock<IUsersRepository<User>>();

            SuccessfulRequest.Setup(repo =>
                repo.Add(It.IsAny<User>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = new User(
                            4,
                            "TestUser3",
                            "Test",
                            "User 3",
                            "Test User 3",
                            "TestUser3@example.com",
                            true,
                            false,
                            "password",
                            false,
                            true,
                            todaysDate,
                            DateTime.MinValue)
                        {
                            Apps = new List<UserApp>()
                            {
                                new UserApp()
                                {
                                    App = new App()
                                }
                            }
                        }
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Users.FirstOrDefault(u => u.Id == 2)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.GetByUserName(It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context
                            .Users
                            .FirstOrDefault(u => u.UserName.Equals("TestSuperUser"))
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.GetByEmail(It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context
                            .Users
                            .FirstOrDefault(u => u.Email.Equals("TestSuperUser@example.com"))
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context
                            .Users
                            .ToList()
                            .ConvertAll(u => (IDomainEntity)u)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.Update(It.IsAny<User>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Users.FirstOrDefault(u => u.Id == 2)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.UpdateRange(It.IsAny<List<User>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context.Users.ToList().ConvertAll(u => (IDomainEntity)u)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.Delete(It.IsAny<User>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.DeleteRange(It.IsAny<List<User>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(repo =>
                repo.AddRoles(It.IsAny<int>(), It.IsAny<List<int>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.RemoveRoles(It.IsAny<int>(), It.IsAny<List<int>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.ConfirmEmail(It.IsAny<EmailConfirmation>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Users.FirstOrDefault(u => u.Id == 1)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.UpdateEmail(It.IsAny<EmailConfirmation>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Users.FirstOrDefault(u => u.Id == 1)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.GetMyApps(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context
                            .Apps
                            .Where(a => a.OwnerId == 2)
                            .ToList()
                            .ConvertAll(a => (IDomainEntity)a)

                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.GetAppLicense(It.IsAny<int>()))
                    .Returns(Task.FromResult(TestData.TestObjects.GetLicense()));

            SuccessfulRequest.Setup(repo =>
                repo.Activate(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(repo =>
                repo.Deactivate(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(repo =>
                repo.PromoteToAdmin(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(repo =>
                repo.IsUserRegistered(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(repo =>
                repo.IsUserNameUnique(It.IsAny<string>()))
                    .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(repo =>
                repo.IsEmailUnique(It.IsAny<string>()))
                    .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(repo =>
                repo.IsUpdatedUserNameUnique(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(repo =>
                repo.IsUpdatedEmailUnique(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(true));

            FailedRequest.Setup(repo =>
                repo.Add(It.IsAny<User>()))
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
                repo.GetByUserName(It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.GetByEmail(It.IsAny<string>()))
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
                repo.Update(It.IsAny<User>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.UpdateRange(It.IsAny<List<User>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.Delete(It.IsAny<User>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.DeleteRange(It.IsAny<List<User>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(false));

            FailedRequest.Setup(repo =>
                repo.AddRoles(It.IsAny<int>(), It.IsAny<List<int>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.RemoveRoles(It.IsAny<int>(), It.IsAny<List<int>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.ConfirmEmail(It.IsAny<IEmailConfirmation>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.UpdateEmail(It.IsAny<EmailConfirmation>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.GetMyApps(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false

                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.GetAppLicense(It.IsAny<int>()))
                    .Returns(Task.FromResult(string.Empty));

            FailedRequest.Setup(repo =>
                repo.Activate(It.IsAny<int>()))
                    .Returns(Task.FromResult(false));

            FailedRequest.Setup(repo =>
                repo.Deactivate(It.IsAny<int>()))
                    .Returns(Task.FromResult(false));

            FailedRequest.Setup(repo =>
                repo.PromoteToAdmin(It.IsAny<int>()))
                    .Returns(Task.FromResult(false));

            FailedRequest.Setup(repo =>
                repo.IsUserRegistered(It.IsAny<int>()))
                    .Returns(Task.FromResult(false));

            FailedRequest.Setup(repo =>
                repo.IsUserNameUnique(It.IsAny<string>()))
                    .Returns(Task.FromResult(false));

            FailedRequest.Setup(repo =>
                repo.IsEmailUnique(It.IsAny<string>()))
                    .Returns(Task.FromResult(false));

            FailedRequest.Setup(repo =>
                repo.IsUpdatedUserNameUnique(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(false));

            FailedRequest.Setup(repo =>
                repo.IsUpdatedEmailUnique(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(false));

            EmailFailedRequest.Setup(repo =>
                repo.Add(It.IsAny<User>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = new User(
                            4,
                            "TestUser3",
                            "Test",
                            "User 3",
                            "Test User 3",
                            "TestUser3@example.com",
                            false,
                            false,
                            "password",
                            false,
                            true,
                            todaysDate,
                            DateTime.MinValue)
                    } as IRepositoryResponse));

            EmailFailedRequest.Setup(repo =>
                repo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Users.FirstOrDefault(u => u.Id == 1)
                    } as IRepositoryResponse));

            EmailFailedRequest.Setup(repo =>
                repo.GetByUserName(It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context
                            .Users
                            .FirstOrDefault(u => u.UserName.Equals("TestSuperUser"))
                    } as IRepositoryResponse));

            EmailFailedRequest.Setup(repo =>
                repo.GetByEmail(It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context
                            .Users
                            .FirstOrDefault(u => u.Email.Equals("TestSuperUser@example.com"))
                    } as IRepositoryResponse));

            EmailFailedRequest.Setup(repo =>
                repo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context
                            .Users
                            .ToList()
                            .ConvertAll(u => (IDomainEntity)u)
                    } as IRepositoryResponse));

            EmailFailedRequest.Setup(repo =>
                repo.Update(It.IsAny<User>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Users.FirstOrDefault(u => u.Id == 1)
                    } as IRepositoryResponse));

            EmailFailedRequest.Setup(repo =>
                repo.UpdateRange(It.IsAny<List<User>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context.Users.ToList().ConvertAll(u => (IDomainEntity)u)
                    } as IRepositoryResponse));

            EmailFailedRequest.Setup(repo =>
                repo.Delete(It.IsAny<User>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            EmailFailedRequest.Setup(repo =>
                repo.DeleteRange(It.IsAny<List<User>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            EmailFailedRequest.Setup(repo =>
                repo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            EmailFailedRequest.Setup(repo =>
                repo.AddRoles(It.IsAny<int>(), It.IsAny<List<int>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            EmailFailedRequest.Setup(repo =>
                repo.RemoveRoles(It.IsAny<int>(), It.IsAny<List<int>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            EmailFailedRequest.Setup(repo =>
                repo.ConfirmEmail(It.IsAny<IEmailConfirmation>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            EmailFailedRequest.Setup(repo =>
                repo.UpdateEmail(It.IsAny<EmailConfirmation>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            EmailFailedRequest.Setup(repo =>
                repo.GetMyApps(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false

                    } as IRepositoryResponse));

            EmailFailedRequest.Setup(repo =>
                repo.GetAppLicense(It.IsAny<int>()))
                    .Returns(Task.FromResult(string.Empty));

            EmailFailedRequest.Setup(repo =>
                repo.Activate(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            EmailFailedRequest.Setup(repo =>
                repo.Deactivate(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            EmailFailedRequest.Setup(repo =>
                repo.PromoteToAdmin(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            EmailFailedRequest.Setup(repo =>
                repo.IsUserRegistered(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            EmailFailedRequest.Setup(repo =>
                repo.IsUserNameUnique(It.IsAny<string>()))
                    .Returns(Task.FromResult(true));

            EmailFailedRequest.Setup(repo =>
                repo.IsEmailUnique(It.IsAny<string>()))
                    .Returns(Task.FromResult(false));

            EmailFailedRequest.Setup(repo =>
                repo.IsUpdatedUserNameUnique(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(true));

            EmailFailedRequest.Setup(repo =>
                repo.IsUpdatedEmailUnique(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(false));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.Add(It.IsAny<User>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = new User(
                            4,
                            "TestUser3",
                            "Test",
                            "User 3",
                            "Test User 3",
                            "TestUser3@example.com",
                            true,
                            false,
                            "password",
                            false,
                            true,
                            todaysDate,
                            DateTime.MinValue)
                        {
                            Apps = new List<UserApp>()
                            {
                                new UserApp()
                                {
                                    App = new App()
                                }
                            }
                        }
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Users.FirstOrDefault(u => u.Id == 3)
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.GetByUserName(It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context
                            .Users
                            .FirstOrDefault(u => u.UserName.Equals("TestSuperUser"))
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.GetByEmail(It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context
                            .Users
                            .FirstOrDefault(u => u.Email.Equals("TestSuperUser@example.com"))
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context
                            .Users
                            .ToList()
                            .ConvertAll(u => (IDomainEntity)u)
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.Update(It.IsAny<User>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Users.FirstOrDefault(u => u.Id == 1)
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.UpdateRange(It.IsAny<List<User>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context.Users.ToList().ConvertAll(u => (IDomainEntity)u)
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.Delete(It.IsAny<User>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.DeleteRange(It.IsAny<List<User>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.AddRoles(It.IsAny<int>(), It.IsAny<List<int>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.RemoveRoles(It.IsAny<int>(), It.IsAny<List<int>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.ConfirmEmail(It.IsAny<EmailConfirmation>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Users.FirstOrDefault(u => u.Id == 1)
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.UpdateEmail(It.IsAny<EmailConfirmation>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Users.FirstOrDefault(u => u.Id == 1)
                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.GetMyApps(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context
                            .Apps
                            .Where(a => a.OwnerId == 2)
                            .ToList()
                            .ConvertAll(a => (IDomainEntity)a)

                    } as IRepositoryResponse));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.GetAppLicense(It.IsAny<int>()))
                    .Returns(Task.FromResult(TestData.TestObjects.GetLicense()));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.Activate(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.Deactivate(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.PromoteToAdmin(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.IsUserRegistered(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.IsUserNameUnique(It.IsAny<string>()))
                    .Returns(Task.FromResult(true));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.IsEmailUnique(It.IsAny<string>()))
                    .Returns(Task.FromResult(true));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.IsUpdatedUserNameUnique(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(true));

            InitiatePasswordSuccessfulRequest.Setup(repo =>
                repo.IsUpdatedEmailUnique(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(true));

            ResendEmailConfirmationSuccessfulRequest.Setup(repo =>
                repo.Add(It.IsAny<User>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = new User(
                            4,
                            "TestUser3",
                            "Test",
                            "User 3",
                            "Test User 3",
                            "TestUser3@example.com",
                            true,
                            false,
                            "password",
                            false,
                            true,
                            todaysDate,
                            DateTime.MinValue)
                        {
                            Apps = new List<UserApp>()
                            {
                                new UserApp()
                                {
                                    App = new App()
                                }
                            }
                        }
                    } as IRepositoryResponse));

            ResendEmailConfirmationSuccessfulRequest.Setup(repo =>
                repo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Users.FirstOrDefault(u => u.Id == 3)
                    } as IRepositoryResponse));

            ResendEmailConfirmationSuccessfulRequest.Setup(repo =>
                repo.GetByUserName(It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context
                            .Users
                            .FirstOrDefault(u => u.UserName.Equals("TestSuperUser"))
                    } as IRepositoryResponse));

            ResendEmailConfirmationSuccessfulRequest.Setup(repo =>
                repo.GetByEmail(It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context
                            .Users
                            .FirstOrDefault(u => u.Email.Equals("TestSuperUser@example.com"))
                    } as IRepositoryResponse));

            ResendEmailConfirmationSuccessfulRequest.Setup(repo =>
                repo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context
                            .Users
                            .ToList()
                            .ConvertAll(u => (IDomainEntity)u)
                    } as IRepositoryResponse));

            ResendEmailConfirmationSuccessfulRequest.Setup(repo =>
                repo.Update(It.IsAny<User>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Users.FirstOrDefault(u => u.Id == 1)
                    } as IRepositoryResponse));

            ResendEmailConfirmationSuccessfulRequest.Setup(repo =>
                repo.UpdateRange(It.IsAny<List<User>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context.Users.ToList().ConvertAll(u => (IDomainEntity)u)
                    } as IRepositoryResponse));

            ResendEmailConfirmationSuccessfulRequest.Setup(repo =>
                repo.Delete(It.IsAny<User>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            ResendEmailConfirmationSuccessfulRequest.Setup(repo =>
                repo.DeleteRange(It.IsAny<List<User>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            ResendEmailConfirmationSuccessfulRequest.Setup(repo =>
                repo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            ResendEmailConfirmationSuccessfulRequest.Setup(repo =>
                repo.AddRoles(It.IsAny<int>(), It.IsAny<List<int>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            ResendEmailConfirmationSuccessfulRequest.Setup(repo =>
                repo.RemoveRoles(It.IsAny<int>(), It.IsAny<List<int>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            ResendEmailConfirmationSuccessfulRequest.Setup(repo =>
                repo.ConfirmEmail(It.IsAny<EmailConfirmation>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Users.FirstOrDefault(u => u.Id == 1)
                    } as IRepositoryResponse));

            ResendEmailConfirmationSuccessfulRequest.Setup(repo =>
                repo.UpdateEmail(It.IsAny<EmailConfirmation>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Users.FirstOrDefault(u => u.Id == 1)
                    } as IRepositoryResponse));

            ResendEmailConfirmationSuccessfulRequest.Setup(repo =>
                repo.GetMyApps(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context
                            .Apps
                            .Where(a => a.OwnerId == 2)
                            .ToList()
                            .ConvertAll(a => (IDomainEntity)a)

                    } as IRepositoryResponse));

            ResendEmailConfirmationSuccessfulRequest.Setup(repo =>
                repo.GetAppLicense(It.IsAny<int>()))
                    .Returns(Task.FromResult(TestData.TestObjects.GetLicense()));

            ResendEmailConfirmationSuccessfulRequest.Setup(repo =>
                repo.Activate(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            ResendEmailConfirmationSuccessfulRequest.Setup(repo =>
                repo.Deactivate(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            ResendEmailConfirmationSuccessfulRequest.Setup(repo =>
                repo.PromoteToAdmin(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            ResendEmailConfirmationSuccessfulRequest.Setup(repo =>
                repo.IsUserRegistered(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            ResendEmailConfirmationSuccessfulRequest.Setup(repo =>
                repo.IsUserNameUnique(It.IsAny<string>()))
                    .Returns(Task.FromResult(true));

            ResendEmailConfirmationSuccessfulRequest.Setup(repo =>
                repo.IsEmailUnique(It.IsAny<string>()))
                    .Returns(Task.FromResult(true));

            ResendEmailConfirmationSuccessfulRequest.Setup(repo =>
                repo.IsUpdatedUserNameUnique(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(true));

            ResendEmailConfirmationSuccessfulRequest.Setup(repo =>
                repo.IsUpdatedEmailUnique(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(true));
        }
    }
}
