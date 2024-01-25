using System.Collections;
using System.Collections.Concurrent;
using Portkey.Core;
using Portkey.Utilities;
using UnityEngine;
using System;
using System.Collections.Specialized;

namespace Portkey.SocialProvider
{
    public class AndroidTelegramLogin : TelegramLoginBase
    {
        private readonly string _clientSecret;
        private string _redirectUri;
        private int _randomPort;
        private string _state;
        private string _codeVerifier;
        private string _url;
        private class TelegramAuthOutput
        {
            public bool isError;
            public string message;
        }
        
        public AndroidTelegramLogin(PortkeyConfig config, IHttp request) : base(request)
        {
            _url = config.TelegramWebGLLoginUrl;
        }
        
        private class AndroidAuthCallback : AndroidJavaProxy
        {
            private ConcurrentQueue<TelegramAuthOutput> _authTokens;
            
            public AndroidAuthCallback(ConcurrentQueue<TelegramAuthOutput> authTokens) : base("com.portkey.nativeTelegramloginactivity.NativeTelegramLoginActivity$Callback")
            {
                _authTokens = authTokens;
            }

            public void onResult(string authToken)
            {
                var output = new TelegramAuthOutput
                {
                    isError = false,
                    message = authToken
                };
                _authTokens.Enqueue(output);
            }

            public void onError(AndroidJavaObject e)
            {
                var message = e.Call<string>("getMessage");
                
                var output = new TelegramAuthOutput
                {
                    isError = true,
                    message = message
                };
                _authTokens.Enqueue(output);
            }
        }

        private ConcurrentQueue<TelegramAuthOutput> AuthTokens { get; } = new ConcurrentQueue<TelegramAuthOutput>();

        protected override void OnAuthenticate()
        {
            // _state = Guid.NewGuid().ToString();

            var _randomPort = 8070;
            var _redirectUri = $"https://3773-202-156-61-238.ngrok-free.app/";
            
            Authenticate();
            Listen();
        }
        
        private IEnumerator HandleAuthTokens()
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
                        RequestSocialInfo(output.message, null, null);
                    }
                    yield break;
                }

                yield return null;
            }
        }
        private void Listen()
        {
            var httpListener = new System.Net.HttpListener();
            httpListener.Prefixes.Add($"http://localhost:{_randomPort}/");
            httpListener.Start();

            var context = System.Threading.SynchronizationContext.Current;
            var asyncResult =
                httpListener.BeginGetContext(result => context.Send(HandleListenerCallback, result), httpListener);

            // Block the thread when background mode is not supported to serve HTTP response while the application is not in focus.
            if (!Application.runInBackground)
            {
                asyncResult.AsyncWaitHandle.WaitOne();
            }
        }
        private void Authenticate()
        {
            var codeChallenge = Utilities.GetCodeChallenge(_codeVerifier);
            // var authorizationRequest = $"{AUTHORIZATION_ENDPOINT}?response_type=code&client_id={ClientId}&state={_state}&scope={Uri.EscapeDataString(ACCESS_SCOPE)}&redirect_uri={Uri.EscapeDataString(_redirectUri)}&code_challenge={codeChallenge}&code_challenge_method=S256";
            var authorizationRequest = $"{_url}/social-login/Telegram";
            Application.OpenURL(authorizationRequest);
        }
        
        private void HandleListenerCallback(object state)
        {
            var result = (IAsyncResult)state;
            var httpListener = (System.Net.HttpListener)result.AsyncState;
            var context = httpListener.EndGetContext(result);

            var response = context.Response;
            var buffer =
                System.Text.Encoding.UTF8.GetBytes(
                    $"Successful login! You may close this tab and return to {Application.productName}.");

            response.ContentLength64 = buffer.Length;

            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
            httpListener.Close();

            HandleAuthenticationResponse(context.Request.QueryString);
        }
        
        private void HandleAuthenticationResponse(NameValueCollection parameters)
        {
            var error = parameters.Get("error");
            if (error != null)
            {
                _errorCallback?.Invoke(error);
                return;
            }

            var accessToken = parameters.Get("token");
            if (accessToken == null)
            {
                _errorCallback?.Invoke("access token is null");
                return;
            }

            RequestSocialInfo(accessToken, null, null);
        }
        
        // private void Authenticate()
        // {
        //     const string loginUri = "social-login/";
        //     const string loginType = "Telegram";
        //     var url = $"{_url}{loginUri}{loginType}";
        //     
        //     var callback = new AndroidAuthCallback(AuthTokens);
        //     
        //     using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //     using var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        //     using var context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
        //     using var unityTelegramLoginPlugin = new AndroidJavaClass("com.example.telegrampassport.NativeTelegramLoginActivity");
        //     unityTelegramLoginPlugin.SetStatic("url", url);
        //     unityTelegramLoginPlugin.CallStatic("SetCallback", callback);
        //     unityTelegramLoginPlugin.CallStatic("Call", currentActivity);
        // }
    }
}