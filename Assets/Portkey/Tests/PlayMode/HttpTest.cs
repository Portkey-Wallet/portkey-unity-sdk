using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Portkey.Core;
using Portkey.Network;

namespace Portkey.Test
{
    /// <summary>
    /// HTTP test for testing if implemented class can do request to web.
    /// </summary>
    /// <remarks>
    /// For testing simple http request for json data.
    /// </remarks>
    public class HttpTest
    {
        private const string URL = "https://my-json-server.typicode.com/typicode/demo/posts";
        private const string FAIL_URL = "https://google.com";

        private IHttp _request = new FetchJsonHttp();

        public void SuccessCallback(string response)
        {
            Debug.Log("successCallback ");
            Assert.NotNull(response);
        }
        
        public void ErrorCallback(string param)
        {
            Debug.Log("errorCallback");
            Assert.Fail(param);
        }
        
        public void FailErrorCallback(string param)
        {
            Debug.Log("FailErrorCallback: " + param);
            Assert.Pass(param);
        }
        
#if TEST_FETCHJSONHTTP
        /// <summary>
        /// Test that TestFetchJsonHttpGet is able to get and retrieve json data.
        /// Only tested locally. Build machines to skip this test with server side dependency.
        /// </summary>
        [UnityTest]
        public IEnumerator TestFetchJsonHttpGet()
        {
            Debug.Log("Get to " + URL);
            yield return _request.Get(URL, "", SuccessCallback, ErrorCallback);
        }
        
        /// <summary>
        /// Test that TestFetchJsonHttpPost is able to post and retrieve json data.
        /// Only tested locally. Build machines to skip this test with server side dependency.
        /// </summary>
        [UnityTest]
        public IEnumerator TestFetchJsonHttpPost()
        {
            Debug.Log("Posting to " + URL);
            yield return _request.Post(URL, "", SuccessCallback, ErrorCallback);
        }
#endif
        
        /// <summary>
        /// Test that TestFetchJsonHttpPostFailURL is fails to retrieve json data and returns into the error callback.
        /// </summary>
        [UnityTest]
        public IEnumerator TestFetchJsonHttpPostFailURL()
        {
            Debug.Log("Posting to " + FAIL_URL);
            yield return _request.Post(FAIL_URL, "", SuccessCallback, FailErrorCallback);
        }
    }
}
