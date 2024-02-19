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
            var verifyAppleParam = Convert(param);
            StaticCoroutine.StartCoroutine(_portkeySocialService.VerifyAppleToken(verifyAppleParam,
                (verificationResult) =>
                {
                    var verifyCodeResult = CreateVerifyCodeResult(verifyAppleParam, verificationResult);
                    //TODO: set guardian list
                    successCallback(verifyCodeResult, param.accessToken);
                }, errorCallback));
        }
    }
}