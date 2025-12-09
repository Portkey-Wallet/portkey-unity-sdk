using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Portkey.Core
{
    /// <summary>
    /// Portkey configuration object. Contains only config data.
    /// </summary>
    [CreateAssetMenu(fileName = "PortkeyConfig", menuName = "Portkey/PortkeyConfig", order = 1)]
    public class PortkeyConfig : ScriptableObject, ISerializationCallbackReceiver
    {
        [Serializable]
        public class ChainInfo : ISerializationCallbackReceiver
        {
            [SerializeField]
            private string chainId;
            [SerializeField]
            private string rpcUrl;
            [SerializeField]
            private ContractInfo[] contractInfos;

            public string ChainId => chainId;
            public string RpcUrl => rpcUrl;
            
            /// <summary>
            /// A getter for the contract infos for contracts that are deployed on this chain.
            /// </summary>
            public Dictionary<string, ContractInfo> ContractInfos { get; private set; }
            public void OnBeforeSerialize()
            {
            }

            public void OnAfterDeserialize()
            {
                // create a runtime dictionary to access the contract infos by contract id since dictionaries are not serializable
                ContractInfos = contractInfos?.ToDictionary(contractInfo => contractInfo.ContractId, contractInfo => contractInfo);
            }
        }
        
        [Serializable]
        public class ContractInfo
        {
            [SerializeField]
            private string contractId;
            [SerializeField]
            private string contractAddress;
            
            public string ContractId => contractId;
            public string ContractAddress => contractAddress;
        }

        [SerializeField]
        private string apiBaseUrl;
        [SerializeField]
        private ChainInfo[] chainInfos;
        
        /// <summary>
        /// A getter for the chain infos.
        /// </summary>
        public Dictionary<string, ChainInfo> ChainInfos { get; private set; }
        public string ApiBaseUrl => apiBaseUrl;

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            // create a runtime dictionary to access the chain infos by chain id since dictionaries are not serializable
            ChainInfos = chainInfos?.ToDictionary(chainInfo => chainInfo.ChainId, chainInfo => chainInfo);
        }
    }
}