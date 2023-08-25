using System;
using System.Collections;
using System.Collections.Generic;
using AElf;
using Google.Protobuf;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using Portkey.Chain.Dto;
using Portkey.Contracts.CA;
using Portkey.Core;
using Portkey.DID;
using Portkey.Encryption;
using Portkey.Network;
using Portkey.Storage;
using Portkey.Utilities;
using UnityEngine;
using UnityEngine.TestTools;
using Guardian = Portkey.Core.Guardian;
using GuardianList = Portkey.Core.GuardianList;

namespace Portkey.Test
{
    public class DIDAccountTest
    {
        private const string CAHASH_MOCK = "594ebf395cdba58b0e725d71eb3c1a17d57662b0667a92f770f341d4e794b76b";
        
        public class DidAccountMock : DIDAccount
        {
            public Dictionary<string, CAInfo> GetCAInfoMap()
            {
                return _data.caInfoMap;
            }
            
            public string GetLoginAccount()
            {
                return _data.socialInfo.LoginAccount;
            }
            
            public string GetEncryptedPrivateKey()
            {
                return _data.aesPrivateKey;
            }

            public new IEnumerator GetLoginStatus(string chainId, string sessionId,
                SuccessCallback<RecoverStatusResult> successCallback, ErrorCallback errorCallback)
            {
                yield return base.GetLoginStatus(chainId, sessionId, successCallback, errorCallback);
            }

            public new IEnumerator GetRegisterStatus(string chainId, string sessionId,
                SuccessCallback<RegisterStatusResult> successCallback, ErrorCallback errorCallback)
            {
                yield return base.GetRegisterStatus(chainId, sessionId, successCallback, errorCallback);
            }

            public DidAccountMock(IPortkeySocialService socialService, IStorageSuite<string> storageSuite, IWalletProvider walletProvider, IConnectionService connectionService, IContractProvider contractProvider, IEncryption encryption) : base(socialService, storageSuite, walletProvider, connectionService, contractProvider, encryption)
            {
            }
        }
        
        private IStorageSuite<string> _storageSuite = new NonPersistentStorageMock<string>("");
        private IConnectionService _connectionService = new ConnectionService<RequestHttp>("", new RequestHttp());
        private static readonly IEncryption Encryption = new AESEncryption();

        private static readonly KeyPair KeyPair = 
            new KeyPair("Q9GTUEz6tJVXdFTTxPhg8MZwWQ4LoHpPSPExRsPb2tBxdruqb",
                            "b1eae0819f3af0189283342c79adcac9028b251da52056e1e5b2ba79a8b4ccf1",
                            "0473eeb8965c3ebabc388055b7a2a8b8f9f4ea0e546055b7207a23a921e9642c5f68a4f0d9e66efba116d3adc018e8bc5eea5f6600b60168ac73e5db4b6cea693a");

        private static Mock<IWalletProvider> GetAccountProviderMock()
        {
            var accountProviderMock = new Mock<IWalletProvider>();
            accountProviderMock.Setup(provider => provider.Create())
                .Returns(new AElfWallet(KeyPair, Encryption));
            return accountProviderMock;
        }
        
        private static Mock<IEncryption> GetEncryptionMock()
        {
            var encryptionMock = new Mock<IEncryption>();
            encryptionMock.Setup(provider => provider.Encrypt(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string data, string password) => "encrypted_mock".GetBytes());
            encryptionMock.Setup(provider => provider.Decrypt(It.IsAny<byte[]>(), It.IsAny<string>()))
                .Returns((byte[] data, string password) => "decrypted_mock");
            return encryptionMock;
        }

