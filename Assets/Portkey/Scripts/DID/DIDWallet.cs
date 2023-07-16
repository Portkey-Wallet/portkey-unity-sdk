using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AElf;
using AElf.Kernel;
using Portkey.Core;
using Portkey.Utilities;

namespace Portkey.DID
{

    public class DIDWallet<T> : IDIDWallet where T : AccountBase
    {
        protected class AccountInfo
        {
            public string LoginAccount { get; set; } = null;
            public string Nickname { get; set; } = null;
            
            public bool IsLoggedIn()
            {
                return LoginAccount != null;
            }
        }
        
        private IPortkeySocialService _socialService;
        private T _managementAccount;
        private IStorageSuite<string> _storageSuite;
        private IAccountProvider<T> _accountProvider;
        private IConnectService _connectService;
        
        private AccountInfo _accountInfo = new AccountInfo();
        private Dictionary<string, ChainInfo> _chainsInfoMap = new Dictionary<string, ChainInfo>();
        private Dictionary<string, CAInfo> _caInfoMap = new Dictionary<string, CAInfo>();

        public DIDWallet(IPortkeySocialService socialService, IStorageSuite<string> storageSuite, IAccountProvider<T> accountProvider, IConnectService connectService)
        {
            _socialService = socialService;
            _storageSuite = storageSuite;
            _accountProvider = accountProvider;
            _connectService = connectService;
        }
        
        private IEnumerator GetChainsInfo(SuccessCallback<Dictionary<string, ChainInfo>> successCallback, ErrorCallback errorCallback)
        {
            yield return _socialService.GetChainsInfo((result =>
            {
                result ??= new ArrayWrapper<ChainInfo>
                {
                    items = new ChainInfo[] { }
                };
                foreach (var chainInfo in result.items)
                {
                    _chainsInfoMap[chainInfo.chainId] = chainInfo;
                }
                successCallback(_chainsInfoMap);
            }), errorCallback);
        }
        
        public void InitializeManagementAccount()
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

        public IEnumerator Login(EditManagerParams param, SuccessCallback<bool> successCallback, ErrorCallback errorCallback)
        {
            InitializeManagementAccount();

            if (!_accountInfo.IsLoggedIn())
            {
                throw new Exception("Account not logged in.");
            }

            return AddManager(param, response =>
            {
                successCallback(response);
            }, errorCallback);

            /*
             *
    if (!this.accountInfo.loginAccount) throw new Error('account not logged in');
      const _params = params as ScanLoginParams;
      const req = await this.addManager(_params);
      if (req?.error) throw req.error;
      return true;
             */
        }

        public IEnumerator Login(AccountLoginParams param, SuccessCallback<LoginResult> successCallback, ErrorCallback errorCallback)
        {
            throw new NotImplementedException();
        }

        public bool Logout(EditManagerParams param)
        {
            throw new System.NotImplementedException();
        }

