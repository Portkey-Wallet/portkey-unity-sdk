using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class GoogleCredentialProvider : SocialCredentialProviderBase<GoogleCredential>
    {
        private readonly ISocialProvider _socialProvider;
        
        public GoogleCredentialProvider(ISocialProvider socialProvider, IAuthMessage authMessage, IVerifierService verifierService, ISocialVerifierProvider socialVerifierProvider) : base(authMessage, verifierService, socialVerifierProvider)
        {
            _socialProvider = socialProvider;
        }
        
        public void Get(SuccessCallback<GoogleCredential> successCallback)
        {
            var socialLogin = _socialProvider.GetSocialLogin(AccountType.Google);
            socialLogin.Authenticate((info) =>
            {
                var appleCredential = new GoogleCredential(info.access_token, info.socialInfo);
                successCallback?.Invoke(appleCredential);
            }, authMessage.Error);
        }

        public override AccountType AccountType => AccountType.Google;
    }
}