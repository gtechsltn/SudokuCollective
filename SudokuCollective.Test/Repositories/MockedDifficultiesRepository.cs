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
    public class MockedDifficultiesRepository
    {
        private readonly DatabaseContext context;
        internal Mock<IDifficultiesRepository<Difficulty>> SuccessfulRequest { get; set; }
        internal Mock<IDifficultiesRepository<Difficulty>> FailedRequest { get; set; }
        internal Mock<IDifficultiesRepository<Difficulty>> CreateDifficultyRequest { get; set; }

        public MockedDifficultiesRepository(DatabaseContext ctxt)
        {
            context = ctxt;

            SuccessfulRequest = new Mock<IDifficultiesRepository<Difficulty>>();
            FailedRequest = new Mock<IDifficultiesRepository<Difficulty>>();
            CreateDifficultyRequest = new Mock<IDifficultiesRepository<Difficulty>>();

            SuccessfulRequest.Setup(repo =>
                repo.Add(It.IsAny<Difficulty>()))
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    {
                        Success = true,
                        Object = new Difficulty() { DifficultyLevel = DifficultyLevel.NULL }
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Difficulties.FirstOrDefault(d => d.Id == 4)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    {
                        Success = true,
                        Objects = context.Difficulties
                            .Where(d => 
                                d.DifficultyLevel != DifficultyLevel.NULL && d.DifficultyLevel != DifficultyLevel.TEST)
                            .ToList()
                            .ConvertAll(d => (IDomainEntity)d)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.Update(It.IsAny<Difficulty>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Difficulties.FirstOrDefault(d => d.Id == 4)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.UpdateRange(It.IsAny<List<Difficulty>>()))
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    { 
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.Delete(It.IsAny<Difficulty>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.DeleteRange(It.IsAny<List<Difficulty>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(repo =>
                repo.HasDifficultyLevel(It.IsAny<DifficultyLevel>()))
                    .Returns(Task.FromResult(true));

            FailedRequest.Setup(repo =>
                repo.Add(It.IsAny<Difficulty>()))
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
                repo.Update(It.IsAny<Difficulty>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.UpdateRange(It.IsAny<List<Difficulty>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.Delete(It.IsAny<Difficulty>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.DeleteRange(It.IsAny<List<Difficulty>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(false));

            FailedRequest.Setup(repo =>
                repo.HasDifficultyLevel(It.IsAny<DifficultyLevel>()))
                    .Returns(Task.FromResult(false));

            CreateDifficultyRequest.Setup(repo =>
                repo.Add(It.IsAny<Difficulty>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = new Difficulty() { DifficultyLevel = DifficultyLevel.NULL }
                    } as IRepositoryResponse));

            CreateDifficultyRequest.Setup(repo =>
                repo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Difficulties.FirstOrDefault(d => d.Id == 4)
                    } as IRepositoryResponse));

            CreateDifficultyRequest.Setup(repo =>
                repo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context.Difficulties
                            .Where(d =>
                                d.DifficultyLevel != DifficultyLevel.NULL && d.DifficultyLevel != DifficultyLevel.TEST)
                            .ToList()
                            .ConvertAll(d => (IDomainEntity)d)
                    } as IRepositoryResponse));

            CreateDifficultyRequest.Setup(repo =>
                repo.Update(It.IsAny<Difficulty>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Difficulties.FirstOrDefault(d => d.Id == 4)
                    } as IRepositoryResponse));

            CreateDifficultyRequest.Setup(repo =>
                repo.UpdateRange(It.IsAny<List<Difficulty>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            CreateDifficultyRequest.Setup(repo =>
                repo.Delete(It.IsAny<Difficulty>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            CreateDifficultyRequest.Setup(repo =>
                repo.DeleteRange(It.IsAny<List<Difficulty>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            CreateDifficultyRequest.Setup(repo =>
                repo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            CreateDifficultyRequest.Setup(repo =>
                repo.HasDifficultyLevel(It.IsAny<DifficultyLevel>()))
                    .Returns(Task.FromResult(false));
        }
    }
}
