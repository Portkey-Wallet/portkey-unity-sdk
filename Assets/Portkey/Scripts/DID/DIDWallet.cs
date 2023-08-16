using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AElf.Types;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using Portkey.Contracts.CA;
using Portkey.Core;
using Portkey.Utilities;
using UnityEngine;
using Empty = Google.Protobuf.WellKnownTypes.Empty;

namespace Portkey.DID
{
    /// <summary>
    /// DID Wallet class. Still WIP. Will be implemented in another branch.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DIDWallet : IDIDWallet
    {
        private const string DEFAULT_KEY_NAME = "portkey_sdk_did_wallet";
        
        public class AccountInfo
        {
            public string LoginAccount { get; set; } = null;
            public string Nickname { get; set; } = null;
            
            public bool IsLoggedIn()
            {
                return LoginAccount != null;
            }
        }
        
        // data struct used for saving state of this DID Wallet to storage
        public class DIDInfo
        {
            public string aesPrivateKey;
            public Dictionary<string, CAInfo> caInfo;
            public AccountInfo accountInfo;
        }
        
        private IPortkeySocialService _socialService;
        private IWallet _managementAccount;
        private IStorageSuite<string> _storageSuite;
        private IWalletProvider _walletProvider;
        private IConnectionService _connectionService;
        private IContractProvider _contractProvider;
        private IEncryption _encryption;

        private AccountInfo _accountInfo = new AccountInfo();
        private Dictionary<string, CAInfo> _caInfoMap = new Dictionary<string, CAInfo>();

        public DIDWallet(IPortkeySocialService socialService, IStorageSuite<string> storageSuite, IWalletProvider walletProvider, IConnectionService connectionService, IContractProvider contractProvider, IEncryption encryption)
        {
            _socialService = socialService;
            _storageSuite = storageSuite;
            _walletProvider = walletProvider;
            _connectionService = connectionService;
            _contractProvider = contractProvider;
            _encryption = encryption;
        }

        public void InitializeManagementAccount()
        {
            if (_managementAccount != null)
            {
                return;
            }
            
            _managementAccount = _walletProvider.CreateAccount();
        }

        public bool Save(string password, string keyName = DEFAULT_KEY_NAME)
        {
            var encryptedPrivateKey = EncryptManagementAccount(password);
            var data = JsonConvert.SerializeObject(new DIDInfo
            {
                aesPrivateKey = Convert.ToBase64String(encryptedPrivateKey),
                caInfo = _caInfoMap,
                accountInfo = _accountInfo
            });
            var encryptedData = _encryption.Encrypt(data, password);
            var encryptedDataStr = Convert.ToBase64String(encryptedData);
            _storageSuite.SetItem(keyName, encryptedDataStr);
            return true;
        }
        
        private byte[] EncryptManagementAccount(string password)
        {
            if (_managementAccount == null) throw new NullReferenceException("Management Account does not exist!");
            return _encryption.Encrypt(_managementAccount.PrivateKey, password);
        }

        public IDIDWallet Load(string password, string keyName = DEFAULT_KEY_NAME)
        {
            var encryptedDataStr = _storageSuite.GetItem(keyName);
            if (encryptedDataStr == null)
            {
                throw new Exception("No data found.");
            }
            var encryptedData = Convert.FromBase64String(encryptedDataStr);
            var data = _encryption.Decrypt(encryptedData, password);
            if (data == null)
            {
                throw new Exception("Wrong password.");
            }
            var info = JsonConvert.DeserializeObject<DIDInfo>(data);
            var privateKey = _encryption.Decrypt(Convert.FromBase64String(info.aesPrivateKey), password);
            _managementAccount = _walletProvider.GetAccountFromPrivateKey(privateKey);
            _caInfoMap = info.caInfo??new Dictionary<string, CAInfo>();
            _accountInfo = info.accountInfo??new AccountInfo();

            return this;
        }

        public IEnumerator Login(EditManagerParams param, SuccessCallback<bool> successCallback, ErrorCallback errorCallback)
        {
            InitializeManagementAccount();

            if (!_accountInfo.IsLoggedIn())
            {
                errorCallback("Account not logged in.");
                yield break;
            }

            yield return AddManager(param, result =>
            {
                // TODO: result.error error handling
                successCallback(result);
            }, errorCallback);
        }

        public IEnumerator Login(AccountLoginParams param, SuccessCallback<LoginResult> successCallback, ErrorCallback errorCallback)
        {
            InitializeManagementAccount();

            if (_accountInfo.IsLoggedIn())
            {
                errorCallback("Account already logged in.");
                yield break;
            }
            
            var context = new Context
            {
                clientId = _managementAccount.Address,
                requestId = Guid.NewGuid().ToString()
            };

            var recoveryParam = new RecoveryParams
            {
                chainId = param.chainId,
                loginGuardianIdentifier = param.loginGuardianIdentifier,
                manager = _managementAccount.Address,
                guardiansApproved = param.guardiansApprovedList,
                extraData = param.extraData,
                context = context
            };
            yield return _socialService.Recovery(recoveryParam, (result) =>
            {
                StaticCoroutine.StartCoroutine(GetLoginStatus(param.chainId, result.sessionId,
                    (status) =>
                    {
                        successCallback(new LoginResult(status, result.sessionId));
                    }, errorCallback));
            }, errorCallback);
        }

        public IEnumerator GetLoginStatus(string chainId, string sessionId, SuccessCallback<RecoverStatusResult> successCallback, ErrorCallback errorCallback)
        {
            return _socialService.GetRecoverStatus(sessionId, QueryOptions.DefaultQueryOptions, (status) =>
                {
                    if(status == null)
                    {
                        errorCallback("Failed to get register status.");
                        return;
                    }
                    if(IsFirstTimeRecoverPassed(chainId, status))
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
        
        private bool IsFirstTimeRecoverPassed(string chainId, RecoverStatusResult response)
        {
            return response!= null && response.IsStatusPass() && _managementAccount?.Address != null && !_caInfoMap.ContainsKey(chainId);
        }
        
        public IEnumerator Logout(EditManagerParams param, SuccessCallback<bool> successCallback, ErrorCallback errorCallback)
        {
            if (_managementAccount == null)
            {
                errorCallback("ManagerAccount does not exist!");
                yield break;
            }
            if(param.caHash == null && _caInfoMap.TryGetValue(param.chainId, out var caInfo))
            {
                param.caHash = caInfo.caHash;
                Debugger.Log($"CAHash: {param.caHash}");
            }
            if(param.caHash == null)
            {
                errorCallback("CAHash does not exist!");
                yield break;
            }
            if (param.managerInfo == null)
            {
                param.managerInfo = new ManagerInfo
                {
                    Address = _managementAccount.Address.ToAddress(),
                    ExtraData = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds().ToString()
                };
            }
            Debugger.Log("Removing Manager...");
            yield return RemoveManager(param, result =>
            {
                successCallback(result);
            }, errorCallback);
        }

        public IEnumerator Register(RegisterParams param, SuccessCallback<RegisterResult> successCallback, ErrorCallback errorCallback)
        {
            if(_accountInfo.IsLoggedIn())
            {
                errorCallback("Account already logged in.");
                yield break;
            }
            
            InitializeManagementAccount();
            param.manager = _managementAccount.Address;
            param.context = new Context
            {
                clientId = _managementAccount.Address,
                requestId = Guid.NewGuid().ToString()
            };
            
            yield return _socialService.Register(param, (result) =>
            {
                StaticCoroutine.StartCoroutine(GetRegisterStatus(param.chainId, result.sessionId, (status) =>
                {
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
        
        private static AccountType GetAccountType(GuardianType type) => type switch
        {
            GuardianType.OfEmail => AccountType.Email,
            GuardianType.OfPhone => AccountType.Phone,
            GuardianType.OfGoogle => AccountType.Google,
            GuardianType.OfApple => AccountType.Apple,
            _ => throw new ArgumentOutOfRangeException(nameof(type), $"Not expected account type: {type}")
        }; 

        public IEnumerator GetHolderInfoByContract(GetHolderInfoParams param, SuccessCallback<IHolderInfo> successCallback, ErrorCallback errorCallback)
        {
            yield return _contractProvider.GetContract(param.chainId,  (contract) =>
            {
                var holderInfoInput = new GetHolderInfoInput
                {
                    CaHash = Hash.LoadFromHex(param.caHash)
                };
                StaticCoroutine.StartCoroutine(contract.CallTransactionAsync<GetHolderInfoOutput>(_managementAccount, "GetHolderInfo", holderInfoInput, result =>
                {
                    var holderInfo = ConvertToHolderInfo(result);
                    UpdateCAInfo(param.chainId, holderInfo.caHash, holderInfo.caAddress);

                    successCallback(holderInfo);
                }, errorCallback));
            }, errorCallback);
        }

        private IHolderInfo ConvertToHolderInfo(GetHolderInfoOutput result)
        {
            var newGuardianList = new Core.GuardianList
            {
                guardians = new Core.Guardian[result.GuardianList.Guardians.Count]
            };
            for (var i = 0; i < result.GuardianList.Guardians.Count; ++i)
            {
                var guardian = result.GuardianList.Guardians[i];
                var newGuardian = new Core.Guardian
                {
                    identifierHash = guardian.IdentifierHash.ToHex(),
                    isLoginGuardian = guardian.IsLoginGuardian,
                    salt = guardian.Salt,
                    type = GetAccountType(guardian.Type),
                    verifierId = guardian.VerifierId.ToHex()
                };
                newGuardianList.guardians[i] = newGuardian;
            }

            var newManagerInfos = new Manager[result.ManagerInfos.Count];
            for (var j = 0; j < result.ManagerInfos.Count; ++j)
            {
                var manager = result.ManagerInfos[j];
                var newManager = new Manager
                {
                    address = manager.Address.ToBase58(),
                    extraData = manager.ExtraData
                };
                newManagerInfos[j] = newManager;
            }

            var holderInfo = new IHolderInfo
            {
                caHash = result.CaHash.ToHex(),
                caAddress = result.CaAddress.ToBase58(),
                guardianList = newGuardianList,
                managerInfos = newManagerInfos
            };
            return holderInfo;
        }

        private bool IsLoginAccountTheRequestedGuardian(GetHolderInfoParams param, IHolderInfo holderInfo)
        {
            return holderInfo != null && param.guardianIdentifier == _accountInfo?.LoginAccount;
        }

        public IEnumerator GetVerifierServers(string chainId, SuccessCallback<VerifierItem[]> successCallback, ErrorCallback errorCallback)
        {
            InitializeManagementAccount();
            yield return _contractProvider.GetContract(chainId,  (contract) =>
            {
                StaticCoroutine.StartCoroutine(GetVerifierServersAsync(contract, successCallback, errorCallback));
            }, errorCallback);
        }
        
        private IEnumerator GetVerifierServersAsync(IContract contract, SuccessCallback<VerifierItem[]> successCallback, ErrorCallback errorCallback) {
            yield return contract.CallTransactionAsync<GetVerifierServersOutput>(_managementAccount, "GetVerifierServers", new Empty(), result =>
            {
                var verifierItems = ConvertToVerifierItems(result);
                successCallback(verifierItems);
            }, errorCallback);
        }

        private static VerifierItem[] ConvertToVerifierItems(GetVerifierServersOutput result)
        {
            var verifierItems = new VerifierItem[result.VerifierServers.Count];
            for (var i = 0; i < result.VerifierServers.Count; i++)
            {
                var verifierServer = result.VerifierServers[i];
                var addresses = new string[verifierServer.VerifierAddresses.Count];
                for (var j = 0; j < verifierServer.VerifierAddresses.Count; j++)
                {
                    addresses[j] = verifierServer.VerifierAddresses[j].ToString();
                }

                var item = new VerifierItem
                {
                    id = verifierServer.Id.ToHex(),
                    name = verifierServer.Name,
                    imageUrl = verifierServer.ImageUrl,
                    endPoints = verifierServer.EndPoints.ToArray(),
                    verifierAddresses = addresses
                };
                verifierItems[i] = item;
            }

            return verifierItems;
        }

        public IEnumerator GetCAHolderInfo(string chainId, SuccessCallback<CAHolderInfo> successCallback, ErrorCallback errorCallback)
        {
            if(_connectionService == null)
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
                signature = signature,
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

        public void Reset()
        {
            ClearDIDWallet();
            _managementAccount = null;
        }

        public IWallet GetWallet()
        {
            return _managementAccount;
        }

        public IEnumerator AddManager(EditManagerParams editManagerParams, SuccessCallback<bool> successCallback,
            ErrorCallback errorCallback)
        {
            if (_managementAccount == null)
            {
                throw new Exception("Manager Account does not exist.");
            }
            
            yield return _contractProvider.GetContract(editManagerParams.chainId, (contract) =>
            {
                var addManagerInfoInput = new AddManagerInfoInput
                {
                    ManagerInfo = editManagerParams.managerInfo,
                    CaHash = Hash.LoadFromHex(editManagerParams.caHash)
                };
                
                StaticCoroutine.StartCoroutine(contract.SendTransactionAsync(_managementAccount, "AddManagerInfo", addManagerInfoInput, result =>
                {
                    successCallback(result.transactionResult.Status == TransactionResultStatus.Mined.ToString());
                }, errorCallback));
            }, errorCallback);
        }

        public IEnumerator RemoveManager(EditManagerParams param, SuccessCallback<bool> successCallback,
            ErrorCallback errorCallback)
        {
            if(_managementAccount == null)
            {
                errorCallback("Management Account does not exist.");
                yield break;
            }
            
            yield return _contractProvider.GetContract(param.chainId, (contract) =>
            {
                var removeManagerInfoInput = new RemoveManagerInfoInput
                {
                    CaHash = Hash.LoadFromHex(param.caHash)
                };
                StaticCoroutine.StartCoroutine(contract.SendTransactionAsync(_managementAccount, "RemoveManagerInfo", removeManagerInfoInput, result =>
                {
                    if (IsCurrentAccount(param))
                    {
                        ClearDIDWallet();
                    }
                
                    successCallback(result.transactionResult.Status == "MINED");
                }, errorCallback));
            }, errorCallback);
        }

        private bool IsCurrentAccount(EditManagerParams param)
        {
            return param.managerInfo?.Address.ToString() == _managementAccount.Address && _caInfoMap[param.chainId].caHash == param.caHash;
        }

        private void ClearDIDWallet()
        {
            _caInfoMap.Clear();
            _accountInfo = new AccountInfo();
        }

        public bool IsLoggedIn()
        {
            return _caInfoMap.Count > 0 && _accountInfo.IsLoggedIn();
        }
    }
}