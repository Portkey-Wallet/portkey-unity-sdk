using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Portkey.Core;
using Portkey.SocialProvider;
using UnityEngine;

namespace Portkey.BrowserWalletExtension
{
    public class CaAddresses
    {
        public List<string> AELF;
        public List<string> tDVV;
    }
    
    public class PortkeyExtension : IBrowserWalletExtension
    {
#if UNITY_WEBGL
            //&& !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern bool IsPortkeyExtensionExist();
        [DllImport("__Internal")]
        private static extern void Connect();
        [DllImport("__Internal")]
        private static extern void GetCurrentManagerAddress();
#endif
        
        public void Connect(SuccessCallback<DIDWalletInfo> successCallback, ErrorCallback errorCallback)
        {
            if (IsPortkeyExtensionExist())
            {
                Listen(successCallback, errorCallback);
                Connect();
            }
            else
            {
                errorCallback("Portkey extension not found!");
            }
        }

        private void Listen(SuccessCallback<DIDWalletInfo> successCallback, ErrorCallback errorCallback)
        {
            CaAddresses caAddresses = null;
            
            var gameObject = new GameObject("PortkeyExtensionConnectCallback");
            var callbackComponent = gameObject.AddComponent<PortkeyExtensionConnectCallback>();
            callbackComponent.OnErrorCallback = OnError;
            callbackComponent.OnConnectCallback = OnConnect;
            callbackComponent.OnGetManagementAccountAddressCallback = OnGetManagementAccountAddress;
            
            void OnConnect(string data)
            {
                try
                {
                    caAddresses = JsonConvert.DeserializeObject<CaAddresses>(data);
                }
                catch (Exception e)
                {
                    Debugger.LogException(e);
                    callbackComponent.OnError(e.Message);
                    return;
                }
                
                GetCurrentManagerAddress();
            }

            void OnGetManagementAccountAddress(string address)
            {
                if (address == null)
                {
                    callbackComponent.OnError("Get management account address failed!");
                    return;
                }
                if(caAddresses?.AELF.Count == 0)
                {
                    callbackComponent.OnError("Connecting to Portkey Extension failed!");
                    return;
                }
                
                var walletInfo = new DIDWalletInfo
                {
                    caInfo = new CAInfo
                    {
                        caAddress = caAddresses.AELF[0],
                        caHash = null
                    },
                    chainId = "AELF",
                    managerInfo = null,
                    wallet = new PortkeyExtensionSigningKey(address)
                };
                
                successCallback?.Invoke(walletInfo);
            }

            void OnError(string error)
            {
                Debugger.LogError(error);
                errorCallback?.Invoke(error);
            }
        }
    }
}