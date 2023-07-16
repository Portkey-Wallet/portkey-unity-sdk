using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf;
using Moq;
using NUnit.Framework;
using Portkey.Contract;
using Portkey.Contracts.CA;
using Portkey.Core;
using Portkey.DID;
using Portkey.Network;
using Portkey.Storage;
using Portkey.Utilities;
using UnityEngine.TestTools;
using Guardian = Portkey.Core.Guardian;
using GuardianList = Portkey.Core.GuardianList;

namespace Portkey.Test
{
    public class DIDWalletTest
    {
        private IPortkeySocialService socialService = new PortkeySocialServiceMock();
        private IStorageSuite<string> storageSuite = new NonPersistentStorageMock<string>("");
        private IConnectService connectService = new ConnectService<RequestHttp>("", new RequestHttp());
        
        private static Mock<IAccountProvider<WalletAccount>> GetAccountProviderMock()
        {
            var accountProviderMock = new Mock<IAccountProvider<WalletAccount>>();
            accountProviderMock.Setup(provider => provider.CreateAccount())
                .Returns(new WalletAccount(
                    new BlockchainWallet("address_mock", 
                        "privateKey_mock", 
                        "mnemonic_mock",
                        "publicKey_mock")));
            return accountProviderMock;
        }
        
        private static Mock<IContractProvider> GetContractProviderMock<T>() where T : IMessage<T>, new()
        {
            // mock contract
            var contractMock = new Mock<IContract>();
            contractMock.Setup(contract => contract.CallTransactionAsync<T>(It.IsAny<BlockchainWallet>(),
                    It.IsAny<string>(), It.IsAny<IMessage>()))
                .Returns((BlockchainWallet wallet, string methodName, IMessage param) =>
                {
                    return new Task<T>(() => new T());
                });
            
            // mock provider
            var contractProviderMock = new Mock<IContractProvider>();
            contractProviderMock.Setup(provider => provider.GetContract(It.IsAny<string>(),
                    It.IsAny<SuccessCallback<IContract>>(), It.IsAny<ErrorCallback>()))
                .Returns((string chainId, SuccessCallback<IContract> successCallback, ErrorCallback errorCallback) =>
                {
                    successCallback(contractMock.Object);
                    return new List<string>().GetEnumerator();
                });
            return contractProviderMock;
        }
        
