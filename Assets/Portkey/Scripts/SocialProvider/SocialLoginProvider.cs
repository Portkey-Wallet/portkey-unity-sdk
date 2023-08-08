using System;
using System.Collections.Generic;
using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class SocialLoginProvider : ISocialProvider
    {
        private PortkeyConfig _config;
        private IHttp _request;
        private Dictionary<AccountType, ISocialLogin> _socialLogins = new Dictionary<AccountType, ISocialLogin>();

        public SocialLoginProvider(PortkeyConfig config, IHttp request)
        {
            _config = config;
            _request = request;
        }
        
        public ISocialLogin GetSocialLogin(AccountType type)
        {
            switch (type)
            {
                case AccountType.Google:
                    if (_socialLogins.TryGetValue(type, out var google))
                    {
                        return google;
                    }
                    google = new GoogleLogin(_config, _request);
                    _socialLogins.Add(type, google);
                    return google;
                case AccountType.Email:
                case AccountType.Phone:
                case AccountType.Apple:
                    throw new NotImplementedException($"{type.ToString()} not yet implemented");
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
