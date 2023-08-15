using System;
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

        private readonly string _clientId;
        private readonly string _clientSecret;
        private string _redirectUri;

        private string CodeVerifier { get; set; }

        public PCGoogleLogin(PortkeyConfig config, IHttp request) : base(request)
        {
            _clientId = config.GooglePCClientId;
            _clientSecret = config.GooglePCClientSecret;
        }
        
        protected override void OnAuthenticate()
        {
            _redirectUri = $"http://localhost:{Utilities.GetRandomUnusedPort()}/";

            Listen();
            Authenticate();
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

        private void Authenticate()
        {
            CodeVerifier = Guid.NewGuid().ToString();

            var codeChallenge = Utilities.GetCodeChallenge(CodeVerifier);
            var authorizationRequest = $"{AUTHORIZATION_ENDPOINT}?response_type=code&client_id={_clientId}&state={_state}&scope={Uri.EscapeDataString(ACCESS_SCOPE)}&redirect_uri={Uri.EscapeDataString(_redirectUri)}&code_challenge={codeChallenge}&code_challenge_method=S256";

            _startLoadCallback?.Invoke(true);
            Application.OpenURL(authorizationRequest);
        }

        protected override PCGetAccessTokenParam CreateGetAccessTokenParam(string code)
        {
            return new PCGetAccessTokenParam
            {
                code = code,
                redirect_uri = _redirectUri,
                client_id = _clientId,
                code_verifier = CodeVerifier,
                client_secret = _clientSecret,
                scope = ACCESS_SCOPE,
                grant_type = "authorization_code"
            };
        }
    }
}
