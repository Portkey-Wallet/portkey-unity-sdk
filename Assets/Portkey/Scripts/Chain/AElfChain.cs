using System;
using System.Collections;
using System.IO;
using System.Linq;
using AElf;
using AElf.Types;
using Google.Protobuf;
using Newtonsoft.Json;
using Portkey.Chain.Dto;
using Portkey.Core;
using Portkey.Utilities;

namespace Portkey.Chain
{
    /// <summary>
    /// AElfChain is a wrapper around AElfClient to implement chain related methods.
    /// </summary>
    public class AElfChain : IChain
    {
        private class TransactionPostDto
        {
            public string RawTransaction;
        }
        
        private readonly string _baseUrl;
        private readonly IHttp _httpService;
        
        public ChainInfo ChainInfo { get; }
        
        public AElfChain(ChainInfo chainInfo, IHttp httpService)
        {
            _baseUrl = chainInfo.endPoint;
            ChainInfo = chainInfo;
            _httpService = httpService;
        }
        
        private static string GetRequestUrl(string baseUrl, string relativeUrl)
        {
            return new Uri(new Uri(baseUrl + (baseUrl.EndsWith("/") ? "" : "/")), relativeUrl).ToString();
        }
        
        private static void AssertValidAddress(params string[] addresses)
        {
            try
            {
                foreach (var address in addresses)
                {
                    Address.FromBase58(address);
                }
            }
            catch (Exception)
            {
                throw new InvalidDataException("Invalid address format.");
            }
        }
        
        private static void AssertValidTransactionId(params string[] transactionIds)
        {
            try
            {
                foreach (var transactionId in transactionIds)
                {
                    Hash.LoadFromHex(transactionId);
                }
            }
            catch (Exception)
            {
                throw new InvalidDataException("Invalid transaction id format.");
            }
        }
        
        public IEnumerator GetChainStatus(SuccessCallback<ChainStatusDto> successCallback, ErrorCallback errorCallback)
        {
            var data = new FieldFormRequestData<Empty>
            {
                Url = GetRequestUrl(_baseUrl, "api/blockChain/chainStatus"),
                FieldFormsObject = Empty.Default
            };
            yield return Get(data, successCallback, errorCallback);
        }

        public IEnumerator GenerateTransactionAsync(string from, string to, string methodName, IMessage input,
            SuccessCallback<Transaction> successCallback, ErrorCallback errorCallback)
        {
            AssertValidAddress(to);

            yield return GetChainStatus(chainStatus =>
            {
                var transaction = new Transaction
                {
                    From = from.ToAddress(),
                    To = to.ToAddress(),
                    MethodName = methodName,
                    Params = input.ToByteString(),
                    RefBlockNumber = chainStatus.BestChainHeight,
                    RefBlockPrefix = ByteString.CopyFrom(Hash.LoadFromHex(chainStatus.BestChainHash).Value
                        .Take(4).ToArray())
                };
                successCallback?.Invoke(transaction);
            }, errorCallback);
        }

        public IEnumerator ExecuteTransactionAsync(ExecuteTransactionDto input, SuccessCallback<string> successCallback, ErrorCallback errorCallback)
        {
            var txnPostDto = new TransactionPostDto
            {
                RawTransaction = input.RawTransaction
            };
            var parameters = new JsonRequestData
            {
                Url = GetRequestUrl(_baseUrl, "api/blockChain/executeTransaction"),
                JsonData = JsonConvert.SerializeObject(txnPostDto),
            };
    
            return _httpService.Post(parameters, result => successCallback?.Invoke(result), error => OnError(error, errorCallback));
        }
        
        public IEnumerator GetBlockByHeightAsync(long blockHeight, SuccessCallback<BlockDto> successCallback, ErrorCallback errorCallback, bool includeTransactions = false)
        {
            var url = GetRequestUrl(_baseUrl,
                $"api/blockChain/blockByHeight?blockHeight={blockHeight}&includeTransactions={includeTransactions}");
            
            var data = new FieldFormRequestData<Empty>
            {
                Url = url,
                FieldFormsObject = Empty.Default
            };
            yield return Get(data, successCallback, errorCallback);
        }
        
        public IEnumerator SendTransactionAsync(SendTransactionInput input, SuccessCallback<SendTransactionOutput> successCallback, ErrorCallback errorCallback)
        {
            var txnPostDto = new TransactionPostDto
            {
                RawTransaction = input.RawTransaction
            };
            var parameters = new JsonRequestData()
            {
                Url = GetRequestUrl(_baseUrl, "api/blockChain/sendTransaction"),
                JsonData = JsonConvert.SerializeObject(txnPostDto),
            };
            
            yield return Post(parameters, successCallback, errorCallback);
        }
        
        public IEnumerator GetTransactionResultAsync(string transactionId, SuccessCallback<TransactionResultDto> successCallback, ErrorCallback errorCallback)
        {
            AssertValidTransactionId(transactionId);
            
            var data = new FieldFormRequestData<Empty>
            {
                Url = GetRequestUrl(_baseUrl, $"api/blockChain/transactionResult?transactionId={transactionId}"),
                FieldFormsObject = Empty.Default
            };
            
            yield return Get(data, successCallback, errorCallback);
        }
        
        private IEnumerator Post<T>(JsonRequestData parameters, SuccessCallback<T> successCallback, ErrorCallback errorCallback)
        {
            yield return _httpService.Post(parameters, result => DeserializeResponse(result, successCallback, errorCallback), error => OnError(error, errorCallback));
        }

        private IEnumerator Get<T1, T2>(FieldFormRequestData<T1> data, SuccessCallback<T2> successCallback, ErrorCallback errorCallback)
        {
            yield return _httpService.Get(data, result => DeserializeResponse(result, successCallback, errorCallback), error => OnError(error, errorCallback));
        }

        private static void OnError(IHttp.ErrorMessage error, ErrorCallback errorCallback)
        {
            errorCallback?.Invoke(error.message);
        }

        private static void DeserializeResponse<T>(string result, SuccessCallback<T> successCallback, ErrorCallback errorCallback)
        {
            if (result == null)
            {
                errorCallback?.Invoke("Failed to get response.");
                return;
            }

            try
            {
                var dto = JsonConvert.DeserializeObject<T>(result);
                successCallback?.Invoke(dto);
            }
            catch (Exception e)
            {
                Debugger.LogException(e);
                errorCallback?.Invoke(e.Message);
            }
        }
    }
}