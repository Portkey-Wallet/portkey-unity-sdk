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
        private static readonly string CHROME_PORTKEY_DOWNLOAD_URL = "https://chrome.google.com/webstore/detail/portkey-did-crypto-nft/hpjiiechbbhefmpggegmahejiiphbmij";
        private static readonly string OTHERS_PORTKEY_DOWNLOAD_URL = "https://portkey.finance/download";
        private PortkeyExtensionConnectCallback _callbackObject = null;
        
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern bool IsPortkeyExtensionExist();
        [DllImport("__Internal")]
        private static extern void Connect();
        [DllImport("__Internal")]
        private static extern void GetCurrentManagerAddress();
        [DllImport("__Internal")]
        private static extern string GetBrowserVersion();
#endif
        
        public void Connect(SuccessCallback<DIDAccountInfo> successCallback, Action OnDisconnected, ErrorCallback errorCallback)
        {
            if (_callbackObject != null)
            {
                _callbackObject.Destroy();
                _callbackObject = null;
            }
            
#if UNITY_WEBGL && !UNITY_EDITOR
            if (IsPortkeyExtensionExist())
            {
                Listen(successCallback, OnDisconnected, errorCallback);
                Connect();
                return;
            }
            
            var browserVersion = GetBrowserVersion();
            if (browserVersion.Contains("Chrome"))
            {
                Application.OpenURL(CHROME_PORTKEY_DOWNLOAD_URL);
            }
            else
            {
                Application.OpenURL(OTHERS_PORTKEY_DOWNLOAD_URL);
            }
#endif
        }

        private void Listen(SuccessCallback<DIDAccountInfo> successCallback, Action OnDisconnected, ErrorCallback errorCallback)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            CaAddresses caAddresses = null;
            
            var gameObject = new GameObject("PortkeyExtensionConnectCallback");
            _callbackObject = gameObject.AddComponent<PortkeyExtensionConnectCallback>();
            _callbackObject.OnErrorCallback = OnError;
            _callbackObject.OnConnectCallback = OnConnect;
            _callbackObject.OnDisconnectedCallback = OnDisconnected;
            _callbackObject.OnGetManagementAccountAddressCallback = OnGetManagementAccountAddress;
            
            void OnConnect(string data)
            {
                try
                {
                    caAddresses = JsonConvert.DeserializeObject<CaAddresses>(data);
                }
                catch (Exception e)
                {
                    Debugger.LogException(e);
                    _callbackObject.OnError(e.Message);
                    return;
                }
                
                GetCurrentManagerAddress();
            }

            void OnGetManagementAccountAddress(string address)
            {
                if (address == null)
                {
                    _callbackObject.OnError("Get management account address failed!");
                    return;
                }
                if(caAddresses?.AELF.Count == 0)
                {
                    _callbackObject.OnError("Connecting to Portkey Extension failed!");
                    return;
                }

                var accountInfo = new DIDAccountInfo("AELF", null, 0, new CAInfo
                {
                    caAddress = caAddresses.AELF[0],
                    caHash = null
                }, null, AddManagerType.Recovery, new PortkeyExtensionSigningKey(address));
                
                successCallback?.Invoke(accountInfo);
            }

            void OnError(string error)
            {
                Debugger.LogError(error);
                errorCallback?.Invoke(error);
            }
#endif
        }
    }
}