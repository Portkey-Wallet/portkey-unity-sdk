using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Portkey.Core;

namespace Portkey.Chain
{
    public class AElfChainProvider : IChainProvider
    {
        private readonly Dictionary<string, IChain> _chains = new Dictionary<string, IChain>();
        private Dictionary<string, ChainInfo> _chainInfos = new Dictionary<string, ChainInfo>();
        private readonly IHttp _http;
        private readonly IPortkeySocialService _service;
        
        public AElfChainProvider(IHttp http, IPortkeySocialService service)
        {
            _http = http;
            _service = service;
        }

        public IEnumerator GetAvailableChainIds(SuccessCallback<string[]> successCallback, ErrorCallback errorCallback)
        {
            if (_chainInfos.Keys.Count != 0)
            {
                CallbackWithUpdatedChainsInfo();
                yield break;
            }
            
            yield return UpdateChainsInfo(CallbackWithUpdatedChainsInfo, errorCallback);
            
            yield break;

            void CallbackWithUpdatedChainsInfo()
            {
                successCallback?.Invoke(_chainInfos.Keys.ToArray());
            }
        }

        public IEnumerator GetChain(string chainId, SuccessCallback<IChain> successCallback, ErrorCallback errorCallback)
        {
            if (_chains.TryGetValue(chainId, out var chain))
            {
                successCallback(chain);
                yield break;
            }
            
            if (_chainInfos.TryGetValue(chainId, out var info))
            {
                CreateChainAndCallback(info);
                yield break;
            }

            yield return UpdateChainsInfo(() =>
            {
                if (!_chainInfos.TryGetValue(chainId, out var chainInfo))
                {
                    errorCallback($"Chain: {chainId} not found.");
                    return;
                }
                CreateChainAndCallback(chainInfo);
            }, errorCallback);
            
            yield break;

            void CreateChainAndCallback(ChainInfo chainInfo)
            {
                var newChain = CreateChain(chainInfo);
                successCallback(newChain);
            }
        }
        
        private IEnumerator UpdateChainsInfo(Action successCallback, ErrorCallback errorCallback)
        {
            yield return _service.GetChainsInfo(chains =>
            {
                SetChainInfos(chains.items);
                
                successCallback?.Invoke();
            }, errorCallback);
        }

        private AElfChain CreateChain(ChainInfo chainInfo)
        {
            var newChain = new AElfChain(chainInfo, _http);
            _chains[chainInfo.chainId] = newChain;
            return newChain;
        }

        private void SetChainInfos(ChainInfo[] chainInfos)
        {
            _chainInfos = chainInfos?.ToDictionary(info => info.chainId, info => info);
        }
    }
}