using System.Runtime.InteropServices;
using Portkey.Core;
using Portkey.Core.Captcha;
using UnityEngine;

namespace Portkey.Captcha
{
    public class IOSGoogleRecaptcha : ICaptcha
    {
        private ICaptchaCallback _callbackObject;
        private readonly PortkeyConfig _config;
#if UNITY_IOS
        [DllImport ("__Internal")]
        private static extern void RecaptchaVerify (string siteKey);
#endif
        public IOSGoogleRecaptcha(PortkeyConfig config)
        {
            _config = config;
        }
        
        public void ExecuteCaptcha(SuccessCallback<string> successCallback, ErrorCallback errorCallback)
        {
            Listen(successCallback, errorCallback);
#if UNITY_IOS
            RecaptchaVerify(_config.RecaptchaWebSitekey);
#endif
        }
        
        private void Listen(SuccessCallback<string> successCallback, ErrorCallback errorCallback)
        {
#if UNITY_IOS
            var gameObject = new GameObject("IOSCaptchaCallback");
            _callbackObject = gameObject.AddComponent<PortkeyGoogleRecaptchaCallback>();
            _callbackObject.OnSuccess += OnCaptchaSuccess;
            _callbackObject.OnError += OnError;
            
            void OnCaptchaSuccess(string data)
            {
                if (string.IsNullOrEmpty(data))
                {
                    OnError("Captcha token is null or empty!");
                    return;
                }
                
                successCallback?.Invoke(data);
            }

            void OnError(string error)
            {
                Debugger.LogError(error);
                errorCallback?.Invoke(error);
            }
#endif
        }
    }
}