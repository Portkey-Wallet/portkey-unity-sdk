using Portkey.Core;

namespace Portkey.DID
{
    public partial class AuthService : IInternalAuthMessage
    {
        private string _chainId;
        
        public event IAuthMessage.OnPendingVerificationCodeInputHandler OnPendingVerificationCodeInputEvent;
        public event IAuthMessage.OnResendVerificationCodeCompleteHandler OnResendVerificationCodeCompleteEvent;
        public event IAuthMessage.OnLoadingHandler OnLoadingEvent;
        public event IAuthMessage.OnErrorHandler OnErrorEvent;
        public event IAuthMessage.OnChainIdChangedHandler OnChainIdChangedEvent;
        public event IAuthMessage.OnVerifierServerSelectedHandler OnVerifierServerSelectedEvent;
        public event IAuthMessage.OnLogoutHandler OnLogoutEvent;
        
        public event IInternalAuthMessage.OnCancelCodeVerificationHandler OnCancelCodeVerificationEvent;
        public event IInternalAuthMessage.OnInputVerificationCodeHandler OnInputVerificationCodeEvent;
        public event IInternalAuthMessage.OnResendVerificationCodeHandler OnResendVerificationCodeEvent;
        public event IInternalAuthMessage.OnConfirmSendCodeHandler OnConfirmSendCodeEvent;
        public event IInternalAuthMessage.OnCancelLoginWithQRCodeHandler OnCancelLoginWithQRCodeEvent;
        public event IInternalAuthMessage.OnCancelLoginWithPortkeyAppHandler OnCancelLoginWithPortkeyAppEvent;


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

        public void VerifierServerSelected(string guardianId, AccountType accountType, Verifier verifier)
        {
            OnVerifierServerSelectedEvent?.Invoke(guardianId, accountType, verifier);
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

        private void OnLogout(LogoutMessage logoutMessage)
        {
            OnLogoutEvent?.Invoke(logoutMessage);
        }
    }
}