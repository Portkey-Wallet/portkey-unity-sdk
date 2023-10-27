using System;

namespace Portkey.Core
{
    public interface IBrowserWalletExtension
    {
        void Connect(SuccessCallback<DIDAccountInfo> successCallback, Action OnDisconnected, ErrorCallback errorCallback);
    }
}
