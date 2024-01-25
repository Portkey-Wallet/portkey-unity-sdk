using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class TelegramCredentialProvider : SocialCredentialProviderBase<TelegramCredential>
    {
        private readonly ISocialProvider _socialProvider;
        
        public TelegramCredentialProvider(ISocialProvider socialProvider, IAuthMessage authMessage, IVerifierService verifierService, ISocialVerifierProvider socialVerifierProvider) : base(authMessage, verifierService, socialVerifierProvider)
        {
            _socialProvider = socialProvider;
        }
        
        public void Get(SuccessCallback<TelegramCredential> successCallback)
        {
            var socialLogin = _socialProvider.GetSocialLogin(AccountType.Telegram);
            socialLogin.Authenticate((info) =>
            {
                var appleCredential = new TelegramCredential(info.access_token, info.socialInfo);
                successCallback?.Invoke(appleCredential);
            }, authMessage.Error);
        }

        public override AccountType AccountType => AccountType.Telegram;
    }
}