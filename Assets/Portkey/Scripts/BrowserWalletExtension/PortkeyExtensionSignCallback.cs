using System;
using Portkey.Core;
using Newtonsoft.Json;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class PortkeyExtensionSignCallback : MonoBehaviour, IBrowserWalletExtensionSignCallback
    {
        private class Signature
        {
            public string r;
            public string s;
            public int recoveryParam;
        }
        
        public Action<string> OnSignCallback { get; set; }
        public Action<string> OnErrorCallback { get; set; }

        public void OnSign(string signatureJson)
        {
            var signatureObj = JsonConvert.DeserializeObject<Signature>(signatureJson);
            var signature = signatureObj.r + signatureObj.s + signatureObj.recoveryParam.ToString("x2");
            
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