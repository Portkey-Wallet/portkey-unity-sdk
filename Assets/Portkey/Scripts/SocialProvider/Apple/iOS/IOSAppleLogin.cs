using System.Runtime.InteropServices;
using Portkey.Core;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class IOSAppleLogin : AppleLoginBase
    {
        private readonly string _redirectUri;

        private readonly string APPLE_AUTH_API = "https://appleid.apple.com/auth/authorize";
        private readonly string SERVICE_ID = "com.portkey.did.extension.service";
        private readonly string APPLE_REGISTERED_REDIRECT_URI = "https://did-portkey.portkey.finance/api/app/AppleAuth/unifyReceive";
        
        public IOSAppleLogin(PortkeyConfig config, IHttp request) : base(request)
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
            callbackComponent.SocialLogin = this;
        }
        
#if UNITY_IOS
        [DllImport ("__Internal")]
        private static extern void SignInApple (string url, string redirectUri);
#endif
        
        private void Authenticate()
        {
            Debugger.Log("Authenticating for Apple");
            var url = $"{APPLE_AUTH_API}?client_id={SERVICE_ID}&redirect_uri={APPLE_REGISTERED_REDIRECT_URI}&response_type=code%20id_token&state=origin%3Aweb&scope=name%20email&response_mode=form_post";
            
#if UNITY_IOS
            SignInApple(url, _redirectUri);
#endif
        }
    }
}