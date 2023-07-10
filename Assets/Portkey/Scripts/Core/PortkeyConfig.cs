using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

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
            public Dictionary<string, ContractInfo> ContractInfos { get; private set; }
            public void OnBeforeSerialize()
            {
            }

            public void OnAfterDeserialize()
            {
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
        private string _apiBaseUrl;
        [SerializeField]
        private ChainInfo[] _chainInfos;
        
        public Dictionary<string, ChainInfo> ChainInfos { get; private set; }
        public string ApiBaseUrl => _apiBaseUrl;

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            ChainInfos = _chainInfos?.ToDictionary(chainInfo => chainInfo.ChainId, chainInfo => chainInfo);
        }
    }
}