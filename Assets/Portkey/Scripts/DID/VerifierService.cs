using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Portkey.Contracts.CA;
using Portkey.Core;
using Portkey.Utilities;
using Empty = Google.Protobuf.WellKnownTypes.Empty;

namespace Portkey.DID
{
    public class VerifierService : IVerifierService
    {
        private readonly Dictionary< string, Dictionary<string, VerifierItem> > _verifiers = new Dictionary<string, Dictionary<string, VerifierItem>>();
        private readonly IPortkeySocialService _portkeySocialService;
        private readonly IContractProvider _contractProvider;
        private readonly IChainProvider _chainProvider;
        
        public VerifierService(IPortkeySocialService portkeySocialService, IContractProvider contractProvider, IChainProvider chainProvider)
        {
            _portkeySocialService = portkeySocialService;
            _contractProvider = contractProvider;
            _chainProvider = chainProvider;
        }
        
        private IEnumerator GetVerifierServers(string chainId, SuccessCallback<VerifierItem[]> successCallback, ErrorCallback errorCallback)
        {
            yield return _chainProvider.GetAvailableChainIds((chainInfos) =>
            {
                chainId = chainInfos.FirstOrDefault(chainInfo => chainInfo == chainId);
                if (chainId == null)
                {
                    errorCallback("Failed to get chain info. Network Error!");
                    return;
                }
                
                StaticCoroutine.StartCoroutine(GetVerifierServersFromContract(chainId, result =>
                {
                    if (result == null || result.Length == 0)
                    {
                        errorCallback("Network Error!");
                        return;
                    }
                    
                    successCallback(result);
                }, errorCallback));
            }, errorCallback);
        }
        
        private IEnumerator GetVerifierServersFromContract(string chainId, SuccessCallback<VerifierItem[]> successCallback, ErrorCallback errorCallback)
        {
            yield return _contractProvider.GetContract(chainId,  (contract) =>
            {
                StaticCoroutine.StartCoroutine(GetVerifierServersByContract(contract, successCallback, errorCallback));
            }, errorCallback);
        }
        
        private static IEnumerator GetVerifierServersByContract(IContract contract, SuccessCallback<VerifierItem[]> successCallback, ErrorCallback errorCallback)
        {
            yield return contract.CallAsync<GetVerifierServersOutput>("GetVerifierServers", new Empty(), result =>
            {
                var verifierItems = ConvertToVerifierItems(result);
                successCallback(verifierItems);
            }, errorCallback);
        }

        private static VerifierItem[] ConvertToVerifierItems(GetVerifierServersOutput result)
        {
            var verifierItems = new VerifierItem[result.VerifierServers.Count];
            for (var i = 0; i < result.VerifierServers.Count; i++)
            {
                var verifierServer = result.VerifierServers[i];
                var addresses = new string[verifierServer.VerifierAddresses.Count];
                for (var j = 0; j < verifierServer.VerifierAddresses.Count; j++)
                {
                    addresses[j] = verifierServer.VerifierAddresses[j].ToString();
                }

                var item = new VerifierItem
                {
                    id = verifierServer.Id.ToHex(),
                    name = verifierServer.Name,
                    imageUrl = verifierServer.ImageUrl,
                    endPoints = verifierServer.EndPoints.ToArray(),
                    verifierAddresses = addresses
                };
                verifierItems[i] = item;
            }

            return verifierItems;
        }

        public bool IsInitialized(string chainId)
        {
            return _verifiers.ContainsKey(chainId);
        }

        public VerifierItem GetVerifier(string chainId, string verifierId)
        {
            if(_verifiers.ContainsKey(chainId) && _verifiers[chainId].ContainsKey(verifierId))
            {
                return _verifiers[chainId][verifierId];
            }

            return null;
        }

        public IEnumerator Initialize(string chainId, SuccessCallback<bool> successCallback, ErrorCallback errorCallback)
        {
            if(IsInitialized(chainId))
            {
                successCallback(true);
                yield break;
            }
            
            yield return GetVerifierServers(chainId, (verifiers) =>
            {
                if (!_verifiers.ContainsKey(chainId))
                {
                    _verifiers.Add(chainId, new Dictionary<string, VerifierItem>());
                }

                _verifiers[chainId] = verifiers.ToDictionary(verifier => verifier.id, verifier => verifier);

                successCallback(true);
            }, errorCallback);
        }

        public IEnumerator GetVerifierServer(string chainId, SuccessCallback<VerifierServerResult> successCallback, ErrorCallback errorCallback)
        {
            yield return _portkeySocialService.GetVerifierServer(chainId, result =>
            {
                if (result == null)
                {
                    errorCallback("Failed to get Verifier Server. Network Error!");
                    return;
                }
                
                successCallback(result);
            }, errorCallback);
        }
    }
}