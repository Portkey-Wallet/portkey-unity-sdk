using System;
using System.Collections;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AElf;
using AElf.Client;
using AElf.Client.Dto;
using AElf.Cryptography;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client.Configuration;
using Portkey.Core;
using Secp256k1Net;

namespace Portkey.Contract
{
    public class AElfContractBasic : IContract
    {
        public IEnumerator CallViewMethod(string methodName, object[] paramOptions, IHttp.successCallback successCallback,
            IHttp.errorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator CallSendMethod(string methodName, object[] paramOptions, SendOptions sendOptions,
            IHttp.successCallback successCallback, IHttp.errorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator EncodeTx(string methodName, object[] paramOptions, IHttp.successCallback successCallback,
            IHttp.errorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }
        
        public async Task<T> CallTransactionAsync<T>(string chainId, string methodName, IMessage param) where T : IMessage<T>, new()
        {
            try
            {
                //var chainInfo = _chainOptions.ChainInfos[chainId];
                var baseUrl = "http://192.168.66.61:8000";
                var privateKey = "83829798ac92d428ed13b29fe60ace1a7a10e7a347bdc9f23a85615339068f1c";
                var contractAddress = "2imqjpkCwnvYzfnr61Lp2XQVN2JU17LPkA9AZzmRZzV5LRRWmR";

                var client = new AElfClient(baseUrl);
                //var ownAddress = client.GetAddressFromPrivateKey(privateKey);
                var ownAddress = "TrmPcaqbqmbrztv6iJuN4zeuDmQxvjF5ujbvDDQo9Q1B4ye2T";
                //var ownAddress = "045ab0516fda4eeb504ad8d7ce8a2a24e5af6004afa9ff3ee26f2c697e334be48c31597e1905711a6aa749fc475787000b5d6260bcf0d457f23c60aa60bb6c8602";
                
                /*
                var secp256K1 = new Secp256k1();
                var privateKeyNonHex = ByteArrayHelper.HexStringToByteArray(privateKey);
                byte[] numArray = new byte[65];
                byte[] message = new byte[32];
                secp256K1.SignRecoverable((Span<byte>) numArray, (Span<byte>) message, (Span<byte>) privateKeyNonHex);
*/
                var transaction =
                    await client.GenerateTransactionAsync(ownAddress, contractAddress,
                        methodName, param);
                var txWithSign = client.SignTransaction(privateKey, transaction);

                var result = await client.ExecuteTransactionAsync(new ExecuteTransactionDto
                {
                    RawTransaction = txWithSign.ToByteArray().ToHex()
                });

                var value = new T();
                value.MergeFrom(ByteArrayHelper.HexStringToByteArray(result));

                return value;
            }
            catch (Exception e)
            {/*
                if (methodName != MethodName.GetHolderInfo)
                {
                    //_logger.LogError(e, methodName + " error: {param}", param);
                }*/
                Debugger.LogException(e);

                return new T();
            }
        }
    }
}