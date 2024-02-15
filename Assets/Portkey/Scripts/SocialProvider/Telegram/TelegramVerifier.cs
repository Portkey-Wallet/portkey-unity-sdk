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
            Debugger.Log($"verify token here {param.accessToken}{param.operationType}{param.verifierId}");
            var verifyTelegramParam = new VerifyTokenParams
            {
                accessToken = param.accessToken,
                chainId = param.chainId,
                verifierId = param.verifierId,
                operationType = param.operationType
            };
            StaticCoroutine.StartCoroutine(_portkeySocialService.VerifyTelegramToken(verifyTelegramParam, (verificationResult) =>
            {
                Debugger.Log($"receive call back {verificationResult}");
                var verificationDoc = LoginHelper.ProcessVerificationDoc(verificationResult.verificationDoc, verifyTelegramParam.verifierId);
                var verifyCodeResult = new VerifyCodeResult
                {
                    verificationDoc = verificationDoc,
                    signature = verificationResult.signature
                };
                //TODO: set guardian list
                Debugger.Log("callback begin");
                successCallback(verifyCodeResult, param.accessToken);
                Debugger.Log("callback end");
            }, errorCallback));
        }
    }
}