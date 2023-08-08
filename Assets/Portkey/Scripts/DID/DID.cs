using System.Collections;
using System.Linq;
using Portkey.Biometric;
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
        private IContractProvider _caContractProvider;
        private IEncryption _encryption;
        private ISocialVerifierProvider _socialVerifierProvider;
        private IBiometricProvider _biometricProvider;
        
        private DIDWallet<WalletAccount> _didWallet;
        public IPortkeySocialService PortkeySocialService => _portkeySocialService;

        public void Awake()
        {
            _socialProvider = new SocialLoginProvider(_config, _request);
            _portkeySocialService = new PortkeySocialService(_config, _request, _graphQL);
            _socialVerifierProvider = new SocialVerifierProvider(_socialProvider, _portkeySocialService);
            _storageSuite = new NonPersistentStorage<string>();
            _accountProvider = new AccountProvider();
            _connectService = new ConnectService<IHttp>(_config.ApiBaseUrl, _request);
            _chainProvider = new AElfChainProvider();
            _caContractProvider = new CAContractProvider(_portkeySocialService, _chainProvider);
            _encryption = new AESEncryption();
            _biometricProvider = new BiometricProvider();
            
            _didWallet = new DIDWallet<WalletAccount>(_portkeySocialService, _storageSuite, _accountProvider, _connectService, _caContractProvider, _encryption);
        }

        public int GetApprovalCount(int length)
        {
            if (length <= _config.MinApprovals)
            {
                return length;
            }
            return (int) (_config.MinApprovals * length / (float)_config.Denominator + 1);
        }
        
        public IBiometric GetBiometric()
        {
            return _biometricProvider.GetBiometric();
        }

        public ISocialLogin GetSocialLogin(AccountType type)
        {
            return _socialProvider.GetSocialLogin(type);
        }

        public ISocialVerifier GetSocialVerifier(AccountType type)
        {
            return _socialVerifierProvider.GetSocialVerifier(type);
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
            yield return _portkeySocialService.GetChainsInfo((chainInfos) =>
            {
                var chainInfo = chainInfos.items.FirstOrDefault(info => info.chainId == chainId);
                if (chainInfo == null)
                {
                    errorCallback("Network Error!");
                    return;
                }

                StartCoroutine(_didWallet.GetVerifierServers(chainInfo.chainId, result =>
                {
                    if (result == null || result.Length == 0)
                    {
                        errorCallback("Network Error!");
                        return;
                    }
                    
                    successCallback(result);
                }, errorCallback));
            }, errorCallback);
        }

        public IEnumerator GetCAHolderInfo(string chainId, SuccessCallback<CAHolderInfo> successCallback, ErrorCallback errorCallback)
        {
            return _didWallet.GetCAHolderInfo(chainId, successCallback, errorCallback);
        }

        public void Reset()
        {
            _didWallet.Reset();
        }

        public BlockchainWallet GetWallet()
        {
            return _didWallet.GetWallet();
        }

        public bool IsLoggedIn()
        {
            return _didWallet.IsLoggedIn();
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