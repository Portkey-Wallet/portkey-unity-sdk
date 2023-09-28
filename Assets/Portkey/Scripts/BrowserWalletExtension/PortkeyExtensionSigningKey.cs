using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AElf;
using AElf.Types;
using Google.Protobuf;
using Portkey.Core;
using Portkey.SocialProvider;
using UnityEngine;

namespace Portkey.BrowserWalletExtension
{
    public class PortkeyExtensionSigningKey : ISigningKey
    {
        private string _signature = null;
        
        [DllImport("__Internal")]
        private static extern void SignMessage(string messageInHex);
        
        public async Task<Transaction> SignTransaction(Transaction transaction)
        {
            var hex = transaction.GetHash().ToByteArray().ToHex();

            _signature = null;
            Listen();
            
            SignMessage(hex);
            
            var signature = await GetSignature();

            transaction.Signature = ByteString.CopyFrom(signature.GetBytes());
            return transaction;
        }

        public byte[] Sign(string data)
        {
            throw new System.NotImplementedException();
        }

        public byte[] Encrypt(string password)
        {
            throw new System.NotImplementedException();
        }

        public string Address => null;
        public string PublicKey => null;
        
        private void Listen()
        {
            var gameObject = new GameObject("PortkeyExtensionSignCallback");
            var callbackComponent = gameObject.AddComponent<PortkeyExtensionSignCallback>();
            callbackComponent.OnErrorCallback = OnError;
            callbackComponent.OnSignCallback = OnSign;
            
            void OnSign(string signature)
            {
                _signature = signature;
            }
        
            void OnError(string error)
            {
                _signature = "";
                Debugger.LogError(error);
            }
        }
        
        private async Task<string> GetSignature()
        {
            while (_signature == null)
            {
                await Task.Delay(100);
            }

            return _signature;
        }
    }
}
