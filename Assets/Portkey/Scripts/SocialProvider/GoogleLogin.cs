using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Portkey.Core;
using Portkey.Utilities;
using UnityEngine;

#if UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

namespace Portkey.SocialProvider
{
    public class GoogleLogin : ISocialLogin
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        public class GoogleAuthOutput
        {
            public bool isError;
            public string message;
        }
        
        private class JavaAuthCallback : AndroidJavaProxy
        {
            private GoogleLogin _googleLogin;
            
            public JavaAuthCallback(GoogleLogin googleLogin) : base("com.portkey.nativegoogleloginactivity.NativeGoogleLoginActivity$Callback")
            {
                _googleLogin = googleLogin;
            }

            public virtual void onResult(string authToken)
            {
                var output = new GoogleAuthOutput
                {
                    isError = false,
                    message = authToken
                };
                _googleLogin.AuthTokens.Enqueue(output);
            }

            public virtual void onError(AndroidJavaObject e)
            {
                var message = e.Call<string>("getMessage");
                
                var output = new GoogleAuthOutput
                {
                    isError = true,
                    message = message
                };
                _googleLogin.AuthTokens.Enqueue(output);
            }
        }
        
        public ConcurrentQueue<GoogleAuthOutput> AuthTokens { get; } = new ConcurrentQueue<GoogleAuthOutput>();

        private IEnumerator HandleGoogleAuthOutput()
        {
            while (true)
            {
                if (AuthTokens.TryDequeue(out var output))
                {
                    if (output.isError)
                    {
                        Debugger.LogError(output.message);
                        _errorCallback?.Invoke("Network Error!");
                    }
                    else
                    {
                        _startLoadCallback?.Invoke(true);
                        RequestToken(output.message, CodeVerifier);
                    }
                    yield break;
                }

                yield return null;
            }
        }
#endif
        
        private class RequestTokenResponse
        {
            public string access_token;
        }
        
        private class GetTokenParam
        {
            public string code;
            public string redirect_uri;
            public string client_id;
#if !UNITY_ANDROID || UNITY_EDITOR
            public string code_verifier;
#endif
#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID
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
        private ISocialLogin.AuthCallback _successCallback;
        private ErrorCallback _errorCallback;
        private SuccessCallback<bool> _startLoadCallback;

        private IHttp _request;
        
        public string CodeVerifier { get; private set; }

        public GoogleLogin(PortkeyConfig config, IHttp request)
        {
            _request = request;
#if UNITY_STANDALONE || UNITY_EDITOR
            _clientId = config.GooglePCClientId;
            _clientSecret = config.GooglePCClientSecret;
#elif UNITY_ANDROID
            _clientId = config.GoogleAndroidClientId;
            _clientSecret = config.GoogleAndroidClientSecret;
#elif UNITY_WSA || UNITY_IOS
            _clientId = config.GoogleIOSClientId;
            _protocol = config.GoogleIOSProtocol;

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
        public void Authenticate(ISocialLogin.AuthCallback successCallback, SuccessCallback<bool> startLoadCallback, ErrorCallback errorCallback)
        {
            _successCallback = successCallback;
            _errorCallback = errorCallback;
            _startLoadCallback = startLoadCallback;
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
#elif UNITY_ANDROID
        public void Authenticate(ISocialLogin.AuthCallback successCallback, SuccessCallback<bool> startLoadCallback, ErrorCallback errorCallback)
        {
            #if UNITY_EDITOR

            Debugger.LogWarning("Deep links don't work inside Editor.");

            #endif

            _successCallback = successCallback;
            _errorCallback = errorCallback;
            _startLoadCallback = startLoadCallback;
            _redirectUri = "";

            StaticCoroutine.StartCoroutine(HandleGoogleAuthOutput());
            
            Authenticate();
        }
        
#elif UNITY_WSA || UNITY_IOS

        public void Authenticate(ISocialLogin.AuthCallback successCallback, SuccessCallback<bool> startLoadCallback, ErrorCallback errorCallback)
        {
            #if UNITY_EDITOR

            Debugger.LogWarning("Deep links don't work inside Editor.");

            #endif

            _successCallback = successCallback;
            _errorCallback = errorCallback;
            _startLoadCallback = startLoadCallback;
            _redirectUri = $"{_protocol}:/oauth2callback";
            
            Authenticate();
        }

#elif UNITY_WEBGL

        private class PostData
        {
            public string clientId;
            public string redirectUri;
        }

        [DllImport("__Internal")]
        private static extern void Listen();

        public void Authenticate(ISocialLogin.AuthCallback successCallback, SuccessCallback<bool> startLoadCallback, ErrorCallback errorCallback)
        {
            /*_successCallback = successCallback;
            _errorCallback = errorCallback;
            _startLoadCallback = startLoadCallback;
            _redirectUri = Application.absoluteURL;

            _startLoadCallback?.Invoke(true);

            var accessToken = Utilities.ParseQueryString(Application.absoluteURL).Get("access_token");
            if (accessToken == null)
            {
                Debugger.LogError($"Authenticate: {_clientId} {_redirectUri}");
                Application.OpenURL($"{AUTHORIZATION_ENDPOINT}?response_type=token&scope={ACCESS_SCOPE}&client_id={_clientId}&redirect_uri={_redirectUri}");
                return;
            }
                
            Debugger.LogError($"RequestSocialInfo: {accessToken}");
            RequestSocialInfo(accessToken, _successCallback, _errorCallback);*/

            _successCallback = successCallback;
            _errorCallback = errorCallback;
            _startLoadCallback = startLoadCallback;
            _redirectUri = "https://openlogin.portkey.finance/auth-callback";
            var loginUrl = "https://openlogin.portkey.finance/";
            var loginUri = "social-login/";
            var loginType = "Google";

            _startLoadCallback?.Invoke(true);

            Listen();

            Debugger.Log("Authenticating for WebGL");
            Application.OpenURL($"{loginUrl}{loginUri}{loginType}?clientId={_clientId}&redirectUri={_redirectUri}");
        }
        
#endif

        private void Authenticate()
        {
            _state = Guid.NewGuid().ToString();
            CodeVerifier = Guid.NewGuid().ToString();

            var codeChallenge = Utilities.GetCodeChallenge(CodeVerifier);
            var authorizationRequest = $"{AUTHORIZATION_ENDPOINT}?response_type=code&client_id={_clientId}&state={_state}&scope={Uri.EscapeDataString(ACCESS_SCOPE)}&redirect_uri={Uri.EscapeDataString(_redirectUri)}&code_challenge={codeChallenge}&code_challenge_method=S256";

#if UNITY_STANDALONE || UNITY_EDITOR
            _startLoadCallback?.Invoke(true);
            Application.OpenURL(authorizationRequest);
#elif UNITY_ANDROID
            var callback = new JavaAuthCallback(this);
            
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            using var context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
            using var unityGoogleLoginPlugin = new AndroidJavaClass("com.portkey.nativegoogleloginactivity.NativeGoogleLoginActivity");
            unityGoogleLoginPlugin.SetStatic("clientId", _clientId);
            unityGoogleLoginPlugin.CallStatic("SetCallback", callback);
            unityGoogleLoginPlugin.CallStatic("Call", currentActivity);
#else
            _startLoadCallback?.Invoke(true);
            Application.OpenURL(authorizationRequest);
#endif
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
                RequestToken(code, CodeVerifier);
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
#if !UNITY_ANDROID || UNITY_EDITOR
                    code_verifier = codeVerifier,
#endif
#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID
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
            successCallback ??= _successCallback;
            errorCallback ??= _errorCallback;
            
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
