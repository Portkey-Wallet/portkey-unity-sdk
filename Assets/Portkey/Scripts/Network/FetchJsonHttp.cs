using System;
using System.Collections;
using System.Text;
using Portkey.Core;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Portkey.Network
{
    public class FetchJsonHttp : IHttp
    {
        public IEnumerator Get<T>(string url, IHttp.successCallback<T> successCallback, IHttp.errorCallback errorCallback)
        {
            UnityWebRequest request = UnityWebRequest.Get(url); ;

            yield return request.SendWebRequest();

            if(request.error != null)
            {
                errorCallback(request.error);
                yield break;
            }
            else if(request.responseCode != 200)
            {
                errorCallback(request.responseCode.ToString());
                yield break;
            }
            
            T ret = JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
            successCallback(ret);
        }

        public IEnumerator Post<T>(string url, string body, IHttp.successCallback<T> successCallback, IHttp.errorCallback errorCallback)
        {
            string jsonData = JsonConvert.SerializeObject(new{query = body});
            byte[] postData = Encoding.ASCII.GetBytes(jsonData);
            UnityWebRequest request = UnityWebRequest.Post(url, UnityWebRequest.kHttpVerbPOST);
            request.uploadHandler = new UploadHandlerRaw(postData);
            request.SetRequestHeader("Content-Type", "application/json");
            
            yield return request.SendWebRequest();

            if(request.error != null)
            {
                errorCallback(request.error);
                yield break;
            }
            else if(request.responseCode != 200)
            {
                errorCallback(request.responseCode.ToString());
                yield break;
            }

            T ret = JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
            successCallback(ret);
        }
        
        
    }
}