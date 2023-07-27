using System;
using Portkey.Contracts.CA;
using Portkey.Core;
using UnityEngine;

namespace Portkey.UI
{
    public class WalletViewController : MonoBehaviour
    {
        [SerializeField] private DID.DID did;

        private DIDWalletInfo walletInfo = null;
        private IContract _contract = null;
        
        public DIDWalletInfo WalletInfo
        {
            set => walletInfo = value;
        }

        private void Start()
        {
        }

        private void OnError(string error)
        {
            Debugger.LogError(error);
        }
    }
}