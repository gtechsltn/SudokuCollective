using System.Threading.Tasks;
using Moq;
using SudokuCollective.Data.Models;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Messages;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Test.Repositories;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using System.Collections.Generic;

namespace SudokuCollective.Test.Services
{
    public class MockedDifficultiesService
    {
        private MockedDifficultiesRepository mockedDifficultiesRepository { get; set; }

        internal Mock<IDifficultiesService> SuccessfulRequest { get; set; }
        internal Mock<IDifficultiesService> FailedRequest { get; set; }

        public MockedDifficultiesService(DatabaseContext context)
        {
            mockedDifficultiesRepository = new MockedDifficultiesRepository(context);

            SuccessfulRequest = new Mock<IDifficultiesService>();
            FailedRequest = new Mock<IDifficultiesService>();

            SuccessfulRequest.Setup(difficultiesService =>
                difficultiesService.Get(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = mockedDifficultiesRepository
                            .SuccessfulRequest
                            .Object
                            .Get(It.IsAny<int>())
                            .Result
                            .Success,
                        Message = DifficultiesMessages.DifficultyFoundMessage,
                        DataPacket = new List<object>()
                        {
                            mockedDifficultiesRepository
                                .SuccessfulRequest
                                .Object
                                .Get(It.IsAny<int>())
                                .Result
                                .Object
                        }
                    } as IResult));

            SuccessfulRequest.Setup(difficultiesService =>
                difficultiesService.GetDifficulties())
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = mockedDifficultiesRepository
                            .SuccessfulRequest
                            .Object
                            .GetAll()
                            .Result
                            .Success,
                        Message = DifficultiesMessages.DifficultiesFoundMessage,
                        DataPacket = mockedDifficultiesRepository
                            .SuccessfulRequest
                            .Object
                            .GetAll()
                            .Result
                            .Objects
                            .ConvertAll(d => (object)d)
                    } as IResult));

            SuccessfulRequest.Setup(difficultiesService =>
                difficultiesService.Create(It.IsAny<IRequest>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = mockedDifficultiesRepository
                            .SuccessfulRequest
                            .Object
                            .Add(It.IsAny<Difficulty>())
                            .Result
                            .Success,
                        Message = DifficultiesMessages.DifficultyCreatedMessage,
                        DataPacket = new List<object>()
                            {
                                mockedDifficultiesRepository
                                    .SuccessfulRequest
                                    .Object
                                    .Get(It.IsAny<int>())
                                    .Result
                                    .Object
                            }
                    } as IResult));

            SuccessfulRequest.Setup(difficultiesService =>
                difficultiesService.Update(It.IsAny<int>(), It.IsAny<IRequest>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = mockedDifficultiesRepository
                            .SuccessfulRequest
                            .Object
                            .Update(It.IsAny<Difficulty>())
                            .Result
                            .Success,
                        Message = DifficultiesMessages.DifficultyUpdatedMessage
                    } as IResult));

            SuccessfulRequest.Setup(difficultiesService =>
                difficultiesService.Delete(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = mockedDifficultiesRepository
                            .SuccessfulRequest
                            .Object
                            .Delete(It.IsAny<Difficulty>())
                            .Result
                            .Success,
                        Message = DifficultiesMessages.DifficultyDeletedMessage
                    } as IResult));

            FailedRequest.Setup(difficultiesService =>
                difficultiesService.Get(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = mockedDifficultiesRepository
                            .FailedRequest
                            .Object
                            .Add(It.IsAny<Difficulty>())
                            .Result
                            .Success,
                        Message = DifficultiesMessages.DifficultyNotFoundMessage
                    } as IResult));

            FailedRequest.Setup(difficultiesService =>
                difficultiesService.GetDifficulties())
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = mockedDifficultiesRepository
                            .FailedRequest
                            .Object
                            .GetAll()
                            .Result
                            .Success,
                        Message = DifficultiesMessages.DifficultiesNotFoundMessage
                    } as IResult));

            FailedRequest.Setup(difficultiesService =>
                difficultiesService.Create(It.IsAny<IRequest>()))
                .Returns(Task.FromResult(new Result()
                {
                    IsSuccess = mockedDifficultiesRepository
                        .FailedRequest
                        .Object
                        .Add(It.IsAny<Difficulty>())
                        .Result
                        .Success,
                    Message = DifficultiesMessages.DifficultyNotCreatedMessage
                } as IResult));

            FailedRequest.Setup(difficultiesService =>
                difficultiesService.Update(It.IsAny<int>(), It.IsAny<IRequest>()))
                    .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = mockedDifficultiesRepository
                            .FailedRequest
                            .Object
                            .Update(It.IsAny<Difficulty>())
                            .Result
                            .Success,
                        Message = DifficultiesMessages.DifficultyNotUpdatedMessage
                    } as IResult));

            FailedRequest.Setup(difficultiesService =>
                difficultiesService.Delete(It.IsAny<int>()))
                .Returns(Task.FromResult(new Result()
                    {
                        IsSuccess = mockedDifficultiesRepository
                            .FailedRequest
                            .Object
                            .Delete(It.IsAny<Difficulty>())
                            .Result
                            .Success,
                        Message = DifficultiesMessages.DifficultyNotDeletedMessage
                    } as IResult));
        }
    }
}
