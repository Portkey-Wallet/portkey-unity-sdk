using UnityEngine;

namespace Portkey.Core
{
    /// <summary>
    /// An interface that handles encryption and decryption.
    /// </summary>
    public abstract class IEncryption : ScriptableObject
    {
        /// <summary>
        /// Encrypts a string with a password.
        /// </summary>
        /// <param name="plainText">The string to encrypt.</param>
        /// <param name="password">The password to encrypt with.</param>
        /// <returns>Encrypted string.</returns>
        public abstract string Encrypt(string plainText, string password);
        /// <summary>
        /// Decrypts a string with a password.
        /// </summary>
        /// <param name="cipherText">The string to decrypt.</param>
        /// <param name="password">The password to decrypt with.</param>
        /// <returns>Derypted string.</returns>
        public abstract string Decrypt(string cipherText, string password);
    }
}

