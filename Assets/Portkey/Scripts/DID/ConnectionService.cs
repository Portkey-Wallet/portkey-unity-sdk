using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Portkey.Core;

namespace Portkey.DID
{
    public class ConnectionService<T> : IConnectionService where T : IHttp
    {
        protected class TokenInfo
        {
            public ConnectToken token;
            public DateTime requestTime;
            
            public bool IsExpired()
            {
                return DateTime.Now - requestTime > TimeSpan.FromSeconds(token.expires_in);
            }
        }
        
        private const string URI = "/connect/token";
        
        private string _apiUrl;
        private T _http;
        
        private Dictionary<string, TokenInfo> _tokenMap = new ();
        
        public ConnectionService(string apiUrl, T http)
        {
            _apiUrl = apiUrl + URI;
            _http = http;
        }

        public IEnumerator GetConnectToken(RequestTokenConfig config, SuccessCallback<ConnectToken> successCallback, ErrorCallback errorCallback)
        {
            if(_tokenMap.TryGetValue(config.chain_id, out var tokenInfo))
            {
                if (!tokenInfo.IsExpired())
                {
                    successCallback(tokenInfo.token);
                    yield break;
                }
            }
            
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
                    
                    _tokenMap[config.chain_id] = new TokenInfo
                    {
                        token = token,
                        requestTime = new DateTime(config.timestamp)
                    };
                    
                    successCallback(token);
                }
                catch (Exception e)
                {
                    errorCallback(e.Message);
                }
            }, (error) =>
            {
                errorCallback(error.message);
            });
        }

        public void Reset()
        {
            _tokenMap.Clear();
        }
    }
}