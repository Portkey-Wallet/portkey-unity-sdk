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
        public string ChainId { get; private set; }
        
        public AElfChain(string chainId, string rpcUrl)
        {
            _aelfClient = new AElfClient(rpcUrl);
            ChainId = chainId;
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

        public async Task<BlockDto> GetBlockByHeightAsync(long blockHeight, bool includeTransactions = false)
        {
            return await _aelfClient.GetBlockByHeightAsync(blockHeight, includeTransactions);
        }

        public async Task<SendTransactionOutput> SendTransactionAsync(SendTransactionInput input)
        {
            return await _aelfClient.SendTransactionAsync(input);
        }

        public async Task<TransactionResultDto> GetTransactionResultAsync(string transactionId)
        {
            return await _aelfClient.GetTransactionResultAsync(transactionId);
        }
    }
}