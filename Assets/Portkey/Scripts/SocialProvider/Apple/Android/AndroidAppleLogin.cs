using System.Collections;
using System.Collections.Concurrent;
using Portkey.Core;
using Portkey.Utilities;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class AndroidAppleLogin : AppleLoginBase
    {
        private class AppleAuthOutput
        {
            public bool isError;
            public string message;
        }
        
        private class AndroidAuthCallback : AndroidJavaProxy
        {
            private ConcurrentQueue<AppleAuthOutput> _authTokens;
            
            public AndroidAuthCallback(ConcurrentQueue<AppleAuthOutput> authTokens) : base("com.portkey.nativeappleloginactivity.NativeAppleLoginActivity$Callback")
            {
                _authTokens = authTokens;
            }

            public void onResult(string authToken)
            {
                var output = new AppleAuthOutput
                {
                    isError = false,
                    message = authToken
                };
                _authTokens.Enqueue(output);
            }

            public void onError(AndroidJavaObject e)
            {
                var message = e.Call<string>("getMessage");
                
                var output = new AppleAuthOutput
                {
                    isError = true,
                    message = message
                };
                _authTokens.Enqueue(output);
            }
        }

        private ConcurrentQueue<AppleAuthOutput> AuthTokens { get; } = new ConcurrentQueue<AppleAuthOutput>();
        private string _url;
        
        public AndroidAppleLogin(PortkeyConfig config, IHttp request) : base(request)
        {
            _url = config.GoogleWebGLLoginUrl;
        }

        protected override void OnAuthenticate()
        {
            StaticCoroutine.StartCoroutine(HandleAuthTokens());
            
            Authenticate();
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
        
        private void Authenticate()
        {
            const string loginUri = "social-login/";
            const string loginType = "Apple";
            var url = $"{_url}{loginUri}{loginType}";
            
            var callback = new AndroidAuthCallback(AuthTokens);
            
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            using var context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
            using var unityAppleLoginPlugin = new AndroidJavaClass("com.portkey.nativeappleloginactivity.NativeAppleLoginActivity");
            unityAppleLoginPlugin.SetStatic("url", url);
            unityAppleLoginPlugin.CallStatic("SetCallback", callback);
            unityAppleLoginPlugin.CallStatic("Call", currentActivity);
        }
    }
}