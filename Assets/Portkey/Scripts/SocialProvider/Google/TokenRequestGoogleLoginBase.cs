using Portkey.Core;
using Portkey.Utilities;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public abstract class GetAccessTokenParam
    {
        public string code;
        public string redirect_uri;
        public string client_id;
        public string scope;
        public string grant_type;
    }
    
    /// <summary>
    /// Token request base class for Google login flow for when processing of the
    /// authentication token is required to acquire the access token from Google API.
    /// </summary>
    /// <typeparam name="T">GetAccessTokenParam or its inherited class used as parameters to get access token from Google API.</typeparam>
    public abstract class TokenRequestGoogleLoginBase<T> : GoogleLoginBase where T : GetAccessTokenParam
    {
        private class RequestTokenResponse
        {
            public string access_token;
        }

        protected TokenRequestGoogleLoginBase(IHttp request) : base(request)
        {
        }

        protected abstract T CreateGetAccessTokenParam(string authCode);

        protected void RequestAccessToken(string authCode)
        {
            var requestData  = new FieldFormRequestData<T>
            {
                Url = TOKEN_ENDPOINT,
                FieldFormsObject = CreateGetAccessTokenParam(authCode)
            };
            
            StaticCoroutine.StartCoroutine(_request.PostFieldForm(requestData, (response) =>
            {
                Debugger.Log($"CodeExchange={response}");

                var exchangeResponse = JsonUtility.FromJson<RequestTokenResponse>(response);
                var accessToken = exchangeResponse.access_token;

                RequestSocialInfo(accessToken, _successCallback, _errorCallback);
            }, OnError(_errorCallback)));
        }
    }
}