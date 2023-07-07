using System;
using System.Threading.Tasks;
using AElf;
using AElf.Client.Dto;
using Google.Protobuf;
using Portkey.Core;

namespace Portkey.Contract
{
    public class ContractBasic : IContract
    {
        private readonly IChain _chain;
        public string ContractAddress { get; protected set; }

        public ContractBasic(IChain chain, string contractAddress)
        {
            _chain = chain;
            ContractAddress = contractAddress;
        }

        public async Task<T> CallTransactionAsync<T>(string methodName, IMessage param) where T : IMessage<T>, new()
        {
            try
            {
                //var chainInfo = _chainOptions.ChainInfos[chainId];
                var baseUrl = "http://192.168.66.61:8000";
                var privateKey = "83829798ac92d428ed13b29fe60ace1a7a10e7a347bdc9f23a85615339068f1c";
                var contractAddress = "2imqjpkCwnvYzfnr61Lp2XQVN2JU17LPkA9AZzmRZzV5LRRWmR";

                //var client = new AElfClient(baseUrl);
                //var ownAddress = client.GetAddressFromPrivateKey(privateKey);
                var ownAddress = "TrmPcaqbqmbrztv6iJuN4zeuDmQxvjF5ujbvDDQo9Q1B4ye2T";
                //var ownAddress = "045ab0516fda4eeb504ad8d7ce8a2a24e5af6004afa9ff3ee26f2c697e334be48c31597e1905711a6aa749fc475787000b5d6260bcf0d457f23c60aa60bb6c8602";
                
                var transaction =
                    await _chain.Client.GenerateTransactionAsync(ownAddress, ContractAddress,
                        methodName, param);
                var txWithSign = _chain.Client.SignTransaction(privateKey, transaction);

                var result = await _chain.Client.ExecuteTransactionAsync(new ExecuteTransactionDto
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
    }
}