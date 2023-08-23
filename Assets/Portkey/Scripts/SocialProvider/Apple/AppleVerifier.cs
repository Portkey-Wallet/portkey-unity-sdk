using Portkey.Core;
using Portkey.Utilities;

namespace Portkey.SocialProvider
{
    public class AppleVerifier : SocialVerifierBase
    {
        private IPortkeySocialService _portkeySocialService;

        public AppleVerifier(ISocialLogin socialLogin, IPortkeySocialService portkeySocialService) : base(socialLogin)
        {
            _portkeySocialService = portkeySocialService;
        }

        protected override void VerifyToken(VerifyAccessTokenParam param, AuthCallback successCallback, ErrorCallback errorCallback)
        {
            var verifyAppleParam = new VerifyAppleTokenParams
            {
                accessToken = param.accessToken,
                chainId = param.chainId,
                verifierId = param.verifierId
            };
            StaticCoroutine.StartCoroutine(_portkeySocialService.VerifyAppleToken(verifyAppleParam,
                (verificationResult) =>
                {
                    var verificationDoc = ProcessVerificationDoc(verificationResult.verificationDoc, param.verifierId);
                    //TODO: set guardian list
                    successCallback(verificationDoc, param.accessToken, verificationResult);
                }, errorCallback));
        }
    }
}