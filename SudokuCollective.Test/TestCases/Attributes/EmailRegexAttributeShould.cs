using NUnit.Framework;
using SudokuCollective.Core.Validation.Attributes;

namespace SudokuCollective.Test.TestCases.Attributes
{
    public class EmailRegexAttributeShould
    {
        private EmailRegexAttribute? sut;

        [SetUp]
        public void Setup()
        {
            sut = new EmailRegexAttribute();
        }

        [Test, Category("Attributes")]
        public void AcceptsProperlyFormattedEmails()
        {
            // Arrange
            if (sut == null)
            {
                sut = new EmailRegexAttribute();
            }

            var email = "TestEmail@example.com";

            // Act
            var result = sut.IsValid(email);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test, Category("Attributes")]
        public void RejectsImproperlyFormattedEmails()
        {
            // Arrange
            if (sut == null)
            {
                sut = new EmailRegexAttribute();
            }

            var email = "TestEmail@example";

            // Act
            var result = sut.IsValid(email);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test, Category("Attributes")]
        public void RejectsEmailsWithoutAtSign()
        {
            // Arrange
            if (sut == null)
            {
                sut = new EmailRegexAttribute();
            }

            var email = "TestEmailexample";

            // Act
            var result = sut.IsValid(email);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}
