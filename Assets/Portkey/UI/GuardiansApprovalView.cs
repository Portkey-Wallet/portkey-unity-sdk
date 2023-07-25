using System.Collections.Generic;
using System.Linq;
using Portkey.Core;
using Portkey.DID;
using Portkey.UI;
using Portkey.Utilities;
using UnityEngine;

public class GuardiansApprovalView : MonoBehaviour
{
    [SerializeField] private DID did;
    
    private GuardianIdentifierInfo _guardianIdentifierInfo;
    private Dictionary<string, VerifierItem> _verifierMap = new Dictionary<string, VerifierItem>();
    private List<Guardian> _guardianList = null;
    private List<Guardian> _approvedGuardians = null;

    private void OnEnable()
    {
        StartCoroutine(did.GetVerifierServers(_guardianIdentifierInfo.chainId, StoreVerifierServers, OnError));
    }
    
    private void StoreVerifierServers(VerifierItem[] verifierServers)
    {
        _verifierMap = verifierServers.ToDictionary(server => server.id, server => server);
        GetGuardianList();
    }

    private void GetGuardianList()
    {
        var param = new GetHolderInfoParams
        {
            guardianIdentifier = _guardianIdentifierInfo.identifier.RemoveAllWhiteSpaces(),
            chainId = _guardianIdentifierInfo.chainId
        };
        StartCoroutine(did.GetHolderInfo(param, (result) =>
        {
            var guardians = result?.guardianList?? new GuardianList();

            Dictionary<string, Guardian> guardianMap = new Dictionary<string, Guardian>();
            if (_guardianList != null)
            {
                foreach (var guardian in _guardianList)
                {
                    var identifier = (guardian.guardianIdentifier == null)? guardian.identifierHash : guardian.guardianIdentifier;
                    var key = $"{identifier}&{guardian.verifierId}";
                    guardianMap[key] = guardian;
                }
            }
            Dictionary<string, Guardian> approvedMap = new Dictionary<string, Guardian>();
            if (_approvedGuardians != null)
            {
                foreach (var guardian in _approvedGuardians)
                {
                    var key = $"{guardian.guardianIdentifier}&{guardian.verifierId}";
                    approvedMap[key] = guardian;
                }
            }
            
        }, OnError));
    }

    public void SetGuardianIdentifierInfo(GuardianIdentifierInfo info)
    {
        _guardianIdentifierInfo = info;
    }
    
    private void OnError(string error)
    {
        Debugger.LogError(error);
    }
}
