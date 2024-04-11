using System;
using System.Collections;
using Portkey.Utilities;
using UnityEngine;

namespace Portkey.Core
{
    public abstract class CodeCredentialProviderBase<TCodeCredential, TCodeIdentifier> : ICodeCredentialProvider where TCodeIdentifier : ICodeIdentifier where TCodeCredential : ICodeCredential
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

        public IEnumerator SendCode(TCodeIdentifier codeIdentifier, SuccessCallback<string> successCallback)
        {
            string chainId = null;
            string verifierId = null;
            
            if (_codeLogin.IsProcessingAccount(codeIdentifier.String, out var processingInfo))
            {
                chainId = processingInfo.chainId;
                verifierId = processingInfo.verifierId;
            }

            yield return SendCode(codeIdentifier.String, successCallback, chainId, verifierId, OperationTypeEnum.register);
        }
        
        public IEnumerator SendCode(Guardian guardian, SuccessCallback<string> successCallback)
        {
            yield return SendCode(guardian.id, successCallback, guardian.chainId, guardian.verifier.id, OperationTypeEnum.communityRecovery);
        }

        private IEnumerator SendCode(string guardianId, SuccessCallback<string> successCallback, string chainId, string verifierId, OperationTypeEnum operationType)
        {
            Initialize();
            
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

            yield return _verifierService.GetVerifierServer(chainId, verifier =>
            {
                _sendCodeParams.verifierId = verifier.id;

                if (EnableCodeSendConfirmationFlow)
                {
                    _message.VerifierServerSelected(guardianId, AccountType, new Verifier(verifier));

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
        
        public IEnumerator Get(TCodeIdentifier codeIdentifier, SuccessCallback<TCodeCredential> successCallback, string chainId = null, string verifierId = null, OperationTypeEnum operationType = OperationTypeEnum.register)
        {
            chainId ??= _message.ChainId;
            yield return GetCredential(codeIdentifier.String, successCallback, chainId, verifierId, operationType);
        }
        
        public ICredential Get(TCodeIdentifier codeIdentifier, string verificationCode)
        {
            if(!_codeLogin.IsProcessingAccount(codeIdentifier.String, out var processingInfo))
            {
                throw new Exception("Please call the corresponding CredentialProvider's SendCode first!");
            }
            if(string.IsNullOrEmpty(verificationCode))
            {
                throw new Exception("Please input verification code!");
            }
            
            return CreateCredential(codeIdentifier.String, processingInfo.verifierId, processingInfo.chainId, verificationCode);
        }

        public IEnumerator Verify(ICredential credential, SuccessCallback<VerifiedCredential> successCallback, OperationTypeEnum operationType = OperationTypeEnum.register)
        {
            if(credential is not ICodeCredential codeCredential)
            {
                throw new Exception("Invalid credential type!");
            }
            
            yield return _codeLogin.VerifyCode(codeCredential, operationType, result =>
            {
                successCallback?.Invoke(new VerifiedCredential(codeCredential, result.VerificationDoc, result.Signature));
            }, _message.Error);
        }
        
        protected abstract TCodeCredential CreateCredential(string guardianId, string verifierId, string chainId, string code);

        protected IEnumerator GetCredential(string guardianId, SuccessCallback<TCodeCredential> successCallback, string chainId, string verifierId, OperationTypeEnum operationType)
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
                    _message.VerifierServerSelected(guardianId, AccountType, new Verifier(verifier));

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
        
        private void ExecuteCaptchaThenSendCode(SuccessCallback<TCodeCredential> successCallback)
        {
            _captcha.ExecuteCaptcha(captchaToken =>
            {
                _sendCodeParams.captchaToken = captchaToken;
                SendCode(_sendCodeParams, successCallback);
            }, _message.Error);
        }

        private void SendCode(SendCodeParams param, SuccessCallback<TCodeCredential> successCallback)
        {
            StaticCoroutine.StartCoroutine(_codeLogin.SendCode(param, session =>
            {
                PendingVerificationCodeInput(param.guardianId, param.chainId, param.verifierId, successCallback);
            }, _message.Error));
        }

        private void PendingVerificationCodeInput(string guardianId, string chainId, string verifierId, SuccessCallback<TCodeCredential> successCallback)
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