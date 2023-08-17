using System;
using System.Collections;
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
                .Callback((ExecuteTransactionDto input, SuccessCallback<string> successCallback, ErrorCallback errorCallback) => successCallback?.Invoke("03bd0cea9730bcfc8045248fd7f4841ea19315995c44801a3dfede0ca872f808"));
            chainMock.Setup(chain => chain.GetBlockByHeightAsync(It.IsAny<long>(), It.IsAny<SuccessCallback<BlockDto>>(),
                    It.IsAny<ErrorCallback>(), It.IsAny<bool>()))
                .Callback((long blockHeight, SuccessCallback<BlockDto> successCallback, ErrorCallback errorCallback, bool includeTransactions) => successCallback?.Invoke(new BlockDto()));
            chainMock.Setup(chain => chain.SendTransactionAsync(It.IsAny<SendTransactionInput>(),
                    It.IsAny<SuccessCallback<SendTransactionOutput>>(), It.IsAny<ErrorCallback>()))
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
                    done = true;
                    Assert.AreEqual("594ebf395cdba58b0e725d71eb3c1a17d57662b0667a92f770f341d4e794b76b", result.VerifierServers[0].Id.ToHex());
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
            
            /*
            Mock<IChain> GetChainMock()
            {
                var chainMock = new Mock<IChain>();
                chainMock.Setup(contract => contract.GenerateTransactionAsync(It.IsAny<string>(),
                        It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IMessage>(), It.IsAny<SuccessCallback<Transaction>>(),
                        It.IsAny<ErrorCallback>()))
                    .Callback((string? from, string? to, string methodName, IMessage input, SuccessCallback<Transaction> successCallback, ErrorCallback errorCallback) => successCallback?.Invoke(new Transaction()));
                chainMock.Setup(contract => contract.ExecuteTransactionAsync(It.IsAny<ExecuteTransactionDto>(),
                        It.IsAny<SuccessCallback<string>>(), It.IsAny<ErrorCallback>()))
                    .Callback((ExecuteTransactionDto input, SuccessCallback<string> successCallback, ErrorCallback errorCallback) => successCallback?.Invoke("success_mock"));
                chainMock.Setup(contract => contract.GetBlockByHeightAsync(It.IsAny<long>(), It.IsAny<SuccessCallback<BlockDto>>(),
                        It.IsAny<ErrorCallback>(), It.IsAny<bool>()))
                    .Callback((long blockHeight, SuccessCallback<BlockDto> successCallback, ErrorCallback errorCallback, bool includeTransactions) => successCallback?.Invoke(new BlockDto()));
                chainMock.Setup(contract => contract.SendTransactionAsync(It.IsAny<SendTransactionInput>(),
                        It.IsAny<SuccessCallback<SendTransactionOutput>>(), It.IsAny<ErrorCallback>()))
                    .Callback((SendTransactionInput input, SuccessCallback<SendTransactionOutput> successCallback, ErrorCallback errorCallback) => successCallback?.Invoke(new SendTransactionOutput()));
                chainMock.Setup(contract => contract.GetTransactionResultAsync(It.IsAny<string>(),
                        It.IsAny<SuccessCallback<TransactionResultDto>>(), It.IsAny<ErrorCallback>()))
                    .Callback((string transactionId, SuccessCallback<TransactionResultDto> successCallback, ErrorCallback errorCallback) => successCallback?.Invoke(new TransactionResultDto()));
                return chainMock;
            }*/
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
