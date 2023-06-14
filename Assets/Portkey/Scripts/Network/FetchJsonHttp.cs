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
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (request.error != null)
                {
                    errorCallback(request.error);
                    yield break;
                }
                else if (request.responseCode != 200)
                {
                    errorCallback(request.responseCode.ToString());
                    yield break;
                }

                T ret;
                try
                {
                    ret = JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
                }
                catch (Exception e)
                {
                    errorCallback(e.Message);
                    throw;
                }
                successCallback(ret);

                request.Dispose();
            }
        }

        public IEnumerator Post<T>(string url, string body, IHttp.successCallback<T> successCallback, IHttp.errorCallback errorCallback)
        {
            string jsonData = JsonConvert.SerializeObject(new{query = body});
            byte[] postData = Encoding.ASCII.GetBytes(jsonData);
            using (UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
            {
                request.uploadHandler = new UploadHandlerRaw(postData);
                request.disposeUploadHandlerOnDispose = true;
                
                request.downloadHandler = new DownloadHandlerBuffer();
                request.disposeDownloadHandlerOnDispose = true;
                
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.error != null)
                {
                    errorCallback(request.error);
                    request.uploadHandler.Dispose();
                    request.downloadHandler.Dispose();
                    request.Dispose();
                    yield break;
                }
                else if (request.result != UnityWebRequest.Result.Success)
                {
                    errorCallback(request.responseCode.ToString());
                    request.uploadHandler.Dispose();
                    request.downloadHandler.Dispose();
                    request.Dispose();
                    yield break;
                }

                T ret;
                try
                {
                    ret = JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
                }
                catch (Exception e)
                {
                    errorCallback(e.Message);
                    throw;
                }
                successCallback(ret);

                request.uploadHandler.Dispose();
                request.downloadHandler.Dispose();
                request.Dispose();
            }
        }
        
        
    }
}