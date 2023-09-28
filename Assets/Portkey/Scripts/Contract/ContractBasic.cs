using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AElf;
using AElf.Types;
using Google.Protobuf;
using Portkey.BrowserWalletExtension;
using Portkey.Chain.Dto;
using Portkey.Core;
using Portkey.Utilities;
using UnityEngine;

namespace Portkey.Contract
{
    /// <summary>
    /// ContractBasic is a basic contract class that can be used to call a contract method.
    /// </summary>
    public class ContractBasic : IContract
    {
        private const int MAX_POLL_TIMES = 30;
        
        private readonly IChain _chain;
        public string ContractAddress { get; protected set; }
        public string ChainId => _chain.ChainInfo.chainId;

        /// <summary>
        /// Constructor for ContractBasic to initialize the contract address and its respective chain.
        /// </summary>
        /// <param name="chain">The chain that the contract resides on.</param>
        /// <param name="contractAddress">The contract address related to this contract.</param>
        public ContractBasic(IChain chain, string contractAddress)
        {
            _chain = chain;
            ContractAddress = contractAddress ?? throw new ArgumentException("Contract address cannot be null.");
        }
        
        public IEnumerator CallAsync<T>(ISigningKey signingKey, string methodName, IMessage param, SuccessCallback<T> successCallback, ErrorCallback errorCallback) where T : IMessage<T>, new()
        {
            yield return _chain.GenerateTransactionAsync(signingKey.Address, ContractAddress, methodName, param, async transaction =>
            {
                var txWithSign = await signingKey.SignTransaction(transaction);
                var executeTxDto = new ExecuteTransactionDto
                {
                    RawTransaction = txWithSign.ToByteArray().ToHex()
                };

                StaticCoroutine.StartCoroutine(_chain.ExecuteTransactionAsync(executeTxDto, result =>
                {
                    var value = new T();
                    value.MergeFrom(ByteArrayHelper.HexStringToByteArray(result));

                    successCallback?.Invoke(value);
                }, errorCallback));
            }, errorCallback);
        }
        
#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void SendTransaction(string payload);

        private class PayloadForExtension
        {
            public class MethodParam
            {
                
            }
            
            public string rpcUrl;
            public string chainId;
            public string contractAddress;
            public string method;
            
        }
#endif

        public IEnumerator SendAsync(ISigningKey signingKey, string methodName, IMessage param, SuccessCallback<IContract.TransactionInfoDto> successCallback, ErrorCallback errorCallback)
        {
            if (signingKey is not PortkeyExtensionSigningKey portkeyExtensionSigningKey)
            {
                yield return _chain.GenerateTransactionAsync(signingKey.Address, ContractAddress, methodName, param, transaction =>
                {
                    // As different nodes have different block height,
                    // we need to give the next transaction a lower height (-5) so transaction can be successful
                    var refBlockNumber = transaction.RefBlockNumber - 5;
                    refBlockNumber = Math.Max(0, refBlockNumber);

                    StaticCoroutine.StartCoroutine(_chain.GetBlockByHeightAsync(refBlockNumber, async blockDto =>
                    {
                        transaction.RefBlockNumber = refBlockNumber;
                        transaction.RefBlockPrefix =
                            BlockHelper.GetRefBlockPrefix(Hash.LoadFromHex(blockDto?.BlockHash));

                        var txWithSign = await signingKey.SignTransaction(transaction);
                        Debugger.Log("Sending Transaction...");

                        var sendTxnInput = new SendTransactionInput
                        {
                            RawTransaction = txWithSign.ToByteArray().ToHex()
                        };
                        StaticCoroutine.StartCoroutine(_chain.SendTransactionAsync(sendTxnInput, result =>
                        {
                            StaticCoroutine.StartCoroutine(PollTransactionResultAsync(result.TransactionId,
                                transactionResult =>
                                {
                                    Debugger.Log(
                                        $"{methodName} on chain: {_chain.ChainInfo.chainId} \nStatus: {transactionResult.Status} \nError:{transactionResult.Error} \nTransactionId: {transactionResult.TransactionId} \nBlockNumber: {transactionResult.BlockNumber}\n");

                                    var txnInfoDto = new IContract.TransactionInfoDto
                                    {
                                        transaction = transaction,
                                        transactionResult = transactionResult
                                    };
                                    successCallback?.Invoke(txnInfoDto);

                                }, errorCallback));
                        }, errorCallback));
                    }, errorCallback));
                }, errorCallback);
            }
            else
            {
#if UNITY_WEBGL
                SendTransaction("");
#endif
            }
        }
        
        private IEnumerator PollTransactionResultAsync(string transactionId, SuccessCallback<TransactionResultDto> successCallback, ErrorCallback errorCallback)
        {
            var times = 0;
            var transactionResult = new TransactionResultDto();
            
            yield return new WaitForSeconds(3);
            yield return _chain.GetTransactionResultAsync(transactionId, result =>
            {
                transactionResult = result;
            }, errorCallback);

            while (transactionResult.Status == "PENDING" && times < MAX_POLL_TIMES)
            {
                times++;
                yield return _chain.GetTransactionResultAsync(transactionId, result =>
                {
                    transactionResult = result;
                }, errorCallback);
                yield return new WaitForSeconds(1);
            }

            if (transactionResult.Status == "PENDING")
            {
                errorCallback?.Invoke("Transaction is still pending.");
            }
            else
            {
                successCallback?.Invoke(transactionResult);
            }
        }
    }
}