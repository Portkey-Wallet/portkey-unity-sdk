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
            [Header("Chain Info")]
            [SerializeField]
            private string chainId;
            [SerializeField]
            private string rpcUrl;
            [Header("Contracts")]
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

        [Header("Portkey Endpoint")]
        [SerializeField]
        private string apiBaseUrl;
        [Header("Blockchain Info")]
        [SerializeField]
        private ChainInfo[] chainInfos;
        
        [Header("Google PC Login")]
        [SerializeField]
        private string googlePCClientId = "931335042992-4cvdlgo4etblfe4t7dk9i1q7oouj2od0.apps.googleusercontent.com";
        [SerializeField]
        private string googlePCClientSecret = "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";
        
        [Header("Google Android Login")]
        [SerializeField]
        private string googleAndroidClientId = "931335042992-ousd4tdbui5n2msmqj94ppp632a27ofv.apps.googleusercontent.com";
        [SerializeField] 
        private string googleAndroidClientSecret = "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";
        
        [Header("Google iOS/WSA Login")]
        [SerializeField]
        private string googleIosClientId = "931335042992-n0aj79qor1t4qekpgbs5ahru9c891ker.apps.googleusercontent.com";
        [SerializeField]
        private string googleIosDotReverseClientId = "com.googleusercontent.apps.931335042992-n0aj79qor1t4qekpgbs5ahru9c891ker";
        [SerializeField]
        private string googleIosProtocol = "portkey.sdk";
        
        [Header("Google WebGL Login")]
        [SerializeField]
        private string googleWebGLClientId = "931335042992-d8jgdbleopnpgjcmbqnf7dqhri93lj2m.apps.googleusercontent.com";
        [SerializeField]
        private string googleWebGLLoginUrl = "https://openlogin.portkey.finance/";
        [SerializeField]
        private string googleWebGLRedirectUri = "https://openlogin.portkey.finance/auth-callback";

        [Header("Approval Settings")]
        [SerializeField] private int minApprovals = 3;
        [SerializeField] private int denominator = 5;

        /// <summary>
        /// A getter for the chain infos.
        /// </summary>
        public Dictionary<string, ChainInfo> ChainInfos { get; private set; }
        public string ApiBaseUrl => apiBaseUrl;
        public string GooglePCClientId => googlePCClientId;
        public string GooglePCClientSecret => googlePCClientSecret;
        public string GoogleAndroidClientId => googleAndroidClientId;
        public string GoogleAndroidClientSecret => googleAndroidClientSecret;
        public string GoogleIOSProtocol => googleIosProtocol;
        public string GoogleIOSClientId => googleIosClientId;
        public string GoogleIOSDotReverseClientId => googleIosDotReverseClientId;
        public string GoogleWebGLClientId => googleWebGLClientId;
        public string GoogleWebGLLoginUrl => googleWebGLLoginUrl;
        public string GoogleWebGLRedirectUri => googleWebGLRedirectUri;
        public int MinApprovals => minApprovals;
        public int Denominator => denominator;
        
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