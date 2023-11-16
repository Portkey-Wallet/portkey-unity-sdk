using System.Collections;
using System.Collections.Generic;
using Portkey.Core;
using Portkey.Utilities;
using UnityEngine;

namespace Portkey.SocialProvider
{
    internal enum PlatformType
    {
        WEB,
        ANDROID,
        IOS
    }
    
    public abstract class VerifyCodeLoginBase : IVerifyCodeLogin
    {
        private IPortkeySocialService _portkeySocialService;
        private string _verifierSessionId = null;
        
        public abstract AccountType AccountType { get; }
        
        private ProcessingInfo _processingInfo = new();

        protected VerifyCodeLoginBase(IPortkeySocialService portkeySocialService)
        {
            _portkeySocialService = portkeySocialService;
        }
        
        protected abstract bool IsCorrectGuardianIdFormat(string id, out string errormessage);

        public bool IsProcessingAccount(string guardianId, out ProcessingInfo processingInfo)
        {
            var ret = _processingInfo.guardianId == guardianId.RemoveAllWhiteSpaces();
            processingInfo = (ret)? _processingInfo: null;

            return ret;
        }

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
                    operationType = (int)param.operationType,
                    platformType = (int)GetPlatformType()
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
                _processingInfo.guardianId = sendCodeParams.body.guardianIdentifier;
                _processingInfo.verifierId = sendCodeParams.body.verifierId;
                _processingInfo.chainId = sendCodeParams.body.chainId;
                
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

                _processingInfo.guardianId = null;
                
                successCallback?.Invoke(verifyCodeResult);
            }, errorCallback);
        }
        
        internal static PlatformType GetPlatformType()
        {
            var platform = Application.platform;

            var type = platform switch
            {
                RuntimePlatform.Android => PlatformType.ANDROID,
                RuntimePlatform.IPhonePlayer => PlatformType.IOS,
                _ => PlatformType.WEB
            };

            return type;
        }
    }
}