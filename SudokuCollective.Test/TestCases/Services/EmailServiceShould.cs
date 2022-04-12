using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Services;
using SudokuCollective.Test.TestData;

namespace SudokuCollective.Test.TestCases.Services
{
    public class EmailServiceShould
    {
        private EmailMetaData emailMetaData;
        private EmailMetaData incorrectEmailMetaData;
        private IEmailService sut;
        private IEmailService sutFailure;
        private Mock<ILogger<EmailService>> mockedLogger;
        private string toEmail;
        private string subject;
        private string html;

        [SetUp]
        public void Setup()
        {
            emailMetaData = TestObjects.GetEmailMetaData();
            incorrectEmailMetaData = TestObjects.GetIncorrectEmailMetaData();
            mockedLogger = new Mock<ILogger<EmailService>>();

            sut = new EmailService(
                emailMetaData, 
                mockedLogger.Object);
                
            sutFailure = new EmailService(
                incorrectEmailMetaData, 
                mockedLogger.Object);

            toEmail = "sudokucollective@gmail.com";
            subject = "testing123...";
            html = "<h1>Hello World!</h1>";
        }

        [Test, Category("Services")]
        public void SendEmails()
        {
            // Arrange

            // Act
            var result = sut.Send(toEmail, subject, html);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test, Category("Services")]
        public void ReturnFalseIfSendEmailsFails()
        {
            // Arrange

            // Act
            var result = sutFailure.Send(toEmail, subject, html);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}
