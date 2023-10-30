using System.Collections;
using AElf.Types;
using Google.Protobuf;
using Portkey.Chain.Dto;

namespace Portkey.Core
{
    /// <summary>
    /// Contract interface for interacting with a smart contract.
    /// </summary>
    public interface IContract
    {
        public class TransactionInfoDto
        {
            public Transaction transaction;
            public TransactionResultDto transactionResult;
        }
        
        /// <summary>
        /// For getting the contract address.
        /// </summary>
        string ContractAddress { get; }
        /// <summary>
        /// For getting the corresponding chain Id to the contract.
        /// </summary>
        string ChainId { get; }
        
        /// <summary>
        /// CallAsync is a generic method that can be used to call a contract method (Read-only operation).
        /// </summary>
        /// <param name="methodName">Name of the method to call from the contract.</param>
        /// <param name="param">Parameters for calling the method from the contract.</param>
        /// <typeparam name="T">Protobuf IMessage inherited classes corresponding to the called contract method.</typeparam>
        /// <param name="successCallback">The callback function when user is successful in calling the method on the contract.</param>
        /// <param name="errorCallback">The callback function when there is an error.</param>
        IEnumerator CallAsync<T>(string methodName, IMessage param, SuccessCallback<T> successCallback, ErrorCallback errorCallback) where T : IMessage<T>, new();

        /// <summary>
        /// SendAsync is a generic method that can be used to call a contract method that changes states and execute the transaction.
        /// </summary>
        /// <param name="signingKey">EOA Wallet to sign the transaction with.</param>
        /// <param name="methodName">Name of the method to call from the contract.</param>
        /// <param name="param">Parameters for calling the method from the contract.</param>
        /// <param name="successCallback">The callback function when user is successful in transaction for the method on the contract.</param>
        /// <param name="errorCallback">The callback function when there is an error.</param>
        IEnumerator SendAsync(ISigningKey signingKey, string methodName, IMessage param, SuccessCallback<TransactionInfoDto> successCallback, ErrorCallback errorCallback);
    }
}