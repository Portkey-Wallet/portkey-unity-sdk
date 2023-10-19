using Portkey.Core;

namespace Portkey.DID
{
    public class AuthMessage : IAuthMessage
    {
        private string _chainId;
        
        public event IAuthMessage.OnCancelCodeVerificationHandler OnCancelCodeVerificationEvent;
        public event IAuthMessage.OnInputVerificationCodeHandler OnInputVerificationCodeEvent;
        public event IAuthMessage.OnPendingVerificationCodeInputHandler OnPendingVerificationCodeInputEvent;
        public event IAuthMessage.OnResendVerificationCodeHandler OnResendVerificationCodeEvent;
        public event IAuthMessage.OnResendVerificationCodeCompleteHandler OnResendVerificationCodeCompleteEvent;
        public event IAuthMessage.OnLoadingHandler OnLoadingEvent;
        public event IAuthMessage.OnErrorHandler OnErrorEvent;
        public event IAuthMessage.OnChainIdChangedHandler OnChainIdChangedEvent;
        public event IAuthMessage.OnVerifierServerSelectedHandler OnVerifierServerSelectedEvent;
        public event IAuthMessage.OnConfirmSendCodeHandler OnConfirmSendCodeEvent;
        public event IAuthMessage.OnCancelLoginWithQRCodeHandler OnCancelLoginWithQRCodeEvent;
        public event IAuthMessage.OnCancelLoginWithPortkeyAppHandler OnCancelLoginWithPortkeyAppEvent;
        public event IAuthMessage.OnLogoutHandler OnLogoutEvent;


        public string ChainId
        {
            get => _chainId;
            set
            {
                if (_chainId != value)
                {
                    _chainId = value;
                    OnChainIdChangedEvent?.Invoke(_chainId);
                }
            }
        }

        public void InputVerificationCode(string code)
        {
            OnInputVerificationCodeEvent?.Invoke(code);
        }

        public void CancelCodeVerification()
        {
            OnCancelCodeVerificationEvent?.Invoke();
        }

        public void PendingVerificationCodeInput()
        {
            OnPendingVerificationCodeInputEvent?.Invoke();
        }

        public void ResendVerificationCode()
        {
            OnResendVerificationCodeEvent?.Invoke();
        }

        public void ResendVerificationCodeComplete()
        {
            OnResendVerificationCodeCompleteEvent?.Invoke();
        }

        public void Loading(bool show, string message)
        {
            OnLoadingEvent?.Invoke(show, message);
        }

        public void Error(string error)
        {
            Loading(false, "");
            OnErrorEvent?.Invoke(error);
        }

        public void VerifierServerSelected(string guardianId, AccountType accountType, string verifierServerName)
        {
            OnVerifierServerSelectedEvent?.Invoke(guardianId, accountType, verifierServerName);
        }

        public void ConfirmSendCode()
        {
            OnConfirmSendCodeEvent?.Invoke();
        }

        public void CancelLoginWithQRCode()
        {
            OnCancelLoginWithQRCodeEvent?.Invoke();
        }

        public void CancelLoginWithPortkeyApp()
        {
            OnCancelLoginWithPortkeyAppEvent?.Invoke();
        }

        public void Logout()
        {
            OnLogoutEvent?.Invoke();
        }
    }
}