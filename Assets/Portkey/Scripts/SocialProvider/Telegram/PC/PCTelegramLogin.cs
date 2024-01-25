using System;
using System.Collections.Specialized;
using Portkey.Core;
using UnityEngine;

namespace Portkey.SocialProvider
{
    
    public class PCTelegramLogin : TelegramLoginBase
    {
        private readonly string _clientSecret;
        private string _redirectUri;
        private int _randomPort;
        private string _state;
        private string _codeVerifier;
        private string _url;

        public PCTelegramLogin(PortkeyConfig config, IHttp request) : base(request)
        {
            _url = config.TelegramWebGLLoginUrl;
        }

        protected override void OnAuthenticate()
        {
            _state = Guid.NewGuid().ToString();
            _codeVerifier = Guid.NewGuid().ToString();

            _randomPort = 8070;
            _redirectUri = $"https://3773-202-156-61-238.ngrok-free.app/";
            
            Authenticate();
            Listen();
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
            var codeChallenge = Utilities.GetCodeChallenge(_codeVerifier);
            // var authorizationRequest = $"{AUTHORIZATION_ENDPOINT}?response_type=code&client_id={ClientId}&state={_state}&scope={Uri.EscapeDataString(ACCESS_SCOPE)}&redirect_uri={Uri.EscapeDataString(_redirectUri)}&code_challenge={codeChallenge}&code_challenge_method=S256";
            var authorizationRequest = $"{_url}/social-login/Telegram";
            Application.OpenURL(authorizationRequest);
        }
        
    }
}