        private static Mock<IContract> GetContractMock<T>() where T : IMessage<T>, new()
        {
            var contractMock = new Mock<IContract>();
            contractMock.Setup(contract => contract.CallAsync<T>(It.IsAny<IWallet>(),
                    It.IsAny<string>(), It.IsAny<IMessage>(), It.IsAny<SuccessCallback<T>>(),
                    It.IsAny<ErrorCallback>()))
                .Callback((IWallet wallet, string methodName, IMessage param, SuccessCallback<T> successCallback, ErrorCallback errorCallback) => successCallback?.Invoke(new T()))
                .Returns((IWallet wallet, string methodName, IMessage param, SuccessCallback<T> successCallback, ErrorCallback errorCallback) => new List<string>().GetEnumerator());
            contractMock.Setup(contract => contract.SendAsync(It.IsAny<IWallet>(),
                    It.IsAny<string>(), It.IsAny<IMessage>(), It.IsAny<SuccessCallback<IContract.TransactionInfoDto>>(),
                    It.IsAny<ErrorCallback>()))
                .Callback((IWallet wallet, string methodName, IMessage param, SuccessCallback<IContract.TransactionInfoDto> successCallback, ErrorCallback errorCallback) => successCallback?.Invoke(new IContract.TransactionInfoDto { transactionResult = new TransactionResultDto { Status = "MINED" }}))
                .Returns((IWallet wallet, string methodName, IMessage param, SuccessCallback<IContract.TransactionInfoDto> successCallback, ErrorCallback errorCallback) => new List<string>().GetEnumerator());
            return contractMock;
        }

        private static Mock<IContractProvider> GetContractProviderMock(Mock<IContract> contractMock)
        {
            var contractProviderMock = new Mock<IContractProvider>();
            contractProviderMock.Setup(provider => provider.GetContract(It.IsAny<string>(),
                    It.IsAny<SuccessCallback<IContract>>(), It.IsAny<ErrorCallback>()))
                .Callback((string chainId, SuccessCallback<IContract> successCallback, ErrorCallback errorCallback) => successCallback?.Invoke(contractMock.Object))
                .Returns((string chainId, SuccessCallback<IContract> successCallback, ErrorCallback errorCallback) => new List<string>().GetEnumerator());
            return contractProviderMock;
        }
        
        private static Mock<IPortkeySocialService> GetSocialServiceMock()
        {
            var socialServiceMock = new Mock<IPortkeySocialService>();
            socialServiceMock.Setup(service => service.GetRecoverStatus(It.IsAny<string>(), It.IsAny<QueryOptions>(),
                    It.IsAny<SuccessCallback<RecoverStatusResult>>(), It.IsAny<ErrorCallback>()))
                .Returns((string sessionId, QueryOptions queryOptions, SuccessCallback<RecoverStatusResult> successCallback,
                    ErrorCallback errorCallback) =>
                {
                    successCallback(new RecoverStatusResult
                    {
                        caAddress = "caAddress_mock",
                        caHash = CAHASH_MOCK,
                        recoveryStatus = "pass"
                    });

                    return new List<string>().GetEnumerator();
                });
            socialServiceMock.Setup(service => service.Recovery(It.IsAny<RecoveryParams>(),
                    It.IsAny<SuccessCallback<SessionIdResult>>(), It.IsAny<ErrorCallback>()))
                .Returns((RecoveryParams registerParams, SuccessCallback<SessionIdResult> successCallback,
                    ErrorCallback errorCallback) =>
                {
                    successCallback(new SessionIdResult
                    {
                        sessionId = "sessionId_mock"
                    });

                    return new List<string>().GetEnumerator();
                });
            socialServiceMock.Setup(service => service.GetRegisterStatus(It.IsAny<string>(), It.IsAny<QueryOptions>(),
                    It.IsAny<SuccessCallback<RegisterStatusResult>>(), It.IsAny<ErrorCallback>()))
                .Returns((string sessionId, QueryOptions queryOptions, SuccessCallback<RegisterStatusResult> successCallback,
                    ErrorCallback errorCallback) =>
                {
                    successCallback(new RegisterStatusResult
                    {
                        caAddress = "caAddress_mock",
                        caHash = CAHASH_MOCK,
                        registerStatus = "pass"
                    });

                    return new List<string>().GetEnumerator();
                });
            socialServiceMock.Setup(service => service.Register(It.IsAny<RegisterParams>(),
                    It.IsAny<SuccessCallback<SessionIdResult>>(), It.IsAny<ErrorCallback>()))
                .Returns((RegisterParams registerParams, SuccessCallback<SessionIdResult> successCallback,
                    ErrorCallback errorCallback) =>
                {
                    successCallback(new SessionIdResult
                    {
                        sessionId = "sessionId_mock"
                    });

                    return new List<string>().GetEnumerator();
                });
            socialServiceMock.Setup(service => service.GetHolderInfo(It.IsAny<GetHolderInfoParams>(),
                    It.IsAny<SuccessCallback<IHolderInfo>>(), It.IsAny<ErrorCallback>()))
                .Returns((GetHolderInfoParams requestParams, SuccessCallback<IHolderInfo> successCallback,
                    ErrorCallback errorCallback) =>
                {
                    successCallback(new IHolderInfo
                    {
                        caAddress = "caAddress_mock",
                        caHash = CAHASH_MOCK,
                        guardianList = new GuardianList {
                            guardians = new Guardian[]
                            {
                                new Guardian
                                {
                                    guardianIdentifier = "guardianIdentifier_mock",
                                }
                            }
                        },
                        managerInfos = new Manager[]
                        {
                            new Manager
                            {
                                address = KeyPair.Address,
                            }
                        }
                    });

                    return new List<string>().GetEnumerator();
                });
            return socialServiceMock;
        }
        
