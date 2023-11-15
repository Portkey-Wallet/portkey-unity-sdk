using System;
using Portkey.Core;

namespace Portkey.Captcha
{
    public class CaptchaProvider : ICaptchaProvider
    {
        private ICaptcha _captcha;
        private readonly PortkeyConfig _config;
        
        public CaptchaProvider(PortkeyConfig config)
        {
            _config = config;
        }
        
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
            return new IOSGoogleRecaptcha(_config);
#elif UNITY_ANDROID
            return new AndroidGoogleRecaptcha(_config);
#elif UNITY_WEBGL
            return new WebGLGoogleRecaptcha(_config);
#else
            throw new NotImplementedException("Platform not supported");
#endif
        }
    }
}