using System;
using System.Collections.Specialized;
using Portkey.Core;
using UnityEngine;

namespace Portkey.SocialProvider
{
    
    public class PCTelegramLogin : TelegramLoginBase
    {
        private int _port;
        private string _url;
        private string _serviceUrl;

        public PCTelegramLogin(PortkeyConfig config, IHttp request) : base(request)
        {
            _url = config.TelegramLoginUrl;
            _port = config.TelegramLoginPort;
            _serviceUrl = config.TelegramServiceUrl;
        }

        protected override void OnAuthenticate()
        {
            Authenticate();
            Listen();
        }
        
        private void Listen()
        {
            var httpListener = new System.Net.HttpListener();
            httpListener.Prefixes.Add($"http://127.0.0.1:{_port}/");
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

        private void Authenticate()
        {
            const string loginUri = "social-login/";
            const string loginType = "Telegram";
            const string loginFrom = "unitysdk";
            var serviceUri = _serviceUrl;
            var authorizationRequest = $"{_url}{loginUri}{loginType}?from={loginFrom}&serviceURI={serviceUri}";
            Application.OpenURL(authorizationRequest);
        }
        
    }
}
