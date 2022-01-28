using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using SudokuCollective.Data.Models;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Messages;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Enums;
using SudokuCollective.Test.Repositories;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Test.TestData;

namespace SudokuCollective.Test.Services
{
    public class MockedGamesService
    {
        private MockedGamesRepository MockedGamesRepository { get; set; }

        internal Mock<IGamesService> SuccessfulRequest { get; set; }
        internal Mock<IGamesService> FailedRequest { get; set; }
        internal Mock<IGamesService> UpdateFailedRequest { get; set; }

        public MockedGamesService(DatabaseContext context)
        {
            MockedGamesRepository = new MockedGamesRepository(context);

            SuccessfulRequest = new Mock<IGamesService>();
            FailedRequest = new Mock<IGamesService>();
            UpdateFailedRequest = new Mock<IGamesService>();

            SuccessfulRequest.Setup(Service =>
                Service.Create(It.IsAny<IRequest>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedGamesRepository
                            .SuccessfulRequest
                            .Object
                            .Add(It.IsAny<Game>())
                            .Result
                            .Success,
                        Message = GamesMessages.GameCreatedMessage,
                        DataPacket = new List<object>()
                        {
                            MockedGamesRepository
                                .SuccessfulRequest
                                .Object
                                .Add(It.IsAny<Game>())
                                .Result
                                .Object
                        }
                    } as IResult));

            SuccessfulRequest.Setup(Service =>
                Service.Update(It.IsAny<int>(), It.IsAny<IRequest>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedGamesRepository
                            .SuccessfulRequest
                            .Object
                            .Update(It.IsAny<Game>())
                            .Result
                            .Success,
                        Message = GamesMessages.GameUpdatedMessage,
                        DataPacket = new List<object>()
                            {
                                MockedGamesRepository
                                    .SuccessfulRequest
                                    .Object
                                    .Add(It.IsAny<Game>())
                                    .Result
                                    .Object
                            }
                    } as IResult));

