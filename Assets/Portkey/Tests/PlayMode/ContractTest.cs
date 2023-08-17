using System.Collections;
using System.Collections.Generic;
using AElf.Types;
using Google.Protobuf;
using Moq;
using NUnit.Framework;
using Portkey.Chain.Dto;
using Portkey.Contract;
using Portkey.Contracts.CA;
using Portkey.Core;
using Portkey.DID;
using Portkey.Encryption;
using Portkey.Utilities;
using UnityEngine.TestTools;
using Empty = Google.Protobuf.WellKnownTypes.Empty;

namespace Portkey.Test
{
    /// <summary>
    /// ContractTest is a class to test ContractBasic functionalities.
    /// </summary>
    public class ContractTest
    {
        private static readonly KeyPair KeyPair = new KeyPair("TrmPcaqbqmbrztv6iJuN4zeuDmQxvjF5ujbvDDQo9Q1B4ye2T",
                                                                        "83829798ac92d428ed13b29fe60ace1a7a10e7a347bdc9f23a85615339068f1c",
                                                                        "045ab0516fda4eeb504ad8d7ce8a2a24e5af6004afa9ff3ee26f2c697e334be48c31597e1905711a6aa749fc475787000b5d6260bcf0d457f23c60aa60bb6c8602");
        private static readonly AElfWallet Wallet = new AElfWallet(KeyPair, new AESEncryption());
        
        private static Mock<IChainProvider> GetChainProviderMock(Mock<IChain> chainMock)
        {
            var chainProviderMock = new Mock<IChainProvider>();
            chainProviderMock.Setup(provider => provider.GetChain(It.IsAny<string>()))
                .Returns((string chainId, SuccessCallback<IContract> successCallback, ErrorCallback errorCallback) =>
                {
                    return chainMock.Object;
                });
            return chainProviderMock;
        }
        
