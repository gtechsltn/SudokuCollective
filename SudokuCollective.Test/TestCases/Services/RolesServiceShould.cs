using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using SudokuCollective.Cache;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Models.Params;
using SudokuCollective.Data.Models.Requests;
using SudokuCollective.Data.Services;
using SudokuCollective.Test.Cache;
using SudokuCollective.Test.Repositories;
using SudokuCollective.Test.TestData;

namespace SudokuCollective.Test.TestCases.Services
{
    public class RolesServiceShould
    {
        private DatabaseContext context;
        private MockedRolesRepository mockedkRolesRepository;
        private MockedCacheService mockedCacheService;
        private MemoryDistributedCache memoryCache;
        private RolesService sut;
        private RolesService sutFailue;
        private Request request;
        private UpdateRoleRequest updateRoleRequest;

        [SetUp]
        public async Task Setup()
        {
            context = await TestDatabase.GetDatabaseContext();
            mockedkRolesRepository = new MockedRolesRepository(context);
            mockedCacheService = new MockedCacheService(context);
            memoryCache = new MemoryDistributedCache(
                Options.Create(new MemoryDistributedCacheOptions()));

            sut = new RolesService(
                mockedkRolesRepository.SuccessfulRequest.Object,
                memoryCache,
                mockedCacheService.SuccessfulRequest.Object,
                new CacheKeys(),
                new CachingStrategy());
            sutFailue = new RolesService(
                mockedkRolesRepository.FailedRequest.Object,
                memoryCache,
                mockedCacheService.FailedRequest.Object,
                new CacheKeys(),
                new CachingStrategy());
        }

        [Test, Category("Services")]
        public async Task GetARole()
        {
            // Arrange

            // Act
            var result = await sut.Get(1);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Role Found"));
            Assert.That((Role)result.DataPacket[0], Is.TypeOf<Role>());
        }

        [Test, Category("Services")]
        public async Task IssueMessageIfRoleNotFound()
        {
            // Arrange

            // Act
            var result = await sutFailue.Get(1);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Role not Found"));
        }

        [Test, Category("Services")]
        public async Task GetRoles()
        {
            // Arrange

            // Act
            var result = await sut.GetRoles();

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Roles Found"));
            Assert.That(result.DataPacket.ConvertAll(r => (IRole)r), Is.TypeOf<List<IRole>>());
        }

        [Test, Category("Services")]
        public async Task IssueMessageIfRolesNotFound()
        {
            // Arrange

            // Act
            var result = await sutFailue.GetRoles();

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Roles not Found"));
        }

        [Test, Category("Services")]
        public async Task GetRolesWithoutNullOrSuperUserRoleLevel()
        {
            // Arrange

            // Act
            var result = await sut.GetRoles();
            var nullAndSuperUserRoleLevelsBlocked = result.DataPacket
                .ConvertAll(r => (Role)r)
                .Any(role =>
                    role.RoleLevel.Equals(RoleLevel.NULL)
                    || role.RoleLevel.Equals(RoleLevel.SUPERUSER));

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Roles Found"));
            Assert.That(nullAndSuperUserRoleLevelsBlocked, Is.False);
        }

        [Test, Category("Services")]
        public async Task UpdateADifficulty()
        {
            // Arrange
            request = TestObjects.GetRequest();
            updateRoleRequest = TestObjects.GetUpdateRoleRequest();
            request.DataPacket = updateRoleRequest;

            // Act
            var result = await sut.Update(1, request);
            var updatedDifficulty = context.Roles
                .FirstOrDefault(role => role.Id == 1);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Role Updated"));
        }

        [Test, Category("Services")]
        public async Task IssueMessageIfUpdateFails()
        {
            // Arrange
            request = TestObjects.GetRequest();
            updateRoleRequest = TestObjects.GetInvalidUpdateRoleRequest();
            request.DataPacket = updateRoleRequest;

            // Act
            var result = await sutFailue.Update(1, request);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Role not Found"));
        }

        [Test, Category("Services")]
        public async Task DeleteADifficulty()
        {
            // Arrange

            // Act
            var result = await sut.Delete(1);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Role Deleted"));
        }

        [Test, Category("Services")]
        public async Task IssueMessageIfRoleNotDeleted()
        {
            // Arrange

            // Act
            var result = await sutFailue.Delete(10);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("Role not Found"));
        }
    }
}
