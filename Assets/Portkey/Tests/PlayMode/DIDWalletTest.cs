using System.Collections;
using NUnit.Framework;
using Portkey.Core;
using Portkey.DID;
using Portkey.Network;
using Portkey.Storage;
using UnityEngine.TestTools;

namespace Portkey.Test
{
    public class DIDWalletTest
    {
        private IPortkeySocialService socialService = new PortkeySocialServiceMock();
        private IStorageSuite<string> storageSuite = new NonPersistentStorageMock<string>("");
        private IAccountProvider<WalletAccount> accountProvider = new AccountProvider();
        private IConnectService connectService = new ConnectService<RequestHttp>("", new RequestHttp());

        [UnityTest]
        public IEnumerator GetRegisterStatusTest()
        {
            var didWallet = new DIDWallet<WalletAccount>(socialService, storageSuite, accountProvider, connectService);
            
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
            var didWallet = new DIDWallet<WalletAccount>(socialService, storageSuite, accountProvider, connectService);
            var registerParam = new RegisterParams
            {
                type = AccountType.Google,
                chainId = "AELF_mock"
            };
            
            return didWallet.Register(registerParam, (result) =>
            {
                Assert.AreEqual(result.Status.caAddress, "caAddress_mock");
                Assert.AreEqual(result.Status.caHash, "caHash_mock");
                Assert.AreEqual(result.SessionId, "sessionId_mock");
            }, error =>
            {
                Assert.Fail(error);
            });
        }
    }
}