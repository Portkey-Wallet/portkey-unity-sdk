using System;
using System.Collections;
using Newtonsoft.Json;
using Portkey.Core;

namespace Portkey.DID
{
    public class ConnectionService<T> : IConnectionService where T : IHttp
    {
        private const string URI = "/connect/token";
        
        private string _apiUrl;
        private T _http;
        
        public ConnectionService(string apiUrl, T http)
        {
            _apiUrl = apiUrl + URI;
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
                try
                {
                    var token = JsonConvert.DeserializeObject<ConnectToken>(response);
                    if (token == null)
                    {
                        errorCallback("Failed to deserialize token");
                        return;
                    }
                    successCallback(token);
                }
                catch (Exception e)
                {
                    errorCallback(e.Message);
                }
            }, errorCallback);
        }
    }
}