        [UnityTest]
        public IEnumerator GetRegisterStatusTest()
        {
            var accountProviderMock = GetAccountProviderMock();
            var socialServiceMock = GetSocialServiceMock();
            var contractMock = GetContractMock<AddManagerInfoInput>();
            var contractProviderMock = GetContractProviderMock(contractMock);
            var encryptionMock = GetEncryptionMock();

            var didWallet = new DidAccountMock(socialServiceMock.Object, _storageSuite, accountProviderMock.Object, _connectionService, contractProviderMock.Object, encryptionMock.Object);
            
            yield return didWallet.GetRegisterStatus("AELF_mock", "sessionId_mock", (response) =>
            {
                Assert.AreEqual(response.caAddress, "caAddress_mock");
                Assert.AreEqual(response.caHash, CAHASH_MOCK);
            }, Assert.Fail);
        }

        [UnityTest]
        public IEnumerator RegisterTest()
        {
            var accountProviderMock = GetAccountProviderMock();
            var socialServiceMock = GetSocialServiceMock();
            var contractMock = GetContractMock<AddManagerInfoInput>();
            var contractProviderMock = GetContractProviderMock(contractMock);
            var encryptionMock = GetEncryptionMock();
            
            var didWallet = new DIDAccount(socialServiceMock.Object, _storageSuite, accountProviderMock.Object, _connectionService, contractProviderMock.Object, encryptionMock.Object);
            var registerParam = new RegisterParams
            {
                type = AccountType.Google,
                chainId = "AELF_mock"
            };
            
            yield return didWallet.Register(registerParam, (result) =>
            {
                accountProviderMock.Verify(provider => provider.Create(), Times.Once());
                socialServiceMock.Verify(service => service.Register(It.IsAny<RegisterParams>(), It.IsAny<SuccessCallback<SessionIdResult>>(), It.IsAny<ErrorCallback>()), Times.Once());
                socialServiceMock.Verify(service => service.GetRegisterStatus(It.IsAny<string>(), It.IsAny<QueryOptions>(), It.IsAny<SuccessCallback<RegisterStatusResult>>(), It.IsAny<ErrorCallback>()), Times.Once());
                Assert.AreEqual(result.Status.caAddress, "caAddress_mock");
                Assert.AreEqual(result.Status.caHash, CAHASH_MOCK);
                Assert.AreEqual(result.SessionId, "sessionId_mock");
            }, Assert.Fail);
        }
        
        [UnityTest]
        public IEnumerator GetLoginStatusTest()
        {
            var accountProviderMock = GetAccountProviderMock();
            var socialServiceMock = GetSocialServiceMock();
            var contractMock = GetContractMock<AddManagerInfoInput>();
            var contractProviderMock = GetContractProviderMock(contractMock);
            var encryptionMock = GetEncryptionMock();
            
            var didWallet = new DidAccountMock(socialServiceMock.Object, _storageSuite, accountProviderMock.Object, _connectionService, contractProviderMock.Object, encryptionMock.Object);
            
            yield return didWallet.GetLoginStatus("AELF_mock", "sessionId_mock", (response) =>
            {
                Assert.AreEqual(response.caAddress, "caAddress_mock");
                Assert.AreEqual(response.caHash, CAHASH_MOCK);
            }, Assert.Fail);
        }
        
