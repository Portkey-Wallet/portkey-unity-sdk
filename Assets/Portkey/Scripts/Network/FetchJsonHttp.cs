using System;
using System.Collections;
using System.Text;
using Portkey.Core;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Portkey.Network
{
    [CreateAssetMenu(fileName = "FetchJsonHttp", menuName = "Portkey/Network/FetchJsonHttp")]
    public class FetchJsonHttp : IHttp
    {
        public override IEnumerator Get(string url, IHttp.successCallback successCallback, IHttp.errorCallback errorCallback)
        {
            using var request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.error != null)
            {
                errorCallback(request.error);
                yield break;
            }
            
            if (request.responseCode != 200)
            {
                errorCallback(request.responseCode.ToString());
                yield break;
            }
            successCallback(request.downloadHandler.text);
        }

        public override IEnumerator Post(string url, string body, IHttp.successCallback successCallback, IHttp.errorCallback errorCallback)
        {
            string jsonData = JsonConvert.SerializeObject(new{query = body});
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
        
        
    }
}