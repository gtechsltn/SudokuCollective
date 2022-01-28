using Moq;
using SudokuCollective.Core.Interfaces.Services;

namespace SudokuCollective.Test.Services
{
    public class MockedEmailService
    {
        internal Mock<IEmailService> SuccessfulRequest { get; set; }
        internal Mock<IEmailService> FailedRequest { get; set; }

        public MockedEmailService()
        {
            SuccessfulRequest = new Mock<IEmailService>();
            FailedRequest = new Mock<IEmailService>();

            SuccessfulRequest.Setup(emailService =>
                emailService.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            FailedRequest.Setup(emailService =>
                emailService.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);
        }
    }
}
