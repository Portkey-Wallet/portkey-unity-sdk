using System.Collections;

namespace Portkey.Core
{
    /// <summary>
    /// An interface to http calls.
    /// </summary>
    public interface IHttp
    {
        public delegate void successCallback(string response);
        
        /// <summary>
        /// Make a GET request to the given url.
        /// </summary>
        /// <param name="url">The url to make the request to.</param>
        /// <param name="body">The body of the request in json.</param>
        /// <param name="successCallback">Callback function when Get is successful.</param>
        /// <param name="errorCallback">Callback function when error occurs. msg contains error message.</param>
        /// <returns>The response from the request.</returns>
        public IEnumerator Get(string url, string body, successCallback successCallback, ErrorCallback errorCallback);
        
        /// <summary>
        /// Make a POST request to the given url.
        /// </summary>
        /// <param name="url">The url to make the request to.</param>
        /// <param name="body">The body of the request in json.</param>
        /// <param name="successCallback">Callback function when Get is successful.</param>
        /// <param name="errorCallback">Callback function when error occurs. msg contains error message.</param>
        /// <returns>The response from the request.</returns>
        public IEnumerator Post(string url, string body, successCallback successCallback, ErrorCallback errorCallback);
        
        /// <summary>
        /// Make a POST request to the given url.
        /// </summary>
        /// <param name="url">The url to make the request to.</param>
        /// <param name="body">The body of the request in Object T.</param>
        /// <param name="successCallback">Callback function when Get is successful.</param>
        /// <param name="errorCallback">Callback function when error occurs. msg contains error message.</param>
        /// <returns>The response from the request.</returns>
        /// <typeparam name="T">The type of the body that is a serializable type.</typeparam>
        public IEnumerator Post<T>(string url, T body, successCallback successCallback, ErrorCallback errorCallback);
    }
}