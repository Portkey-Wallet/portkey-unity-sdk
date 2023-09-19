using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class AppleCredentialProvider
    {
        private readonly ISocialProvider _socialProvider;
        private readonly IAuthMessage _authMessage;
        
        public AppleCredentialProvider(ISocialProvider socialProvider, IAuthMessage authMessage)
        {
            _socialProvider = socialProvider;
            _authMessage = authMessage;
        }
        
        public void Get(SuccessCallback<AppleCredential> successCallback)
        {
            var socialLogin = _socialProvider.GetSocialLogin(AccountType.Apple);
            socialLogin.Authenticate((info) =>
            {
                var appleCredential = new AppleCredential(info.access_token, info.socialInfo);
                successCallback?.Invoke(appleCredential);
            }, _authMessage.Error);
        }
    }
}