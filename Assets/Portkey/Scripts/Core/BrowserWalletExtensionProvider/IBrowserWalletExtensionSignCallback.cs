using System;

namespace Portkey.Core
{
    public interface IBrowserWalletExtensionSignCallback
    {
        Action<string> OnSignCallback { get; set; }
        Action<string> OnErrorCallback { get; set; }
        void OnSign(string signature);
        void OnError(string error);
    }
}