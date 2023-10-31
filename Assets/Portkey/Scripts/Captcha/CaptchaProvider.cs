using System;
using Portkey.Core;

namespace Portkey.Captcha
{
    public class CaptchaProvider : ICaptchaProvider
    {
        private ICaptcha _captcha;
        
        public ICaptcha GetCaptcha()
        {
            _captcha ??= GetCaptchaByPlatform();
            return _captcha;
        }
        
        private ICaptcha GetCaptchaByPlatform()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            throw new NotImplementedException("Platform not supported");
#elif UNITY_IOS
            throw new NotImplementedException("Platform not supported");
#elif UNITY_ANDROID
            throw new NotImplementedException("Platform not supported");
#elif UNITY_WEBGL
            return new WebGLGoogleRecaptcha();
#else
            throw new NotImplementedException("Platform not supported");
#endif
        }
    }
}