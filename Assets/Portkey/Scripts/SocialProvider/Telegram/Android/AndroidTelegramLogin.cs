using System.Collections;
using System.Collections.Specialized;
using UnityEngine;
using Portkey.Core;
using System.Collections.Generic;
using System.Linq;
using Portkey.Utilities;
using Portkey.Contracts.CA;

namespace Portkey.SocialProvider
{
    public class AndroidTelegramLogin : TelegramLoginBase
    {
        private System.Net.HttpListener _httpListener;
        private string _url;
        private string _serviceUrl;
        private int _port;
        private IEnumerator _listenCoroutine;
        
        public AndroidTelegramLogin(PortkeyConfig config, IHttp request) : base(request)
        {
            _url = config.TelegramLoginUrl;
            _port = config.TelegramLoginPort;
            _serviceUrl = config.TelegramServiceUrl;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        protected override void OnAuthenticate()
        {
            const string loginUri = "social-login/";
            const string loginType = "Telegram";
            const string loginFrom = "unitysdk";
            var serviceUri = _serviceUrl;
            var url = $"{_url}{loginUri}{loginType}?from={loginFrom}&serviceURI={serviceUri}";
            Application.OpenURL(url);

            if (Application.runInBackground)
            {
                _listenCoroutine = Listen();
                StaticCoroutine.StartCoroutine(_listenCoroutine);
            }
        }

        private IEnumerator Listen()
        {
            _httpListener = new System.Net.HttpListener();
            _httpListener.Prefixes.Add($"http://127.0.0.1:{_port}/");
            _httpListener.Start();
            
            while (Application.runInBackground && !_httpListener.IsListening)
            {
                yield return null;
            }

            var context = _httpListener.GetContext();
            
            HandleListenerCallback(context);

            yield return null;
        }

        private void HandleListenerCallback(System.Net.HttpListenerContext context)
        {
            var response = context.Response;

            // Use productName on the main thread
            var buffer = System.Text.Encoding.UTF8.GetBytes(
                $"Successful login! You may close this tab and return to {Application.productName}.");

            response.ContentLength64 = buffer.Length;

            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();

            HandleAuthenticationResponse(context.Request.QueryString);
            StopListening();
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

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                // Code to execute when the application is paused (goes to background)
                Debug.Log("Application paused");
                StopListening();
            }
            else
            {
                // Code to execute when the application resumes (comes to foreground)
                Debug.Log("Application resumed");
                _listenCoroutine = Listen();
            }
        }

        private void StopListening()
        {
            if (_httpListener != null && _httpListener.IsListening)
            {
                _httpListener.Stop();
            }
        }
    }
}