        [UnityTest]
        public IEnumerator LoginTest()
        {
            var accountProviderMock = GetAccountProviderMock();
            var socialServiceMock = GetSocialServiceMock();
            var contractMock = GetContractMock<AddManagerInfoInput>();
            var contractProviderMock = GetContractProviderMock(contractMock);
            var encryptionMock = GetEncryptionMock();
            
            var didWallet = new DIDAccount(socialServiceMock.Object, _storageSuite, accountProviderMock.Object, _connectionService, contractProviderMock.Object, encryptionMock.Object);
            var accountLoginParams = new AccountLoginParams
            {
                loginGuardianIdentifier = "loginGuardianIdentifier_mock",
                guardiansApprovedList = new GuardiansApproved[] { new GuardiansApproved() },
                chainId = "chainId_mock",
                extraData = "extraData_mock"
            };
            
            yield return didWallet.Login(accountLoginParams, (result) =>
            {
                accountProviderMock.Verify(provider => provider.Create(), Times.Once());
                socialServiceMock.Verify(service => service.Recovery(It.IsAny<RecoveryParams>(), It.IsAny<SuccessCallback<SessionIdResult>>(), It.IsAny<ErrorCallback>()), Times.Once());
                socialServiceMock.Verify(service => service.GetRecoverStatus(It.IsAny<string>(), It.IsAny<QueryOptions>(), It.IsAny<SuccessCallback<RecoverStatusResult>>(), It.IsAny<ErrorCallback>()), Times.Once());
                Assert.AreEqual(result.Status.caAddress, "caAddress_mock");
                Assert.AreEqual(result.Status.caHash, CAHASH_MOCK);
                Assert.AreEqual(result.SessionId, "sessionId_mock");
            }, Assert.Fail);
        }
        
        [UnityTest]
        public IEnumerator LogoutNotLoggedInTest()
        {
            var accountProviderMock = GetAccountProviderMock();
            var socialServiceMock = GetSocialServiceMock();
            var contractMock = GetContractMock<AddManagerInfoInput>();
            var contractProviderMock = GetContractProviderMock(contractMock);
            var encryptionMock = GetEncryptionMock();
            
            var didWallet = new DIDAccount(socialServiceMock.Object, _storageSuite, accountProviderMock.Object, _connectionService, contractProviderMock.Object, encryptionMock.Object);
            var param = new EditManagerParams
            {
                chainId = "chainId_mock"
            };
            
            yield return didWallet.Logout(param, (result) =>
            {
                Assert.Fail("Should not be logged in, therefore logout should fail.");
            }, Assert.Pass);
        }
        
        [UnityTest]
        public IEnumerator LogoutTest()
        {
            var accountProviderMock = GetAccountProviderMock();
            var socialServiceMock = GetSocialServiceMock();
            var contractMock = GetContractMock<AddManagerInfoInput>();
            var contractProviderMock = GetContractProviderMock(contractMock);
            var encryptionMock = GetEncryptionMock();
            
            var didWallet = new DIDAccount(socialServiceMock.Object, _storageSuite, accountProviderMock.Object, _connectionService, contractProviderMock.Object, encryptionMock.Object);
            var accountLoginParams = new AccountLoginParams
            {
                loginGuardianIdentifier = "loginGuardianIdentifier_mock",
                guardiansApprovedList = new GuardiansApproved[] { new GuardiansApproved() },
                chainId = "chainId_mock",
                extraData = "extraData_mock"
            };
            
            var done = false;
            
            yield return didWallet.Login(accountLoginParams, (result) =>
            {
                done = true;
            }, error =>
            {
                done = true;
                Assert.Fail(error);
            });
            
            while (!done)
            {
                yield return new WaitForSeconds(0.5f);
            }
            
            var logoutParam = new EditManagerParams
            {
                chainId = "chainId_mock"
            };
                
            yield return didWallet.Logout(logoutParam, (result) =>
            {
                contractProviderMock.Verify(provider => provider.GetContract(It.IsAny<string>(), It.IsAny<SuccessCallback<IContract>>(), It.IsAny<ErrorCallback>()), Times.Once());
                contractMock.Verify(contract => contract.SendAsync(It.IsAny<IWallet>(), "RemoveManagerInfo", It.IsAny<RemoveManagerInfoInput>(), It.IsAny<SuccessCallback<IContract.TransactionInfoDto>>(), It.IsAny<ErrorCallback>()), Times.Once());
                accountProviderMock.Verify(provider => provider.Create(), Times.Once());
                socialServiceMock.Verify(service => service.Recovery(It.IsAny<RecoveryParams>(), It.IsAny<SuccessCallback<SessionIdResult>>(), It.IsAny<ErrorCallback>()), Times.Once());
                socialServiceMock.Verify(service => service.GetRecoverStatus(It.IsAny<string>(), It.IsAny<QueryOptions>(), It.IsAny<SuccessCallback<RecoverStatusResult>>(), It.IsAny<ErrorCallback>()), Times.Once());
                Assert.AreEqual(result, true);
            }, Assert.Fail);
        }
        
