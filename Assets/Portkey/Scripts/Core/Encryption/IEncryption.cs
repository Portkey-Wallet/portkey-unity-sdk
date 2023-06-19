using UnityEngine;

namespace Portkey.Core
{
    /// <summary>
    /// An
    /// </summary>
    public abstract class IEncryption : ScriptableObject
    {
        /// <summary>
        /// Make a GET request to the given url.
        /// </summary>
        /// <param name="url">The url to make the request to.</param>
        /// <param name="successCallback">Callback function when Get is successful.</param>
        /// <param name="errorCallback">Callback function when error occurs. msg contains error message.</param>
        /// <returns>The response from the request.</returns>
        public string abstract Encrypt(string plainText, string password);
        public string abstract Decrypt(string cipherText, string password);
    }
}

