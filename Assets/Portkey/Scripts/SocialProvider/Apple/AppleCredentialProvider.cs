using Portkey.Core;

namespace Portkey.SocialProvider
{
    public class AppleCredentialProvider
    {
        private readonly ISocialProvider _socialProvider;
        
        public AppleCredentialProvider(ISocialProvider socialProvider)
        {
            _socialProvider = socialProvider;
        }
        
        public void Get(SuccessCallback<AppleCredential> successCallback, ErrorCallback errorCallback)
        {
            var socialLogin = _socialProvider.GetSocialLogin(AccountType.Apple);
            socialLogin.Authenticate((info) =>
            {
                var appleCredential = new AppleCredential(info.access_token, info.socialInfo);
                successCallback?.Invoke(appleCredential);
            }, null, errorCallback);
        }
    }
}