using System.Collections;
using System.Collections.Generic;
using Portkey.Core;
using Portkey.Utilities;

namespace Portkey.SocialProvider
{
    public abstract class VerifyCodeLoginBase : IVerifyCodeLogin
    {
        private IPortkeySocialService _portkeySocialService;
        private string _verifierSessionId = null;
        
        public abstract AccountType AccountType { get; }
        public string VerifierId { get; private set; } = null;

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
                },
                headers = new Dictionary<string, string>
                {
                    {"version", "1.3.5"},
                    {"Recaptchatoken", param.captchaToken ?? ""}
                }
            };
            yield return _portkeySocialService.GetVerificationCode(sendCodeParams, (response) =>
            {
                _verifierSessionId = response.verifierSessionId;
                VerifierId = sendCodeParams.body.verifierId;
                
                successCallback?.Invoke(param.guardianId);
            }, errorCallback);
        }

        public IEnumerator VerifyCode(ICodeCredential credential, OperationTypeEnum operationType, SuccessCallback<VerifyCodeResult> successCallback, ErrorCallback errorCallback)
        {
            var verifyCodeParams = new VerifyVerificationCodeParams
            {
                verifierSessionId = _verifierSessionId,
                verificationCode = credential.SignInToken,
                guardianIdentifier = credential.SocialInfo.sub,
                verifierId = credential.VerifierId,
                chainId = credential.ChainId,
                operationType = operationType
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