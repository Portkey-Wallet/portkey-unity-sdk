using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Portkey.Core;

namespace Portkey.Contract
{
    public class CAContractProvider : IContractProvider<ContractBasic>
    {
        private Dictionary<string, IContract> _contracts = new Dictionary<string, IContract>();
        private IPortkeySocialService _service;
        private IChainProvider _chainProvider;
        private Dictionary<string, ChainInfo> _chainInfos = new Dictionary<string, ChainInfo>();

        public CAContractProvider(IPortkeySocialService service, IChainProvider chainProvider)
        {
            _service = service;
            _chainProvider = chainProvider;
        }

        public IEnumerator GetContract(string chainId, SuccessCallback<IContract> successCallback, ErrorCallback errorCallback)
        {
            if (_chainInfos.ContainsKey(chainId))
            {
                if (_contracts.TryGetValue(chainId, out var contract))
                {
                    successCallback(contract);
                    yield break;
                }
                
                var newContract = CreateContractBasic(chainId);
                
                successCallback(newContract);
            }
            else
            {
                yield return _service.GetChainsInfo(chains =>
                {
                    _chainProvider.SetChainInfo(chains.items);
                    _chainInfos = chains.items?.ToDictionary(info => info.chainId, info => info);

                    var newContract = CreateContractBasic(chainId);

                    successCallback(newContract);
                }, errorCallback);
            }
        }

        private ContractBasic CreateContractBasic(string chainId)
        {
            var chain = _chainProvider.GetChain(chainId);
            var newContract = new ContractBasic(chain, _chainInfos?[chainId].caContractAddress);
            _contracts[chainId] = newContract;
            return newContract;
        }
    }
}