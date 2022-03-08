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

namespace SudokuCollective.Test.Repositories
{
    public class MockedGamesRepository
    {
        private readonly DatabaseContext context;
        internal Mock<IGamesRepository<Game>> SuccessfulRequest { get; set; }
        internal Mock<IGamesRepository<Game>> FailedRequest { get; set; }
        internal Mock<IGamesRepository<Game>> SolvedRequest { get; set; }
        internal Mock<IGamesRepository<Game>> UpdateFailedRequest { get; set; }

        public MockedGamesRepository(DatabaseContext ctxt)
        {
            context = ctxt;
            var todaysDate = DateTime.UtcNow;

            SuccessfulRequest = new Mock<IGamesRepository<Game>>();
            FailedRequest = new Mock<IGamesRepository<Game>>();
            SolvedRequest = new Mock<IGamesRepository<Game>>();
            UpdateFailedRequest = new Mock<IGamesRepository<Game>>();

            SuccessfulRequest.Setup(repo =>
                repo.Add(It.IsAny<Game>()))
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    {
                        IsSuccess = true,
                        Object = new Game(
                            context.Users.FirstOrDefault(u => u.Id == 1),
                            new SudokuMatrix(),
                            context.Difficulties.FirstOrDefault(d => d.DifficultyLevel == DifficultyLevel.TEST),
                            context.Apps.Where(a => a.Id == 1).Select(a => a.Id).FirstOrDefault())
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    {
                        IsSuccess = true,
                        Object = context.Games.FirstOrDefault(g => g.Id == 1)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Objects = context.Games.ToList().ConvertAll(g => (IDomainEntity)g)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.Update(It.IsAny<Game>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Object = context.Games.FirstOrDefault(g => g.Id == 1)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.UpdateRange(It.IsAny<List<Game>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    { 
                        IsSuccess = true,
                        Objects = context.Games.ToList().ConvertAll(g => (IDomainEntity)g)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.Delete(It.IsAny<Game>()))
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    {
                        IsSuccess = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.DeleteRange(It.IsAny<List<Game>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(repo =>
                repo.GetAppGame(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Object = context.Games.FirstOrDefault(g => g.Id == 1)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.GetAppGames(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Objects = context.Games.ToList().ConvertAll(g => (IDomainEntity)g)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.GetMyGame(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Object = context.Games.FirstOrDefault(g => g.Id == 1)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.GetMyGames(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Objects = context.Games.ToList().ConvertAll(g => (IDomainEntity)g)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.DeleteMyGame(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.Add(It.IsAny<Game>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.Update(It.IsAny<Game>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.UpdateRange(It.IsAny<List<Game>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.Delete(It.IsAny<Game>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.DeleteRange(It.IsAny<List<Game>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(false));

            FailedRequest.Setup(repo =>
                repo.GetAppGame(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.GetAppGames(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.GetMyGame(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.GetMyGames(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.DeleteMyGame(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false
                    } as IRepositoryResponse));

            UpdateFailedRequest.Setup(repo =>
                repo.Add(It.IsAny<Game>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Object = new Game(
                            context.Users.FirstOrDefault(u => u.Id == 1),
                            new SudokuMatrix(),
                            context.Difficulties.FirstOrDefault(d => d.DifficultyLevel == DifficultyLevel.TEST),
                            context.Apps.Where(a => a.Id == 1).Select(a => a.Id).FirstOrDefault())
                    } as IRepositoryResponse));

            SolvedRequest.Setup(repo =>
                repo.Add(It.IsAny<Game>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Object = new Game(
                            context.Users.FirstOrDefault(u => u.Id == 1),
                            new SudokuMatrix(),
                            context.Difficulties.FirstOrDefault(d => d.DifficultyLevel == DifficultyLevel.TEST),
                            context.Apps.Where(a => a.Id == 1).Select(a => a.Id).FirstOrDefault())
                    } as IRepositoryResponse));

            SolvedRequest.Setup(repo =>
                repo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Object = context.Games.FirstOrDefault(g => g.Id == 2)
                    } as IRepositoryResponse));

            SolvedRequest.Setup(repo =>
                repo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Objects = context.Games.ToList().ConvertAll(g => (IDomainEntity)g)
                    } as IRepositoryResponse));

            SolvedRequest.Setup(repo =>
                repo.Update(It.IsAny<Game>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Object = context.Games.FirstOrDefault(g => g.Id == 1)
                    } as IRepositoryResponse));

            SolvedRequest.Setup(repo =>
                repo.UpdateRange(It.IsAny<List<Game>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Objects = context.Games.ToList().ConvertAll(g => (IDomainEntity)g)
                    } as IRepositoryResponse));

            SolvedRequest.Setup(repo =>
                repo.Delete(It.IsAny<Game>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true
                    } as IRepositoryResponse));

            SolvedRequest.Setup(repo =>
                repo.DeleteRange(It.IsAny<List<Game>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true
                    } as IRepositoryResponse));

            SolvedRequest.Setup(repo =>
                repo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            SolvedRequest.Setup(repo =>
                repo.GetAppGame(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Object = context.Games.FirstOrDefault(g => g.Id == 1)
                    } as IRepositoryResponse));

            SolvedRequest.Setup(repo =>
                repo.GetAppGames(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Objects = context.Games.ToList().ConvertAll(g => (IDomainEntity)g)
                    } as IRepositoryResponse));

            SolvedRequest.Setup(repo =>
                repo.GetMyGame(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Object = context.Games.FirstOrDefault(g => g.Id == 1)
                    } as IRepositoryResponse));

            SolvedRequest.Setup(repo =>
                repo.GetMyGames(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Objects = context.Games.ToList().ConvertAll(g => (IDomainEntity)g)
                    } as IRepositoryResponse));

            SolvedRequest.Setup(repo =>
                repo.DeleteMyGame(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true
                    } as IRepositoryResponse));

            UpdateFailedRequest.Setup(repo =>
                repo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Object = context.Games.FirstOrDefault(g => g.Id == 1)
                    } as IRepositoryResponse));

            UpdateFailedRequest.Setup(repo =>
                repo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Objects = context.Games.ToList().ConvertAll(g => (IDomainEntity)g)
                    } as IRepositoryResponse));

            UpdateFailedRequest.Setup(repo =>
                repo.Update(It.IsAny<Game>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = false
                    } as IRepositoryResponse));

            UpdateFailedRequest.Setup(repo =>
                repo.UpdateRange(It.IsAny<List<Game>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Objects = context.Games.ToList().ConvertAll(g => (IDomainEntity)g)
                    } as IRepositoryResponse));

