using System.Collections;
using Portkey.Biometric;
using Portkey.BrowserWalletExtension;
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
    public class DID : MonoBehaviour
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
        private IContractProvider _caContractProvider;
        private IEncryption _encryption;
        private ISocialVerifierProvider _socialVerifierProvider;
        private IBiometricProvider _biometricProvider;
        private IAccountGenerator _accountGenerator;
        private IAccountRepository _accountRepository;
        private IAppLogin _appLogin;
        private IQRLogin _qrLogin;
        private IBrowserWalletExtension _browserWalletExtension;
        private ILoginPoller _loginPoller;
        private IQRCodeGenerator _qrCodeGenerator;
        private IVerifierService _verifierService;
        
        private DIDAccount _didWallet;

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
            ChainProvider = new AElfChainProvider(_request, _portkeySocialService);
            _accountGenerator = new AccountGenerator();
            _accountRepository = new AccountRepository(_storageSuite, _encryption, _signingKeyGenerator, _accountGenerator);
            _caContractProvider = new CAContractProvider(ChainProvider);
            _biometricProvider = new BiometricProvider();
            _loginPoller = new LoginPoller(_portkeySocialService);
            _appLogin = new PortkeyAppLogin(_config.PortkeyTransportConfig, _signingKeyGenerator, _loginPoller);
            _browserWalletExtension = new PortkeyExtension();
            _qrCodeGenerator = new QRCodeGenerator();
            _qrLogin = new QRLogin(_loginPoller, _signingKeyGenerator, _qrCodeGenerator);
            _verifierService = new VerifierService(_portkeySocialService, _caContractProvider, ChainProvider);
            
            _didWallet = new DIDAccount(_portkeySocialService, _signingKeyGenerator, _connectService, _caContractProvider, _accountRepository, _accountGenerator, _appLogin, _qrLogin, _browserWalletExtension);
            AuthService = new AuthService(_portkeySocialService, _didWallet, _socialProvider, _socialVerifierProvider, _config, _verifierService);
        }
        
        public IPortkeySocialService PortkeySocialService => _portkeySocialService;
        public IAuthService AuthService { get; private set; }
        public IChainProvider ChainProvider { get; private set; }

        public IBiometric GetBiometric()
        {
            return _biometricProvider.GetBiometric();
        }

        public bool Save(string password, string keyName)
        {
            return _didWallet.Save(password, keyName);
        }

        public bool Load(string password, string keyName)
        {
            return _didWallet.Load(password, keyName);
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

        public bool IsLoggedIn()
        {
            return _didWallet.IsLoggedIn();
        }
    }
}