using System.Runtime.InteropServices;
using Portkey.Core;
using Portkey.Core.Captcha;
using UnityEngine;

namespace Portkey.Captcha
{
    public class WebGLGoogleRecaptcha : ICaptcha
    {
        private ICaptchaCallback _callbackObject;
#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void ExecuteRecaptcha();
#endif
        
        public void ExecuteCaptcha(SuccessCallback<string> successCallback, ErrorCallback errorCallback)
        {
            Listen(successCallback, errorCallback);
#if UNITY_WEBGL
            ExecuteRecaptcha();
#endif
        }
        
        private void Listen(SuccessCallback<string> successCallback, ErrorCallback errorCallback)
        {
#if UNITY_WEBGL
            var gameObject = new GameObject("PortkeyGoogleRecaptchaCallback");
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