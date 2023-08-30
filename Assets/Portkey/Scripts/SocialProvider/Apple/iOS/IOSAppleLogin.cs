using System;
using System.Runtime.InteropServices;
using Portkey.Core;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class IOSAppleLogin : AppleLoginBase
    {
        private readonly string _url;
        private readonly string _redirectUri;
        
        public IOSAppleLogin(PortkeyConfig config, IHttp request) : base(request)
        {
            _url = config.GoogleWebGLLoginUrl;
            _redirectUri = config.GoogleWebGLRedirectUri;
        }

        protected override void OnAuthenticate()
        {
            SetupAuthenticationCallback();
            
            Authenticate();
        }
        
        private void SetupAuthenticationCallback()
        {
            var gameObject = new GameObject("IOSPortkeyAppleLoginCallback");
            var callbackComponent = gameObject.AddComponent<IOSPortkeyAppleLoginCallback>();
            callbackComponent.OnSuccessCallback = OnSuccess;
            callbackComponent.OnFailureCallback = OnFailure;
        }
        
#if UNITY_IOS
        [DllImport ("__Internal")]
        private static extern void SignInApple (string url, string redirectUri);
#endif
        
        private void Authenticate()
        {
            _startLoadCallback?.Invoke(true);
            
            const string loginUri = "social-login/";
            const string loginType = "Apple";

            Debugger.Log("Authenticating for Apple");
            var url = $"{_url}{loginUri}{loginType}";
            
#if UNITY_IOS
            SignInApple(url, _redirectUri);
#endif
        }

        private void OnSuccess(string identifyToken)
        {
            try
            {
                RequestSocialInfo(identifyToken, null, null);
            }
            catch (Exception e)
            {
                Debugger.LogException(e);
                HandleError("Error in logging in. Please try again.");
            }
        }
        
        private void OnFailure(string error)
        {
            HandleError("Login Cancelled!");
        }
    }
}