        public RecoverStatusResult GetLoginStatus(string chainId, string sessionId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator Register(RegisterParams param, SuccessCallback<RegisterResult> successCallback, ErrorCallback errorCallback)
        {
            if(_accountInfo.LoginAccount != null)
            {
                errorCallback("Account already logged in.");
                yield break;
            }
            
            InitializeManagementAccount();
            param.manager = _managementAccount.Address;
            
            yield return _socialService.Register(param, (result) =>
            {
                StaticCoroutine.StartCoroutine(GetRegisterStatus(param.chainId, result.sessionId,
                    (status) =>
                    {
                        if (status.IsStatusPass())
                        {
                            //UpdateAccountInfo(param.loginGuardianIdentifier);
                            UpdateCAInfo(param.chainId, status.caHash, status.caAddress);
                        }
                        else
                        {
                            errorCallback($"Register failed: {status.registerMessage}");
                        }

                        successCallback(new RegisterResult(status, result.sessionId));
                    }, errorCallback));
            }, errorCallback);
        }

        public IEnumerator GetRegisterStatus(string chainId, string sessionId, SuccessCallback<RegisterStatusResult> successCallback, ErrorCallback errorCallback)
        {
            return _socialService.GetRegisterStatus(sessionId, QueryOptions.DefaultQueryOptions, (status) =>
                {
                    if(status == null)
                    {
                        errorCallback("Failed to get register status.");
                        return;
                    }
                    if(IsFirstTimeRegisterPassed(chainId, status))
                    {
                        var holderInfoParams = new GetHolderInfoParams
                        {
                            chainId = chainId,
                            caHash = status.caHash
                        };
                        StaticCoroutine.StartCoroutine(GetHolderInfo(holderInfoParams, (info) =>
                        {
                            var isCurrentAccountAManager = info.managerInfos.Any(manager => manager.address == _managementAccount.Address);
                            if (isCurrentAccountAManager)
                            {
                                UpdateAccountInfo(info.guardianList.guardians[0].guardianIdentifier);
                                UpdateCAInfo(chainId, status.caHash, status.caAddress);
                            }

                            successCallback(status);
                        }, errorCallback));
                    }
                    else
                    {
                        successCallback(status);
                    }
                },
                errorCallback);
        }

        private void UpdateAccountInfo(string guardianIdentifier)
        {
            _accountInfo = new AccountInfo
            {
                LoginAccount = guardianIdentifier
            };
        }
        
        private void UpdateCAInfo(string chainId, string caHash, string caAddress)
        {
            _caInfoMap[chainId] = new CAInfo
            {
                caHash = caHash,
                caAddress = caAddress
            };
        }

        private bool IsFirstTimeRegisterPassed(string chainId, RegisterStatusResult response)
        {
            return response!= null && response.IsStatusPass() && _managementAccount?.Address != null && !_caInfoMap.ContainsKey(chainId);
        }

        public IEnumerator GetHolderInfo(GetHolderInfoParams param, SuccessCallback<IHolderInfo> successCallback, ErrorCallback errorCallback)
        {
            return _socialService.GetHolderInfo(param, (holderInfo) =>
            {
                if (IsLoginAccountTheRequestedGuardian(param, holderInfo))
                {
                    UpdateCAInfo(param.chainId, holderInfo.caHash, holderInfo.caAddress);
                }

                successCallback(holderInfo);
            }, errorCallback);
        }

        public IEnumerator GetHolderInfo(GetHolderInfoByManagerParams param, SuccessCallback<CaHolderWithGuardian> successCallback, ErrorCallback errorCallback)
        {
            var manager = param.manager;
            
            // If manager is not specified, use the management account.
            if(manager == null && _managementAccount != null)
            {
                manager = _managementAccount.Address;
            }
            if (manager == null)
            {
                errorCallback("No manager account!");
                yield break;
            }

            var caHolderInfoByManagerParams = new GetCAHolderByManagerParams
            {
                chainId = param.chainId,
                manager = manager
            };
            yield return _socialService.GetHolderInfoByManager(caHolderInfoByManagerParams, (result) =>
            {
                var info = result.caHolders[0];
                if (info != null && manager == _managementAccount?.Address &&
                    info.holderManagerInfo.caAddress != null && info.holderManagerInfo.caHash != null)
                {
                    UpdateCAInfo(param.chainId, info.holderManagerInfo.caHash, info.holderManagerInfo.caAddress);
                    var loginAccount = info.loginGuardianInfo[0]?.loginGuardian?.identifierHash;
                    if (!_accountInfo.IsLoggedIn() && loginAccount != null)
                    {
                        UpdateAccountInfo(loginAccount);
                    }
                }

                successCallback(info);
            }, errorCallback);
        }

        private bool IsLoginAccountTheRequestedGuardian(GetHolderInfoParams param, IHolderInfo holderInfo)
        {
            return holderInfo != null && param.guardianIdentifier == _accountInfo?.LoginAccount;
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
            if(_connectService == null)
            {
                throw new Exception("ConnectService is not initialized.");
            }
            if(_managementAccount == null)
            {
                throw new Exception("Management Account is not initialized.");
            }
            var caHash = _caInfoMap[chainId]?.caHash;
            if(caHash == null)
            {
                throw new Exception($"CA Hash on Chain ID: ({chainId}) does not exists.");
            }

            var timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            var signature = BitConverter.ToString(_managementAccount.Sign($"{_managementAccount.Address}-{timestamp}"));
            var publicKey = _managementAccount.PublicKey;
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
            
            yield return _connectService.GetConnectToken(requestTokenConfig, (token) =>
            {
                if(token == null)
                {
                    errorCallback("Failed to get token.");
                    return;
                }
                
                _socialService.GetCAHolderInfo($"Bearer {token.access_token}", caHash, (caHolderInfo) =>
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
                }, errorCallback);
            }, errorCallback);
        }

        public IEnumerator AddManager(EditManagerParams editManagerParams, SuccessCallback<bool> successCallback,
            ErrorCallback errorCallback)
        {
            if (_managementAccount == null)
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