using System.Text.RegularExpressions;
using NUnit.Framework;
using SudokuCollective.Core.Validation;

namespace SudokuCollective.Test.TestCases.RegularExpressions
{
    public class EmailRegexShould
    {
        private Regex? sut;

        [SetUp]
        public void Setup()
        {
            sut = new Regex(RegexValidators.EmailRegexPattern);
        }

        [Test, Category("Regex")]
        public void AcceptsProperlyFormattedEmails()
        {
            // Arrange
            if (sut == null)
            {
                sut = new Regex(RegexValidators.EmailRegexPattern);
            }

            var email = "TestEmail@example.com";

            // Act
            var result = sut.IsMatch(email);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test, Category("Regex")]
        public void RejectsImproperlyFormattedEmails()
        {
            // Arrange
            if (sut == null)
            {
                sut = new Regex(RegexValidators.EmailRegexPattern);
            }

            var email = "TestEmail@example";

            // Act
            var result = sut.IsMatch(email);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test, Category("Regex")]
        public void RejectsEmailsWithoutAtSign()
        {
            // Arrange
            if (sut == null)
            {
                sut = new Regex(RegexValidators.EmailRegexPattern);
            }

            var email = "TestEmailexample";

            // Act
            var result = sut.IsMatch(email);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}
