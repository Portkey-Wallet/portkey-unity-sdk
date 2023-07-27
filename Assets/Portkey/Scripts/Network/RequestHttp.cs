using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Portkey.Core;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Portkey.Network
{
    [CreateAssetMenu(fileName = "RequestHttp", menuName = "Portkey/Network/RequestHttp")]
    public class RequestHttp : IHttp
    {
        public override IEnumerator Get<T>(FieldFormRequestData<T> data, SuccessCallback successCallback, ErrorCallback errorCallback)
        {
            var url = GetUrlWithParameters(data);

            using var request = UnityWebRequest.Get(url);
            
            foreach (var header in data.Headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }
            
            Debugger.Log(url);
            
            yield return request.SendWebRequest();

            if (request.error != null)
            {
                var errorMessage = GetErrorMessage(request);
                errorCallback(errorMessage);
                yield break;
            }
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                errorCallback(request.responseCode.ToString());
                yield break;
            }
            
            Debugger.Log(request.downloadHandler.text);
            successCallback(request.downloadHandler.text);
        }

        private static string GetErrorMessage(UnityWebRequest request)
        {
            var errorMessage = request.error;
            if (request.downloadHandler.text != null)
            {
                errorMessage += $"\n{request.downloadHandler.text}";
            }

            return errorMessage;
        }

        private static string GetUrlWithParameters<T>(FieldFormRequestData<T> data)
        {
            if (data.FieldFormsObject == null)
            {
                return data.Url;
            }
            
            var url = new StringBuilder();
            url.Append(data.Url);
            var formFields = ConvertToDictionary(JObject.FromObject(data.FieldFormsObject));
            if (formFields.Count > 0)
            {
                url.Append("?");
            }

            foreach (var field in formFields.Where(field => !string.IsNullOrEmpty(field.Value)))
            {
                url.Append($"{field.Key}={field.Value}&");
            }
            
            var ret = url.ToString().TrimEnd('&');

            return ret;
        }

        public override IEnumerator Post(JsonRequestData data, SuccessCallback successCallback, ErrorCallback errorCallback)
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
                var errorMessage = GetErrorMessage(request);
                errorCallback(errorMessage);
                yield break;
            }
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                errorCallback(request.responseCode.ToString());
                yield break;
            }
            successCallback(request.downloadHandler.text);
        }

        public override IEnumerator PostFieldForm<T>(FieldFormRequestData<T> data, SuccessCallback successCallback, ErrorCallback errorCallback)
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
                var errorMessage = GetErrorMessage(request);
                errorCallback(errorMessage);
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