using System;
using System.Collections.Generic;
using System.Linq;
using Portkey.Core;
using Portkey.DID;
using Portkey.UI;
using Portkey.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuardiansApprovalView : MonoBehaviour
{
    [SerializeField] private DID did;
    
    [Header("Guardian Item List")]
    [SerializeField] private GameObject guardianItemList;
    [SerializeField] private GameObject guardianItemPrefab;
    
    [Header("Guardian Info")]
    [SerializeField] private TextMeshProUGUI totalGuardiansText;
    [SerializeField] private TextMeshProUGUI totalVerifiedGuardiansText;
    [SerializeField] private Button completeButton;
    
    [Header("Progress Dial")]
    [SerializeField] private Image guardianProgressDial;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float minProgress = 0.0f;
    
    [Header("Expiry Time")]
    [SerializeField] private int expiryInMilliseconds = 360000;
    
    private GuardianIdentifierInfo _guardianIdentifierInfo;
    private List<UserGuardianStatus> _guardianStatusList = new List<UserGuardianStatus>();
    private List<GuardiansApproved> _approvedGuardians = new List<GuardiansApproved>();
    private float _timeElapsed = 0.0f;
    private bool _startTimer = false;

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
            var guardianStatusMap = _guardianStatusList.ToDictionary(status => $"{(status.guardianItem.identifier ?? status.guardianItem.guardian.identifierHash)}&{status.guardianItem.guardian.verifierId}", status => status);
            var approvedMap = _approvedGuardians.ToDictionary(approval => $"{approval.identifier}&{approval.verifierId}", approval => approval);
            
            var currentGuardiansList = new List<UserGuardianStatus>();
            foreach (var guardian in guardians)
            {
                var newGuardianStatus = CreateUserGuardianStatus(guardian, guardianStatusMap);
                newGuardianStatus.guardianItem.verifier = verifierMap[guardian.verifierId];
                
                newGuardianStatus = UpdateStatusIfApproved(GetGuardianKey(guardian), newGuardianStatus, approvedMap);
                
                currentGuardiansList.Add(newGuardianStatus);
            }

            _guardianStatusList = currentGuardiansList;
            
            InitializeUI();
        }, OnError));
    }

    private void InitializeUI()
    {
        totalGuardiansText.text = $"/{_guardianStatusList.Count.ToString()}";
        UpdateUI();

        ClearGuardianItems();
        CreateGuardianItems(_guardianStatusList);
    }

    private void UpdateUI()
    {
        UpdateTotalVerifiedGuardiansText();
        UpdateGuardianProgressDial();

        SetSendButtonInteractable(VerifiedCount(_guardianStatusList) >= did.GetApprovalCount(_guardianStatusList.Count));
    }

    private void Update()
    {
        if (!_startTimer)
        {
            return;
        }
        
        _timeElapsed += Time.deltaTime;
        if(_timeElapsed >= expiryInMilliseconds / 1000.0f)
        {
            ResetGuardianStatusList();
            InitializeUI();
            ResetTimer();
        }
    }
    
    private void ResetGuardianStatusList()
    {
        _guardianStatusList.ForEach(status => status.status = VerifierStatus.NotVerified);
    }

    private void ResetTimer()
    {
        _startTimer = false;
        _timeElapsed = 0.0f;
    }

    private void SetSendButtonInteractable(bool interactable)
    {
        completeButton.interactable = interactable;
    }

    private void ClearGuardianItems()
    {
        foreach (Transform child in guardianItemList.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private UserGuardianStatus CreateUserGuardianStatus(Guardian guardian, Dictionary<string, UserGuardianStatus> guardianStatusMap)
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
                guardianItem.SetDID(did);
                guardianItem.SetUserGuardianStatus(userGuardianStatus, OnUserGuardianStatusChanged);
                guardianItem.SetGuardianIdentifierInfo(_guardianIdentifierInfo);
            }
        }
    }

    private void OnUserGuardianStatusChanged(UserGuardianStatus status)
    {
        UpdateUI();
        _startTimer = true;
    }

    private void UpdateGuardianProgressDial()
    {
        var percentage = (float)VerifiedCount(_guardianStatusList) / _guardianStatusList.Count;
        percentage = Mathf.Clamp(percentage, minProgress, 1.0f);
        guardianProgressDial.fillAmount = percentage;
    }

    private void UpdateTotalVerifiedGuardiansText()
    {
        totalVerifiedGuardiansText.text = VerifiedCount(_guardianStatusList).ToString();
    }

    private static int VerifiedCount(List<UserGuardianStatus> statusList)
    {
        return statusList.Count(guardianStatus => guardianStatus.status == VerifierStatus.Verified);
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
