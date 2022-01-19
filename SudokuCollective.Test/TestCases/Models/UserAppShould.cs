using NUnit.Framework;
using SudokuCollective.Core.Interfaces.Models;
using SudokuCollective.Core.Interfaces.Models.DomainEntities;
using SudokuCollective.Core.Models;

namespace SudokuCollective.Test.TestCases.Models
{
    public class UserAppShould
    {
        private IUserApp? sut;

        [SetUp]
        public void Setup()
        {
            sut = new UserApp();
        }

        [Test, Category("Models")]
        public void ImplementIDomainEntity()
        {
            // Arrange and Act
            if (sut == null)
            {
                sut = new UserApp();
            }

            // Assert
            Assert.That(sut, Is.InstanceOf<IDomainEntity>());
        }

        [Test, Category("Models")]
        public void HaveAnID()
        {
            // Arrange and Act
            if (sut == null)
            {
                sut = new UserApp();
            }

            // Assert
            Assert.That(sut.Id, Is.TypeOf<int>());
            Assert.That(sut.Id, Is.EqualTo(0));
        }

        [Test, Category("Models")]
        public void UserIdAndAppIdAreInts()
        {
            // Arrange and Act
            if (sut == null)
            {
                sut = new UserApp();
            }

            // Assert
            Assert.That(sut.UserId, Is.TypeOf<int>());
            Assert.That(sut.AppId, Is.TypeOf<int>());
        }

        [Test, Category("Models")]
        public void AcceptsReferenceToUserObjects()
        {
            // Arrange
            if (sut == null)
            {
                sut = new UserApp();
            }

            var user = new User();

            // Act
            sut.User = user;

            // Assert
            Assert.That(user, Is.InstanceOf<User>());
            Assert.That(((UserApp)sut).User, Is.InstanceOf<User>());
        }

        [Test, Category("Models")]
        public void AcceptsReferenceToAppObjects()
        {
            // Arrange
            if (sut == null)
            {
                sut = new UserApp();
            }

            var app = new App();

            // Act
            sut.App = app;

            // Assert
            Assert.That(app, Is.InstanceOf<App>());
            Assert.That(((UserApp)sut).App, Is.InstanceOf<App>());
        }
    }
}
