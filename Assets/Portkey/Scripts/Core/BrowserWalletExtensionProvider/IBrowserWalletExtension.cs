using System;

namespace Portkey.Core
{
    public interface IBrowserWalletExtension
    {
        void Connect(SuccessCallback<DIDWalletInfo> successCallback, Action OnDisconnected, ErrorCallback errorCallback);
    }
}
