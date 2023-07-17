using System;
using System.Threading.Tasks;
using AElf;
using AElf.Client.Dto;
using AElf.Types;
using Google.Protobuf;
using Portkey.Core;
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
        
        public async Task<T> CallTransactionAsync<T>(BlockchainWallet wallet, string methodName, IMessage param) where T : IMessage<T>, new()
        {
            try
            {
                var transaction = await _chain.GenerateTransactionAsync(wallet.Address, ContractAddress, methodName, param);
                
                var txWithSign = _chain.SignTransaction(wallet.PrivateKey, transaction);

                var result = await _chain.ExecuteTransactionAsync(new ExecuteTransactionDto
                {
                    RawTransaction = txWithSign.ToByteArray().ToHex()
                });

                var value = new T();
                value.MergeFrom(ByteArrayHelper.HexStringToByteArray(result));

                return value;
            }
            catch (Exception e)
            {
                Debugger.LogException(e);

                return new T();
            }
        }

        public async Task<IContract.TransactionInfoDto> SendTransactionAsync(BlockchainWallet wallet, string methodName, IMessage param)
        {
            var transaction = await _chain.GenerateTransactionAsync(wallet.Address, ContractAddress, methodName, param);

            // As different nodes have different block height,
            // we need to give the next transaction a lower height (-50) so transaction can be successful
            var refBlockNumber = transaction.RefBlockNumber - 50;
            refBlockNumber = Math.Max(0, refBlockNumber);

            var blockDto = await _chain.GetBlockByHeightAsync(refBlockNumber);

            transaction.RefBlockNumber = refBlockNumber;
            transaction.RefBlockPrefix = BlockHelper.GetRefBlockPrefix(Hash.LoadFromHex(blockDto?.BlockHash));

            var txWithSign = _chain.SignTransaction(wallet.PrivateKey, transaction);

            // Send the transfer transaction to chain node
            var result = await _chain.SendTransactionAsync(new SendTransactionInput
            {
                RawTransaction = txWithSign.ToByteArray().ToHex()
            });

            // We wait for 3 seconds to make sure the transaction is on chain
            await Task.Delay(3000);

            var transactionResult = await _chain.GetTransactionResultAsync(result.TransactionId);

            var times = 0;
            while (transactionResult.Status == "PENDING" && times < MAX_POLL_TIMES)
            {
                times++;
                await Task.Delay(1000);
                transactionResult = await _chain.GetTransactionResultAsync(result.TransactionId);
            }

            Debugger.Log(
                $"{methodName} on chain: {_chain.ChainId} \nStatus: {transactionResult.Status} \nError:{transactionResult.Error} \nTransactionId: {transactionResult.TransactionId} \nBlockNumber: {transactionResult.BlockNumber}\n");

            return new IContract.TransactionInfoDto
            {
                transaction = transaction,
                transactionResult = transactionResult
            };
        }
    }
}