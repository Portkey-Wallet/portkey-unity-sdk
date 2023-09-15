namespace Portkey.Core
{
    public interface IAuthMessage
    {
        delegate void OnCancelVerificationCodeInputHandler();
        delegate void OnInputVerificationCodeHandler(string code);
        delegate void OnPendingVerificationCodeInputHandler();
        delegate void OnLoadingHandler(string message);
        delegate void OnErrorHandler(string error);
        delegate void OnChainIdChangedHandler(string chainId);
        
        event OnCancelVerificationCodeInputHandler OnCancelVerificationCodeInputEvent;
        event OnInputVerificationCodeHandler OnInputVerificationCodeEvent;
        event OnPendingVerificationCodeInputHandler OnPendingVerificationCodeInputEvent;
        event OnLoadingHandler OnLoadingEvent;
        event OnErrorHandler OnErrorEvent;
        event OnChainIdChangedHandler OnChainIdChangedEvent;

        string ChainId { get; set; }
        void InputVerificationCode(string code);
        void CancelVerificationCodeInput();
        void PendingVerificationCodeInput();
        void Loading(string message);
        void Error(string error);
    }
}