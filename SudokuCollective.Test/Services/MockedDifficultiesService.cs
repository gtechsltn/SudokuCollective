using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using SudokuCollective.Data.Models;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Messages;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Test.Repositories;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;

namespace SudokuCollective.Test.Services
{
    public class MockedDifficultiesService
    {
        private MockedDifficultiesRepository MockedDifficultiesRepository { get; set; }

        internal Mock<IDifficultiesService> SuccessfulRequest { get; set; }
        internal Mock<IDifficultiesService> FailedRequest { get; set; }

        public MockedDifficultiesService(DatabaseContext context)
        {
            MockedDifficultiesRepository = new MockedDifficultiesRepository(context);

            SuccessfulRequest = new Mock<IDifficultiesService>();
            FailedRequest = new Mock<IDifficultiesService>();

            SuccessfulRequest.Setup(Service =>
                Service.Get(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedDifficultiesRepository
                            .SuccessfulRequest
                            .Object
                            .Get(It.IsAny<int>())
                            .Result
                            .IsSuccess,
                        Message = DifficultiesMessages.DifficultyFoundMessage,
                        Payload = new List<object>()
                        {
                            MockedDifficultiesRepository
                                .SuccessfulRequest
                                .Object
                                .Get(It.IsAny<int>())
                                .Result
                                .Object
                        }
                    } as IResult));

            SuccessfulRequest.Setup(Service =>
                Service.GetDifficulties())
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedDifficultiesRepository
                            .SuccessfulRequest
                            .Object
                            .GetAll()
                            .Result
                            .IsSuccess,
                        Message = DifficultiesMessages.DifficultiesFoundMessage,
                        Payload = MockedDifficultiesRepository
                            .SuccessfulRequest
                            .Object
                            .GetAll()
                            .Result
                            .Objects
                            .ConvertAll(d => (object)d)
                    } as IResult));

            SuccessfulRequest.Setup(Service =>
                Service.Create(It.IsAny<IRequest>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedDifficultiesRepository
                            .SuccessfulRequest
                            .Object
                            .Add(It.IsAny<Difficulty>())
                            .Result
                            .IsSuccess,
                        Message = DifficultiesMessages.DifficultyCreatedMessage,
                        Payload = new List<object>()
                            {
                                MockedDifficultiesRepository
                                    .SuccessfulRequest
                                    .Object
                                    .Get(It.IsAny<int>())
                                    .Result
                                    .Object
                            }
                    } as IResult));

            SuccessfulRequest.Setup(Service =>
                Service.Update(It.IsAny<int>(), It.IsAny<IRequest>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedDifficultiesRepository
                            .SuccessfulRequest
                            .Object
                            .Update(It.IsAny<Difficulty>())
                            .Result
                            .IsSuccess,
                        Message = DifficultiesMessages.DifficultyUpdatedMessage
                    } as IResult));

            SuccessfulRequest.Setup(Service =>
                Service.Delete(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedDifficultiesRepository
                            .SuccessfulRequest
                            .Object
                            .Delete(It.IsAny<Difficulty>())
                            .Result
                            .IsSuccess,
                        Message = DifficultiesMessages.DifficultyDeletedMessage
                    } as IResult));

            FailedRequest.Setup(Service =>
                Service.Get(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedDifficultiesRepository
                            .FailedRequest
                            .Object
                            .Add(It.IsAny<Difficulty>())
                            .Result
                            .IsSuccess,
                        Message = DifficultiesMessages.DifficultyNotFoundMessage
                    } as IResult));

            FailedRequest.Setup(Service =>
                Service.GetDifficulties())
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedDifficultiesRepository
                            .FailedRequest
                            .Object
                            .GetAll()
                            .Result
                            .IsSuccess,
                        Message = DifficultiesMessages.DifficultiesNotFoundMessage
                    } as IResult));

            FailedRequest.Setup(Service =>
                Service.Create(It.IsAny<IRequest>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = MockedDifficultiesRepository
                        .FailedRequest
                        .Object
                        .Add(It.IsAny<Difficulty>())
                        .Result
                        .IsSuccess,
                    Message = DifficultiesMessages.DifficultyNotCreatedMessage
                } as IResult));

            FailedRequest.Setup(Service =>
                Service.Update(It.IsAny<int>(), It.IsAny<IRequest>()))
                    .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedDifficultiesRepository
                            .FailedRequest
                            .Object
                            .Update(It.IsAny<Difficulty>())
                            .Result
                            .IsSuccess,
                        Message = DifficultiesMessages.DifficultyNotUpdatedMessage
                    } as IResult));

            FailedRequest.Setup(Service =>
                Service.Delete(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = MockedDifficultiesRepository
                            .FailedRequest
                            .Object
                            .Delete(It.IsAny<Difficulty>())
                            .Result
                            .IsSuccess,
                        Message = DifficultiesMessages.DifficultyNotDeletedMessage
                    } as IResult));
        }
    }
}
