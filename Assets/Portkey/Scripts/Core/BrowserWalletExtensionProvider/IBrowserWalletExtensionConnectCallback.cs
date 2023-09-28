using System;

namespace Portkey.Core
{
    public interface IBrowserWalletExtensionConnectCallback
    {
        Action<string> OnConnectCallback { get; set; }
        Action<string> OnErrorCallback { get; set; }
        void OnConnect(string data);
        void OnError(string error);
    }
}