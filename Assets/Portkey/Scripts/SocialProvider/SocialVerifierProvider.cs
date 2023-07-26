using System;
using System.Collections.Generic;
using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class SocialVerifierProvider : ISocialVerifierProvider
    {
        private Dictionary<AccountType, ISocialVerifier> _socialVerifiers = new Dictionary<AccountType, ISocialVerifier>();
        private ISocialProvider _socialProvider;
        private IPortkeySocialService _portkeySocialService;

        public SocialVerifierProvider(ISocialProvider socialProvider, IPortkeySocialService portkeySocialService)
        {
            _socialProvider = socialProvider;
            _portkeySocialService = portkeySocialService;
        }
        
        public ISocialVerifier GetSocialVerifier(AccountType type)
        {
            switch (type)
            {
                case AccountType.Google:
                    if (_socialVerifiers.TryGetValue(type, out var google))
                    {
                        return google;
                    }
                    google = new GoogleVerifier(_socialProvider.GetSocialLogin(AccountType.Google), _portkeySocialService);
                    _socialVerifiers.Add(type, google);
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
