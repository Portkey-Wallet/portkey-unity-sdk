using System.Collections;
using System.Collections.Generic;
using System.Text;
using Portkey.Core;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

namespace Portkey.Network
{
    public class FetchFormHttp : IHttp
    {
        public IEnumerator Get(string url, string parameters, IHttp.successCallback successCallback, IHttp.errorCallback errorCallback)
        {
            parameters??=string.Empty;
            
            byte[] data = Encoding.ASCII.GetBytes(parameters);
            using var request = new UnityWebRequest(url, 
                                                    UnityWebRequest.kHttpVerbGET,
                                                    new DownloadHandlerBuffer(), 
                                                    new UploadHandlerRaw(data))
            {
                disposeUploadHandlerOnDispose = true,
                disposeDownloadHandlerOnDispose = true
            };
            
            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            
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
        
        private IEnumerator Post(string url, JObject body, IHttp.successCallback successCallback, IHttp.errorCallback errorCallback)
        {
            var formFields = ConvertToDictionary(body);
            var request = UnityWebRequest.Post(url, formFields);

            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

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

        public IEnumerator Post(string url, string jsonData, IHttp.successCallback successCallback, IHttp.errorCallback errorCallback)
        {
            return Post(url, JObject.Parse(jsonData), successCallback, errorCallback);
        }

        public IEnumerator Post<T>(string url, T body, IHttp.successCallback successCallback, IHttp.errorCallback errorCallback)
        {
            return Post(url, JObject.FromObject(body), successCallback, errorCallback);
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