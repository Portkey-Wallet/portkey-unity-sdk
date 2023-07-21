using System.Collections;
using Portkey.Chain;
using Portkey.Contract;
using Portkey.Core;
using Portkey.Encryption;
using Portkey.SocialProvider;
using Portkey.Storage;
using UnityEngine;

namespace Portkey.DID
{
    public class DID : MonoBehaviour, ISocialProvider, IDIDWallet
    {
        [SerializeField] private IHttp _request;
        [SerializeField] private PortkeyConfig _config;
        [SerializeField] private GraphQL.GraphQL _graphQL;
        
        private ISocialProvider _socialProvider;
        private IPortkeySocialService _portkeySocialService;
        private IStorageSuite<string> _storageSuite;
        private IAccountProvider<WalletAccount> _accountProvider;
        private IConnectService _connectService;
        private IChainProvider _chainProvider;
        private IContractProvider _contractProvider;
        private IEncryption _encryption;
        
        private DIDWallet<WalletAccount> _didWallet;
        public IPortkeySocialService PortkeySocialService => _portkeySocialService;

        public void Awake()
        {
            _socialProvider = new SocialLoginProvider(_config, _request);
            _portkeySocialService = new PortkeySocialService(_config, _request, _graphQL);
            _storageSuite = new NonPersistentStorage<string>();
            _accountProvider = new AccountProvider();
            _connectService = new ConnectService<IHttp>(_config.ApiBaseUrl, _request);
            _chainProvider = new AElfChainProvider();
            _contractProvider = new CAContractProvider(_portkeySocialService, _chainProvider);
            _encryption = new AESEncryption();
            
            _didWallet = new DIDWallet<WalletAccount>(_portkeySocialService, _storageSuite, _accountProvider, _connectService, _contractProvider, _encryption);
        }

        public ISocialLogin GetSocialLogin(AccountType type)
        {
            return _socialProvider.GetSocialLogin(type);
        }

        public bool Save(string password, string keyName)
        {
            return _didWallet.Save(password, keyName);
        }

        public IWallet Load(string password, string keyName)
        {
            return _didWallet.Load(password, keyName);
        }

        public IEnumerator Login(EditManagerParams param, SuccessCallback<bool> successCallback, ErrorCallback errorCallback)
        {
            return _didWallet.Login(param, successCallback, errorCallback);
        }

        public IEnumerator Login(AccountLoginParams param, SuccessCallback<LoginResult> successCallback, ErrorCallback errorCallback)
        {
            return _didWallet.Login(param, successCallback, errorCallback);
        }

        public IEnumerator Logout(EditManagerParams param, SuccessCallback<bool> successCallback, ErrorCallback errorCallback)
        {
            return _didWallet.Logout(param, successCallback, errorCallback);
        }

        public IEnumerator GetLoginStatus(string chainId, string sessionId, SuccessCallback<RecoverStatusResult> successCallback,
            ErrorCallback errorCallback)
        {
            return _didWallet.GetLoginStatus(chainId, sessionId, successCallback, errorCallback);
        }

        public IEnumerator Register(RegisterParams param, SuccessCallback<RegisterResult> successCallback, ErrorCallback errorCallback)
        {
            return _didWallet.Register(param, successCallback, errorCallback);
        }

        public IEnumerator GetRegisterStatus(string chainId, string sessionId, SuccessCallback<RegisterStatusResult> successCallback,
            ErrorCallback errorCallback)
        {
            return _didWallet.GetRegisterStatus(chainId, sessionId, successCallback, errorCallback);
        }

        public IEnumerator GetHolderInfo(GetHolderInfoParams param, SuccessCallback<IHolderInfo> successCallback, ErrorCallback errorCallback)
        {
            return _didWallet.GetHolderInfo(param, successCallback, errorCallback);
        }

        public IEnumerator GetHolderInfo(GetHolderInfoByManagerParams param, SuccessCallback<CaHolderWithGuardian> successCallback,
            ErrorCallback errorCallback)
        {
            return _didWallet.GetHolderInfo(param, successCallback, errorCallback);
        }

        public IEnumerator GetVerifierServers(string chainId, SuccessCallback<VerifierItem[]> successCallback, ErrorCallback errorCallback)
        {
            return _didWallet.GetVerifierServers(chainId, successCallback, errorCallback);
        }

        public IEnumerator GetCAHolderInfo(string chainId, SuccessCallback<CAHolderInfo> successCallback, ErrorCallback errorCallback)
        {
            return _didWallet.GetCAHolderInfo(chainId, successCallback, errorCallback);
        }

        public IEnumerator AddManager(EditManagerParams editManagerParams, SuccessCallback<bool> successCallback,
            ErrorCallback errorCallback)
        {
            return _didWallet.AddManager(editManagerParams, successCallback, errorCallback);
        }

        public IEnumerator RemoveManager(EditManagerParams editManagerParams, SuccessCallback<bool> successCallback,
            ErrorCallback errorCallback)
        {
            return _didWallet.RemoveManager(editManagerParams, successCallback, errorCallback);
        }
    }
}