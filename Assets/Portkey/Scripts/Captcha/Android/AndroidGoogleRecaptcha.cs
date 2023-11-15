using System.Collections;
using System.Collections.Concurrent;
using Portkey.Core;
using Portkey.Utilities;
using UnityEngine;

namespace Portkey.Captcha
{
    public class AndroidGoogleRecaptcha : ICaptcha
    {
        private class CaptchaOutput
        {
            public bool error = false;
            public string result = null;
        }
        
        private class RecaptchaJavaAuthCallback : AndroidJavaProxy
        {
            private ConcurrentQueue<CaptchaOutput> _captchaResults;
            public RecaptchaJavaAuthCallback(ConcurrentQueue<CaptchaOutput> captchaResults) : base("com.portkey.nativerecaptcha.NativeRecaptcha$Callback")
            {
                _captchaResults = captchaResults;
            }

            public virtual void onSuccess(string captchaToken)
            {
                if (string.IsNullOrEmpty(captchaToken))
                {
                    onFailure(new AndroidJavaObject("java.lang.Exception", "Captcha token is null or empty!"));
                    return;
                }

                var output = new CaptchaOutput
                {
                    error = false,
                    result = captchaToken
                };
                _captchaResults.Enqueue(output);
            }

            public virtual void onFailure(AndroidJavaObject e)
            {
                var message = e.Call<string>("getMessage");
                
                var output = new CaptchaOutput
                {
                    error = true,
                    result = message
                };
                _captchaResults.Enqueue(output);
            }
        }
        
        private ConcurrentQueue<CaptchaOutput> _captchaResults = new ConcurrentQueue<CaptchaOutput>();
        private SuccessCallback<string> _onSuccess = null;
        private ErrorCallback _onError = null;
        private readonly PortkeyConfig _config;
        
        public AndroidGoogleRecaptcha(PortkeyConfig config)
        {
            _config = config;
        }
        
        public void ExecuteCaptcha(SuccessCallback<string> successCallback, ErrorCallback errorCallback)
        {
            _onSuccess = successCallback;
            _onError = errorCallback;
            
            StaticCoroutine.StartCoroutine(HandleCaptchaOutput());

            ExecuteCaptchaJava();
        }
        
        private void ExecuteCaptchaJava()
        {
            var callback = new RecaptchaJavaAuthCallback(_captchaResults);

            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            using var context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
            using var recaptchaPlugin = new AndroidJavaClass("com.portkey.nativerecaptcha.NativeRecaptcha");
            recaptchaPlugin.CallStatic("SetCallback", callback);
            recaptchaPlugin.CallStatic("VerifyGoogleReCAPTCHA", context, _config.RecaptchaAndroidSitekey);
        }
        
        private IEnumerator HandleCaptchaOutput()
        {
            while (true)
            {
                if (_captchaResults.TryDequeue(out var output))
                {
                    if (output.error)
                    {
                        Debugger.LogError(output.result);
                        _onError?.Invoke(output.result);
                    }
                    else
                    {
                        _onSuccess?.Invoke(output.result);
                    }
                    yield break;
                }
                yield return null;
            }
        }
    }
}