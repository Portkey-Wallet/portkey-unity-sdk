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
        private CaAddresses _caAddresses = null;
        
#if UNITY_WEBGL
            //&& !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern bool IsPortkeyExtensionExist();
        [DllImport("__Internal")]
        private static extern void Connect();
        [DllImport("__Internal")]
        private static extern void GetManagementAccount();
#endif
        
        public void Connect(SuccessCallback<DIDWalletInfo> successCallback, ErrorCallback errorCallback)
        {
            if (IsPortkeyExtensionExist())
            {
                _caAddresses = null;
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
            var gameObject = new GameObject("PortkeyExtensionConnectCallback");
            var callbackComponent = gameObject.AddComponent<PortkeyExtensionConnectCallback>();
            callbackComponent.OnErrorCallback = OnError;
            callbackComponent.OnConnectCallback = OnConnect;
            callbackComponent.OnGetManagementAccountCallback = OnGetManagementAccount;
            
            void OnConnect(string data)
            {
                try
                {
                    _caAddresses = JsonConvert.DeserializeObject<CaAddresses>(data);
                }
                catch (Exception e)
                {
                    Debugger.LogException(e);
                    callbackComponent.OnError(e.Message);
                    return;
                }

                GetManagementAccount();
            }
            
            void OnGetManagementAccount(string data)
            {
                var walletInfo = new DIDWalletInfo
                {
                    caInfo = new CAInfo
                    {
                        caAddress = _caAddresses.AELF[0],
                        caHash = null
                    },
                    chainId = "AELF",
                    managerInfo = null,
                    wallet = new PortkeyExtensionSigningKey()
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