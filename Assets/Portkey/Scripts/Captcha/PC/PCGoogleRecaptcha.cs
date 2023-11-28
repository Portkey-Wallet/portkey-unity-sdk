using System;
using System.Collections.Specialized;
using Portkey.Core;
using UnityEngine;

namespace Portkey.Captcha
{
    public class PCGoogleRecaptcha : ICaptcha
    {
        private readonly string _webSiteKey;
        private readonly string _url;
        private SuccessCallback<string> _successCallback;
        private ErrorCallback _errorCallback;
        
        public PCGoogleRecaptcha(PortkeyConfig config)
        {
            _webSiteKey = config.RecaptchaWebSitekey;
            //_url = config.ApplePCLoginUrl;
            //_url = "http://localhost:3000/";
            _url = "https://openlogin-test.portkey.finance/";
        }
        
        public void ExecuteCaptcha(SuccessCallback<string> successCallback, ErrorCallback errorCallback)
        {
            _successCallback = successCallback;
            _errorCallback = errorCallback;
            
            Execute();
        }
        
        private void Execute()
        {
            var port = SocialProvider.Utilities.GetRandomUnusedPort();
            var param = $"siteKey={_webSiteKey}&port={port}";
            var escapedParam = Uri.EscapeUriString(param);
            var url = $"{_url}unity-recaptcha?{escapedParam}";
      
            Listen(port);
            
            Debugger.LogError($"Calling recaptcha url: {url}");
#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            Debugger.LogError($"Calling PC recaptcha url: {url}");
            Application.OpenURL(url);
#endif
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
                    $"Successful verification! You may close this tab and return to {Application.productName}.");

            response.ContentLength64 = buffer.Length;

            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
            httpListener.Close();

            HandleCaptchaResponse(context.Request.QueryString);
        }

        private void HandleCaptchaResponse(NameValueCollection parameters)
        {
            var error = parameters.Get("err");
            if (error != null)
            {
                _errorCallback?.Invoke(error);
                return;
            }
            
            var token = parameters.Get("res");
            if (token == null)
            {
                _errorCallback?.Invoke("Captcha token is null");
                return;
            }

            _successCallback?.Invoke(token);
        }
    }
}