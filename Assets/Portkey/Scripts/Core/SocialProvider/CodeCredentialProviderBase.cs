using System;
using System.Collections;
using Portkey.Core.Captcha;
using Portkey.Utilities;
using UnityEngine;

namespace Portkey.Core
{
    public abstract class CodeCredentialProviderBase<T> : ICodeCredentialProvider where T : ICodeCredential
    {
        protected readonly IInternalAuthMessage _message;
        protected readonly IVerifyCodeLogin _codeLogin;
        private readonly IVerifierService _verifierService;
        private readonly ICaptcha _captcha;
        private string _verificationCode = null;
        private Coroutine _coroutine = null;
        private bool _sendCodeConfirmation = false;
        private SendCodeParams _sendCodeParams;
        public bool EnableCodeSendConfirmationFlow { get; set; } = false;
        
        public abstract AccountType AccountType { get; }
        
        protected CodeCredentialProviderBase(IVerifyCodeLogin codeLogin, IInternalAuthMessage message, IVerifierService verifierService, ICaptcha captcha)
        {
            _message = message;
            _codeLogin = codeLogin;
            _verifierService = verifierService;
            _captcha = captcha;
        }
        
        private void OnInputVerificationCode(string code)
        {
            _verificationCode = code;
        }
        
        private void OnCancelCodeVerification()
        {
            StaticCoroutine.StopCoroutine(_coroutine);
            CleanUp();
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

        public IEnumerator SendCode(string guardianId, SuccessCallback<string> successCallback, string chainId = null, string verifierId = null, OperationTypeEnum operationType = OperationTypeEnum.register)
        {
            chainId ??= _message.ChainId;
            
            _sendCodeParams = new SendCodeParams
            {
                guardianId = guardianId,
                chainId = chainId,
                operationType = operationType
            };
            
            if (verifierId != null)
            {
                _sendCodeParams.verifierId = verifierId;
                ExecuteCaptchaThenSendCode(successCallback);
                yield break;
            }

            yield return _verifierService.GetVerifierServer(chainId, verifierServer =>
            {
                _sendCodeParams.verifierId = verifierServer.id;

                if (EnableCodeSendConfirmationFlow)
                {
                    _message.VerifierServerSelected(guardianId, AccountType, verifierServer);

                    _coroutine = StaticCoroutine.StartCoroutine(WaitForSendCodeConfirmation(() =>
                    {
                        ExecuteCaptchaThenSendCode(successCallback);
                    }));
                }
                else
                {
                    ExecuteCaptchaThenSendCode(successCallback);
                }
            }, _message.Error);
        }

        public IEnumerator Verify(ICredential credential, SuccessCallback<VerifiedCredential> successCallback, OperationTypeEnum operationType = OperationTypeEnum.register)
        {
            if(credential is not ICodeCredential codeCredential)
            {
                throw new Exception("Invalid credential type!");
            }
            
            yield return _codeLogin.VerifyCode(codeCredential, operationType, result =>
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
                ExecuteCaptchaThenSendCode(successCallback);
                yield break;
            }

            yield return _verifierService.GetVerifierServer(chainId, verifier =>
            {
                _sendCodeParams.verifierId = verifier.id;

                if (EnableCodeSendConfirmationFlow)
                {
                    _message.VerifierServerSelected(guardianId, AccountType, verifier);

                    _coroutine = StaticCoroutine.StartCoroutine(WaitForSendCodeConfirmation(() =>
                    {
                        ExecuteCaptchaThenSendCode(successCallback);
                    }));
                }
                else
                {
                    ExecuteCaptchaThenSendCode(successCallback);
                }
            }, _message.Error);
        }

        private void ExecuteCaptchaThenSendCode(SuccessCallback<string> successCallback)
        {
            _captcha.ExecuteCaptcha(captchaToken =>
            {
                _sendCodeParams.captchaToken = captchaToken;
                SendCode(_sendCodeParams, successCallback);
            }, _message.Error);
        }
        
        private void SendCode(SendCodeParams param, SuccessCallback<string> successCallback)
        {
            StaticCoroutine.StartCoroutine(_codeLogin.SendCode(param, successCallback, _message.Error));
        }
        
        private void ExecuteCaptchaThenSendCode(SuccessCallback<T> successCallback)
        {
            _captcha.ExecuteCaptcha(captchaToken =>
            {
                _sendCodeParams.captchaToken = captchaToken;
                SendCode(_sendCodeParams, successCallback);
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

            _coroutine = StaticCoroutine.StartCoroutine(WaitForInputCode((code) =>
            {
                Debugger.LogError("Received verification code!");
                CleanUp();
                
                var newCredential = CreateCredential(guardianId, verifierId, chainId, code);
                successCallback(newCredential);
            }));
        }

        private IEnumerator WaitForInputCode(Action<string> onComplete)
        {
            while (_verificationCode == null)
            {
                yield return null;
            }

            var ret = new string(_verificationCode);
            _verificationCode = null;
            onComplete?.Invoke(ret);
        }
        
        private IEnumerator WaitForSendCodeConfirmation(Action onConfirm)
        {
            while (!_sendCodeConfirmation)
            {
                yield return null;
            }
            
            _sendCodeConfirmation = false;
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
            _coroutine = null;
            _verificationCode = null;
            
            _message.OnInputVerificationCodeEvent -= OnInputVerificationCode;
            _message.OnCancelCodeVerificationEvent -= OnCancelCodeVerification;
            _message.OnConfirmSendCodeEvent -= OnConfirmSendCode;
            _message.OnResendVerificationCodeEvent -= OnResendVerificationCode;
        }
    }
}