            UpdateFailedRequest.Setup(repo =>
                repo.Delete(It.IsAny<Game>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true
                    } as IRepositoryResponse));

            UpdateFailedRequest.Setup(repo =>
                repo.DeleteRange(It.IsAny<List<Game>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true
                    } as IRepositoryResponse));

            UpdateFailedRequest.Setup(repo =>
                repo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            UpdateFailedRequest.Setup(repo =>
                repo.GetAppGame(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Object = context.Games.FirstOrDefault(g => g.Id == 1)
                    } as IRepositoryResponse));

            UpdateFailedRequest.Setup(repo =>
                repo.GetAppGames(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Objects = context.Games.ToList().ConvertAll(g => (IDomainEntity)g)
                    } as IRepositoryResponse));

            UpdateFailedRequest.Setup(repo =>
                repo.GetMyGame(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Object = context.Games.FirstOrDefault(g => g.Id == 1)
                    } as IRepositoryResponse));

            UpdateFailedRequest.Setup(repo =>
                repo.GetMyGames(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true,
                        Objects = context.Games.ToList().ConvertAll(g => (IDomainEntity)g)
                    } as IRepositoryResponse));

            UpdateFailedRequest.Setup(repo =>
                repo.DeleteMyGame(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        IsSuccess = true
                    } as IRepositoryResponse));
        }
    }
}