        [UnityTest]
        public IEnumerator GetVerifierServersTest()
        {
            var accountProviderMock = GetAccountProviderMock();
            var socialServiceMock = GetSocialServiceMock();
            var contractMock = GetContractMock<GetVerifierServersOutput>();
            var contractProviderMock = GetContractProviderMock(contractMock);
            var encryptionMock = GetEncryptionMock();
            
            var didWallet = new DIDAccount(socialServiceMock.Object, _storageSuite, accountProviderMock.Object, _connectionService, contractProviderMock.Object, encryptionMock.Object);

            var done = false;
            yield return didWallet.GetVerifierServers("AELF", (result) =>
            {
                done = true;
                accountProviderMock.Verify(provider => provider.Create(), Times.Once());
                contractProviderMock.Verify(provider => provider.GetContract(It.IsAny<string>(), It.IsAny<SuccessCallback<IContract>>(), It.IsAny<ErrorCallback>()), Times.Once());
                contractMock.Verify(contract => contract.CallAsync<GetVerifierServersOutput>(It.IsAny<IWallet>(), It.Is((string s) => s == "GetVerifierServers"), It.IsAny<IMessage>(), It.IsAny<SuccessCallback<GetVerifierServersOutput>>(),
                    It.IsAny<ErrorCallback>()), Times.Once());
                Assert.AreNotEqual(null, result);
            }, error =>
            {
                done = true;
                Assert.Fail(error);
            });

            while (!done)
            {
                yield return new WaitForSeconds(0.5f);
            }

        }
        
        [UnityTest]
        public IEnumerator SaveTest()
        {
            const string PASSWORD = "myPassword";
            const string KEY = "myKey";
            
            var accountProviderMock = GetAccountProviderMock();
            var socialServiceMock = GetSocialServiceMock();
            var contractMock = GetContractMock<AddManagerInfoInput>();
            var contractProviderMock = GetContractProviderMock(contractMock);
            var encryption = new AESEncryption();
            
            var didWallet = new DidAccountMock(socialServiceMock.Object, _storageSuite, accountProviderMock.Object, _connectionService, contractProviderMock.Object, encryption);
            var accountLoginParams = new AccountLoginParams
            {
                loginGuardianIdentifier = "loginGuardianIdentifier_mock",
                guardiansApprovedList = new GuardiansApproved[] { new GuardiansApproved() },
                chainId = "chainId_mock",
                extraData = "extraData_mock"
            };
            
            yield return didWallet.Login(accountLoginParams, (result) =>
            {
                var actualLoginAccount = didWallet.GetLoginAccount();
                var actualCaInfoMap = didWallet.GetCAInfoMap();
                didWallet.Save(PASSWORD, KEY);
                didWallet.Load(PASSWORD, KEY);
                var afterLoginAccount = didWallet.GetLoginAccount();
                var afterCaInfoMap = didWallet.GetCAInfoMap();
                var afterEncryptedPrivateKey = didWallet.GetEncryptedPrivateKey();
                var afterPrivateKey = encryption.Decrypt(Convert.FromBase64String(afterEncryptedPrivateKey), PASSWORD);
                
                Assert.AreEqual(afterPrivateKey, KeyPair.PrivateKey);
                Assert.AreEqual(actualLoginAccount, afterLoginAccount);
                Assert.AreEqual(actualCaInfoMap["chainId_mock"].caAddress, afterCaInfoMap["chainId_mock"].caAddress);
            }, Assert.Fail);
        }
    }
}