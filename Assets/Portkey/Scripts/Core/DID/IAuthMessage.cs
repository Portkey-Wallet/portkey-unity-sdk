namespace Portkey.Core
{
    /// <summary>
    /// Enum for logout message. This tells the user which application has requested for logout.
    /// </summary>
    public enum LogoutMessage
    {
        Logout,
        PortkeyExtensionLogout,
        Error
    }
    
    /// <summary>
    /// An interface for the authentication message. This is used to communicate with the user during authentication.
    /// </summary>
    public interface IAuthMessage
    {
        delegate void OnPendingVerificationCodeInputHandler();
        delegate void OnResendVerificationCodeCompleteHandler();
        delegate void OnLoadingHandler(bool show, string message);
        delegate void OnErrorHandler(string error);
        delegate void OnChainIdChangedHandler(string chainId);
        delegate void OnVerifierServerSelectedHandler(string guardianId, AccountType accountType, Verifier verifier);
        delegate void OnLogoutHandler(LogoutMessage logoutMessage);
        
        /// <summary>
        /// The PendingVerificationCodeInputEvent is invoked to notify the user that they have pending verification and would require user input of the verification code sent.
        /// </summary>
        event OnPendingVerificationCodeInputHandler OnPendingVerificationCodeInputEvent;
        /// <summary>
        /// The OnResendVerificationCodeCompleteEvent is invoked after verification code is sent to the user after calling ResendVerificationCode.
        /// </summary>
        event OnResendVerificationCodeCompleteHandler OnResendVerificationCodeCompleteEvent;
        /// <summary>
        /// Called when the application is loading its authentication operations.
        /// </summary>
        event OnLoadingHandler OnLoadingEvent;
        /// <summary>
        /// Called when an error occurs during authentication.
        /// </summary>
        event OnErrorHandler OnErrorEvent;
        /// <summary>
        /// Called when chain id changes.
        /// </summary>
        event OnChainIdChangedHandler OnChainIdChangedEvent;
        /// <summary>
        /// Used only for Phone/Email Credentials. Called when a verifier server has been selected to verify a credential.
        /// </summary>
        event OnVerifierServerSelectedHandler OnVerifierServerSelectedEvent;
        /// <summary>
        /// Called when logout is requested from the user either from the application itself or from browser extension.
        /// </summary>
        event OnLogoutHandler OnLogoutEvent;
        /// <summary>
        /// Gets or sets the chain id. The user will be logged in or signed up on the said chain id by default. This is a string.
        /// </summary>
        string ChainId { get; set; }

        /// <summary>
        /// Used only for Phone/Email Credentials. Called when program has gotten a verification code from the user. This continues the process for verifying Phone/Email Credentials.
        /// </summary>
        /// <param name="code">The code entered by the user for verification.</param>
        void InputVerificationCode(string code);
        /// <summary>
        /// Used only for Phone/Email Credentials. Confirm sending of verification code. This continues the process for verifying Phone/Email Credentials.
        /// </summary>
        void ConfirmSendCode();
        /// <summary>
        /// Used only for Phone/Email Credentials. Cancels verification of code.
        /// </summary>
        void CancelCodeVerification();
        /// <summary>
        /// Used only for Phone/Email Credentials. Resends the verification code.
        /// </summary>
        void ResendVerificationCode();
        /// <summary>
        /// Called when authentication operation is loading.
        /// </summary>
        /// <param name="show">Whether or not to show the loading</param>
        /// <param name="message">Message to show for loading (optional)</param>
        void Loading(bool show, string message = null);
        /// <summary>
        /// Reports an authentication operation error to the user.
        /// </summary>
        /// <param name="error">The error that occurred.</param>
        void Error(string error);
        /// <summary>
        /// Cancel logging flow when logging in with QR code.
        /// </summary>
        void CancelLoginWithQRCode();
        /// <summary>
        /// Cancel logging flow when logging in with portkey app.
        /// </summary>
        void CancelLoginWithPortkeyApp();
    }
}