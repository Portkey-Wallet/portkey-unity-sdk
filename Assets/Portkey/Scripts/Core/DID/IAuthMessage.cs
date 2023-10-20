namespace Portkey.Core
{
    public enum LogoutMessage
    {
        Logout,
        PortkeyExtensionLogout,
        Error
    }
    
    public interface IAuthMessage
    {
        delegate void OnInputVerificationCodeHandler(string code);
        delegate void OnCancelCodeVerificationHandler();
        delegate void OnPendingVerificationCodeInputHandler();
        delegate void OnResendVerificationCodeHandler();
        delegate void OnResendVerificationCodeCompleteHandler();
        delegate void OnLoadingHandler(bool show, string message);
        delegate void OnErrorHandler(string error);
        delegate void OnChainIdChangedHandler(string chainId);
        delegate void OnVerifierServerSelectedHandler(string guardianId, AccountType accountType, string verifierServerName);
        delegate void OnConfirmSendCodeHandler();
        delegate void OnCancelLoginWithQRCodeHandler();
        delegate void OnCancelLoginWithPortkeyAppHandler();
        delegate void OnLogoutHandler(LogoutMessage logoutMessage);
        
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
        event OnCancelLoginWithQRCodeHandler OnCancelLoginWithQRCodeEvent;
        event OnCancelLoginWithPortkeyAppHandler OnCancelLoginWithPortkeyAppEvent;
        event OnLogoutHandler OnLogoutEvent;
        
        string ChainId { get; set; }
        void InputVerificationCode(string code);
        void CancelCodeVerification();
        void PendingVerificationCodeInput();
        void ResendVerificationCode();
        void ResendVerificationCodeComplete();
        void Loading(bool show, string message = null);
        void Error(string error);
        void VerifierServerSelected(string guardianId, AccountType accountType, string verifierServerName);
        void ConfirmSendCode();
        void CancelLoginWithQRCode();
        void CancelLoginWithPortkeyApp();
        void Logout(LogoutMessage logoutMessage);
    }
}