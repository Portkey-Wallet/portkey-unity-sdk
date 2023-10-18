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
        /// <summary>
        /// For getting the chain information such as endpoints and corresponding default token information.
        /// </summary>
        ChainInfo ChainInfo { get; }
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
        /// <summary>
        /// The GetBlockByHeightAsync function retrieves a block by its height.
        /// </summary>        
        /// <param name="blockHeight">The block height to query.</param>
        /// <param name="SuccessCallback&lt;BlockDto&gt; successCallback"> /// this is the success callback for the getblockbyheightasync function. it returns a blockdto object that contains information about a block in the blockchain.</param>
        /// <param name="errorCallback">Callback when an error occurs.</param>
        /// <param name="includeTransactions"> Include transactions in the response. default is false.</param>
        /// <returns> A blockdto object.</returns>
        IEnumerator GetBlockByHeightAsync(long blockHeight, SuccessCallback<BlockDto> successCallback, ErrorCallback errorCallback, bool includeTransactions = false);
        /// <summary> The SendTransactionAsync function sends a transaction to the blockchain.</summary>
        /// <param name="input"> This is the input object for the sendtransactionasync function.</param>
        /// <param name="SuccessCallback&lt;SendTransactionOutput&gt; successCallback"> This is the callback that will be invoked when the transaction has been sent successfully. 
        /// The sendtransactionoutput object returned by this function contains a transactionhash property, which is a string containing the hash of your transaction. 
        /// </param>
        /// <param name="ErrorCallback errorCallback"> The error callback.</param>
        /// <returns> A SendTransactionOutput containing the transaction ID.</returns>
        IEnumerator SendTransactionAsync(SendTransactionInput input, SuccessCallback<SendTransactionOutput> successCallback, ErrorCallback errorCallback);
        /// <summary> The GetTransactionResultAsync function is used to get the result of a transaction.</summary>        
        /// <param name="string transactionId"> The transaction id</param>
        /// <param name="SuccessCallback&lt;TransactionResultDto&gt; successCallback">The success callback when a transaction result is retrieved.</param>
        /// <param name="errorCallback"> The error callback.</param>
        /// <returns> A TransactionResultDto object containing the transaction result.</returns>
        IEnumerator GetTransactionResultAsync(string transactionId, SuccessCallback<TransactionResultDto> successCallback, ErrorCallback errorCallback);
    }
}