using System;
using UnityEngine;

namespace Portkey.Core
{
    /// <summary>
    /// Portkey configuration object. Contains only config data.
    /// </summary>
    [CreateAssetMenu(fileName = "PortkeyConfig", menuName = "Portkey/PortkeyConfig", order = 1)]
    public class PortkeyConfig : ScriptableObject
    {
        [Serializable]
        public class ChainInfo
        {
            public string chainId;
            public string rpcUrl;
            public ContractInfo[] contracts;
        }
        
        [Serializable]
        public class ContractInfo
        {
            public string contractName;
            public string contractAddress;
        }
            
        public string apiBaseUrl;
        public ChainInfo[] chainInfos;
    }
}