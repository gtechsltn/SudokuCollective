using NUnit.Framework;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads;
using SudokuCollective.Data.Models.Payloads;

namespace SudokuCollective.Test.TestCases.Payloads
{
    public class AnnonymousGamePayloadShould
    {
        private IAnnonymousGamePayload sut;

        [SetUp]
        public void Setup()
        {
            sut = new AnnonymousGamePayload();
        }

        [Test, Category("Payloads")]
        public void HasRequiredProperties()
        {
            // Arrange and Act
            
            // Assert
            Assert.That(sut.DifficultyLevel, Is.InstanceOf<DifficultyLevel>());
        }

        [Test, Category("Payloads")]
        public void HasADefaultConstructor()
        {
            // Arrange and Act
            sut = new AnnonymousGamePayload();

            // Assert
            Assert.That(sut, Is.InstanceOf<AnnonymousGamePayload>());
        }

        [Test, Category("Payloads")]
        public void HasAConstructorThatAcceptsInts()
        {
            // Arrange and Act
            sut = new AnnonymousGamePayload(1);

            // Assert
            Assert.That(sut, Is.InstanceOf<AnnonymousGamePayload>());
        }

        [Test, Category("Payloads")]
        public void HasAConstructorThatAcceptsDifficultyLevels()
        {
            // Arrange and Act
            sut = new AnnonymousGamePayload(DifficultyLevel.EASY);

            // Assert
            Assert.That(sut, Is.InstanceOf<AnnonymousGamePayload>());
        }
    }
}
