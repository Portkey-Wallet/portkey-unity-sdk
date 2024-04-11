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
            if (_socialVerifiers.TryGetValue(type, out var socialVerifier))
            {
                return socialVerifier;
            }
            
            socialVerifier = CreateSocialVerifier(type);
            _socialVerifiers.Add(type, socialVerifier);
            
            return socialVerifier;
        }

        private ISocialVerifier CreateSocialVerifier(AccountType type)
        {
            ISocialVerifier socialVerifier = type switch
            {
                AccountType.Google => new GoogleVerifier(_socialProvider.GetSocialLogin(AccountType.Google), _portkeySocialService),
                AccountType.Telegram => new TelegramVerifier(_socialProvider.GetSocialLogin(AccountType.Telegram), _portkeySocialService),
                AccountType.Email => throw new NotImplementedException("Email not implemented yet"),
                AccountType.Phone => throw new NotImplementedException("Phone not implemented yet"),
                AccountType.Apple => new AppleVerifier(_socialProvider.GetSocialLogin(AccountType.Apple), _portkeySocialService),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
            return socialVerifier;
        }
    }
}
