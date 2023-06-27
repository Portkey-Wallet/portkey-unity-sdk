using System.Collections;

namespace Portkey.Core
{
    /// <summary>
    /// An interface to http calls.
    /// </summary>
    public interface IHttp
    {
        public delegate void successCallback(string response);
        public delegate void errorCallback(string msg);
        
        /// <summary>
        /// Make a GET request to the given url.
        /// </summary>
        /// <param name="url">The url to make the request to.</param>
        /// <param name="jsonData">The body of the request in json.</param>
        /// <param name="successCallback">Callback function when Get is successful.</param>
        /// <param name="errorCallback">Callback function when error occurs. msg contains error message.</param>
        /// <returns>The response from the request.</returns>
        public IEnumerator Get(string url, string jsonData, successCallback successCallback, errorCallback errorCallback);

        /// <summary>
        /// Make a POST request to the given url.
        /// </summary>
        /// <param name="url">The url to make the request to.</param>
        /// <param name="jsonData">The body of the request in json.</param>
        /// <param name="successCallback">Callback function when Get is successful.</param>
        /// <param name="errorCallback">Callback function when error occurs. msg contains error message.</param>
        /// <returns>The response from the request.</returns>
        public IEnumerator Post(string url, string jsonData, successCallback successCallback, errorCallback errorCallback);
    }
}