        private static Mock<IChain> GetChainMock()
        {
            var chainMock = new Mock<IChain>();
            chainMock.Setup(chain => chain.GenerateTransactionAsync(It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IMessage>(), It.IsAny<SuccessCallback<Transaction>>(),
                    It.IsAny<ErrorCallback>()))
                .Callback((string from, string to, string methodName, IMessage input, SuccessCallback<Transaction> successCallback, ErrorCallback errorCallback) => successCallback?.Invoke(new Transaction
                {
                    From = "TrmPcaqbqmbrztv6iJuN4zeuDmQxvjF5ujbvDDQo9Q1B4ye2T".ToAddress(),
                    To = "TrmPcaqbqmbrztv6iJuN4zeuDmQxvjF5ujbvDDQo9Q1B4ye2T".ToAddress(),
                    MethodName = methodName,
                    Params = input.ToByteString(),
                    RefBlockNumber = 10,
                    RefBlockPrefix = ByteString.CopyFromUtf8("prefix_mock")
                }));
            chainMock.Setup(chain => chain.ExecuteTransactionAsync(It.IsAny<ExecuteTransactionDto>(),
                    It.IsAny<SuccessCallback<string>>(), It.IsAny<ErrorCallback>()))
                .Returns(new List<int>().GetEnumerator())
                .Callback((ExecuteTransactionDto input, SuccessCallback<string> successCallback, ErrorCallback errorCallback) => successCallback?.Invoke("0ab3010a220a20594ebf395cdba58b0e725d71eb3c1a17d57662b0667a92f770f341d4e794b76b1207506f72746b65791a4368747470733a2f2f706f72746b65792d6469642e73332e61702d6e6f727468656173742d312e616d617a6f6e6177732e636f6d2f696d672f506f72746b65792e706e67221b687474703a2f2f3139322e3136382e36362e3234303a31363031302a220a20e8068f3e64503c3d41a7a5cd8bd7d9a4e25c0caf7fea1e3699f08579ad626b030ab3010a220a20b06fb2fb382204673f2e48511f0488444cf8da7929ffbf2e80efdbd1e1aba05112074d696e657276611a4368747470733a2f2f706f72746b65792d6469642e73332e61702d6e6f727468656173742d312e616d617a6f6e6177732e636f6d2f696d672f4d696e657276612e706e67221b687474703a2f2f3139322e3136382e36362e3234303a31363032302a220a200684b7c48f47ce3e513fc2fa48e44cd5582e5a525d9e257fd908c265ff2fac400abd010a220a20f793489110aa3c1eb026b50308af4b6f1c4cf3d94d10300028d9af3ee8fcc0d6120c446f6b65774361706974616c1a4868747470733a2f2f706f72746b65792d6469642e73332e61702d6e6f727468656173742d312e616d617a6f6e6177732e636f6d2f696d672f446f6b65774361706974616c2e706e67221b687474703a2f2f3139322e3136382e36362e3234303a31363033302a220a20e73d31be0072d6895475a70b5a384d7428e7700ff50274b4d3343faca738824c0aaf010a220a20ca29e14f096608ee7afcb0506625cbfacb17c33289f91a8bf626d34ce9a70cd5120547617573731a4168747470733a2f2f706f72746b65792d6469642e73332e61702d6e6f727468656173742d312e616d617a6f6e6177732e636f6d2f696d672f47617573732e706e67221b687474703a2f2f3139322e3136382e36362e3234303a31363034302a220a2009de663a130e78cbe47b052ae186ebb0eecca1d7dc858b71702391d3095fedc10ac1010a220a2049f1846852d3c69c68da3781e840ab556d46d5a0a72f45dad120920c9c99dafa120e43727970746f477561726469616e1a4a68747470733a2f2f706f72746b65792d6469642e73332e61702d6e6f727468656173742d312e616d617a6f6e6177732e636f6d2f696d672f43727970746f477561726469616e2e706e67221b687474703a2f2f3139322e3136382e36362e3234303a31363035302a220a20d211cf723f17396f7cf282dc03a26368e82a026d56989303f35665ecb43c7086"));
            chainMock.Setup(chain => chain.GetBlockByHeightAsync(It.IsAny<long>(), It.IsAny<SuccessCallback<BlockDto>>(),
                    It.IsAny<ErrorCallback>(), It.IsAny<bool>()))
                .Returns(new List<int>().GetEnumerator())
                .Callback((long blockHeight, SuccessCallback<BlockDto> successCallback, ErrorCallback errorCallback, bool includeTransactions) => successCallback?.Invoke(new BlockDto
                {
                    BlockHash = "03bd0cea9730bcfc8045248fd7f4841ea19315995c44801a3dfede0ca872f808"
                }));
            chainMock.Setup(chain => chain.SendTransactionAsync(It.IsAny<SendTransactionInput>(),
                    It.IsAny<SuccessCallback<SendTransactionOutput>>(), It.IsAny<ErrorCallback>()))
                .Returns(new List<int>().GetEnumerator())
                .Callback((SendTransactionInput input, SuccessCallback<SendTransactionOutput> successCallback, ErrorCallback errorCallback) => successCallback?.Invoke(new SendTransactionOutput()));
            chainMock.Setup(chain => chain.GetTransactionResultAsync(It.IsAny<string>(),
                    It.IsAny<SuccessCallback<TransactionResultDto>>(), It.IsAny<ErrorCallback>()))
                .Callback((string transactionId, SuccessCallback<TransactionResultDto> successCallback, ErrorCallback errorCallback) => successCallback?.Invoke(new TransactionResultDto()));
            return chainMock;
        }

        private static Mock<IWallet> GetWalletMock()
        {
            var walletMock = new Mock<IWallet>();
            walletMock.Setup(wallet => wallet.SignTransaction(It.IsAny<Transaction>())).Returns(new Transaction());

            return walletMock;
        }
        
