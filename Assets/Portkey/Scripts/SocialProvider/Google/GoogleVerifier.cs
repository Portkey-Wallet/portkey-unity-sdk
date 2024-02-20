using Portkey.Core;
using Portkey.Utilities;

namespace Portkey.SocialProvider
{
    public class GoogleVerifier : SocialVerifierBase
    {
        private IPortkeySocialService _portkeySocialService;
        
        public GoogleVerifier(ISocialLogin socialLogin, IPortkeySocialService portkeySocialService) : base(socialLogin)
        {
            _portkeySocialService = portkeySocialService;
        }

        protected override void VerifyToken(VerifyAccessTokenParam param, AuthCallback successCallback, ErrorCallback errorCallback)
        {
            var verifyGoogleParam = Convert(param);
            StaticCoroutine.StartCoroutine(_portkeySocialService.VerifyGoogleToken(verifyGoogleParam, (verificationResult) =>
            {
                var verifyCodeResult = new VerifyCodeResult(verificationResult, verifyGoogleParam.verifierId);
                //TODO: set guardian list
                successCallback(verifyCodeResult, param.accessToken);
            }, errorCallback));
        }
    }
}