using System.Threading.Tasks;
using AElf.Client;
using AElf.Client.Dto;
using AElf.Types;
using Google.Protobuf;
using Portkey.Core;

namespace Portkey.Chain
{
    /// <summary>
    /// AElfChain is a wrapper around AElfClient to implement chain related methods.
    /// </summary>
    public class AElfChain : IChain
    {
        private AElfClient _aelfClient;

        public AElfChain(string rpcUrl)
        {
            _aelfClient = new AElfClient(rpcUrl);
        }

        public async Task<Transaction> GenerateTransactionAsync(string from, string to, string methodName, IMessage input)
        {
            return await _aelfClient.GenerateTransactionAsync(from, to, methodName, input);
        }

        public Transaction SignTransaction(string privateKeyHex, Transaction transaction)
        {
            return _aelfClient.SignTransaction(privateKeyHex, transaction);
        }

        public async Task<string> ExecuteTransactionAsync(ExecuteTransactionDto input)
        {
            return await _aelfClient.ExecuteTransactionAsync(input);
        }
    }
}