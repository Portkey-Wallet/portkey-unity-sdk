using System.Collections;
using Newtonsoft.Json;
using Portkey.Core;

namespace Portkey.DID
{
    public class ConnectService<T> : IConnectService where T : IHttp
    {
        private string _apiUrl;
        private T _http;
        
        public ConnectService(string apiUrl, T http)
        {
            _apiUrl = apiUrl + "/connect/token";
            _http = http;
        }

        public IEnumerator GetConnectToken(RequestTokenConfig config, SuccessCallback<ConnectToken> successCallback, ErrorCallback errorCallback)
        {
            var fieldForm = new FieldFormRequestData<RequestTokenConfig>
            {
                Url = _apiUrl,
                FieldFormsObject = config
            };
            
            yield return _http.PostFieldForm(fieldForm, (response) =>
            {
                var token = JsonConvert.DeserializeObject<ConnectToken>(response);
                if (token == null)
                {
                    errorCallback("Failed to deserialize token");
                }
                successCallback(token);
            }, errorCallback);
        }
    }
}