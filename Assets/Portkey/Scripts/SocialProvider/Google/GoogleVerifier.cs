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
        
        public void AuthenticateIfAccessTokenExpired(VerifyAccessTokenParam param, AuthCallback successCallback, SuccessCallback<bool> startLoadCallback, ErrorCallback errorCallback)
        {
            if(param.accessToken == null)
            {
                _socialLogin.Authenticate((info) =>
                {
                    param.accessToken = info.access_token;
                    VerifyGoogleToken();
                }, startLoadCallback, errorCallback);
                return;
            }
            
            startLoadCallback(true);
            // check if login access token is expired
            _socialLogin.RequestSocialInfo(param.accessToken, (socialLoginInfo) =>
            {
                if(socialLoginInfo.isExpired)
                {
                    startLoadCallback(false);
                    //login expired, need to re-login
                    _socialLogin.Authenticate((info) =>
                    {
                        param.accessToken = info.access_token;
                        VerifyGoogleToken();
                    }, startLoadCallback, errorCallback);
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
                    var verificationDoc = ProcessVerificationDoc(verificationResult.verificationDoc, param.verifierId);
                    //TODO: set guardian list
                    successCallback(verificationDoc, param.accessToken, verificationResult);
                }, errorCallback));
            }
        }
        
        private static VerificationDoc ProcessVerificationDoc(string verificationDoc, string verifierId)
        {
            var documentValue = verificationDoc.Split(',');
            var verificationDocObj = new VerificationDoc
            {
                verifierId = verifierId,
                type = documentValue[0],
                identifierHash = documentValue[1],
                verificationTime = documentValue[2],
                verifierAddress = documentValue[3],
                salt = documentValue[4]
            };
            return verificationDocObj;
        }
    }
}