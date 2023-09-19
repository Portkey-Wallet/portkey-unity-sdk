using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class GoogleCredentialProvider
    {
        private readonly ISocialProvider _socialProvider;
        private readonly IAuthMessage _authMessage;
        
        public GoogleCredentialProvider(ISocialProvider socialProvider, IAuthMessage authMessage)
        {
            _socialProvider = socialProvider;
            _authMessage = authMessage;
        }
        
        public void Get(SuccessCallback<GoogleCredential> successCallback)
        {
            var socialLogin = _socialProvider.GetSocialLogin(AccountType.Google);
            socialLogin.Authenticate((info) =>
            {
                var appleCredential = new GoogleCredential(info.access_token, info.socialInfo);
                successCallback?.Invoke(appleCredential);
            }, _authMessage.Error);
        }
    }
}