            SuccessfulRequest.Setup(Service =>
                Service.Delete(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedGamesRepository
                            .SuccessfulRequest
                            .Object
                            .Delete(It.IsAny<Game>())
                            .Result
                            .Success,
                        Message = GamesMessages.GameDeletedMessage
                    } as IResult));

            SuccessfulRequest.Setup(Service =>
                Service.GetGame(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedGamesRepository
                            .SuccessfulRequest
                            .Object
                            .Get(It.IsAny<int>())
                            .Result
                            .Success,
                        Message = GamesMessages.GameFoundMessage,
                        DataPacket = new List<object>()
                            {
                                MockedGamesRepository
                                    .SuccessfulRequest
                                    .Object
                                    .Add(It.IsAny<Game>())
                                    .Result
                                    .Object
                            }
                    } as IResult));

            SuccessfulRequest.Setup(Service =>
                Service.GetGames(It.IsAny<IRequest>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedGamesRepository
                            .SuccessfulRequest
                            .Object
                            .GetAll()
                            .Result
                            .Success,
                        Message = GamesMessages.GamesFoundMessage,
                        DataPacket = MockedGamesRepository
                            .SuccessfulRequest
                            .Object
                            .GetAll()
                            .Result
                            .Objects
                            .ConvertAll(g => (object)g)
                    } as IResult));

            SuccessfulRequest.Setup(Service =>
                Service.GetMyGame(It.IsAny<int>(), It.IsAny<IRequest>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedGamesRepository
                            .SuccessfulRequest
                            .Object
                            .GetMyGame(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())
                            .Result
                            .Success,
                        Message = GamesMessages.GameFoundMessage,
                        DataPacket = new List<object>()
                            {
                                MockedGamesRepository
                                    .SuccessfulRequest
                                    .Object
                                    .Add(It.IsAny<Game>())
                                    .Result
                                    .Object
                            }
                    } as IResult));

            SuccessfulRequest.Setup(Service =>
                Service.GetMyGames(It.IsAny<Request>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedGamesRepository
                            .SuccessfulRequest
                            .Object
                            .GetMyGames(It.IsAny<int>(), It.IsAny<int>())
                            .Result
                            .Success,
                        Message = GamesMessages.GamesFoundMessage,
                        DataPacket = MockedGamesRepository
                            .SuccessfulRequest
                            .Object
                            .GetMyGames(It.IsAny<int>(), It.IsAny<int>())
                            .Result
                            .Objects
                            .ConvertAll(g => (object)g)
                    } as IResult));

            SuccessfulRequest.Setup(Service =>
                Service.DeleteMyGame(It.IsAny<int>(), It.IsAny<IRequest>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedGamesRepository
                        .SuccessfulRequest
                        .Object
                        .DeleteMyGame(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())
                        .Result
                        .Success,
                    Message = GamesMessages.GameDeletedMessage
                } as IResult));

            SuccessfulRequest.Setup(Service =>
                Service.Check(It.IsAny<int>(), It.IsAny<IRequest>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedGamesRepository
                            .SuccessfulRequest
                            .Object
                            .Update(It.IsAny<Game>())
                            .Result
                            .Success,
                        Message = GamesMessages.GameSolvedMessage,
                        DataPacket = new List<object>()
                            {
                                MockedGamesRepository
                                    .SuccessfulRequest
                                    .Object
                                    .Add(It.IsAny<Game>())
                                    .Result
                                    .Object
                            }
                    } as IResult));

            SuccessfulRequest.Setup(Service =>
                Service.CreateAnnonymous(It.IsAny<DifficultyLevel>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = true,
                        Message = GamesMessages.GameCreatedMessage,
                        DataPacket = new List<object>() { TestObjects.GetAnnonymousGame() }
                    } as IResult));

            SuccessfulRequest.Setup(Service =>
                Service.CheckAnnonymous(It.IsAny<List<int>>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = true,
                        Message = GamesMessages.GameSolvedMessage
                    } as IResult));

            FailedRequest.Setup(Service =>
                Service.Create(It.IsAny<IRequest>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedGamesRepository
                            .FailedRequest
                            .Object
                            .Add(It.IsAny<Game>())
                            .Result
                            .Success,
                        Message = GamesMessages.GameNotCreatedMessage
                    } as IResult));

            FailedRequest.Setup(Service =>
                Service.Update(It.IsAny<int>(), It.IsAny<IRequest>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedGamesRepository
                            .FailedRequest
                            .Object
                            .Update(It.IsAny<Game>())
                            .Result
                            .Success,
                        Message = GamesMessages.GameNotUpdatedMessage
                    } as IResult));

            FailedRequest.Setup(Service =>
                Service.Delete(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedGamesRepository
                            .FailedRequest
                            .Object
                            .Delete(It.IsAny<Game>())
                            .Result
                            .Success,
                        Message = GamesMessages.GameNotDeletedMessage
                    } as IResult));

            FailedRequest.Setup(Service =>
                Service.GetGame(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedGamesRepository
                            .FailedRequest
                            .Object
                            .GetAppGame(It.IsAny<int>(), It.IsAny<int>())
                            .Result
                            .Success,
                        Message = GamesMessages.GameNotFoundMessage
                    } as IResult));

            FailedRequest.Setup(Service =>
                Service.GetGames(It.IsAny<IRequest>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedGamesRepository
                            .FailedRequest
                            .Object
                            .GetAppGames(It.IsAny<int>())
                            .Result
                            .Success,
                        Message = GamesMessages.GamesNotFoundMessage
                    } as IResult));

            FailedRequest.Setup(Service =>
                Service.GetMyGame(It.IsAny<int>(), It.IsAny<IRequest>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedGamesRepository
                            .FailedRequest
                            .Object
                            .GetMyGame(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())
                            .Result
                            .Success,
                        Message = GamesMessages.GameNotFoundMessage
                    } as IResult));

            FailedRequest.Setup(Service =>
                Service.GetMyGames(It.IsAny<IRequest>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedGamesRepository
                            .FailedRequest
                            .Object
                            .GetMyGame(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())
                            .Result
                            .Success,
                        Message = GamesMessages.GamesNotFoundMessage
                    } as IResult));

            FailedRequest.Setup(Service =>
                Service.DeleteMyGame(It.IsAny<int>(), It.IsAny<IRequest>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedGamesRepository
                            .FailedRequest
                            .Object
                            .DeleteMyGame(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())
                            .Result
                            .Success,
                        Message = GamesMessages.GameNotDeletedMessage
                    } as IResult));

            FailedRequest.Setup(Service =>
                Service.Check(It.IsAny<int>(), It.IsAny<IRequest>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedGamesRepository
                            .FailedRequest
                            .Object
                            .Update(It.IsAny<Game>())
                            .Result
                            .Success,
                        Message = GamesMessages.GameNotUpdatedMessage
                    } as IResult));

            FailedRequest.Setup(Service =>
                Service.CreateAnnonymous(It.IsAny<DifficultyLevel>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = false,
                        Message = GamesMessages.GameNotCreatedMessage
                    } as IResult));

            FailedRequest.Setup(Service =>
                Service.CheckAnnonymous(It.IsAny<List<int>>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = false,
                        Message = GamesMessages.GameNotSolvedMessage
                    } as IResult));
        }
    }
}
