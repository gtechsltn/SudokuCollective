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

namespace SudokuCollective.Test.Repositories
{
    public class MockGamesRepository
    {
        private readonly DatabaseContext context;
        internal Mock<IGamesRepository<Game>> SuccessfulRequest { get; set; }
        internal Mock<IGamesRepository<Game>> FailedRequest { get; set; }
        internal Mock<IGamesRepository<Game>> UpdateFailedRequest { get; set; }

        public MockGamesRepository(DatabaseContext ctxt)
        {
            context = ctxt;
            var todaysDate = DateTime.UtcNow;

            SuccessfulRequest = new Mock<IGamesRepository<Game>>();
            FailedRequest = new Mock<IGamesRepository<Game>>();
            UpdateFailedRequest = new Mock<IGamesRepository<Game>>();

            SuccessfulRequest.Setup(gamesRepo =>
                gamesRepo.Add(It.IsAny<Game>()))
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    {
                        Success = true,
                        Object = new Game(
                            context.Users.FirstOrDefault(u => u.Id == 1),
                            new SudokuMatrix(),
                            context.Difficulties.FirstOrDefault(d => d.DifficultyLevel == DifficultyLevel.TEST),
                            context.Apps.Where(a => a.Id == 1).Select(a => a.Id).FirstOrDefault())
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(gamesRepo =>
                gamesRepo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    {
                        Success = true,
                        Object = context.Games.FirstOrDefault(g => g.Id == 1)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(gamesRepo =>
                gamesRepo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context.Games.ToList().ConvertAll(g => (IDomainEntity)g)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(gamesRepo =>
                gamesRepo.Update(It.IsAny<Game>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Games.FirstOrDefault(g => g.Id == 1)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(gamesRepo =>
                gamesRepo.UpdateRange(It.IsAny<List<Game>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    { 
                        Success = true,
                        Objects = context.Games.ToList().ConvertAll(g => (IDomainEntity)g)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(gamesRepo =>
                gamesRepo.Delete(It.IsAny<Game>()))
                    .Returns(Task.FromResult(new RepositoryResponse() 
                    {
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(gamesRepo =>
                gamesRepo.DeleteRange(It.IsAny<List<Game>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(gamesRepo =>
                gamesRepo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            SuccessfulRequest.Setup(gamesRepo =>
                gamesRepo.GetAppGame(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Games.FirstOrDefault(g => g.Id == 1)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(gamesRepo =>
                gamesRepo.GetAppGames(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context.Games.ToList().ConvertAll(g => (IDomainEntity)g)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(gamesRepo =>
                gamesRepo.GetMyGame(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Games.FirstOrDefault(g => g.Id == 1)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(gamesRepo =>
                gamesRepo.GetMyGames(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context.Games.ToList().ConvertAll(g => (IDomainEntity)g)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(gamesRepo =>
                gamesRepo.DeleteMyGame(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            FailedRequest.Setup(gamesRepo =>
                gamesRepo.Add(It.IsAny<Game>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(gamesRepo =>
                gamesRepo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(gamesRepo =>
                gamesRepo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(gamesRepo =>
                gamesRepo.Update(It.IsAny<Game>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(gamesRepo =>
                gamesRepo.UpdateRange(It.IsAny<List<Game>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(gamesRepo =>
                gamesRepo.Delete(It.IsAny<Game>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(gamesRepo =>
                gamesRepo.DeleteRange(It.IsAny<List<Game>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(gamesRepo =>
                gamesRepo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(false));

            FailedRequest.Setup(gamesRepo =>
                gamesRepo.GetAppGame(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(gamesRepo =>
                gamesRepo.GetAppGames(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(gamesRepo =>
                gamesRepo.GetMyGame(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(gamesRepo =>
                gamesRepo.GetMyGames(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(gamesRepo =>
                gamesRepo.DeleteMyGame(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            UpdateFailedRequest.Setup(gamesRepo =>
                gamesRepo.Add(It.IsAny<Game>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = new Game(
                            context.Users.FirstOrDefault(u => u.Id == 1),
                            new SudokuMatrix(),
                            context.Difficulties.FirstOrDefault(d => d.DifficultyLevel == DifficultyLevel.TEST),
                            context.Apps.Where(a => a.Id == 1).Select(a => a.Id).FirstOrDefault())
                    } as IRepositoryResponse));

            UpdateFailedRequest.Setup(gamesRepo =>
                gamesRepo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Games.FirstOrDefault(g => g.Id == 1)
                    } as IRepositoryResponse));

            UpdateFailedRequest.Setup(gamesRepo =>
                gamesRepo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context.Games.ToList().ConvertAll(g => (IDomainEntity)g)
                    } as IRepositoryResponse));

            UpdateFailedRequest.Setup(gamesRepo =>
                gamesRepo.Update(It.IsAny<Game>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            UpdateFailedRequest.Setup(gamesRepo =>
                gamesRepo.UpdateRange(It.IsAny<List<Game>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context.Games.ToList().ConvertAll(g => (IDomainEntity)g)
                    } as IRepositoryResponse));

            UpdateFailedRequest.Setup(gamesRepo =>
                gamesRepo.Delete(It.IsAny<Game>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            UpdateFailedRequest.Setup(gamesRepo =>
                gamesRepo.DeleteRange(It.IsAny<List<Game>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            UpdateFailedRequest.Setup(gamesRepo =>
                gamesRepo.HasEntity(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            UpdateFailedRequest.Setup(gamesRepo =>
                gamesRepo.GetAppGame(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Games.FirstOrDefault(g => g.Id == 1)
                    } as IRepositoryResponse));

            UpdateFailedRequest.Setup(gamesRepo =>
                gamesRepo.GetAppGames(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context.Games.ToList().ConvertAll(g => (IDomainEntity)g)
                    } as IRepositoryResponse));

            UpdateFailedRequest.Setup(gamesRepo =>
                gamesRepo.GetMyGame(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.Games.FirstOrDefault(g => g.Id == 1)
                    } as IRepositoryResponse));

            UpdateFailedRequest.Setup(gamesRepo =>
                gamesRepo.GetMyGames(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context.Games.ToList().ConvertAll(g => (IDomainEntity)g)
                    } as IRepositoryResponse));

            UpdateFailedRequest.Setup(gamesRepo =>
                gamesRepo.DeleteMyGame(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));
        }
    }
}
