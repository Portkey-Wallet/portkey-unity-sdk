#nullable enable
using System.Collections;
using AElf.Types;
using Google.Protobuf;
using Portkey.Chain.Dto;

namespace Portkey.Core
{
    /// <summary>
    /// Chain interface for interacting with a blockchain.
    /// </summary>
    public interface IChain
    {
        string ChainId { get; }
        IEnumerator GetChainStatus(SuccessCallback<ChainStatusDto> successCallback, ErrorCallback errorCallback);
        /// <summary>
        /// GenerateTransactionAsync is a method that can be used to generate a transaction for a method contract call.
        /// </summary>
        /// <param name="from">Address of the user's wallet.</param>
        /// <param name="to">Contract Address to execute method from.</param>
        /// <param name="methodName">Name of the method to call from the contract.</param>
        /// <param name="input">Parameter input corresponding to the method called from the contract.</param>
        /// <returns>Transaction data corresponding to this transaction.</returns>
        IEnumerator GenerateTransactionAsync(string? from, string? to, string methodName, IMessage input, SuccessCallback<Transaction> successCallback, ErrorCallback errorCallback);
        /// <summary>
        /// Execute the transaction on the blockchain.
        /// </summary>
        /// <param name="input">Contains the transaction in byte array in hexadecimal.</param>
        /// <returns>The result of the transaction in byte array in hexadecimal.</returns>
        IEnumerator ExecuteTransactionAsync(ExecuteTransactionDto input, SuccessCallback<string> successCallback, ErrorCallback errorCallback);

        IEnumerator GetBlockByHeightAsync(long blockHeight, SuccessCallback<BlockDto> successCallback, ErrorCallback errorCallback, bool includeTransactions = false);
        IEnumerator SendTransactionAsync(SendTransactionInput input, SuccessCallback<SendTransactionOutput> successCallback, ErrorCallback errorCallback);
        IEnumerator GetTransactionResultAsync(string transactionId, SuccessCallback<TransactionResultDto> successCallback, ErrorCallback errorCallback);
    }
}