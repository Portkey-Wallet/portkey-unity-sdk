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
    [SerializeField] private GameObject guardianItemPrefab;
    
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
                var newGuardianStatus = CreateUserGuardianStatus(guardian, guardianStatusMap, approvedMap);
                newGuardianStatus.guardianItem.verifier = verifierMap[guardian.verifierId];
                
                newGuardianStatus = UpdateStatusIfApproved(GetGuardianKey(guardian), newGuardianStatus, approvedMap);
                
                currentGuardiansList.Add(newGuardianStatus);
            }

            _guardianStatusList = currentGuardiansList;
            
            ClearGuardianItems();
            CreateGuardianItems(_guardianStatusList);
        }, OnError));
    }

    private void ClearGuardianItems()
    {
        foreach (Transform child in guardianItemList.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private UserGuardianStatus CreateUserGuardianStatus(Guardian guardian,
        Dictionary<string, UserGuardianStatus> guardianStatusMap, Dictionary<string, GuardiansApproved> approvedMap)
    {
        var key = GetGuardianKey(guardian);
        var identifier = guardian.guardianIdentifier ?? guardian.identifierHash;
        var guardianItem = new GuardianItem
        {
            guardian = guardian,
            identifier = identifier,
            key = key
        };

        if (IsMatchingAccessTokenInfo(_guardianIdentifierInfo, guardianItem))
        {
            guardianItem.accessToken = _guardianIdentifierInfo.token;
        }

        var newGuardianStatus = guardianStatusMap.TryGetValue(key, out var guardianStatus)
            ? guardianStatus
            : new UserGuardianStatus();

        newGuardianStatus.guardianItem = guardianItem;
        return newGuardianStatus;
    }

    private static string GetGuardianKey(Guardian guardian)
    {
        return $"{guardian.guardianIdentifier}&{guardian.verifierId}";
    }

    private static UserGuardianStatus UpdateStatusIfApproved(string key, UserGuardianStatus newGuardianStatus, IReadOnlyDictionary<string, GuardiansApproved> approvedMap)
    {
        if (approvedMap.TryGetValue(key, out var approvedGuardian))
        {
            newGuardianStatus.status = VerifierStatus.Verified;
            newGuardianStatus.verificationDoc = approvedGuardian.verificationDoc;
            newGuardianStatus.signature = approvedGuardian.signature;
        }
        return newGuardianStatus;
    }

    private void CreateGuardianItems(List<UserGuardianStatus> guardianStatusList)
    {
        foreach (var userGuardianStatus in guardianStatusList)
        {
            var guardianItem = Instantiate(guardianItemPrefab, guardianItemList.transform).GetComponent<GuardianItemComponent>();
            if(guardianItem != null)
            {
                guardianItem.SetUserGuardianStatus(userGuardianStatus);
            }
        }
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
