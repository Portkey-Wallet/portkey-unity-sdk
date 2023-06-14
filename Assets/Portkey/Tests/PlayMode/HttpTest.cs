using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Portkey.Core;
using Portkey.Network;
using Portkey.Storage;
using UnityEditor.VersionControl;

namespace Portkey.Test
{
    /// <summary>
    /// HTTP test for testing if implemented class can do request to web.
    /// </summary>
    /// <remarks>
    /// Check NonPersistentStorageMock.cs and NonPersistentStorage.cs for implementation details.
    /// </remarks>
    public class HttpTest
    {
        private const string URL = "https://my-json-server.typicode.com/typicode/demo/posts";
        private const string FAIL_URL = "https://google.com";

        private IHttp request = new FetchJsonHttp();
        public class JsonResponse
        {
            public string query;
            public int id;
        }

        public class Comment
        {
            public string title;
            public int id;
        }

        public void SuccessCallback<T>(T param)
        {
            Debug.Log("successCallback ");
            Assert.NotNull(param);
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

        /// <summary>
        /// Test that TestFetchJsonHttpGet is able to get and retrieve json data.
        /// </summary>
        [UnityTest]
        public IEnumerator TestFetchJsonHttpGet()
        {
            Debug.Log("Get to " + URL);
            yield return request.Get< IList<Comment> >(URL, SuccessCallback, ErrorCallback);
        }
        
        /// <summary>
        /// Test that TestFetchJsonHttpPost is able to post and retrieve json data.
        /// </summary>
        [UnityTest]
        public IEnumerator TestFetchJsonHttpPost()
        {
            Debug.Log("Posting to " + URL);
            yield return request.Post<JsonResponse>(URL, "", SuccessCallback, ErrorCallback);
        }
        
        /// <summary>
        /// Test that TestFetchJsonHttpPostFailURL is fails to retrieve json data and returns into the error callback.
        /// </summary>
        [UnityTest]
        public IEnumerator TestFetchJsonHttpPostFailURL()
        {
            Debug.Log("Posting to " + FAIL_URL);
            yield return request.Post<JsonResponse>(FAIL_URL, "", SuccessCallback, FailErrorCallback);
        }
        
        /// <summary>
        /// Test that TestFetchJsonHttpPostFailType is fails to retrieve json data due to type mismatch and returns into the error callback.
        /// </summary>
        [UnityTest]
        public IEnumerator TestFetchJsonHttpPostFailType()
        {
            Debug.Log("Posting to " + URL);
            yield return request.Post<IList<Comment>>(URL, "", SuccessCallback, FailErrorCallback);
        }
    }
}
