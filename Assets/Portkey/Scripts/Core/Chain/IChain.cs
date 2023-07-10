#nullable enable
using System.Threading.Tasks;
using AElf.Client;
using AElf.Client.Dto;
using AElf.Types;
using Google.Protobuf;

namespace Portkey.Core
{
    public interface IChain
    {
        Task<Transaction> GenerateTransactionAsync(string? from, string? to, string methodName, IMessage input);
        Transaction SignTransaction(string? privateKeyHex, Transaction transaction);
        Task<string> ExecuteTransactionAsync(ExecuteTransactionDto input);
    }
}