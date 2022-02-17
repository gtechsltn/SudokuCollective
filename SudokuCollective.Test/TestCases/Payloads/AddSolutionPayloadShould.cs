using NUnit.Framework;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads;
using SudokuCollective.Data.Models.Payloads;

namespace SudokuCollective.Test.TestCases.Payloads
{
    public class AddSolutionPayloadShould
    {
        private IAddSolutionPayload sut;

        [SetUp]
        public void Setup()
        {
            sut = new AddSolutionPayload();
        }

        [Test, Category("Payloads")]
        public void HasRequiredProperties()
        {
            // Arrange and Act

            // Assert
            Assert.That(sut.Limit, Is.InstanceOf<int>());
        }

        [Test, Category("Payloads")]
        public void HaveADefaultConstructor()
        {
            // Arrange and Act
            sut = new AddSolutionPayload();

            // Assert
            Assert.That(sut, Is.InstanceOf<AddSolutionPayload>());
        }

        [Test, Category("Payloads")]
        public void HaveAConstructorThatAcceptsIntsForEnums()
        {
            // Arrange and Act
            sut = new AddSolutionPayload(100);

            // Assert
            Assert.That(sut, Is.InstanceOf<AddSolutionPayload>());
        }
    }
}
