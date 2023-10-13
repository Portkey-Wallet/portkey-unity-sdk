using System;
using Portkey.Core;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class PortkeyExtensionConnectCallback : MonoBehaviour, IBrowserWalletExtensionConnectCallback
    {
        public Action<string> OnConnectCallback { get; set; }
        public Action<string> OnGetManagementAccountAddressCallback { get; set; }
        public Action<string> OnErrorCallback { get; set; }
        
        public void OnConnect(string data)
        {
            Debugger.Log($"PortkeyExtensionCallback OnConnect {data}");
            OnConnectCallback?.Invoke(data);
        }

        public void OnGetManagementAccountAddress(string data)
        {
            Debugger.Log($"PortkeyExtensionCallback OnGetManagementAccountAddress {data}");
            OnGetManagementAccountAddressCallback?.Invoke(data);
            Destroy(gameObject);
        }

        public void OnError(string error)
        {
            OnErrorCallback?.Invoke(error);
            Destroy(gameObject);
        }
    }
}