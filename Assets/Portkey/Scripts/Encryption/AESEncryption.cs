using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Portkey.Core;

namespace Portkey.Encryption
{
    /// <summary>
    /// Implementation of IEncryption using AES.
    /// </summary>
    public class AESEncryption : IEncryption
    {
        public byte[] Encrypt(string plainText, string password)
        {
            GenerateKeyAndIVFromPassword(password, out var aesKey, out var aesIV);
            
            return EncryptStringToBytes(plainText, aesKey, aesIV);
        }

        public string Decrypt(byte[] cipherText, string password)
        {
            GenerateKeyAndIVFromPassword(password, out var aesKey, out var aesIV);

            return DecryptStringFromBytes(cipherText, aesKey, aesIV);
        }

        /// <summary>
        /// Generates AES key and IV from password.
        /// </summary>
        /// <param name="password">The password to encrypt with.</param>
        /// <param name="aesKey">The output AES key.</param>
        /// <param name="aesIV">The output AES IV.</param>
        private void GenerateKeyAndIVFromPassword(string password, out byte[] aesKey, out byte[] aesIV)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            aesKey = SHA256Managed.Create().ComputeHash(passwordBytes);
            aesIV = MD5.Create().ComputeHash(passwordBytes);
        }

        /// <summary>
        /// Encrypts a string with a key and IV.
        /// </summary>
        /// <param name="plainText">The string to encrypt.</param>
        /// <param name="Key">The key to encrypt with.</param>
        /// <param name="IV">The IV to encrypt with.</param>
        /// <returns>Encrypted plainText in byte array.</returns>
        private byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }
            if (Key == null || Key.Length <= 0)
            {
                throw new ArgumentNullException("Key");
            }
            if (IV == null || IV.Length <= 0)
            {
                throw new ArgumentNullException("IV");
            }

            // Create an Aes object
            // with the specified key and IV.
            using var aesAlg = Aes.Create();
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            // Create an encryptor to perform the stream transform.
            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for encryption.
            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                //Write all data to the stream.
                swEncrypt.Write(plainText);
            }
            var encrypted = msEncrypt.ToArray();

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        /// <summary>
        /// Decrypts a string with a key and IV.
        /// </summary>
        /// <param name="cipherText">The byte array to decrypt.</param>
        /// <param name="Key">The key to decrypt with.</param>
        /// <param name="IV">The IV to decrypt with.</param>
        /// <returns>Decrypted plainText in string.</returns>
        private string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (Key == null || Key.Length <= 0)
            {
                throw new ArgumentNullException("Key");
            }
            if (IV == null || IV.Length <= 0)
            {
                throw new ArgumentNullException("IV");
            }

            // Declare the string used to hold
            // the decrypted text.
            string plainText = null;

            // Create an Aes object
            // with the specified key and IV.
            using var aesAlg = Aes.Create();
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            // Create a decryptor to perform the stream transform.
            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for decryption.
            using var msDecrypt = new MemoryStream(cipherText);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            // Read the decrypted bytes from the decrypting stream
            // and place them in a string.
            plainText = srDecrypt.ReadToEnd();

            return plainText;
        }
    }
}