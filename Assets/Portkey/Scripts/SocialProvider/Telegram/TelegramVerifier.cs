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
            var verifyTelegramParam = ConvertToken(param);
            StaticCoroutine.StartCoroutine(_portkeySocialService.VerifyTelegramToken(verifyTelegramParam, (verificationResult) =>
            {
                Debugger.Log($"receive call back {verificationResult}");
                var verifyCodeResult = VerifyDoc(verifyTelegramParam, verificationResult);
                //TODO: set guardian list
                successCallback(verifyCodeResult, param.accessToken);
            }, errorCallback));
        }
    }
}