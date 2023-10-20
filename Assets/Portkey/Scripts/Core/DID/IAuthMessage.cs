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
        delegate void OnPendingVerificationCodeInputHandler();
        delegate void OnResendVerificationCodeCompleteHandler();
        delegate void OnLoadingHandler(bool show, string message);
        delegate void OnErrorHandler(string error);
        delegate void OnChainIdChangedHandler(string chainId);
        delegate void OnVerifierServerSelectedHandler(string guardianId, AccountType accountType, string verifierServerName);
        delegate void OnLogoutHandler(LogoutMessage logoutMessage);
        
        event OnPendingVerificationCodeInputHandler OnPendingVerificationCodeInputEvent;
        event OnResendVerificationCodeCompleteHandler OnResendVerificationCodeCompleteEvent;
        event OnLoadingHandler OnLoadingEvent;
        event OnErrorHandler OnErrorEvent;
        event OnChainIdChangedHandler OnChainIdChangedEvent;
        event OnVerifierServerSelectedHandler OnVerifierServerSelectedEvent;
        event OnLogoutHandler OnLogoutEvent;
        
        string ChainId { get; set; }
        void InputVerificationCode(string code);
        void CancelCodeVerification();
        void ResendVerificationCode();
        void Loading(bool show, string message = null);
        void Error(string error);
        void ConfirmSendCode();
        void CancelLoginWithQRCode();
        void CancelLoginWithPortkeyApp();
    }
}