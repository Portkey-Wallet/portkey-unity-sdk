namespace Portkey.Core
{
    public interface IBrowserWalletExtension
    {
        void Connect(SuccessCallback<DIDWalletInfo> successCallback, ErrorCallback errorCallback);
    }
}
