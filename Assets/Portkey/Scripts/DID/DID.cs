using System.Collections;
using System.Linq;
using Portkey.Biometric;
using Portkey.Chain;
using Portkey.Contract;
using Portkey.Core;
using Portkey.Encryption;
using Portkey.GraphQL;
using Portkey.SocialProvider;
using Portkey.Storage;
using Portkey.Utilities;
using UnityEngine;

namespace Portkey.DID
{
    public class DID : MonoBehaviour, ISocialProvider, IDIDAccountApi
    {
        [SerializeField] private IHttp _request;
        [SerializeField] private PortkeyConfig _config;
        [SerializeField] protected GraphQLConfig _graphQLConfig;

        private GraphQL.GraphQL _graphQL;
        private ISocialProvider _socialProvider;
        private IPortkeySocialService _portkeySocialService;
        private IStorageSuite<string> _storageSuite;
        private ISigningKeyGenerator _signingKeyGenerator;
        private IConnectionService _connectService;
        private IChainProvider _chainProvider;
        private IContractProvider _caContractProvider;
        private IEncryption _encryption;
        private ISocialVerifierProvider _socialVerifierProvider;
        private IBiometricProvider _biometricProvider;
        private IAuthService _authService;
        private IAccountGenerator _accountGenerator;
        private IAccountRepository _accountRepository;
        private IAppLogin _appLogin;
        private IQRLogin _qrLogin;
        private ILoginPoller _loginPoller;
        private IQRCodeGenerator _qrCodeGenerator;
        
        private DIDAccount _didWallet;
        public IPortkeySocialService PortkeySocialService => _portkeySocialService;

        public void Awake()
        {
            _graphQL = new GraphQL.GraphQL(_graphQLConfig);
            _encryption = new AESEncryption();
            _socialProvider = new SocialLoginProvider(_config, _request);
            _portkeySocialService = new PortkeySocialService(_config, _request, _graphQL);
            _socialVerifierProvider = new SocialVerifierProvider(_socialProvider, _portkeySocialService);
            _storageSuite = new NonPersistentStorage<string>();
            _signingKeyGenerator = new SigningKeyGenerator(_encryption);
            _connectService = new ConnectionService<IHttp>(_config.ApiBaseUrl, _request);
            _chainProvider = new AElfChainProvider(_request, _portkeySocialService);
            _accountGenerator = new AccountGenerator();
            _accountRepository = new AccountRepository(_storageSuite, _encryption, _signingKeyGenerator, _accountGenerator);
            _caContractProvider = new CAContractProvider(_chainProvider);
            _biometricProvider = new BiometricProvider();
            _loginPoller = new LoginPoller(_portkeySocialService);
            _appLogin = new PortkeyAppLogin(_config.PortkeyTransportConfig, _signingKeyGenerator, _loginPoller);
            _qrCodeGenerator = new QRCodeGenerator();
            _qrLogin = new QRLogin(_loginPoller, _signingKeyGenerator, _qrCodeGenerator);
            
            _didWallet = new DIDAccount(_portkeySocialService, _signingKeyGenerator, _connectService, _caContractProvider, _accountRepository, _accountGenerator, _appLogin, _qrLogin);
            _authService = new AuthService(_portkeySocialService, _didWallet, _socialProvider, _socialVerifierProvider, _config);
        }
        
        public IAuthService AuthService => _authService;
        
        /// <summary>
        /// Get the chain object with a specified chain ID.
        /// </summary>
        /// <param name="chainId">The chain ID related to the chain to get.</param>
        /// <returns>Chain object related to the specified chain ID.</returns>
        public IEnumerator GetChain(string chainId, SuccessCallback<IChain> successCallback, ErrorCallback errorCallback) => _chainProvider.GetChain(chainId, successCallback, errorCallback);

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

        bool IDIDAccountApi.Load(string password, string keyName)
        {
            return _didWallet.Load(password, keyName);
        }

        public IEnumerator Login(AccountLoginParams param, SuccessCallback<LoginResult> successCallback, ErrorCallback errorCallback)
        {
            return _didWallet.Login(param, successCallback, errorCallback);
        }

        public IEnumerator Logout(EditManagerParams param, SuccessCallback<bool> successCallback, ErrorCallback errorCallback)
        {
            return _didWallet.Logout(param, successCallback, errorCallback);
        }

        public IEnumerator Register(RegisterParams param, SuccessCallback<RegisterResult> successCallback, ErrorCallback errorCallback)
        {
            return _didWallet.Register(param, successCallback, errorCallback);
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

        public IEnumerator GetHolderInfoByContract(GetHolderInfoParams param, SuccessCallback<IHolderInfo> successCallback,
            ErrorCallback errorCallback)
        {
            return _didWallet.GetHolderInfoByContract(param, successCallback, errorCallback);
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

        public ISigningKey GetManagementSigningKey()
        {
            return _didWallet.GetManagementSigningKey();
        }

        public IEnumerator LoginWithPortkeyApp(SuccessCallback<PortkeyAppLoginResult> successCallback,
            ErrorCallback errorCallback)
        {
            return _didWallet.LoginWithPortkeyApp(successCallback, errorCallback);
        }

        public IEnumerator LoginWithQRCode(SuccessCallback<Texture2D> qrCodeCallback,
            SuccessCallback<PortkeyAppLoginResult> successCallback,
            ErrorCallback errorCallback)
        {
            return _didWallet.LoginWithQRCode(qrCodeCallback, successCallback, errorCallback);
        }

        public void Reset()
        {
            _didWallet.Reset();
        }

        public bool IsLoggedIn()
        {
            return _didWallet.IsLoggedIn();
        }
    }
}