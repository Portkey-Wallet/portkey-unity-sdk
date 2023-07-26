using Portkey.Core;
using Portkey.Utilities;

namespace Portkey.SocialProvider
{
    public class GoogleVerifier : ISocialVerifier
    {
        private ISocialLogin _socialLogin;
        private IPortkeySocialService _portkeySocialService;
        
        public GoogleVerifier(ISocialLogin socialLogin, IPortkeySocialService portkeySocialService)
        {
            _socialLogin = socialLogin;
            _portkeySocialService = portkeySocialService;
        }
        
        public void AuthenticateIfAccessTokenExpired(VerifyAccessTokenParam param, AuthCallback successCallback, ErrorCallback errorCallback)
        {
            if(param.accessToken == null)
            {
                _socialLogin.Authenticate((info) =>
                {
                    param.accessToken = info.access_token;
                    VerifyGoogleToken();
                }, errorCallback);
                return;
            }
            // check if login access token is expired
            _socialLogin.RequestSocialInfo(param.accessToken, (socialInfo) =>
            {
                if(socialInfo == null)
                {
                    //login expired, need to re-login
                    _socialLogin.Authenticate((info) =>
                    {
                        param.accessToken = info.access_token;
                        VerifyGoogleToken();
                    }, errorCallback);
                }
                else
                {
                    VerifyGoogleToken();
                }
            }, errorCallback);
            
            void VerifyGoogleToken()
            {
                var verifyGoogleParam = new VerifyGoogleTokenParams
                {
                    accessToken = param.accessToken,
                    chainId = param.chainId,
                    verifierId = param.verifierId
                };
                StaticCoroutine.StartCoroutine(_portkeySocialService.VerifyGoogleToken(verifyGoogleParam, (verificationResult) =>
                {
                    //var verificationDoc = ProcessVerificationDoc(verificationResult.verificationDoc);
                    //TODO: set guardian list
                    successCallback(param.verifierId, param.accessToken, verificationResult);
                }, errorCallback));
            }
        }
    }
}