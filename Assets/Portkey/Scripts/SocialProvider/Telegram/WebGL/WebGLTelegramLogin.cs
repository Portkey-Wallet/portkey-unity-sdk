using System;
using System.Runtime.InteropServices;
using Portkey.Core;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class WebGLTelegramLogin : TelegramLoginBase
    {
        private string _url;
        private int _port;
        
        public WebGLTelegramLogin(PortkeyConfig config, IHttp request) : base(request)
        {
            _url = config.TelegramLoginUrl;
            _port = config.TelegramLoginPort;
        }
        
#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void OpenURL(string url);
#endif

        protected override void OnAuthenticate()
        {
            const string loginUri = "social-login/";
            const string loginType = "Telegram";
            const string loginFrom = "unitysdk";
            const string serviceUri = "https://test3-applesign-v2.portkey.finance";
            SetupAuthenticationCallback();
            var url = $"{_url}{loginUri}{loginType}?from={loginFrom}&serviceURI={serviceUri}";
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
            Debugger.Log("on success");
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
            Debugger.Log("on failure");
            HandleError("Login Cancelled!");
        }
    }
}