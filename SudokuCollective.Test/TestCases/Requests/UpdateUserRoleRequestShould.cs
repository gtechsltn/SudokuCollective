using System.Collections.Generic;
using NUnit.Framework;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Data.Models.Requests;

namespace SudokuCollective.Test.TestCases.Requests
{
    internal class UpdateUserRoleRequestShould
    {
        private IUpdateUserRoleRequest sut;

        [SetUp]
        public void Setup()
        {
            sut = new UpdateUserRoleRequest();
        }

        [Test, Category("Requests")]
        public void HasRequiredProperties()
        {
            // Arrange and Act

            // Assert
            Assert.That(sut.RoleIds, Is.InstanceOf<List<int>>());
        }

        [Test, Category("Requests")]
        public void HasADefaultConstructor()
        {
            // Arrange and Act
            sut = new UpdateUserRoleRequest();

            // Assert
            Assert.That(sut, Is.InstanceOf<UpdateUserRoleRequest>());
        }

        [Test, Category("Requests")]
        public void HasAConstructorThatAcceptsIntArray()
        {
            // Arrange
            var intArray = new int[1];

            // Act
            sut = new UpdateUserRoleRequest(intArray);

            // Assert
            Assert.That(sut, Is.InstanceOf<UpdateUserRoleRequest>());
        }

        [Test, Category("Requests")]
        public void HasAConstructorThatAcceptsIntList()
        {
            // Arrange
            var intList = new List<int> { 1 };

            // Act
            sut = new UpdateUserRoleRequest(intList);

            // Assert
            Assert.That(sut, Is.InstanceOf<UpdateUserRoleRequest>());
        }
    }
}
