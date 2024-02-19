using System;
using System.Collections.Specialized;
using Portkey.Core;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class PCAppleLogin : AppleLoginBase
    {
        private readonly string _url;

        public PCAppleLogin(PortkeyConfig config, IHttp request) : base(request)
        {
            _url = config.OpenLoginUrl;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        protected override void OnAuthenticate()
        {
            var port = Utilities.GetRandomUnusedPort();
            const string loginUri = "social-login/";
            const string loginType = "Apple";
            var redirectUri = $"https://openlogin.portkey.finance/unity-sdk-callback&state={port}/";
            
            Debugger.Log("Authenticating for PC");
            var url = $"{_url}{loginUri}{loginType}/?redirectURI={redirectUri}";
#if UNITY_STANDALONE || UNITY_EDITOR
            Application.OpenURL(url);
#endif
            Listen(port);

        }

        private void Listen(int port)
        {
            var httpListener = new System.Net.HttpListener();
            httpListener.Prefixes.Add($"http://localhost:{port}/");
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
    }
}