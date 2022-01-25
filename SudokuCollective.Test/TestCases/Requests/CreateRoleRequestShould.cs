using NUnit.Framework;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Data.Models.Requests;

namespace SudokuCollective.Test.TestCases.Requests
{
    public class CreateRoleRequestShould
    {
        private ICreateRoleRequest sut;

        [SetUp]
        public void Setup()
        {
            sut = new CreateRoleRequest();
        }

        [Test, Category("Requests")]
        public void HasRequiredProperties()
        {
            // Arrange and Act

            // Assert
            Assert.That(sut.Name, Is.InstanceOf<string>());
            Assert.That(sut.RoleLevel, Is.InstanceOf<RoleLevel>());
        }

        [Test, Category("Requests")]
        public void HaveADefaultConstructor()
        {
            // Arrange and Act
            sut = new CreateRoleRequest();

            // Assert
            Assert.That(sut, Is.InstanceOf<CreateRoleRequest>());
        }

        [Test, Category("Requests")]
        public void HaveAConstructorThatAcceptsIntsForEnums()
        {
            // Arrange and Act
            sut = new CreateRoleRequest(
                "name",
                3);

            // Assert
            Assert.That(sut, Is.InstanceOf<CreateRoleRequest>());
        }

        [Test, Category("Requests")]
        public void HaveAConstructorThatAcceptsEnums()
        {
            // Arrange and Act
            sut = new CreateRoleRequest(
                "name",
                RoleLevel.USER);

            // Assert
            Assert.That(sut, Is.InstanceOf<CreateRoleRequest>());
        }
    }
}
