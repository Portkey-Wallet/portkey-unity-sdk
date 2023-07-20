using System.Collections.Generic;
using System.Linq;
using Portkey.Core;

namespace Portkey.Chain
{
    public class AElfChainProvider : IChainProvider
    {
        private Dictionary<string, IChain> _chains = new Dictionary<string, IChain>();
        private Dictionary<string, string> _chainUrls = null;
        
        public IChain GetChain(string chainId)
        {
            if(_chainUrls == null)
            {
                throw new System.NullReferenceException("ChainInfo is not set");
            }
            
            if (_chains.TryGetValue(chainId, out var chain))
            {
                return chain;
            }
            
            if (!_chainUrls.TryGetValue(chainId, out var chainUrl))
            {
                throw new System.ArgumentException($"ChainInfo for chainId {chainId} is not found");
            }
            var newChain = new AElfChain(chainId, chainUrl);
            _chains[chainId] = newChain;

            return newChain;
        }

        public void SetChainInfo(ChainInfo[] chainInfos)
        {
            _chainUrls = chainInfos?.ToDictionary(info => info.chainId, info => info.endPoint);
        }
    }
}