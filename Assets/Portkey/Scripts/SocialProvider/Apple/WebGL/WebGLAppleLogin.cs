using System;
using System.Runtime.InteropServices;
using Portkey.Core;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class WebGLAppleLogin : AppleLoginBase
    {
        private string _url;
        
        public WebGLAppleLogin(PortkeyConfig config, IHttp request) : base(request)
        {
            _url = config.GoogleWebGLLoginUrl;
        }
        
#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void OpenURL(string url);
#endif

        protected override void OnAuthenticate()
        {
            const string loginUri = "social-login/";
            const string loginType = "Apple";

            SetupAuthenticationCallback();

            Debugger.Log("Authenticating for WebGL");
            var url = $"{_url}{loginUri}{loginType}";
#if UNITY_WEBGL
            OpenURL(url);
#endif
        }

        private void SetupAuthenticationCallback()
        {
            var gameObject = new GameObject("WebGLPortkeyLoginCallback");
            var callbackComponent = gameObject.AddComponent<WebGLPortkeyLoginCallback>();
            callbackComponent.OnSuccessCallback = OnSuccess;
            callbackComponent.OnFailureCallback = OnFailure;
        }
        
        private void OnSuccess(string accessToken)
        {
            try
            {
                RequestSocialInfo(accessToken, null, null);
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