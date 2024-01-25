using System;
using System.Runtime.InteropServices;
using Portkey.Core;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class IOSTelegramLogin : TelegramLoginBase
    {
        private readonly string _redirectUri;

        private readonly string APPLE_AUTH_API = "https://appleid.apple.com/auth/authorize";
        private readonly string SERVICE_ID = "com.portkey.did.extension.service";
        private readonly string APPLE_REGISTERED_REDIRECT_URI = "https://did-portkey.portkey.finance/api/app/AppleAuth/unifyReceive";
        
        public IOSTelegramLogin(PortkeyConfig config, IHttp request) : base(request)
        {
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
            Debugger.Log("Authenticating for Apple");
            var url = $"https://openlogin-test.portkey.finance/social-login/Telegram?from=portkey&network=TESTNET";
            
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