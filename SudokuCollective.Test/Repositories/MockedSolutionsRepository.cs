using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using SudokuCollective.Core.Interfaces.DataModels;
using SudokuCollective.Core.Interfaces.Models;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models;

namespace SudokuCollective.Test.Repositories
{
    public class MockedSolutionsRepository
    {
        private readonly DatabaseContext context;
        internal Mock<ISolutionsRepository<SudokuSolution>> SuccessfulRequest { get; set; }
        internal Mock<ISolutionsRepository<SudokuSolution>> FailedRequest { get; set; }

        public MockedSolutionsRepository(DatabaseContext ctxt)
        {
            context = ctxt;

            SuccessfulRequest = new Mock<ISolutionsRepository<SudokuSolution>>();
            FailedRequest = new Mock<ISolutionsRepository<SudokuSolution>>();

            SuccessfulRequest.Setup(repo =>
                repo.Add(It.IsAny<SudokuSolution>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = new SudokuSolution()
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.Get(It.IsAny<int>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Object = context.SudokuSolutions.FirstOrDefault(s => s.Id == 1)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.GetAll())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context.SudokuSolutions.ToList().ConvertAll(s => (IDomainEntity)s)
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.AddSolutions(It.IsAny<List<ISudokuSolution>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true
                    } as IRepositoryResponse));

            SuccessfulRequest.Setup(repo =>
                repo.GetSolvedSolutions())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = true,
                        Objects = context.SudokuSolutions
                            .Where(s => s.DateSolved > DateTime.MinValue)
                            .ToList()
                            .ConvertAll(s => (IDomainEntity)s)
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.Add(It.IsAny<SudokuSolution>()))
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
                repo.AddSolutions(It.IsAny<List<ISudokuSolution>>()))
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));

            FailedRequest.Setup(repo =>
                repo.GetSolvedSolutions())
                    .Returns(Task.FromResult(new RepositoryResponse()
                    {
                        Success = false
                    } as IRepositoryResponse));
        }
    }
}
