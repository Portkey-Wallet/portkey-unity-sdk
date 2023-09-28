using System;
using Portkey.Core;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class PortkeyExtensionSignCallback : MonoBehaviour, IBrowserWalletExtensionSignCallback
    {
        public Action<string> OnSignCallback { get; set; }
        public Action<string> OnErrorCallback { get; set; }

        public void OnSign(string signature)
        {
            Debugger.Log($"PortkeyExtensionSignCallback OnSign {signature}");
            OnSignCallback?.Invoke(signature);
            Destroy(gameObject);
        }

        public void OnError(string error)
        {
            OnErrorCallback?.Invoke(error);
            Destroy(gameObject);
        }
    }
}