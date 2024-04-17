using System.Collections;
using System.Collections.Concurrent;
using Portkey.Core;
using Portkey.Utilities;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class AndroidGetAccessTokenParam : GetAccessTokenParam
    {
        public string client_secret;
    }

    public class AndroidGoogleLogin : TokenRequestGoogleLoginBase<AndroidGetAccessTokenParam>
    {
        public class GoogleAuthOutput
        {
            public bool isError;
            public string message;
        }
        
        private class AndroidAuthCallback : AndroidJavaProxy
        {
            private ConcurrentQueue<GoogleAuthOutput> _authTokens;
            
            public AndroidAuthCallback(ConcurrentQueue<GoogleAuthOutput> authTokens) : base("com.portkey.nativegoogleloginactivity.NativeGoogleLoginActivity$Callback")
            {
                _authTokens = authTokens;
            }

            public void onResult(string authToken)
            {
                var output = new GoogleAuthOutput
                {
                    isError = false,
                    message = authToken
                };
                _authTokens.Enqueue(output);
            }

            public void onError(AndroidJavaObject e)
            {
                var message = e.Call<string>("getMessage");
                
                var output = new GoogleAuthOutput
                {
                    isError = true,
                    message = message
                };
                _authTokens.Enqueue(output);
            }
        }

        private ConcurrentQueue<GoogleAuthOutput> AuthTokens { get; } = new ConcurrentQueue<GoogleAuthOutput>();
        private readonly string _clientSecret;
        
        public AndroidGoogleLogin(PortkeyConfig config, IHttp request) : base(request)
        {
            ClientId = config.GoogleAndroidClientId;
            _clientSecret = config.GoogleAndroidClientSecret;
        }

        protected override void OnAuthenticate()
        {
            StaticCoroutine.StartCoroutine(HandleAndroidAuthTokens());
            
            Authenticate();
        }
        
        private IEnumerator HandleAndroidAuthTokens()
        {
            while (true)
            {
                if (AuthTokens.TryDequeue(out var output))
                {
                    if (output.isError)
                    {
                        Debugger.LogError(output.message);
                        _errorCallback?.Invoke("User cancelled Google Login!");
                    }
                    else
                    {
                        RequestAccessToken(output.message);
                    }
                    yield break;
                }

                yield return null;
            }
        }
        
        private void Authenticate()
        {
            var callback = new AndroidAuthCallback(AuthTokens);
            
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            using var context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
            using var unityGoogleLoginPlugin = new AndroidJavaClass("com.portkey.nativegoogleloginactivity.NativeGoogleLoginActivity");
            unityGoogleLoginPlugin.CallStatic("Call", currentActivity, ClientId, callback);
        }

        protected override AndroidGetAccessTokenParam CreateGetAccessTokenParam(string authCode)
        {
            return new AndroidGetAccessTokenParam
            {
                code = authCode,
                redirect_uri = "",
                client_id = ClientId,
                client_secret = _clientSecret,
                scope = ACCESS_SCOPE,
                grant_type = "authorization_code"
            };
        }
    }
}