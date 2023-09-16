namespace Portkey.Core
{
    public interface IAuthMessage
    {
        delegate void OnInputVerificationCodeHandler(string code);
        delegate void OnCancelCodeVerificationHandler();
        delegate void OnPendingVerificationCodeInputHandler();
        delegate void OnResendVerificationCodeHandler();
        delegate void OnResendVerificationCodeCompleteHandler();
        delegate void OnLoadingHandler(string message);
        delegate void OnErrorHandler(string error);
        delegate void OnChainIdChangedHandler(string chainId);
        delegate void OnVerifierServerSelectedHandler(string guardianId, AccountType accountType, string verifierServerName);
        delegate void OnConfirmSendCodeHandler();
        
        event OnInputVerificationCodeHandler OnInputVerificationCodeEvent;
        event OnCancelCodeVerificationHandler OnCancelCodeVerificationEvent;
        event OnPendingVerificationCodeInputHandler OnPendingVerificationCodeInputEvent;
        event OnResendVerificationCodeHandler OnResendVerificationCodeEvent;
        event OnResendVerificationCodeCompleteHandler OnResendVerificationCodeCompleteEvent;
        event OnLoadingHandler OnLoadingEvent;
        event OnErrorHandler OnErrorEvent;
        event OnChainIdChangedHandler OnChainIdChangedEvent;
        event OnVerifierServerSelectedHandler OnVerifierServerSelectedEvent;
        event OnConfirmSendCodeHandler OnConfirmSendCodeEvent;

        string ChainId { get; set; }
        void InputVerificationCode(string code);
        void CancelCodeVerification();
        void PendingVerificationCodeInput();
        void ResendVerificationCode();
        void ResendVerificationCodeComplete();
        void Loading(string message);
        void Error(string error);
        void VerifierServerSelected(string guardianId, AccountType accountType, string verifierServerName);
        void ConfirmSendCode();
    }
}