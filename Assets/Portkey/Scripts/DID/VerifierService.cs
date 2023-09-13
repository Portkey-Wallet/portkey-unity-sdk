using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Portkey.Core;
using Portkey.Utilities;

namespace Portkey.DID
{
    public class VerifierService : IVerifierService
    {
        private readonly Dictionary< string, Dictionary<string, VerifierItem> > _verifiers = new Dictionary<string, Dictionary<string, VerifierItem>>();
        private readonly DIDWallet<WalletAccount> _didWallet;
        private readonly IPortkeySocialService _portkeySocialService;
        
        public VerifierService(DIDWallet<WalletAccount> didWallet, IPortkeySocialService portkeySocialService)
        {
            _didWallet = didWallet;
            _portkeySocialService = portkeySocialService;
        }
        
        private IEnumerator GetVerifierServers(string chainId, SuccessCallback<VerifierItem[]> successCallback, ErrorCallback errorCallback)
        {
            yield return _portkeySocialService.GetChainsInfo((chainInfos) =>
            {
                var chainInfo = chainInfos.items.FirstOrDefault(info => info.chainId == chainId);
                if (chainInfo == null)
                {
                    errorCallback("Network Error!");
                    return;
                }
                
                StaticCoroutine.StartCoroutine(_didWallet.GetVerifierServers(chainInfo.chainId, result =>
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
    }
}