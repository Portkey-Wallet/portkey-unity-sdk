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
}