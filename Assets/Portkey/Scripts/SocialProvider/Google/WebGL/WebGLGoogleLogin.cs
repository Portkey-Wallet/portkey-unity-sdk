using System.Runtime.InteropServices;
using Portkey.Core;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class WebGLGoogleLogin : GoogleLoginBase
    {
        private string _redirectUri;
        private string _url;
        
        public WebGLGoogleLogin(PortkeyConfig config, IHttp request) : base(request)
        {
            ClientId = config.GoogleWebGLClientId;
            _url = config.GoogleWebGLLoginUrl;
            _redirectUri = config.GoogleWebGLRedirectUri;
        }

        protected override void OnAuthenticate()
        {
            const string loginUri = "social-login/";
            const string loginType = "Google";

            _startLoadCallback?.Invoke(true);

            SetupAuthenticationCallback();

            Debugger.Log("Authenticating for WebGL");
            Application.OpenURL($"{_url}{loginUri}{loginType}?clientId={ClientId}&redirectUri={_redirectUri}");
        }
        
#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void Listen();
#endif
        
        private void SetupAuthenticationCallback()
        {
            var gameObject = new GameObject("WebGLPortkeyGoogleLoginCallback");
            var callbackComponent = gameObject.AddComponent<WebGLPortkeyGooglelLoginCallback>();
            callbackComponent.SocialLogin = this;
            
#if UNITY_WEBGL
            Listen();
#endif
        }
    }
}