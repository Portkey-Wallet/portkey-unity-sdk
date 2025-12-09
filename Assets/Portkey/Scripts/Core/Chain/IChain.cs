#nullable enable
using System.Threading.Tasks;
using AElf.Client;
using AElf.Client.Dto;
using AElf.Types;
using Google.Protobuf;

namespace Portkey.Core
{
    /// <summary>
    /// Chain interface for interacting with a blockchain.
    /// </summary>
    public interface IChain
    {
        /// <summary>
        /// GenerateTransactionAsync is a method that can be used to generate a transaction for a method contract call.
        /// </summary>
        /// <param name="from">Address of the user's wallet.</param>
        /// <param name="to">Contract Address to execute method from.</param>
        /// <param name="methodName">Name of the method to call from the contract.</param>
        /// <param name="input">Parameter input corresponding to the method called from the contract.</param>
        /// <returns>Transaction data corresponding to this transaction.</returns>
        Task<Transaction> GenerateTransactionAsync(string? from, string? to, string methodName, IMessage input);
        /// <summary>
        /// Sign transaction with private key for a given transaction data.
        /// </summary>
        /// <param name="privateKeyHex">Private key related to the user's wallet in hexadecimal.</param>
        /// <param name="transaction">Transaction data to sign.</param>
        /// <returns>Signed transaction data.</returns>
        Transaction SignTransaction(string? privateKeyHex, Transaction transaction);
        /// <summary>
        /// Execute the transaction on the blockchain.
        /// </summary>
        /// <param name="input">Contains the transaction in byte array in hexadecimal.</param>
        /// <returns>The result of the transaction in byte array in hexadecimal.</returns>
        Task<string> ExecuteTransactionAsync(ExecuteTransactionDto input);
    }
}