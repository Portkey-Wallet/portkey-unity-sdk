using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class GoogleCredentialProvider
    {
        private readonly ISocialProvider _socialProvider;
        
        public GoogleCredentialProvider(ISocialProvider socialProvider)
        {
            _socialProvider = socialProvider;
        }
        
        public void Get(SuccessCallback<GoogleCredential> successCallback, ErrorCallback errorCallback)
        {
            var socialLogin = _socialProvider.GetSocialLogin(AccountType.Google);
            socialLogin.Authenticate((info) =>
            {
                var appleCredential = new GoogleCredential(info.access_token, info.socialInfo);
                successCallback?.Invoke(appleCredential);
            }, null, errorCallback);
        }
    }
}