        private static Mock<IPortkeySocialService> GetSocialServiceMock()
        {
            var socialServiceMock = new Mock<IPortkeySocialService>();
            socialServiceMock.Setup(service => service.GetRegisterStatus(It.IsAny<string>(), It.IsAny<QueryOptions>(),
                    It.IsAny<SuccessCallback<RegisterStatusResult>>(), It.IsAny<ErrorCallback>()))
                .Returns((string sessionId, QueryOptions queryOptions, SuccessCallback<RegisterStatusResult> successCallback,
                    ErrorCallback errorCallback) =>
                {
                    successCallback(new RegisterStatusResult
                    {
                        caAddress = "caAddress_mock",
                        caHash = "caHash_mock",
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
                        caHash = "caHash_mock",
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
                                address = "address_mock",
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
            var contractProviderMock = GetContractProviderMock<AddManagerInfoInput>();

            var didWallet = new DIDWallet<WalletAccount>(socialServiceMock.Object, storageSuite, accountProviderMock.Object, connectService, contractProviderMock.Object);
            
            return didWallet.GetRegisterStatus("AELF_mock", "sessionId_mock", (response) =>
            {
                Assert.AreEqual(response.caAddress, "caAddress_mock");
                Assert.AreEqual(response.caHash, "caHash_mock");
            }, error =>
            {
                Assert.Fail(error);
            });
        }

        [UnityTest]
        public IEnumerator RegisterTest()
        {
            var accountProviderMock = GetAccountProviderMock();
            var socialServiceMock = GetSocialServiceMock();
            var contractProviderMock = GetContractProviderMock<AddManagerInfoInput>();

            var didWallet = new DIDWallet<WalletAccount>(socialServiceMock.Object, storageSuite, accountProviderMock.Object, connectService, contractProviderMock.Object);
            var registerParam = new RegisterParams
            {
                type = AccountType.Google,
                chainId = "AELF_mock"
            };
            
            return didWallet.Register(registerParam, (result) =>
            {
                accountProviderMock.Verify(provider => provider.CreateAccount(), Times.AtMostOnce());
                socialServiceMock.Verify(service => service.Register(It.IsAny<RegisterParams>(), It.IsAny<SuccessCallback<SessionIdResult>>(), It.IsAny<ErrorCallback>()), Times.AtMostOnce());
                socialServiceMock.Verify(service => service.GetRegisterStatus(It.IsAny<string>(), It.IsAny<QueryOptions>(), It.IsAny<SuccessCallback<RegisterStatusResult>>(), It.IsAny<ErrorCallback>()), Times.AtMostOnce());
                socialServiceMock.Verify(service => service.GetHolderInfo(It.IsAny<GetHolderInfoParams>(), It.IsAny<SuccessCallback<IHolderInfo>>(), It.IsAny<ErrorCallback>()), Times.AtMostOnce());
                Assert.AreEqual(result.Status.caAddress, "caAddress_mock");
                Assert.AreEqual(result.Status.caHash, "caHash_mock");
                Assert.AreEqual(result.SessionId, "sessionId_mock");
            }, error =>
            {
                Assert.Fail(error);
            });
        }
        
        [UnityTest]
        public IEnumerator RegisterTwiceTest()
        {
            var accountProviderMock = GetAccountProviderMock();
            var socialServiceMock = GetSocialServiceMock();
            var contractProviderMock = GetContractProviderMock<AddManagerInfoInput>();

            var didWallet = new DIDWallet<WalletAccount>(socialServiceMock.Object, storageSuite, accountProviderMock.Object, connectService, contractProviderMock.Object);
            var registerParam = new RegisterParams
            {
                type = AccountType.Google,
                chainId = "AELF_mock",
                loginGuardianIdentifier = "loginGuardianIdentifier_mock"
            };
            
            return didWallet.Register(registerParam, (result) =>
            {
                StaticCoroutine.StartCoroutine(didWallet.Register(registerParam, (result) =>
                {
                    Assert.Fail("Should not be here");
                }, error =>
                {
                    accountProviderMock.Verify(provider => provider.CreateAccount(), Times.AtMostOnce());
                    socialServiceMock.Verify(service => service.Register(It.IsAny<RegisterParams>(), It.IsAny<SuccessCallback<SessionIdResult>>(), It.IsAny<ErrorCallback>()), Times.AtMostOnce());
                    socialServiceMock.Verify(service => service.GetRegisterStatus(It.IsAny<string>(), It.IsAny<QueryOptions>(), It.IsAny<SuccessCallback<RegisterStatusResult>>(), It.IsAny<ErrorCallback>()), Times.AtMostOnce());
                    socialServiceMock.Verify(service => service.GetHolderInfo(It.IsAny<GetHolderInfoParams>(), It.IsAny<SuccessCallback<IHolderInfo>>(), It.IsAny<ErrorCallback>()), Times.AtMostOnce());
                    
                    Assert.AreEqual(error, "Account already logged in.");
                }));
            }, error =>
            {
                Assert.Fail(error);
            });
        }
        
        [UnityTest]
        public IEnumerator GetLoginStatusTest()
        {
            var accountProviderMock = GetAccountProviderMock();
            var socialServiceMock = GetSocialServiceMock();
            var contractProviderMock = GetContractProviderMock<AddManagerInfoInput>();

            var didWallet = new DIDWallet<WalletAccount>(socialServiceMock.Object, storageSuite, accountProviderMock.Object, connectService, contractProviderMock.Object);
            
            return didWallet.GetLoginStatus("AELF_mock", "sessionId_mock", (response) =>
            {
                Assert.AreEqual(response.caAddress, "caAddress_mock");
                Assert.AreEqual(response.caHash, "caHash_mock");
            }, error =>
            {
                Assert.Fail(error);
            });
        }
        
        [UnityTest]
        public IEnumerator LoginTest()
        {
            var accountProviderMock = GetAccountProviderMock();
            var socialServiceMock = GetSocialServiceMock();
            var contractProviderMock = GetContractProviderMock<AddManagerInfoInput>();

            var didWallet = new DIDWallet<WalletAccount>(socialServiceMock.Object, storageSuite, accountProviderMock.Object, connectService, contractProviderMock.Object);
            var accountLoginParams = new AccountLoginParams("loginGuardianIdentifier_mock",
                                            new GuardiansApproved[] { new GuardiansApproved() },
                                                    "extraData_mock",
                                                        "chainId_mock",
                                                                new Context());
            
            return didWallet.Login(accountLoginParams, (result) =>
            {
                accountProviderMock.Verify(provider => provider.CreateAccount(), Times.AtMostOnce());
                socialServiceMock.Verify(service => service.Recovery(It.IsAny<RecoveryParams>(), It.IsAny<SuccessCallback<SessionIdResult>>(), It.IsAny<ErrorCallback>()), Times.AtMostOnce());
                socialServiceMock.Verify(service => service.GetRecoverStatus(It.IsAny<string>(), It.IsAny<QueryOptions>(), It.IsAny<SuccessCallback<RecoverStatusResult>>(), It.IsAny<ErrorCallback>()), Times.AtMostOnce());
                socialServiceMock.Verify(service => service.GetHolderInfo(It.IsAny<GetHolderInfoParams>(), It.IsAny<SuccessCallback<IHolderInfo>>(), It.IsAny<ErrorCallback>()), Times.AtMostOnce());
                Assert.AreEqual(result.Status.caAddress, "caAddress_mock");
                Assert.AreEqual(result.Status.caHash, "caHash_mock");
                Assert.AreEqual(result.SessionId, "sessionId_mock");
            }, error =>
            {
                Assert.Fail(error);
            });
        }
        
        [UnityTest]
        public IEnumerator LoginTwiceTest()
        {
            var accountProviderMock = GetAccountProviderMock();
            var socialServiceMock = GetSocialServiceMock();
            var contractProviderMock = GetContractProviderMock<AddManagerInfoInput>();

            var didWallet = new DIDWallet<WalletAccount>(socialServiceMock.Object, storageSuite, accountProviderMock.Object, connectService, contractProviderMock.Object);
            var accountLoginParams = new AccountLoginParams("loginGuardianIdentifier_mock",
                                                    new GuardiansApproved[] { new GuardiansApproved() },
                                                            "extraData_mock",
                                                            "chainId_mock",
                                                            new Context());
            
            return didWallet.Login(accountLoginParams, (result) =>
            {
                StaticCoroutine.StartCoroutine(didWallet.Login(accountLoginParams, (result) =>
                {
                    Assert.Fail("Should not be here");
                }, error =>
                {
                    accountProviderMock.Verify(provider => provider.CreateAccount(), Times.AtMostOnce());
                    socialServiceMock.Verify(service => service.Recovery(It.IsAny<RecoveryParams>(), It.IsAny<SuccessCallback<SessionIdResult>>(), It.IsAny<ErrorCallback>()), Times.AtMostOnce());
                    socialServiceMock.Verify(service => service.GetRecoverStatus(It.IsAny<string>(), It.IsAny<QueryOptions>(), It.IsAny<SuccessCallback<RecoverStatusResult>>(), It.IsAny<ErrorCallback>()), Times.AtMostOnce());
                    socialServiceMock.Verify(service => service.GetHolderInfo(It.IsAny<GetHolderInfoParams>(), It.IsAny<SuccessCallback<IHolderInfo>>(), It.IsAny<ErrorCallback>()), Times.AtMostOnce());
                    
                    Assert.AreEqual(error, "Account already logged in.");
                }));
            }, error =>
            {
                Assert.Fail(error);
            });
        }
    }
}