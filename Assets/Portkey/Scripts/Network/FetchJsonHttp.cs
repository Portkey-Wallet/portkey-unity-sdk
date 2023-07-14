using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Portkey.Core;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Portkey.Network
{
    [CreateAssetMenu(fileName = "FetchJsonHttp", menuName = "Portkey/Network/FetchJsonHttp")]
    public class FetchJsonHttp : IHttp
    {
        /*
        public IEnumerator Post<T>(string url, T body, IHttp.successCallback successCallback, IHttp.errorCallback errorCallback)
        {
            var jsonData = JsonConvert.SerializeObject(body);
            return Post(url, jsonData, successCallback, errorCallback);
        }*/

        public override IEnumerator Get(JsonRequestData data, successCallback successCallback, ErrorCallback errorCallback)
        {
            data.JsonData??=string.Empty;
            
            byte[] postData = Encoding.ASCII.GetBytes(data.JsonData);
            using var request = new UnityWebRequest(data.Url, 
                UnityWebRequest.kHttpVerbGET,
                new DownloadHandlerBuffer(), 
                new UploadHandlerRaw(postData))
            {
                disposeUploadHandlerOnDispose = true,
                disposeDownloadHandlerOnDispose = true
            };
            
            request.SetRequestHeader("Content-Type", data.ContentType);
            foreach (var header in data.Headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }
            
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

        public override IEnumerator Post(JsonRequestData data, successCallback successCallback, ErrorCallback errorCallback)
        {
            data.JsonData??=string.Empty;

            var postData = Encoding.ASCII.GetBytes(data.JsonData);
            using var request = new UnityWebRequest(data.Url, 
                UnityWebRequest.kHttpVerbPOST,
                new DownloadHandlerBuffer(), 
                new UploadHandlerRaw(postData))
            {
                disposeUploadHandlerOnDispose = true,
                disposeDownloadHandlerOnDispose = true
            };
            
            request.SetRequestHeader("Content-Type", data.ContentType);
            foreach (var header in data.Headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }

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

        public override IEnumerator PostFieldForm<T>(FieldFormRequestData<T> data, successCallback successCallback, ErrorCallback errorCallback)
        {
            var formFields = ConvertToDictionary(JObject.FromObject(data.FieldFormsObject));
            
            using var request = UnityWebRequest.Post(data.Url, formFields);
            
            request.SetRequestHeader("Content-Type", data.ContentType);
            foreach (var header in data.Headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }

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
        
        private static Dictionary<string, string> ConvertToDictionary(JObject jsonObject)
        {
            var result = new Dictionary<string, string>();
            foreach (var property in jsonObject.Properties())
            {
                result[property.Name] = property.Value.ToString();
            }

            return result;
        }
    }
}