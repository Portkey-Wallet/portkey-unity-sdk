using System.Collections;
using Portkey.Core;
using Portkey.Utilities;

namespace Portkey.SocialProvider
{
    internal class VerificationCodeSession
    {
        public string verifierSessionId = null;
        public SendCodeParams sendCodeParams = null;
    }
    
    public abstract class VerifyCodeLoginBase : IVerifyCodeLogin
    {
        private IPortkeySocialService _portkeySocialService;
        private VerificationCodeSession _verifierCodeSession = new VerificationCodeSession();
        
        public abstract AccountType AccountType { get; }

        protected VerifyCodeLoginBase(IPortkeySocialService portkeySocialService)
        {
            _portkeySocialService = portkeySocialService;
        }
        
        protected abstract bool IsCorrectGuardianIdFormat(string id, out string errormessage);

        public IEnumerator SendCode(SendCodeParams param, SuccessCallback<string> successCallback, ErrorCallback errorCallback)
        {
            if (!IsCorrectGuardianIdFormat(param.guardianId, out var errorMessage))
            {
                errorCallback?.Invoke(errorMessage);
                yield break;
            }

            var sendCodeParams = new SendVerificationCodeRequestParams
            {
                body = new SendVerificationCodeParams
                {
                    guardianIdentifier = param.guardianId.RemoveAllWhiteSpaces(),
                    verifierId = param.verifierId,
                    chainId = param.chainId,
                    type = AccountType.ToString(),
                    operationType = (int)param.operationType
                }
            };
            yield return _portkeySocialService.GetVerificationCode(sendCodeParams, (response) =>
            {
                _verifierCodeSession.verifierSessionId = response.verifierSessionId;
                _verifierCodeSession.sendCodeParams = param;
                
                successCallback?.Invoke(param.guardianId);
            }, errorCallback);
        }

        public IEnumerator VerifyCode(string code, SuccessCallback<VerifyCodeResult> successCallback, ErrorCallback errorCallback)
        {
            var verifyCodeParams = new VerifyVerificationCodeParams
            {
                verifierSessionId = _verifierCodeSession.verifierSessionId,
                verificationCode = code,
                guardianIdentifier = _verifierCodeSession.sendCodeParams.guardianId,
                verifierId = _verifierCodeSession.sendCodeParams.verifierId,
                chainId = _verifierCodeSession.sendCodeParams.chainId
            };
            yield return _portkeySocialService.VerifyVerificationCode(verifyCodeParams, (response) =>
            {
                var verificationDoc = LoginHelper.ProcessVerificationDoc(response.verificationDoc, verifyCodeParams.verifierId);
                var verifyCodeResult = new VerifyCodeResult
                {
                    verificationDoc = verificationDoc,
                    signature = response.signature
                };
                
                successCallback?.Invoke(verifyCodeResult);
            }, errorCallback);
        }
    }
}