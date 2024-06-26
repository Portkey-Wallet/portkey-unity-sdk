using System;
using System.Collections;
using System.Collections.Generic;
using AElf;
using AElf.Kernel;
using Portkey.Core;
using Portkey.Utilities;

namespace Portkey.DID
{
    /// <summary>
    /// DID Wallet class. Still WIP. Will be implemented in another branch.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DIDWallet : IDIDWallet
    {
        protected class AccountInfo
        {
            public string LoginAccount { get; set; } = null;
            public string Nickname { get; set; } = null;
        }
        
        private IPortkeySocialService _socialService;
        private IWallet _managementWallet;
        private IStorageSuite<string> _storageSuite;
        private Core.IWalletProvider _walletProvider;
        private IConnectionService _connectionService;
        
        private AccountInfo _accountInfo = new AccountInfo();
        private Dictionary<string, ChainInfo> _chainsInfoMap = new Dictionary<string, ChainInfo>();
        private Dictionary<string, CAInfo> _caInfoMap = new Dictionary<string, CAInfo>();

        public DIDWallet(IPortkeySocialService socialService, IStorageSuite<string> storageSuite, Core.IWalletProvider walletProvider, IConnectionService connectionService)
        {
            _socialService = socialService;
            _storageSuite = storageSuite;
            _walletProvider = walletProvider;
            _connectionService = connectionService;
        }
        
        private IEnumerator GetChainsInfo(SuccessCallback<Dictionary<string, ChainInfo>> successCallback, ErrorCallback errorCallback)
        {
            yield return _socialService.GetChainsInfo((result =>
            {
                result ??= new ChainInfo[] { };
                foreach (var chainInfo in result)
                {
                    _chainsInfoMap[chainInfo.chainId] = chainInfo;
                }
                successCallback(_chainsInfoMap);
            }), errorCallback);
        }
        
        public void InitializeAccount()
        {
            if (_managementWallet != null)
            {
                return;
            }
            
            _managementWallet = _walletProvider.Create();
        }

        public bool Save(string password, string keyName)
        {
            throw new System.NotImplementedException();
        }

        public void Load(string password, string keyName)
        {
            throw new System.NotImplementedException();
        }

        public bool Login(EditManagerParams param)
        {
            InitializeAccount();

            if (_accountInfo.LoginAccount == null)
            {
                throw new Exception("Account not logged in.");
            }

            //TODO
            return false;
        }

        public LoginResult Login(AccountLoginParams param)
        {
            throw new System.NotImplementedException();
        }

        public bool Logout(EditManagerParams param)
        {
            throw new System.NotImplementedException();
        }

        public RecoverStatusResult GetLoginStatus(string chainId, string sessionId)
        {
            throw new System.NotImplementedException();
        }

        public RegisterResult Register(RegisterParams param)
        {
            throw new System.NotImplementedException();
        }

        public RegisterStatusResult GetRegisterStatus(string chainId, string sessionId)
        {
            throw new System.NotImplementedException();
        }

        public GetCAHolderByManagerResult GetHolderInfo(GetHolderInfoParams param)
        {
            throw new System.NotImplementedException();
        }

        public VerifierItem[] GetVerifierServers(string chainId)
        {
            throw new System.NotImplementedException();
        }

        public CAHolderInfo GetCAHolderInfo(string chainId)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetCAHolderInfo(string chainId, SuccessCallback<CAHolderInfo> successCallback, ErrorCallback errorCallback)
        {
            if(_connectionService == null)
            {
                errorCallback("ConnectService is not initialized.");
                yield break;
            }
            if(_managementWallet == null)
            {
                errorCallback("Management Account is not initialized.");
                yield break;
            }
            var caHash = _caInfoMap[chainId]?.caHash;
            if(caHash == null)
            {
                errorCallback($"CA Hash on Chain ID: ({chainId}) does not exists.");
                yield break;
            }

            var timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            var signature = BitConverter.ToString(_managementWallet.Sign($"{_managementWallet.Address}-{timestamp}"));
            var publicKey = _managementWallet.PublicKey;
            var requestTokenConfig = new RequestTokenConfig
            {
                grant_type = "signature",
                client_id = "CAServer_App",
                scope = "CAServer",
                signature = signature.ToString(),
                pubkey = publicKey,
                timestamp = timestamp,
                ca_hash = caHash,
                chain_id = chainId
            };
            
            yield return _connectionService.GetConnectToken(requestTokenConfig, (token) =>
            {
                if(token == null)
                {
                    errorCallback("Failed to get token.");
                    return;
                }
                
                StaticCoroutine.StartCoroutine(_socialService.GetCAHolderInfo($"Bearer {token.access_token}", caHash, (caHolderInfo) =>
                {
                    if(caHolderInfo == null)
                    {
                        errorCallback("Failed to get CA Holder Info.");
                        return;
                    }

                    if (caHolderInfo.nickName != null)
                    {
                        _accountInfo.Nickname = caHolderInfo.nickName;
                    }
                    successCallback(caHolderInfo);
                }, errorCallback));
            }, errorCallback);
        }

        public IEnumerator AddManager(EditManagerParams editManagerParams, IHttp.successCallback successCallback,
            ErrorCallback errorCallback)
        {
            if (_managementWallet == null)
            {
                throw new Exception("Manager Account does not exist.");
            }
            
            //TODO
            yield return null;
        }

        public IEnumerator RemoveManager(EditManagerParams editManagerParams, IHttp.successCallback successCallback,
            ErrorCallback errorCallback)
        {
            throw new NotImplementedException();
        }
    }
}