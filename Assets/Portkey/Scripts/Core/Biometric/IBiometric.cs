namespace Portkey.Core
{
    public interface IBiometric
    {
        public class BiometricPromptInfo
        {
            public string title;
            public string subtitle;
            public string description;
            public string negativeButtonText;
        }
        
        public delegate void SuccessCallback(bool result);
        
        void Authenticate(BiometricPromptInfo info, SuccessCallback onSuccess, ErrorCallback onError);
    }
}