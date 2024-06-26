using System;
using System.Text;
using NUnit.Framework;
using Portkey.Core;
using Portkey.Encryption;

namespace Portkey.Test
{
    /// <summary>
    /// Encryption test for testing all encryption related classes with interface IEncryption.
    /// </summary>
    /// <remarks>
    /// Check AESEncryption.cs for implementation details.
    /// </remarks>
    public class EncryptionTest
    {
        private const string PASSWORD = "password123!";
        private const string PLAINTEXT = "lorem ipsum"; 

        /// <summary>
        /// Test that AESEncryption is able to encrypt and decrypt data.
        /// </summary>
        [Test]
        public void TestAESEncryptionEncryptAndDecryptPass()
        {
            IEncryption aes = new AESEncryption();
            var cipherText = aes.Encrypt(PLAINTEXT, PASSWORD);
            var plainText = aes.Decrypt(cipherText, PASSWORD);
            Assert.AreEqual(PLAINTEXT, plainText);
        }

        /// <summary>
        /// Test that AESEncryption is able to encrypt and decrypt data with non-utf8 password.
        /// </summary>
        [Test]
        public void TestAESEncryptionEncryptAndDecryptNonUTF8Password()
        {
            byte[] nonUtf8Bytes = {0xC0, 0xC1, 0xF5, 0xFF, 0xF6};
            var weirdPassword = Convert.ToBase64String(nonUtf8Bytes);

            IEncryption aes = new AESEncryption();
            var cipherText = aes.Encrypt(PLAINTEXT, weirdPassword);
            var plainText = aes.Decrypt(cipherText, weirdPassword);
            Assert.AreEqual(PLAINTEXT, plainText);
        }
        
        /// <summary>
        /// Test that AESEncryption is able to encrypt and decrypt data with non-utf8 plaintext.
        /// </summary>
        [Test]
        public void TestAESEncryptionEncryptAndDecryptNonUTF8Plaintext()
        {
            byte[] nonUtf8Bytes = { 0xC0, 0xC1, 0xF5, 0xFF, 0xF6 };
            var weirdPlainText = Convert.ToBase64String(nonUtf8Bytes);

            IEncryption aes = new AESEncryption();
            var cipherText = aes.Encrypt(weirdPlainText, PASSWORD);
            var plainText = aes.Decrypt(cipherText, PASSWORD);
            Assert.AreEqual(weirdPlainText, plainText);
        }
        
        /// <summary>
        /// Test that AESEncryption is able to throw exception when decrypting with incorrect cipher.
        /// </summary>
        [Test]
        public void TestAESEncryptionDecryptNonWorkableCipher()
        {
            var randomText = Convert.FromBase64String("random string");
            IEncryption aes = new AESEncryption();
            try
            {
                var plainText = aes.Decrypt(randomText, PASSWORD);
            }
            catch (Exception e)
            {
                Assert.Pass(e.Message);
            }
            Assert.Fail("Exception is supposed to be thrown.");
        }
        
        /// <summary>
        /// Test that AESEncryption is able to throw exception when decrypting with incorrect password.
        /// </summary>
        [Test]
        public void TestAESEncryptionDecryptIncorrectPassword()
        {
            IEncryption aes = new AESEncryption();
            var cipherText = aes.Encrypt(PLAINTEXT, PASSWORD);
            try
            {
                var plainText = aes.Decrypt(cipherText, "incorrect password");
            }
            catch (Exception e)
            {
                Assert.Pass(e.Message);
            }
            Assert.Fail("Exception is supposed to be thrown.");
        }
        
        /// <summary>
        /// Test that AESEncryption is able to throw exception when encrypting with null input.
        /// </summary>
        [Test]
        public void TestAESEncryptionEncryptInvalidInput()
        {
            IEncryption aes = new AESEncryption();
            try
            {
                var cipherText = aes.Encrypt(null, PASSWORD);
            }
            catch (ArgumentNullException e)
            {
                Assert.Pass(e.Message);
            }
            Assert.Fail("Exception is supposed to be thrown.");
        }
        
        /// <summary>
        /// Test that AESEncryption is able to throw exception when decrypting with null input.
        /// </summary>
        [Test]
        public void TestAESEncryptionDecryptInvalidInput()
        {
            IEncryption aes = new AESEncryption();
            var cipherText = aes.Encrypt(PLAINTEXT, PASSWORD);
            try
            {
                var plainText = aes.Decrypt(cipherText, null);
            }
            catch (ArgumentNullException e)
            {
                Assert.Pass(e.Message);
            }
            Assert.Fail("Exception is supposed to be thrown.");
        }
    }
}