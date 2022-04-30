using NUnit.Framework;
using SudokuCollective.Data.Encryption;

namespace SudokuCollective.Test.TestCases.Encryption
{
    public class DataEncryptionShould
    {
        private string key;

        [SetUp]
        public void SetUp()
        {
            key = "f03e9baaf3e24b90a6e5dac09547aead498eb688cc54442b800a3fe24efb9e94a3c692dffb4f428daf7512cd68628e4c240c2f30949a4903be60457b19f213254d4cb3d0611244cbb7b0e62b53ca0b34";
        }

        [Test, Category("Encryption")]
        public void EncryptStrings()
        {
            // Arrange
            var value = "value";

            // Act
            var encryptedValue = DataEncryption.EncryptString(value, key);

            // Assert
            Assert.That(string.Equals(value, encryptedValue), Is.False);
        }

        [Test, Category("Encryption")]
        public void DecryptStrings()
        {
            // Arrange
            var value = "value";
            var encryptedValue = DataEncryption.EncryptString(value, key);

            // Act
            var decryptedValue = DataEncryption.DecryptString(encryptedValue, key);

            // Assert
            Assert.That(string.Equals(value, decryptedValue), Is.True);
        }
    }
}
