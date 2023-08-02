using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Portkey.Core;
using Portkey.Utilities;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class GoogleLogin : ISocialLogin
    {
        private class RequestTokenResponse
        {
            public string access_token;
        }
        
        private class GetTokenParam
        {
            public string code;
            public string redirect_uri;
            public string client_id;
            public string code_verifier;
#if UNITY_STANDALONE || UNITY_EDITOR
            public string client_secret;
#endif
            public string scope;
            public string grant_type;
        }
        
        private const string AUTHORIZATION_ENDPOINT = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string TOKEN_ENDPOINT = "https://www.googleapis.com/oauth2/v4/token";
        private const string USERINFO_ENDPOINT = "https://www.googleapis.com/oauth2/v3/userinfo";
        private const string ACCESS_SCOPE = "openid email profile";

        private string _clientId;
        private string _clientSecret;
        private string _protocol;
        private string _redirectUri;
        private string _state;
        private string _codeVerifier;
        private ISocialLogin.AuthCallback _successCallback;
        private ErrorCallback _errorCallback;

        private IHttp _request;
        
        public GoogleLogin(PortkeyConfig config, IHttp request)
        {
            _request = request;
#if UNITY_STANDALONE || UNITY_EDITOR
            _clientId = config.GooglePCClientId;
            _clientSecret = config.GooglePCClientSecret;
#elif UNITY_WSA || UNITY_ANDROID || UNITY_IOS
            _clientId = config.GoogleMobileClientId;
            _protocol = config.GoogleMobileProtocol;

            Application.deepLinkActivated += deepLink =>
            {
                Debugger.Log($"Application.deepLinkActivated={deepLink}");
                HandleAuthenticationResponse(Utilities.ParseQueryString(deepLink));
            };
#elif UNITY_WEBGL
            _clientId = config.GoogleWebGLClientId;
#endif
        }
        
#if UNITY_STANDALONE || UNITY_EDITOR
        public void Authenticate(ISocialLogin.AuthCallback successCallback, ErrorCallback errorCallback)
        {
            _successCallback = successCallback;
            _errorCallback = errorCallback;
            _redirectUri = $"http://localhost:{Utilities.GetRandomUnusedPort()}/";

            Authenticate();
            Listen();
        }
        
        private void Listen()
        {
            var httpListener = new System.Net.HttpListener();
            httpListener.Prefixes.Add(_redirectUri);
            httpListener.Start();

            var context = System.Threading.SynchronizationContext.Current;
            var asyncResult = httpListener.BeginGetContext(result => context.Send(HandleListenerCallback, result), httpListener);

            // Block the thread when background mode is not supported to serve HTTP response while the application is not in focus.
            if (!Application.runInBackground)
            {
                asyncResult.AsyncWaitHandle.WaitOne();
            }
        }

        private void HandleListenerCallback(object state)
        {
            var result = (IAsyncResult) state;
            var httpListener = (System.Net.HttpListener) result.AsyncState;
            var context = httpListener.EndGetContext(result);

            var response = context.Response;
            var buffer = System.Text.Encoding.UTF8.GetBytes($"Successful login! You may close this tab and return to {Application.productName}.");

            response.ContentLength64 = buffer.Length;

            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
            httpListener.Close();

            HandleAuthenticationResponse(context.Request.QueryString);
        }
        
#elif UNITY_WSA || UNITY_ANDROID || UNITY_IOS

        public void Authenticate(ISocialLogin.AuthCallback successCallback, ErrorCallback errorCallback)
        {
            #if UNITY_EDITOR

            Debugger.LogWarning("Deep links don't work inside Editor.");

            #endif

            _successCallback = successCallback;
            _errorCallback = errorCallback;
            _redirectUri = $"{_protocol}:/oauth2callback";
            
            Authenticate();
        }

#elif UNITY_WEBGL

        public void Authenticate(ISocialLogin.AuthCallback successCallback, ErrorCallback errorCallback)
        {
            _successCallback = successCallback;
            _errorCallback = errorCallback;
            _redirectUri = Application.absoluteURL;

            var accessToken = Utilities.ParseQueryString(Application.absoluteURL).Get("access_token");
            if (accessToken == null)
            {
                Application.OpenURL($"{AUTHORIZATION_ENDPOINT}?response_type=token&scope={ACCESS_SCOPE}&client_id={_clientId}&redirect_uri={_redirectUri}");
                return;
            }
                
            RequestSocialInfo(accessToken);
        }
        
#endif
        
        private void Authenticate()
        {
            _state = Guid.NewGuid().ToString();
            _codeVerifier = Guid.NewGuid().ToString();

            var codeChallenge = Utilities.GetCodeChallenge(_codeVerifier);
            var authorizationRequest = $"{AUTHORIZATION_ENDPOINT}?response_type=code&client_id={_clientId}&state={_state}&scope={Uri.EscapeDataString(ACCESS_SCOPE)}&redirect_uri={Uri.EscapeDataString(_redirectUri)}&code_challenge={codeChallenge}&code_challenge_method=S256";

            Application.OpenURL(authorizationRequest);
        }
        
        private void HandleAuthenticationResponse(NameValueCollection parameters)
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
                RequestToken(code, _codeVerifier);
            }
            else
            {
                Debugger.LogError("Unsynchronized state.");
            }
        }

        private void RequestToken(string code, string codeVerifier)
        {
            var requestData  = new FieldFormRequestData<GetTokenParam>
            {
                Url = TOKEN_ENDPOINT,
                FieldFormsObject = new GetTokenParam
                {
                    code = code,
                    redirect_uri = _redirectUri,
                    client_id = _clientId,
                    code_verifier = codeVerifier,
#if UNITY_STANDALONE || UNITY_EDITOR
                    client_secret = _clientSecret,
#endif
                    scope = ACCESS_SCOPE,
                    grant_type = "authorization_code"
                }
            };

            StaticCoroutine.StartCoroutine(_request.PostFieldForm(requestData, (response) =>
            {
                Debugger.Log($"CodeExchange={response}");

                var exchangeResponse = JsonUtility.FromJson<RequestTokenResponse>(response);
                var accessToken = exchangeResponse.access_token;

                RequestSocialInfo(accessToken, _successCallback, _errorCallback);
            }, OnError(_errorCallback)));
        }

        private static IHttp.ErrorCallback OnError(ErrorCallback errorCallback)
        {
            return (error) =>
            {
                errorCallback(error.message + error.details);
            };
        }

        public void RequestSocialInfo(string accessToken, ISocialLogin.AuthCallback successCallback, ErrorCallback errorCallback)
        {
            var param = new FieldFormRequestData<Empty>()
            {
                Url = USERINFO_ENDPOINT,
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {accessToken}" }
                }
            };
            StaticCoroutine.StartCoroutine(_request.Get(param, (response) =>
            {
                var socialInfo = JsonConvert.DeserializeObject<SocialInfo>(response);

                var socialLoginInfo = new SocialLoginInfo
                {
                    access_token = accessToken,
                    accountType = AccountType.Google,
                    socialInfo = socialInfo
                };
                
                successCallback(socialLoginInfo);
            }, OnError(errorCallback)));
        }
    }
}
