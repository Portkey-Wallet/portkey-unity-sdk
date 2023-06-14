using System.Collections;
using UnityEngine;

namespace Portkey.Core
{
    /// <summary>
    /// An interface to http calls.
    /// </summary>
    public abstract class IHttp : ScriptableObject
    {
        public delegate void successCallback<T>(T param);
        public delegate void errorCallback(string msg);
        
        /// <summary>
        /// Make a GET request to the given url.
        /// </summary>
        /// <param name="url">The url to make the request to.</param>
        /// <returns>The response from the request.</returns>
        public abstract IEnumerator Get<T>(string url, successCallback<T> successCallback, errorCallback errorCallback);

        /// <summary>
        /// Make a POST request to the given url.
        /// </summary>
        /// <param name="url">The url to make the request to.</param>
        /// <param name="body">The body of the request.</param>
        /// <returns>The response from the request.</returns>
        public abstract IEnumerator Post<T>(string url, string body, successCallback<T> successCallback, errorCallback errorCallback);
    }
}