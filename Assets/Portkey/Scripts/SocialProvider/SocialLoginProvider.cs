using System;
using System.Collections.Generic;
using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class SocialLoginProvider : ISocialProvider
    {
        private readonly PortkeyConfig _config;
        private readonly IHttp _request;
        private readonly Dictionary<AccountType, ISocialLogin> _socialLogins = new Dictionary<AccountType, ISocialLogin>();

        public SocialLoginProvider(PortkeyConfig config, IHttp request)
        {
            _config = config;
            _request = request;
        }
        
        public ISocialLogin GetSocialLogin(AccountType type)
        {
            if (_socialLogins.TryGetValue(type, out var socialLogin))
            {
                return socialLogin;
            }
            
            socialLogin = CreateSocialLogin(type);
            _socialLogins.Add(type, socialLogin);
            return socialLogin;
        }

        private ISocialLogin CreateSocialLogin(AccountType type) => type switch
        {
            AccountType.Google => GetGoogleLoginByPlatform(),
            AccountType.Email => null,
            AccountType.Phone => null,
            AccountType.Apple => GetAppleLoginByPlatform(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), $"Not expected account type: {type}")
        };

        private ISocialLogin GetGoogleLoginByPlatform()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return new PCGoogleLogin(_config, _request);
#elif UNITY_IOS
            return new IOSGoogleLogin(_config, _request);
#elif UNITY_ANDROID
            return new AndroidGoogleLogin(_config, _request);
#elif UNITY_WEBGL
            return new WebGLGoogleLogin(_config, _request);
#else
            throw new NotImplementedException("Platform not supported");
#endif
        }
        
        private ISocialLogin GetAppleLoginByPlatform()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            throw new NotImplementedException("Platform not supported");
#elif UNITY_IOS
            throw new NotImplementedException("Platform not supported");
#elif UNITY_ANDROID
            return new AndroidAppleLogin(_config, _request);
#elif UNITY_WEBGL
            throw new NotImplementedException("Platform not supported");
#else
            throw new NotImplementedException("Platform not supported");
#endif
        }
    }
}
