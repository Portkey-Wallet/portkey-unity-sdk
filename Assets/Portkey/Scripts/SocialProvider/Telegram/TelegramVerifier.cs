using Portkey.Core;
using Portkey.Utilities;

namespace Portkey.SocialProvider
{
    public class TelegramVerifier : SocialVerifierBase
    {
        private IPortkeySocialService _portkeySocialService;
        
        public TelegramVerifier(ISocialLogin socialLogin, IPortkeySocialService portkeySocialService) : base(socialLogin)
        {
            _portkeySocialService = portkeySocialService;
        }

        protected override void VerifyToken(VerifyAccessTokenParam param, AuthCallback successCallback, ErrorCallback errorCallback)
        {
            var verifyTelegramParam = Convert(param);
            StaticCoroutine.StartCoroutine(_portkeySocialService.VerifyTelegramToken(verifyTelegramParam, (verificationResult) =>
            {
                var verifyCodeResult = new VerifyCodeResult(verificationResult, verifyTelegramParam.verifierId);
                //TODO: set guardian list
                successCallback(verifyCodeResult, param.accessToken);
            }, errorCallback));
        }
    }
}