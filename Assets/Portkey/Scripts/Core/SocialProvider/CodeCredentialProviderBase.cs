using System;
using System.Collections;
using Portkey.Utilities;

namespace Portkey.Core
{
    public abstract class CodeCredentialProviderBase<T> : ICodeCredentialProvider where T : ICodeCredential
    {
        protected readonly IAuthMessage _message;
        private readonly IVerifyCodeLogin _codeLogin;
        private readonly IVerifierService _verifierService;
        private string _verificationCode = null;
        private bool _cancel = false;
        private bool _sendCodeConfirmation = false;
        private SendCodeParams _sendCodeParams;
        public bool EnableCodeSendConfirmationFlow { get; set; } = false;
        
        public abstract AccountType AccountType { get; }
        
        protected CodeCredentialProviderBase(IVerifyCodeLogin codeLogin, IAuthMessage message, IVerifierService verifierService)
        {
            _message = message;
            _codeLogin = codeLogin;
            _verifierService = verifierService;
        }
        
        private void OnInputVerificationCode(string code)
        {
            _verificationCode = code;
        }
        
        private void OnCancelCodeVerification()
        {
            _cancel = true;
        }
        
        private void OnConfirmSendCode()
        {
            _sendCodeConfirmation = true;
        }
        
        private void OnResendVerificationCode()
        {
            StaticCoroutine.StartCoroutine(_codeLogin.SendCode(_sendCodeParams, session =>
            {
                _message.ResendVerificationCodeComplete();
            }, _message.Error));
        }
        
        public IEnumerator Verify(ICredential credential, SuccessCallback<VerifiedCredential> successCallback)
        {
            if(credential is not ICodeCredential codeCredential)
            {
                throw new Exception("Invalid credential type!");
            }
            
            yield return _codeLogin.VerifyCode(codeCredential, result =>
            {
                successCallback?.Invoke(new VerifiedCredential(codeCredential, result.verificationDoc, result.signature));
            }, _message.Error);
        }
        
        protected abstract T CreateCredential(string guardianId, string verifierId, string chainId, string code);

        protected IEnumerator GetCredential(string guardianId, SuccessCallback<T> successCallback, string chainId, string verifierId, OperationTypeEnum operationType)
        {
            Initialize();
            
            _sendCodeParams = new SendCodeParams
            {
                guardianId = guardianId,
                chainId = chainId,
                operationType = operationType
            };
            
            if (verifierId != null)
            {
                _sendCodeParams.verifierId = verifierId;
                SendCode(_sendCodeParams, successCallback);
                yield break;
            }

            yield return _verifierService.GetVerifierServer(chainId, verifierServer =>
            {
                _sendCodeParams.verifierId = verifierServer.id;

                if (EnableCodeSendConfirmationFlow)
                {
                    _message.VerifierServerSelected(guardianId, AccountType, verifierServer.name);

                    StaticCoroutine.StartCoroutine(WaitForSendCodeConfirmation(() =>
                    {
                        SendCode(_sendCodeParams, successCallback);
                    }));
                }
                else
                {
                    SendCode(_sendCodeParams, successCallback);
                }
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
                CleanUp();
                
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
                CleanUp();
                yield break;
            }

            var ret = new string(_verificationCode);
            _verificationCode = null;
            onComplete?.Invoke(ret);
        }
        
        private IEnumerator WaitForSendCodeConfirmation(Action onConfirm)
        {
            while (!_sendCodeConfirmation && !_cancel)
            {
                yield return null;
            }
            
            _sendCodeConfirmation = false;
            
            if (_cancel)
            {
                _cancel = false;
                CleanUp();
                yield break;
            }
            
            onConfirm?.Invoke();
        }

        private void Initialize()
        {
            _message.OnInputVerificationCodeEvent += OnInputVerificationCode;
            _message.OnCancelCodeVerificationEvent += OnCancelCodeVerification;
            _message.OnConfirmSendCodeEvent += OnConfirmSendCode;
            _message.OnResendVerificationCodeEvent += OnResendVerificationCode;
        }

        private void CleanUp()
        {
            _message.OnInputVerificationCodeEvent -= OnInputVerificationCode;
            _message.OnCancelCodeVerificationEvent -= OnCancelCodeVerification;
            _message.OnConfirmSendCodeEvent -= OnConfirmSendCode;
            _message.OnResendVerificationCodeEvent -= OnResendVerificationCode;
        }
    }
}