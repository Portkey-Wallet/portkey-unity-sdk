namespace Portkey.Core
{
    public interface IBiometric
    {
        public delegate void SuccessCallback(bool result);
        
        void Authenticate(SuccessCallback onSuccess, ErrorCallback onError);
    }
}