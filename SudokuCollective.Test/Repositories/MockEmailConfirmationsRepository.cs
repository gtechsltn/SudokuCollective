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
    public class MockEmailConfirmationsRepository
    {
        private readonly DatabaseContext context;
        internal Mock<IEmailConfirmationsRepository<EmailConfirmation>> SuccessfulRequest { get; set; }
        internal Mock<IEmailConfirmationsRepository<EmailConfirmation>> FailedRequest { get; set; }

        public MockEmailConfirmationsRepository(DatabaseContext ctxt)
        {
            context = ctxt;

            SuccessfulRequest = new Mock<IEmailConfirmationsRepository<EmailConfirmation>>();
            FailedRequest = new Mock<IEmailConfirmationsRepository<EmailConfirmation>>();

            SuccessfulRequest.Setup(emailConfirmationsRepo =>
                emailConfirmationsRepo.Create(It.IsAny<EmailConfirmation>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = new EmailConfirmation()
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(emailConfirmationsRepo =>
                emailConfirmationsRepo.Get(It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.EmailConfirmations.FirstOrDefault(ec => ec.Id == 1)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(emailConfirmationsRepo =>
                emailConfirmationsRepo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context.EmailConfirmations
                            .ToList()
                            .ConvertAll(d => (IDomainEntity)d)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(emailConfirmationsRepo =>
                emailConfirmationsRepo.Update(It.IsAny<EmailConfirmation>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.EmailConfirmations.FirstOrDefault(ec => ec.Id == 1)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(emailConfirmationsRepo =>
                emailConfirmationsRepo.Delete(It.IsAny<EmailConfirmation>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(emailConfirmationsRepo =>
                emailConfirmationsRepo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(emailConfirmationsRepo =>
                emailConfirmationsRepo.HasOutstandingEmailConfirmation(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(emailConfirmationsRepo =>
                emailConfirmationsRepo.RetrieveEmailConfirmation(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    {
                        Success = true,
                        Object = context.EmailConfirmations.FirstOrDefault(ec => ec.Id == 1)
                    } as IRepositoryResponse));

            FailedRequest.Setup(emailConfirmationsRepo =>
                emailConfirmationsRepo.Create(It.IsAny<EmailConfirmation>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(emailConfirmationsRepo =>
                emailConfirmationsRepo.Get(It.IsAny<string>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(emailConfirmationsRepo =>
                emailConfirmationsRepo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(emailConfirmationsRepo =>
                emailConfirmationsRepo.Update(It.IsAny<EmailConfirmation>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(emailConfirmationsRepo =>
                emailConfirmationsRepo.Delete(It.IsAny<EmailConfirmation>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(emailConfirmationsRepo =>
                emailConfirmationsRepo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(false));

            FailedRequest.Setup(emailConfirmationsRepo =>
                emailConfirmationsRepo.HasOutstandingEmailConfirmation(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(false));

            FailedRequest.Setup(emailConfirmationsRepo =>
                emailConfirmationsRepo.RetrieveEmailConfirmation(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    {
                        Success = false
                    } as IRepositoryResponse));
        }
    }
}
