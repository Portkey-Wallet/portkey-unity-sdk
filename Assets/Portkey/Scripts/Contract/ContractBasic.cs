using System;
using System.Threading.Tasks;
using AElf;
using AElf.Client.Dto;
using Google.Protobuf;
using Portkey.Core;
using UnityEngine;

namespace Portkey.Contract
{
    /// <summary>
    /// ContractBasic is a basic contract class that can be used to call a contract method.
    /// </summary>
    public class ContractBasic : IContract
    {
        private readonly IChain _chain;
        public string ContractAddress { get; protected set; }

        /// <summary>
        /// Constructor for ContractBasic to initialize the contract address and its respective chain.
        /// </summary>
        /// <param name="chain">The chain that the contract resides on.</param>
        /// <param name="contractAddress">The contract address related to this contract.</param>
        public ContractBasic(IChain chain, string contractAddress)
        {
            _chain = chain;
            ContractAddress = contractAddress;
        }
        
        public async Task<T> CallAsync<T>(KeyPair keyPair, string methodName, IMessage param) where T : IMessage<T>, new()
        {
            try
            {
                var transaction = await _chain.GenerateTransactionAsync(keyPair.Address, ContractAddress, methodName, param);
                
                var txWithSign = _chain.SignTransaction(keyPair.PrivateKey, transaction);

                var result = await _chain.ExecuteTransactionAsync(new ExecuteTransactionDto
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