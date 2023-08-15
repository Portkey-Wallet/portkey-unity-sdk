using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Portkey.Core;
using Portkey.SocialProvider;
using Portkey.Utilities;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class GetAccessTokenParam
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

        protected string _state;

        public TokenRequestGoogleLoginBase(IHttp request) : base(request)
        {
        }
        
        public override void Authenticate(ISocialLogin.AuthCallback successCallback, SuccessCallback<bool> startLoadCallback, ErrorCallback errorCallback)
        {
            _state = Guid.NewGuid().ToString();
            base.Authenticate(successCallback, startLoadCallback, errorCallback);
        }
        
        protected void HandleAuthenticationResponse(NameValueCollection parameters)
        {
            var error = parameters.Get("error");
            if (error != null)
            {
                _errorCallback?.Invoke(error);
                return;
            }

            var state = parameters.Get("state");
            var code = parameters.Get("code");
            var scope = parameters.Get("scope");
            if (state == null || code == null || scope == null)
            {
                return;
            }

            if (state == _state)
            {
                RequestToken(code);
            }
            else
            {
                Debugger.LogError("Unsynchronized state.");
            }
        }

        protected abstract T CreateGetAccessTokenParam(string code);

        protected void RequestToken(string code)
        {
            var requestData  = new FieldFormRequestData<T>
            {
                Url = TOKEN_ENDPOINT,
                FieldFormsObject = CreateGetAccessTokenParam(code)
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