using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Portkey.Core;
using Portkey.DID;
using Portkey.Network;
using Portkey.Storage;
using Portkey.Utilities;
using UnityEngine.TestTools;

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

            var didWallet = new DIDWallet<WalletAccount>(socialServiceMock.Object, storageSuite, accountProviderMock.Object, connectService);
            
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

            var didWallet = new DIDWallet<WalletAccount>(socialServiceMock.Object, storageSuite, accountProviderMock.Object, connectService);
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

            var didWallet = new DIDWallet<WalletAccount>(socialServiceMock.Object, storageSuite, accountProviderMock.Object, connectService);
            var registerParam = new RegisterParams
            {
                type = AccountType.Google,
                chainId = "AELF_mock"
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
    }
}