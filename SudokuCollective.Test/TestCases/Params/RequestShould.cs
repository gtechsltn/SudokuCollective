using NUnit.Framework;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Params;
using SudokuCollective.Data.Models.Params;

namespace SudokuCollective.Test.TestCases.Params
{
    public class RequestShould
    {
        private IRequest sut;

        [SetUp]
        public void Setup()
        {
            sut = new Request();
        }

        [Test, Category("Params")]
        public void HasRequiredProperties()
        {
            // Arrange and Act
            
            // Assert
            Assert.That(sut.License, Is.InstanceOf<string>());
            Assert.That(sut.RequestorId, Is.InstanceOf<int>());
            Assert.That(sut.AppId, Is.InstanceOf<int>());
            Assert.That(sut.Paginator, Is.InstanceOf<IPaginator>());
        }

        [Test, Category("Params")]
        public void AcceptsObectsInTheDataPacket()
        {
            // Arrange
            var obj = new object();

            // Act
            sut.DataPacket = obj;
            
            // Assert
            Assert.That(sut.DataPacket, Is.InstanceOf<object>());
        }
    }
}
