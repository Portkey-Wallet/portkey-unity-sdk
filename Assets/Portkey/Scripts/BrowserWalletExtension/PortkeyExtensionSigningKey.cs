using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AElf;
using AElf.Types;
using Google.Protobuf;
using Portkey.Core;
using Portkey.SocialProvider;
using Portkey.Utilities;
using UnityEngine;

namespace Portkey.BrowserWalletExtension
{
    public class PortkeyExtensionSigningKey : ISigningKey
    {
        public PortkeyExtensionSigningKey(string address)
        {
            Address = address;
        }

        public byte[] Encrypt(string password)
        {
            Debugger.Log("Portkey Extension does not support encryption.");
            return null;
        }

        public string Address { get; } = null;

        public string PublicKey => null;
        
        private void Listen(SuccessCallback<byte[]> successCallback, ErrorCallback errorCallback)
        {
            var gameObject = new GameObject("PortkeyExtensionSignCallback");
            var callbackComponent = gameObject.AddComponent<PortkeyExtensionSignCallback>();
            callbackComponent.OnErrorCallback = OnError;
            callbackComponent.OnSignCallback = OnSign;
            
            void OnSign(string signature)
            {
                successCallback?.Invoke(signature.HexToBytes());
            }
        
            void OnError(string error)
            {
                errorCallback?.Invoke(error);
            }
        }

        public IEnumerator SignTransaction(Transaction transaction, SuccessCallback<Transaction> successCallback, ErrorCallback errorCallback)
        {
            var messageHash = transaction.GetHash().ToByteArray().ToHex();
            
            yield return Sign(messageHash, signature =>
            {
                transaction.Signature = ByteString.CopyFrom(signature);
                successCallback?.Invoke(transaction);
            }, errorCallback);
        }
        
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void SignMessage(string messageInHex);
#endif
        
        public IEnumerator Sign(string data, SuccessCallback<byte[]> successCallback, ErrorCallback errorCallback)
        {
            Listen(successCallback, errorCallback);
            
#if UNITY_WEBGL && !UNITY_EDITOR
            SignMessage(data);
#endif
            yield break;
        }
    }
}
