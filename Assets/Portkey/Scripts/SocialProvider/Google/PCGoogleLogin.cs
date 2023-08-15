using System;
using System.Collections.Specialized;
using Portkey.Core;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class PCGetAccessTokenParam : GetAccessTokenParam
    {
        public string code_verifier;
        public string client_secret;
    }
    
    public class PCGoogleLogin : TokenRequestGoogleLoginBase<PCGetAccessTokenParam>
    {
        private readonly string _clientSecret;
        private string _redirectUri;
        private string _state;
        private string _codeVerifier;

        public PCGoogleLogin(PortkeyConfig config, IHttp request) : base(request)
        {
            ClientId = config.GooglePCClientId;
            _clientSecret = config.GooglePCClientSecret;
        }

        protected override void OnAuthenticate()
        {
            _state = Guid.NewGuid().ToString();
            _codeVerifier = Guid.NewGuid().ToString();
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
                RequestAccessToken(code);
            }
            else
            {
                Debugger.LogError("Unsynchronized state.");
            }
        }

        private void Authenticate()
        {
            var codeChallenge = Utilities.GetCodeChallenge(_codeVerifier);
            var authorizationRequest = $"{AUTHORIZATION_ENDPOINT}?response_type=code&client_id={ClientId}&state={_state}&scope={Uri.EscapeDataString(ACCESS_SCOPE)}&redirect_uri={Uri.EscapeDataString(_redirectUri)}&code_challenge={codeChallenge}&code_challenge_method=S256";

            _startLoadCallback?.Invoke(true);
            Application.OpenURL(authorizationRequest);
        }

        protected override PCGetAccessTokenParam CreateGetAccessTokenParam(string authCode)
        {
            return new PCGetAccessTokenParam
            {
                code = authCode,
                redirect_uri = _redirectUri,
                client_id = ClientId,
                code_verifier = _codeVerifier,
                client_secret = _clientSecret,
                scope = ACCESS_SCOPE,
                grant_type = "authorization_code"
            };
        }
    }
}
