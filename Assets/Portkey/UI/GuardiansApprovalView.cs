using System;
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
    [SerializeField] private GameObject guardianItemList;
    
    private GuardianIdentifierInfo _guardianIdentifierInfo;
    private List<UserGuardianStatus> _guardianStatusList = new List<UserGuardianStatus>();
    private List<GuardiansApproved> _approvedGuardians = new List<GuardiansApproved>();

    private void OnEnable()
    {
        StartCoroutine(did.GetVerifierServers(_guardianIdentifierInfo.chainId, StoreVerifierServers, OnError));
    }
    
    private void StoreVerifierServers(VerifierItem[] verifierServers)
    {
        var verifierMap = verifierServers.ToDictionary(verifier => verifier.id, verifier => verifier);
        GetGuardianStatusList(verifierMap);
    }

    private void GetGuardianStatusList(IReadOnlyDictionary<string, VerifierItem> verifierMap)
    {
        var param = new GetHolderInfoParams
        {
            guardianIdentifier = _guardianIdentifierInfo.identifier.RemoveAllWhiteSpaces(),
            chainId = _guardianIdentifierInfo.chainId
        };
        StartCoroutine(did.GetHolderInfo(param, (result) =>
        {
            var guardians = (result?.guardianList == null)? Array.Empty<Guardian>() : result.guardianList.guardians;
            var guardianStatusMap = _guardianStatusList.ToDictionary(status => $"{(status.guardianItem.guardian.guardianIdentifier ?? status.guardianItem.guardian.identifierHash)}&{status.guardianItem.guardian.verifierId}", status => status);
            var approvedMap = _approvedGuardians.ToDictionary(approval => $"{approval.identifier}&{approval.verifierId}", approval => approval);

            var currentGuardiansList = new List<UserGuardianStatus>();
            foreach (var guardian in guardians)
            {
                var key = $"{guardian.guardianIdentifier}&{guardian.verifierId}";
                var identifier = guardian.guardianIdentifier ?? guardian.identifierHash;
                var verifier = verifierMap[guardian.verifierId];
                var guardianItem = new GuardianItem
                {
                    guardian = guardian,
                    verifier = verifier,
                    identifier = identifier,
                    key = key
                };
                
                if(IsMatchingAccessTokenInfo(_guardianIdentifierInfo, guardianItem))
                {
                    guardianItem.accessToken = _guardianIdentifierInfo.token;
                }

                var newGuardianStatus = guardianStatusMap.TryGetValue(key, out var guardianStatus) ? guardianStatus : new UserGuardianStatus();
                if (approvedMap.TryGetValue(key, out var approvedGuardian))
                {
                    newGuardianStatus.status = VerifierStatus.Verified;
                    newGuardianStatus.verificationDoc = approvedGuardian.verificationDoc;
                    newGuardianStatus.signature = approvedGuardian.signature;
                }

                newGuardianStatus.guardianItem = guardianItem;
                currentGuardiansList.Add(newGuardianStatus);
            }

            _guardianStatusList = currentGuardiansList;
            CreateGuardianItems(_guardianStatusList);
        }, OnError));
    }

    private static void CreateGuardianItems(List<UserGuardianStatus> guardianStatusList)
    {
        
    }

    private static bool IsMatchingAccessTokenInfo(GuardianIdentifierInfo guardianIdentifierInfo, GuardianItem baseGuardian)
    {
        return guardianIdentifierInfo.token != null && guardianIdentifierInfo.identifier == baseGuardian.identifier && guardianIdentifierInfo.accountType == baseGuardian.guardian.type;
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
