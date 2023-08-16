using System.Threading.Tasks;
using AElf.Client.Dto;
using AElf.Types;
using Google.Protobuf;

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
        
        string ContractAddress { get; }
        
        /// <summary>
        /// CallTransactionAsync is a generic method that can be used to call a contract Get method and execute the transaction.
        /// </summary>
        /// <param name="keyPair">Keypair to sign the transaction with.</param>
        /// <param name="methodName">Name of the method to call from the contract.</param>
        /// <param name="param">Parameters for calling the method from the contract.</param>
        /// <typeparam name="T">Protobuf IMessage inherited classes corresponding to the called contract method.</typeparam>
        /// <returns>Results in the form of IMessage.</returns>
        Task<T> CallTransactionAsync<T>(KeyPair keyPair, string methodName, IMessage param) where T : IMessage<T>, new();

        /// <summary>
        /// SendTransactionAsync is a generic method that can be used to call a contract Set method and execute the transaction.
        /// </summary>
        /// <param name="wallet">EOA Wallet to sign the transaction with.</param>
        /// <param name="methodName">Name of the method to call from the contract.</param>
        /// <param name="param">Parameters for calling the method from the contract.</param>
        /// <returns>Result information of the transaction.</returns>
        Task<TransactionInfoDto> SendTransactionAsync(KeyPair keyPair, string methodName, IMessage param);
    }
}