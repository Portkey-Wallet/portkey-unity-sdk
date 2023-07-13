using System;
using System.Collections;
using System.Text;
using Portkey.Core;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Portkey.Network
{
    [CreateAssetMenu(fileName = "FetchJsonHttp", menuName = "Portkey/Network/FetchJsonHttp")]
    public class FetchJsonHttp : IHttp
    {
        public override IEnumerator Get(string url, string jsonData, IHttp.successCallback successCallback, IHttp.errorCallback errorCallback)
        {
            jsonData??=string.Empty;
            
            byte[] postData = Encoding.ASCII.GetBytes(jsonData);
            using var request = new UnityWebRequest(url, 
                                                    UnityWebRequest.kHttpVerbGET,
                                                    new DownloadHandlerBuffer(), 
                                                    new UploadHandlerRaw(postData))
            {
                disposeUploadHandlerOnDispose = true,
                disposeDownloadHandlerOnDispose = true
            };
            
            request.SetRequestHeader("Content-Type", "application/json");
            
            yield return request.SendWebRequest();

            if (request.error != null)
            {
                errorCallback(request.error);
                yield break;
            }
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                errorCallback(request.responseCode.ToString());
                yield break;
            }
            successCallback(request.downloadHandler.text);
        }

        public override IEnumerator Post(string url, string jsonData, IHttp.successCallback successCallback, IHttp.errorCallback errorCallback)
        {
            jsonData??=string.Empty;
            
            byte[] postData = Encoding.ASCII.GetBytes(jsonData);
            using var request = new UnityWebRequest(url, 
                                                    UnityWebRequest.kHttpVerbPOST,
                                                    new DownloadHandlerBuffer(), 
                                                    new UploadHandlerRaw(postData))
            {
                disposeUploadHandlerOnDispose = true,
                disposeDownloadHandlerOnDispose = true
            };
            
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.error != null)
            {
                errorCallback(request.error);
                yield break;
            }
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                errorCallback(request.responseCode.ToString());
                yield break;
            }
            successCallback(request.downloadHandler.text);
        }

        public IEnumerator Post<T>(string url, T body, IHttp.successCallback successCallback, IHttp.errorCallback errorCallback)
        {
            var jsonData = JsonConvert.SerializeObject(body);
            return Post(url, jsonData, successCallback, errorCallback);
        }
    }
}