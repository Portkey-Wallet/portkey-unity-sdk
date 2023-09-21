using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Portkey.Core;

namespace Portkey.Contract
{
    public class CAContractProvider : IContractProvider
    {
        private readonly Dictionary<string, IContract> _contracts = new Dictionary<string, IContract>();
        private readonly IChainProvider _chainProvider;

        public CAContractProvider(IChainProvider chainProvider)
        {
            _chainProvider = chainProvider;
        }

        public IEnumerator GetContract(string chainId, SuccessCallback<IContract> successCallback, ErrorCallback errorCallback)
        {
            if (_contracts.TryGetValue(chainId, out var contract))
            {
                successCallback(contract);
                yield break;
            }

            yield return _chainProvider.GetChain(chainId, chain =>
            {
                var newContract = CreateCAContract(chain);
                successCallback(newContract);
            }, errorCallback);
        }

        private ContractBasic CreateCAContract(IChain chain)
        {
            var chainInfo = chain.ChainInfo;
            var newContract = new ContractBasic(chain, chainInfo.caContractAddress);
            _contracts[chainInfo.chainId] = newContract;
            return newContract;
        }
    }
}