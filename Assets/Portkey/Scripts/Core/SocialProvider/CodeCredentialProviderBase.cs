using System;
using System.Collections;
using Portkey.Utilities;

namespace Portkey.Core
{
    public abstract class CodeCredentialProviderBase<T> where T : ICodeCredential
    {
        protected readonly IAuthMessage _message;
        private readonly IVerifyCodeLogin _codeLogin;
        private readonly IVerifierService _verifierService;
        private string _verificationCode = null;
        private bool _cancel = false;
        
        protected CodeCredentialProviderBase(IVerifyCodeLogin codeLogin, IAuthMessage message, IVerifierService verifierService)
        {
            _message = message;
            _codeLogin = codeLogin;
            _verifierService = verifierService;
            
            _message.OnInputVerificationCodeEvent += OnInputVerificationCode;
            _message.OnCancelVerificationCodeInputEvent += OnCancelVerificationCodeInput;
        }
        
        private void OnInputVerificationCode(string code)
        {
            _verificationCode = code;
        }
        
        private void OnCancelVerificationCodeInput()
        {
            _cancel = true;
        }
        
        public IEnumerator Verify(T credential, SuccessCallback<VerifiedCredential> successCallback)
        {
            yield return _codeLogin.VerifyCode(credential, result =>
            {
                successCallback?.Invoke(new VerifiedCredential(credential, result.verificationDoc, result.signature));
            }, _message.Error);
        }
        
        protected abstract T CreateCredential(string guardianId, string verifierId, string chainId, string code);

        protected IEnumerator GetCredential(string guardianId, SuccessCallback<T> successCallback, string chainId, string verifierId, OperationTypeEnum operationType)
        {
            var param = new SendCodeParams
            {
                guardianId = guardianId,
                chainId = chainId,
                operationType = operationType
            };
            
            if (verifierId != null)
            {
                param.verifierId = verifierId;
                SendCode(param, successCallback);
                yield break;
            }

            yield return _verifierService.GetVerifierServer(chainId, verifierServer =>
            {
                param.verifierId = verifierServer.id;
                SendCode(param, successCallback);
            }, _message.Error);
        }
        
        private void SendCode(SendCodeParams param, SuccessCallback<T> successCallback)
        {
            StaticCoroutine.StartCoroutine(_codeLogin.SendCode(param, session =>
            {
                PendingVerificationCodeInput(param.guardianId, param.chainId, param.verifierId, successCallback);
            }, _message.Error));
        }

        private void PendingVerificationCodeInput(string guardianId, string chainId, string verifierId, SuccessCallback<T> successCallback)
        {
            _message.PendingVerificationCodeInput();

            StaticCoroutine.StartCoroutine(WaitForInputCode((code) =>
            {
                var newCredential = CreateCredential(guardianId, code, chainId, verifierId);
                successCallback(newCredential);
            }));
        }

        private IEnumerator WaitForInputCode(Action<string> onComplete)
        {
            while (_verificationCode == null && !_cancel)
            {
                yield return null;
            }

            if (_cancel)
            {
                _cancel = false;
                _verificationCode = null;
                _message.Error("Verification code input cancelled!");
                yield break;
            }

            var ret = new string(_verificationCode);
            _verificationCode = null;
            onComplete?.Invoke(ret);
        }
    }
}