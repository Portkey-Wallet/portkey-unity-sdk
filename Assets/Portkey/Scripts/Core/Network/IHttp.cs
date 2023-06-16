using System.Collections;
using UnityEngine;

namespace Portkey.Core
{
    /// <summary>
    /// An interface to http calls.
    /// </summary>
    public abstract class IHttp : ScriptableObject
    {
        public delegate void successCallback(string response);
        public delegate void errorCallback(string msg);
        
        /// <summary>
        /// Make a GET request to the given url.
        /// </summary>
        /// <param name="url">The url to make the request to.</param>
        /// <param name="successCallback">Callback function when Get is successful.</param>
        /// <param name="errorCallback">Callback function when error occurs. msg contains error message.</param>
        /// <returns>The response from the request.</returns>
        public abstract IEnumerator Get(string url, successCallback successCallback, errorCallback errorCallback);

        /// <summary>
        /// Make a POST request to the given url.
        /// </summary>
        /// <param name="url">The url to make the request to.</param>
        /// <param name="body">The body of the request.</param>
        /// <param name="successCallback">Callback function when Get is successful.</param>
        /// <param name="errorCallback">Callback function when error occurs. msg contains error message.</param>
        /// <returns>The response from the request.</returns>
        public abstract IEnumerator Post(string url, string body, successCallback successCallback, errorCallback errorCallback);
    }
}