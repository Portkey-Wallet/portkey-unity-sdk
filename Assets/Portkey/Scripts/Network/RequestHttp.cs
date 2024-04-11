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
        private readonly int HTTP_TIMEOUT = 30;
        
        public override IEnumerator Get<T>(FieldFormRequestData<T> data, SuccessCallback successCallback, ErrorCallback errorCallback)
        {
            var url = GetUrlWithParameters(data);

            using var request = UnityWebRequest.Get(url);
            request.timeout = HTTP_TIMEOUT;
            
            foreach (var header in data.Headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }
            
            Debugger.Log($"[GET] url: {url}");
            
            yield return request.SendWebRequest();

            if (request.error != null || request.result != UnityWebRequest.Result.Success)
            {
                var errorMessage = GetErrorMessage(request);
                errorCallback(errorMessage);
                yield break;
            }
            
            Debugger.Log($"[GET] Responsed from url: {url}\n{request.downloadHandler.text}");
            successCallback(request.downloadHandler.text);
        }

        private static ErrorMessage GetErrorMessage(UnityWebRequest request)
        {
            var errorDetails = (request.downloadHandler.text != null) ? request.downloadHandler.text : string.Empty;
            
            var errorMessage = new ErrorMessage
            {
                message = request.error,
                details = errorDetails,
                code = request.responseCode
            };

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
                url.Append($"{field.Key}={UnityWebRequest.EscapeURL(field.Value)}&");
            }
            
            var ret = url.ToString().TrimEnd('&');

            return ret;
        }

        public override IEnumerator Post(JsonRequestData data, SuccessCallback successCallback, ErrorCallback errorCallback)
        {
            data.JsonData??=string.Empty;

            var postData = Encoding.UTF8.GetBytes(data.JsonData);
            using var request = new UnityWebRequest(data.Url, 
                UnityWebRequest.kHttpVerbPOST,
                new DownloadHandlerBuffer(), 
                new UploadHandlerRaw(postData))
            {
                disposeUploadHandlerOnDispose = true,
                disposeDownloadHandlerOnDispose = true,
                timeout = HTTP_TIMEOUT
            };
            
            request.SetRequestHeader("Content-Type", data.ContentType);
            foreach (var header in data.Headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }
            
            Debugger.Log($"[POST] url: {data.Url}");

            yield return request.SendWebRequest();

            if (request.error != null || request.result != UnityWebRequest.Result.Success)
            {
                var errorMessage = GetErrorMessage(request);
                errorCallback(errorMessage);
                yield break;
            }
            
            Debugger.Log($"[POST] Responsed from url: {data.Url}\n{request.downloadHandler.text}");
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
            request.timeout = HTTP_TIMEOUT;

            Debugger.Log($"[POST] error url: {data.Url}");
            Debugger.Log($"[POST] error data: {data.ContentType} {data.Headers} {data.ToString()}");
            
            yield return request.SendWebRequest();

            if (request.error != null || request.result != UnityWebRequest.Result.Success)
            {
                var errorMessage = GetErrorMessage(request);
                yield break;
            }
            
            Debugger.Log($"[POST] Responsed from url: {data.Url}\n{request.downloadHandler.text}");
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