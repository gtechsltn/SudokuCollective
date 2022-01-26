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

namespace SudokuCollective.Test.Repositories
{
    public class MockDifficultiesRepository
    {
        private readonly DatabaseContext context;
        internal Mock<IDifficultiesRepository<Difficulty>> SuccessfulRequest { get; set; }
        internal Mock<IDifficultiesRepository<Difficulty>> FailedRequest { get; set; }
        internal Mock<IDifficultiesRepository<Difficulty>> CreateDifficultyRequest { get; set; }

        public MockDifficultiesRepository(DatabaseContext ctxt)
        {
            context = ctxt;

            SuccessfulRequest = new Mock<IDifficultiesRepository<Difficulty>>();
            FailedRequest = new Mock<IDifficultiesRepository<Difficulty>>();
            CreateDifficultyRequest = new Mock<IDifficultiesRepository<Difficulty>>();

            SuccessfulRequest.Setup(difficultiesRepo =>
                difficultiesRepo.Add(It.IsAny<Difficulty>()))
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    {
                        Success = true,
                        Object = new Difficulty() { DifficultyLevel = DifficultyLevel.NULL }
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(difficultiesRepo =>
                difficultiesRepo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Difficulties.FirstOrDefault(d => d.Id == 4)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(difficultiesRepo =>
                difficultiesRepo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    {
                        Success = true,
                        Objects = context.Difficulties
                            .Where(d => 
                                d.DifficultyLevel != DifficultyLevel.NULL && d.DifficultyLevel != DifficultyLevel.TEST)
                            .ToList()
                            .ConvertAll(d => (IDomainEntity)d)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(difficultiesRepo =>
                difficultiesRepo.Update(It.IsAny<Difficulty>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Difficulties.FirstOrDefault(d => d.Id == 4)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(difficultiesRepo =>
                difficultiesRepo.UpdateRange(It.IsAny<List<Difficulty>>()))
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    { 
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(difficultiesRepo =>
                difficultiesRepo.Delete(It.IsAny<Difficulty>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(difficultiesRepo =>
                difficultiesRepo.DeleteRange(It.IsAny<List<Difficulty>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(difficultiesRepo =>
                difficultiesRepo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(difficultiesRepo =>
                difficultiesRepo.HasDifficultyLevel(It.IsAny<DifficultyLevel>()))
                    .Returns(Task.FromResult(true));

            FailedRequest.Setup(difficultiesRepo =>
                difficultiesRepo.Add(It.IsAny<Difficulty>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(difficultiesRepo =>
                difficultiesRepo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(difficultiesRepo =>
                difficultiesRepo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(difficultiesRepo =>
                difficultiesRepo.Update(It.IsAny<Difficulty>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(difficultiesRepo =>
                difficultiesRepo.UpdateRange(It.IsAny<List<Difficulty>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(difficultiesRepo =>
                difficultiesRepo.Delete(It.IsAny<Difficulty>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(difficultiesRepo =>
                difficultiesRepo.DeleteRange(It.IsAny<List<Difficulty>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(difficultiesRepo =>
                difficultiesRepo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(false));

            FailedRequest.Setup(difficultiesRepo =>
                difficultiesRepo.HasDifficultyLevel(It.IsAny<DifficultyLevel>()))
                    .Returns(Task.FromResult(false));

            CreateDifficultyRequest.Setup(difficultiesRepo =>
                difficultiesRepo.Add(It.IsAny<Difficulty>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = new Difficulty() { DifficultyLevel = DifficultyLevel.NULL }
                    } as IRepositoryResponse));

            CreateDifficultyRequest.Setup(difficultiesRepo =>
                difficultiesRepo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Difficulties.FirstOrDefault(d => d.Id == 4)
                    } as IRepositoryResponse));

            CreateDifficultyRequest.Setup(difficultiesRepo =>
                difficultiesRepo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context.Difficulties
                            .Where(d =>
                                d.DifficultyLevel != DifficultyLevel.NULL && d.DifficultyLevel != DifficultyLevel.TEST)
                            .ToList()
                            .ConvertAll(d => (IDomainEntity)d)
                    } as IRepositoryResponse));

            CreateDifficultyRequest.Setup(difficultiesRepo =>
                difficultiesRepo.Update(It.IsAny<Difficulty>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Difficulties.FirstOrDefault(d => d.Id == 4)
                    } as IRepositoryResponse));

            CreateDifficultyRequest.Setup(difficultiesRepo =>
                difficultiesRepo.UpdateRange(It.IsAny<List<Difficulty>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            CreateDifficultyRequest.Setup(difficultiesRepo =>
                difficultiesRepo.Delete(It.IsAny<Difficulty>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            CreateDifficultyRequest.Setup(difficultiesRepo =>
                difficultiesRepo.DeleteRange(It.IsAny<List<Difficulty>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            CreateDifficultyRequest.Setup(difficultiesRepo =>
                difficultiesRepo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            CreateDifficultyRequest.Setup(difficultiesRepo =>
                difficultiesRepo.HasDifficultyLevel(It.IsAny<DifficultyLevel>()))
                    .Returns(Task.FromResult(false));
        }
    }
}
