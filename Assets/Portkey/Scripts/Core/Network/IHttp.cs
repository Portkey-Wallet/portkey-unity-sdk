using System.Collections;
using UnityEngine;

namespace Portkey.Core
{
    /// <summary>
    /// An interface to http calls.
    /// </summary>
    public abstract class IHttp : ScriptableObject
    {
        public delegate void SuccessCallback(string response);
        
        /// <summary>
        /// Make a GET request to the given url.
        /// </summary>
        /// <param name="url">The url to make the request to.</param>
        /// <param name="body">The body of the request in json.</param>
        /// <param name="successCallback">Callback function when Get is successful.</param>
        /// <param name="errorCallback">Callback function when error occurs. msg contains error message.</param>
        /// <returns>The response from the request.</returns>
        public abstract IEnumerator Get(JsonRequestData data, SuccessCallback successCallback, ErrorCallback errorCallback);
        
        /// <summary>
        /// Make a POST request to the given url.
        /// </summary>
        /// <param name="url">The url to make the request to.</param>
        /// <param name="body">The body of the request in json.</param>
        /// <param name="successCallback">Callback function when Get is successful.</param>
        /// <param name="errorCallback">Callback function when error occurs. msg contains error message.</param>
        /// <returns>The response from the request.</returns>
        public abstract IEnumerator Post(JsonRequestData data, SuccessCallback successCallback, ErrorCallback errorCallback);
        
        /// <summary>
        /// Make a POST request to the given url using field forms.
        /// </summary>
        /// <param name="url">The url to make the request to.</param>
        /// <param name="body">The body of the request in Object T.</param>
        /// <param name="successCallback">Callback function when Get is successful.</param>
        /// <param name="errorCallback">Callback function when error occurs. msg contains error message.</param>
        /// <returns>The response from the request.</returns>
        /// <typeparam name="T">The type of the body that is a serializable type.</typeparam>
        public abstract IEnumerator PostFieldForm<T>(FieldFormRequestData<T> data, SuccessCallback successCallback, ErrorCallback errorCallback);
    }
}