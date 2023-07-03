using System;
using System.Collections;
using System.Collections.Generic;
using AElf.Kernel;
using Portkey.Core;

namespace Portkey.DID
{

    public class DIDWallet<T> : IDIDWallet where T : AccountBase
    {
        protected class AccountInfo
        {
            public string LoginAccount { get; set; } = null;
            public string Nickname { get; set; } = null;
        }
        
        private IPortkeySocialService _socialService;
        private T _managementAccount = null;
        private IStorageSuite<string> _storageSuite;
        private IAccountProvider<T> _accountProvider;
        
        private AccountInfo _accountInfo = new AccountInfo();
        private Dictionary<string, ChainInfo> chainsInfo = new Dictionary<string, ChainInfo>();

        private IEnumerator GetChainsInfo(SuccessCallback<Dictionary<string, ChainInfo>> successCallback, ErrorCallback errorCallback)
        {
            yield return _socialService.GetChainsInfo((result =>
            {
                result ??= new ChainInfo[] { };
                foreach (var chainInfo in result)
                {
                    chainsInfo[chainInfo.chainId] = chainInfo;
                }
                successCallback(chainsInfo);
            }), errorCallback);
        }
        
        public void InitializeAccount()
        {
            if (_managementAccount != null)
            {
                return;
            }
            
            _managementAccount = _accountProvider.CreateAccount();
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
            throw new System.NotImplementedException();
        }

        public IEnumerator AddManager(EditManagerParams editManagerParams, IHttp.successCallback successCallback,
            IHttp.errorCallback errorCallback)
        {
            if (_managementAccount == null)
            {
                throw new Exception("Manager Account does not exist.");
            }
            
            //TODO
            yield return null;
        }

        public IEnumerator RemoveManager(EditManagerParams editManagerParams, IHttp.successCallback successCallback,
            IHttp.errorCallback errorCallback)
        {
            throw new NotImplementedException();
        }
    }
}