        /// <summary>
        /// Create a test to get verifier servers from contract.
        /// </summary>
        [UnityTest]
        public IEnumerator ContractGetVerifierServersTest()
        {
            var done = false;
            
            var config = UnityHelper.GetConfig<PortkeyConfig>("PortkeyTestConfig");
            var testMainChain = config.ChainInfos["TestMain"];
            var chainMock = GetChainMock();
            var walletMock = GetWalletMock();
            IContract contract = new ContractBasic(chainMock.Object, testMainChain.ContractInfos["CAContract"].ContractAddress);

            yield return contract.CallTransactionAsync<GetVerifierServersOutput>(walletMock.Object, "GetVerifierServers", new Empty(),
                result =>
                {
                    Assert.AreEqual("594ebf395cdba58b0e725d71eb3c1a17d57662b0667a92f770f341d4e794b76b", result.VerifierServers[0].Id.ToHex());
                    done = true;
                }, error =>
                {
                    done = true;
                    Assert.Fail("Should not have error.");
                });
            
            while (!done)
                yield return null;
        }
        
        /// <summary>
        /// Create a test to make sure the handling of errors for contract basic is working.
        /// </summary>
        [UnityTest]
        public IEnumerator ContractGetVerifierServersExceptionTest()
        {
            var done = false;
            
            const string EXCEPION_MESSAGE = "Failed to execute tx.";
            LogAssert.ignoreFailingMessages = true;
            
            var config = UnityHelper.GetConfig<PortkeyConfig>("PortkeyTestConfig");
            var testMainChain = config.ChainInfos["TestMain"];
            var chainMock = GetChainMock();
            chainMock.Setup(chain => chain.ExecuteTransactionAsync(It.IsAny<ExecuteTransactionDto>(),
                    It.IsAny<SuccessCallback<string>>(), It.IsAny<ErrorCallback>()))
                .Returns(new List<int>().GetEnumerator())
                .Callback((ExecuteTransactionDto input, SuccessCallback<string> successCallback, ErrorCallback errorCallback) => errorCallback?.Invoke(EXCEPION_MESSAGE));

            IContract contract = new ContractBasic(chainMock.Object, testMainChain.ContractInfos["CAContract"].ContractAddress);

            yield return contract.CallTransactionAsync<GetVerifierServersOutput>(Wallet, "GetVerifierServers", new Empty(),
                result =>
                {
                    done = true;
                    Assert.Fail("Should not have been successful.");
                }, error =>
                {
                    done = true;
                    Assert.AreEqual(EXCEPION_MESSAGE, error);
                });
            
            while (!done)
                yield return null;
        }
        
        [UnityTest]
        public IEnumerator ContractAddManagerInfoTest()
        {
            var done = false;
            const string TRANSACTION_ID = "0x1234567890";
            
            var config = UnityHelper.GetConfig<PortkeyConfig>("PortkeyTestConfig");
            var testMainChain = config.ChainInfos["TestMain"];
            var chainMock = GetChainMock();
            chainMock.Setup(chain => chain.GetTransactionResultAsync(It.IsAny<string>(),
                    It.IsAny<SuccessCallback<TransactionResultDto>>(), It.IsAny<ErrorCallback>()))
                .Callback((string transactionId, SuccessCallback<TransactionResultDto> successCallback, ErrorCallback errorCallback) => successCallback?.Invoke(new TransactionResultDto
                {
                    Status = "Mined",
                    TransactionId = TRANSACTION_ID
                }));


            IContract contract = new ContractBasic(chainMock.Object, testMainChain.ContractInfos["CAContract"].ContractAddress);
            var input = new AddManagerInfoInput
            {
                CaHash = Hash.LoadFromHex("594ebf395cdba58b0e725d71eb3c1a17d57662b0667a92f770f341d4e794b76b")
            };
            
            yield return contract.SendTransactionAsync(Wallet, "AddManagerInfo", input, result =>
            {
                done = true;
                Assert.AreEqual(TRANSACTION_ID, result.transactionResult.TransactionId);
            }, error =>
            {
                done = true;
                Assert.Fail("Should not have error.");
            });
            
            while (!done)
                yield return null;
        }
    }
}
