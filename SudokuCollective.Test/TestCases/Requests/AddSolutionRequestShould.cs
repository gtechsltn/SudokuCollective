using NUnit.Framework;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;
using SudokuCollective.Data.Models.Requests;

namespace SudokuCollective.Test.TestCases.Requests
{
    public class AddSolutionRequestShould
    {
        private IAddSolutionRequest sut;

        [SetUp]
        public void Setup()
        {
            sut = new AddSolutionRequest();
        }

        [Test, Category("Requests")]
        public void HasRequiredProperties()
        {
            // Arrange and Act

            // Assert
            Assert.That(sut.Limit, Is.InstanceOf<int>());
        }

        [Test, Category("Requests")]
        public void HaveADefaultConstructor()
        {
            // Arrange and Act
            sut = new AddSolutionRequest();

            // Assert
            Assert.That(sut, Is.InstanceOf<AddSolutionRequest>());
        }

        [Test, Category("Requests")]
        public void HaveAConstructorThatAcceptsIntsForEnums()
        {
            // Arrange and Act
            sut = new AddSolutionRequest(100);

            // Assert
            Assert.That(sut, Is.InstanceOf<AddSolutionRequest>());
        }
    }
}
