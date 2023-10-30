using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class AppleCredentialProvider : SocialCredentialProviderBase<AppleCredential>
    {
        private readonly ISocialProvider _socialProvider;
        
        public AppleCredentialProvider(ISocialProvider socialProvider, IAuthMessage authMessage, IVerifierService verifierService, ISocialVerifierProvider socialVerifierProvider) : base(authMessage, verifierService, socialVerifierProvider)
        {
            _socialProvider = socialProvider;
        }
        
        public void Get(SuccessCallback<AppleCredential> successCallback)
        {
            var socialLogin = _socialProvider.GetSocialLogin(AccountType.Apple);
            socialLogin.Authenticate((info) =>
            {
                var appleCredential = new AppleCredential(info.access_token, info.socialInfo);
                successCallback?.Invoke(appleCredential);
            }, authMessage.Error);
        }

        public override AccountType AccountType => AccountType.Apple;
    }
}