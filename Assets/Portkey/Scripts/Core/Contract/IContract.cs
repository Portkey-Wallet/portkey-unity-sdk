using System.Threading.Tasks;
using Google.Protobuf;

namespace Portkey.Core
{
    /// <summary>
    /// Contract interface for interacting with a smart contract.
    /// </summary>
    public interface IContract
    {
        string ContractAddress { get; }
        
        /// <summary>
        /// CallAsync is a generic method that can be used to call a contract method (Read-only operation).
        /// </summary>
        /// <param name="keyPair">Keypair to sign the transaction with.</param>
        /// <param name="methodName">Name of the method to call from the contract.</param>
        /// <param name="param">Parameters for calling the method from the contract.</param>
        /// <typeparam name="T">Protobuf IMessage inherited classes corresponding to the called contract method.</typeparam>
        /// <returns>Results in the form of IMessage.</returns>
        Task<T> CallAsync<T>(KeyPair keyPair, string methodName, IMessage param) where T : IMessage<T>, new();
    }
}