using System;
using System.Threading.Tasks;
using AElf.Client.Dto;
using AElf.Client.Extensions;
using AElf.Types;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using NUnit.Framework;
using Portkey.Contract;
using Portkey.Contracts.CA;
using Portkey.Core;
using Portkey.Utilities;
using UnityEngine.TestTools;
using Transaction = AElf.Types.Transaction;

namespace Portkey.Test
{
    /// <summary>
    /// AElfChainMock is a mock of AElfChain, which is used to test the ContractBasic class.
    /// It basically mocks the method of AElfClient.
    /// </summary>
    public class AElfChainMock : IChain
    { 
        public delegate void BodyMock();

        private BodyMock GenerateTransactionAsyncBodyMock { get; set; }
        
        public AElfChainMock(BodyMock body = null)
        {
            GenerateTransactionAsyncBodyMock = body ?? (() => { });
        }
        
        public async Task<Transaction> GenerateTransactionAsync(string from, string to, string methodName, IMessage input)
        {
            await Task.Delay(200);
            
            var transactionAsync = new Transaction()
            {
                From = from.ToAddress(),
                To = Address.FromBase58(to),
                MethodName = methodName,
                Params = input.ToByteString()
            };

            GenerateTransactionAsyncBodyMock();
            
            return transactionAsync;
        }

        public async Task<string> ExecuteTransactionAsync(ExecuteTransactionDto input)
        {
            await Task.Delay(200);
            
            return "0ab3010a220a20594ebf395cdba58b0e725d71eb3c1a17d57662b0667a92f770f341d4e794b76b1207506f72746b65791a4368747470733a2f2f706f72746b65792d6469642e73332e61702d6e6f727468656173742d312e616d617a6f6e6177732e636f6d2f696d672f506f72746b65792e706e67221b687474703a2f2f3139322e3136382e36362e3234303a31363031302a220a20e8068f3e64503c3d41a7a5cd8bd7d9a4e25c0caf7fea1e3699f08579ad626b030ab3010a220a20b06fb2fb382204673f2e48511f0488444cf8da7929ffbf2e80efdbd1e1aba05112074d696e657276611a4368747470733a2f2f706f72746b65792d6469642e73332e61702d6e6f727468656173742d312e616d617a6f6e6177732e636f6d2f696d672f4d696e657276612e706e67221b687474703a2f2f3139322e3136382e36362e3234303a31363032302a220a200684b7c48f47ce3e513fc2fa48e44cd5582e5a525d9e257fd908c265ff2fac400abd010a220a20f793489110aa3c1eb026b50308af4b6f1c4cf3d94d10300028d9af3ee8fcc0d6120c446f6b65774361706974616c1a4868747470733a2f2f706f72746b65792d6469642e73332e61702d6e6f727468656173742d312e616d617a6f6e6177732e636f6d2f696d672f446f6b65774361706974616c2e706e67221b687474703a2f2f3139322e3136382e36362e3234303a31363033302a220a20e73d31be0072d6895475a70b5a384d7428e7700ff50274b4d3343faca738824c0aaf010a220a20ca29e14f096608ee7afcb0506625cbfacb17c33289f91a8bf626d34ce9a70cd5120547617573731a4168747470733a2f2f706f72746b65792d6469642e73332e61702d6e6f727468656173742d312e616d617a6f6e6177732e636f6d2f696d672f47617573732e706e67221b687474703a2f2f3139322e3136382e36362e3234303a31363034302a220a2009de663a130e78cbe47b052ae186ebb0eecca1d7dc858b71702391d3095fedc10ac1010a220a2049f1846852d3c69c68da3781e840ab556d46d5a0a72f45dad120920c9c99dafa120e43727970746f477561726469616e1a4a68747470733a2f2f706f72746b65792d6469642e73332e61702d6e6f727468656173742d312e616d617a6f6e6177732e636f6d2f696d672f43727970746f477561726469616e2e706e67221b687474703a2f2f3139322e3136382e36362e3234303a31363035302a220a20d211cf723f17396f7cf282dc03a26368e82a026d56989303f35665ecb43c7086";
        }

        public Transaction SignTransaction(string privateKeyHex, Transaction transaction)
        {
            return transaction;
        }
    }

    /// <summary>
    /// ContractTest is a class to test ContractBasic functionalities.
    /// </summary>
    public class ContractTest
    {
        private readonly BlockchainWallet _wallet = new BlockchainWallet("TrmPcaqbqmbrztv6iJuN4zeuDmQxvjF5ujbvDDQo9Q1B4ye2T",
                                                                        "83829798ac92d428ed13b29fe60ace1a7a10e7a347bdc9f23a85615339068f1c",
                                                                        "mock");
        
        /// <summary>
        /// Create a test to get verifier servers from contract.
        /// </summary>
        [Test]
        public void ContractGetVerifierServersTest()
        {
            var config = PortkeyUtilities.GetConfig<PortkeyConfig>("PortkeyTestConfig");
            var testMainChain = config.ChainInfos["TestMain"];
            IChain aelfChainMock = new AElfChainMock();
            IContract contract = new ContractBasic(aelfChainMock, testMainChain.ContractInfos["CAContract"].ContractAddress);

            var result = UnityTestUtilities.RunAsyncMethodToSync(() => contract.CallTransactionAsync<GetVerifierServersOutput>(_wallet, "GetVerifierServers", new Empty()));
        
            Assert.AreEqual("594ebf395cdba58b0e725d71eb3c1a17d57662b0667a92f770f341d4e794b76b", result.VerifierServers[0].Id.ToHex());
        }
        
        /// <summary>
        /// Create a test to make sure the handling of errors for contract basic is working.
        /// </summary>
        [Test]
        public void ContractGetVerifierServersExceptionTest()
        {
            const string EXCEPION_MESSAGE = "Failed to execute tx.";
            LogAssert.ignoreFailingMessages = true;
            
            var config = PortkeyUtilities.GetConfig<PortkeyConfig>("PortkeyTestConfig");
            var testMainChain = config.ChainInfos["TestMain"];
            IChain aelfChainMock = new AElfChainMock((() => throw new Exception(EXCEPION_MESSAGE)));
            IContract contract = new ContractBasic(aelfChainMock, testMainChain.ContractInfos["CAContract"].ContractAddress);

            var result = UnityTestUtilities.RunAsyncMethodToSync(() => contract.CallTransactionAsync<GetVerifierServersOutput>(_wallet, "GetVerifierServers", new Empty()));
        
            Assert.AreEqual(0, result.VerifierServers.Count);
        }
    }
}
