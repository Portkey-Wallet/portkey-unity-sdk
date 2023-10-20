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
    /// <summary>
    /// PortkeySDK is the main class to interact with Portkey.
    /// </summary>
    public class DID : MonoBehaviour
    {
        [SerializeField] private IHttp _request;
        [SerializeField] private PortkeyConfig _config;
        [SerializeField] protected GraphQLConfig _graphQLConfig;

        private PortkeyDIDGraphQL _portkeyDidGraphQl;
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
            _portkeyDidGraphQl = new PortkeyDIDGraphQL(_graphQLConfig);
            _encryption = new AESEncryption();
            _socialProvider = new SocialLoginProvider(_config, _request);
            _portkeySocialService = new PortkeySocialService(_config, _request, _portkeyDidGraphQl);
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
        
        /// <summary>
        /// AuthService provides functions to login and logout.
        /// </summary>
        public IAuthService AuthService { get; private set; }
        
        /// <summary>
        /// Chain provider provides IChain objects which corresponds to chain deployed on the blockchain.
        /// </summary>
        public IChainProvider ChainProvider { get; private set; }
        
        /// <summary>
        /// Biometric feature available on iOS and Android.
        /// </summary>
        public IBiometric Biometric => _biometricProvider.GetBiometric();

        /// <summary> The Save function saves the current logged in DID wallet to a file.        
        /// &lt;para&gt;@param password The password used to encrypt the wallet.&lt;/para&gt;
        /// &lt;para&gt;@param keyName The name of the key that will be saved in this wallet.&lt;/para&gt;</summary>
        ///
        /// <param name="string password"> The password used to encrypt the wallet.</param>
        /// <param name="string keyName"> ///     the name of the key to be saved. 
        /// </param>
        ///
        /// <returns> A boolean value that indicates whether the operation was successful.</returns>
        public bool Save(string password, string keyName)
        {
            return _didWallet.Save(password, keyName);
        }

        /// <summary> The Load function loads a DID wallet from the file system.        
        /// &lt;para&gt;@param password The password used to decrypt the DID wallet.&lt;/para&gt;
        /// &lt;para&gt;@param keyName The name of the key to be loaded.&lt;/para&gt;</summary>
        ///
        /// <param name="string password"> The password used to encrypt the wallet</param>
        /// <param name="string keyName"> /// the name of the key to be loaded.
        /// </param>
        ///
        /// <returns> A boolean value that indicates whether the operation was successful.</returns>
        public bool Load(string password, string keyName)
        {
            return _didWallet.Load(password, keyName);
        }

        /// <summary> The GetHolderInfo function is used to get the holder information of a DID.</summary>        
        ///
        /// <param name="GetHolderInfoByManagerParams param"> Parameters for searching for the holder information through management account address and/or chain ID.</param>
        /// <param name="SuccessCallback successCallback"> The success callback returning the holder information.</param>
        /// <param name="ErrorCallback errorCallback"> The error callback.</param>
        ///
        /// <returns> A caholderwithguardian object that contains the holder information.</returns>
        public IEnumerator GetHolderInfo(GetHolderInfoByManagerParams param, SuccessCallback<CaHolderWithGuardian> successCallback,
            ErrorCallback errorCallback)
        {
            yield return _didWallet.GetHolderInfo(param, successCallback, errorCallback);
        }

        /// <summary> The GetHolderInfo function is used to get the holder information of a DID through contract view call.</summary>        
        ///
        /// <param name="GetHolderInfoParams param"> Parameters for searching for the holder information through caHash and/or chain ID.</param>
        /// <param name="SuccessCallback successCallback"> The success callback returning the holder information.</param>
        /// <param name="ErrorCallback errorCallback"> The error callback.</param>
        ///
        /// <returns> A IHolderInfo object that contains the holder information.</returns>
        public IEnumerator GetHolderInfoByContract(GetHolderInfoParams param, SuccessCallback<IHolderInfo> successCallback,
            ErrorCallback errorCallback)
        {
            yield return _didWallet.GetHolderInfoByContract(param, successCallback, errorCallback);
        }

        public IEnumerator GetPhoneCountryCodeWithLocal(SuccessCallback<IPhoneCountryCodeResult> successCallback,
            ErrorCallback errorCallback)
        {
            yield return _portkeySocialService.GetPhoneCountryCodeWithLocal(successCallback, errorCallback);
        }

        /// <summary> The IsLoggedIn function checks to see if the user is logged in.        
        /// &lt;para&gt;If the user is logged in, it returns true.&lt;/para&gt;
        /// &lt;para&gt;If the user is not logged in, it returns false.&lt;/para&gt;</summary>
        ///
        ///
        /// <returns> A boolean value indication if the user is logged in.</returns>
        public bool IsLoggedIn()
        {
            return _didWallet.IsLoggedIn();
        }
    }
}