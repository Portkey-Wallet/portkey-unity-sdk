using Portkey.Core;

namespace Portkey.DID
{
    public class AuthMessage : IAuthMessage
    {
        private string _chainId;
        
        public event IAuthMessage.OnCancelVerificationCodeInputHandler OnCancelVerificationCodeInputEvent;
        public event IAuthMessage.OnInputVerificationCodeHandler OnInputVerificationCodeEvent;
        public event IAuthMessage.OnPendingVerificationCodeInputHandler OnPendingVerificationCodeInputEvent;
        public event IAuthMessage.OnLoadingHandler OnLoadingEvent;
        public event IAuthMessage.OnErrorHandler OnErrorEvent;
        public event IAuthMessage.OnChainIdChangedHandler OnChainIdChangedEvent;

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

        public void CancelVerificationCodeInput()
        {
            OnCancelVerificationCodeInputEvent?.Invoke();
        }

        public void PendingVerificationCodeInput()
        {
            OnPendingVerificationCodeInputEvent?.Invoke();
        }

        public void Loading(string message)
        {
            OnLoadingEvent?.Invoke(message);
        }

        public void Error(string error)
        {
            OnErrorEvent?.Invoke(error);
        }
    }
}