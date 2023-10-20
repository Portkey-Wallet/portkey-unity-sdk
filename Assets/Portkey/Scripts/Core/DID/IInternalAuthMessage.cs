namespace Portkey.Core
{
    
    public interface IInternalAuthMessage : IAuthMessage
    {
        delegate void OnInputVerificationCodeHandler(string code);
        delegate void OnCancelCodeVerificationHandler();
        delegate void OnResendVerificationCodeHandler();
        delegate void OnConfirmSendCodeHandler();
        delegate void OnCancelLoginWithQRCodeHandler();
        delegate void OnCancelLoginWithPortkeyAppHandler();
        
        event OnInputVerificationCodeHandler OnInputVerificationCodeEvent;
        event OnCancelCodeVerificationHandler OnCancelCodeVerificationEvent;
        event OnResendVerificationCodeHandler OnResendVerificationCodeEvent;
        event OnConfirmSendCodeHandler OnConfirmSendCodeEvent;
        event OnCancelLoginWithQRCodeHandler OnCancelLoginWithQRCodeEvent;
        event OnCancelLoginWithPortkeyAppHandler OnCancelLoginWithPortkeyAppEvent;
        
        void PendingVerificationCodeInput();
        void ResendVerificationCodeComplete();
        void VerifierServerSelected(string guardianId, AccountType accountType, string verifierServerName);
    }
}