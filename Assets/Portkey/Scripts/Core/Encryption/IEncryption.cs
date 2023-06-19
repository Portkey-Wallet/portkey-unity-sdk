namespace Portkey.Core
{
    /// <summary>
    /// An interface that handles encryption and decryption.
    /// </summary>
    public interface IEncryption
    {
        /// <summary>
        /// Encrypts a string with a password.
        /// </summary>
        /// <param name="plainText">The string to encrypt.</param>
        /// <param name="password">The password to encrypt with.</param>
        /// <returns>Encrypted string.</returns>
        public string Encrypt(string plainText, string password);
        /// <summary>
        /// Decrypts a string with a password.
        /// </summary>
        /// <param name="cipherText">The string to decrypt.</param>
        /// <param name="password">The password to decrypt with.</param>
        /// <returns>Derypted string.</returns>
        public string Decrypt(string cipherText, string password);
    }
}

