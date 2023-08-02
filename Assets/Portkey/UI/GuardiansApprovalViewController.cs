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

public class GuardiansApprovalViewController : MonoBehaviour
{
    [SerializeField] private DID did;
    [SerializeField] private SetPINViewController setPINViewController;
    [SerializeField] private SignInViewController signInViewController;
    [SerializeField] private ErrorViewController errorView;
    [SerializeField] private LoadingViewController loadingView;
    [SerializeField] private GameObject infoDialog;
    
    [Header("Guardian Item List")]
    [SerializeField] private GameObject guardianItemList;
    [SerializeField] private GameObject guardianItemPrefab;

    [Header("Guardian Info")] 
    [SerializeField] private TextMeshProUGUI expireText;
    [SerializeField] private TextMeshProUGUI totalGuardiansText;
    [SerializeField] private TextMeshProUGUI totalVerifiedGuardiansText;
    [SerializeField] private GameObject completeButtonGameObject;
    [SerializeField] private Button completeButton;

    [Header("Progress Dial")]
    [SerializeField] private Image guardianProgressDial;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float maxProgress = 0.875f;
    [SerializeField] private GameObject completeProgress;
    [SerializeField] private GameObject dialProgress;
    
    
    [Header("Expiry Time")]
    [SerializeField] private int expiryInMilliseconds = 360000;
    
    private GuardianIdentifierInfo _guardianIdentifierInfo;
    private List<UserGuardianStatus> _guardianStatusList = new List<UserGuardianStatus>();
    private List<GuardiansApproved> _approvedGuardians = new List<GuardiansApproved>();
    private List<GuardianItemComponent> _guardianItemComponents = new List<GuardianItemComponent>();
    private float _timeElapsed = 0.0f;
    private bool _startTimer = false;
    private Action _onCompleted;
    private ErrorCallback _onErrorInitData;

    private void Start()
    {
        _startTimer = true;
    }

    public void InitializeData(Action onCompleted, ErrorCallback errorCallback)
    {
        _onCompleted = onCompleted;
        _onErrorInitData = errorCallback;
        
        ShowLoading(true, "Loading...");
        StaticCoroutine.StartCoroutine(did.GetVerifierServers(_guardianIdentifierInfo.chainId, StoreVerifierServers, _onErrorInitData));
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
        StaticCoroutine.StartCoroutine(did.GetHolderInfo(param, (result) =>
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
        }, _onErrorInitData));
    }

    private void InitializeUI()
    {
        expireText.text = "Expire after 1 hour.";
        totalGuardiansText.text = $"/{did.GetApprovalCount(_guardianStatusList.Count).ToString()}";
        completeButtonGameObject.SetActive(true);
        infoDialog.SetActive(false);
        
        UpdateGuardianInfoUI();

        ClearGuardianItems();
        CreateGuardianItems(_guardianStatusList);
        
        _onCompleted?.Invoke();
        ShowLoading(false);
    }

    private void UpdateGuardianInfoUI()
    {
        UpdateTotalVerifiedGuardiansText();
        UpdateGuardianProgressDial();

        var isCompletedVerification = VerifiedCount(_guardianStatusList) >= did.GetApprovalCount(_guardianStatusList.Count);
        SetSendButtonInteractable(isCompletedVerification);

        if (isCompletedVerification)
        {
            CompleteVerificationForGuardianItems();
        }
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
            UpdateExpiredUI();
            ResetTimer();
        }
    }

    private void UpdateExpiredUI()
    {
        expireText.text = "Expired";
        ExpireGuardianItems();
        completeButtonGameObject.SetActive(false);
    }

    private void ExpireGuardianItems()
    {
        _guardianItemComponents.Where(item => item.VerifierStatus != VerifierStatus.Verified).ToList().ForEach(item => item.SetExpired(true));
    }
    
    private void CompleteVerificationForGuardianItems()
    {
        _guardianItemComponents.Where(item => item.VerifierStatus != VerifierStatus.Verified).ToList().ForEach(item => item.SetEndOperation());
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

        _guardianItemComponents = new List<GuardianItemComponent>();
    }

    private UserGuardianStatus CreateUserGuardianStatus(Guardian guardian, IReadOnlyDictionary<string, UserGuardianStatus> guardianStatusMap)
    {
        var key = GetGuardianKey(guardian);
        var identifier = guardian.guardianIdentifier ?? guardian.identifierHash;
        var guardianItem = new GuardianItem
        {
            guardian = guardian,
            identifier = identifier,
            key = key
        };

        if (IsMatchingGuardianIdentifier(_guardianIdentifierInfo, guardianItem))
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
                guardianItem.ErrorView = errorView;
                guardianItem.LoadingView = loadingView;
                guardianItem.InitializeUI();
                
                _guardianItemComponents.Add(guardianItem);
            }
        }
    }

    private void OnUserGuardianStatusChanged(UserGuardianStatus status)
    {
        if (IsMatchingGuardianIdentifier(_guardianIdentifierInfo, status.guardianItem))
        {
            _guardianIdentifierInfo.token = status.guardianItem.accessToken;
        }
        
        UpdateGuardianInfoUI();
    }

    private void UpdateGuardianProgressDial()
    {
        var verifiedCount = VerifiedCount(_guardianStatusList);
        var approvalCount = did.GetApprovalCount(_guardianStatusList.Count);

        if (verifiedCount == approvalCount)
        {
            SetCompleteProgressDial(true);
        }
        else
        {
            SetCompleteProgressDial(false);
            
            var percentage = (float)verifiedCount / approvalCount;
            percentage = Mathf.Clamp(percentage, 0.0f, maxProgress);
            guardianProgressDial.fillAmount = percentage;
        }
    }

    private void SetCompleteProgressDial(bool complete)
    {
        completeProgress.SetActive(complete);
        dialProgress.SetActive(!complete);
    }

    private void UpdateTotalVerifiedGuardiansText()
    {
        totalVerifiedGuardiansText.text = VerifiedCount(_guardianStatusList).ToString();
    }

    private static int VerifiedCount(List<UserGuardianStatus> statusList)
    {
        return statusList.Count(guardianStatus => guardianStatus.status == VerifierStatus.Verified);
    }

    private static bool IsMatchingGuardianIdentifier(GuardianIdentifierInfo guardianIdentifierInfo, GuardianItem guardianItem)
    {
        return guardianIdentifierInfo.token != null && guardianIdentifierInfo.identifier == guardianItem.identifier && guardianIdentifierInfo.accountType == guardianItem.guardian.type;
    }

    public void SetGuardianIdentifierInfo(GuardianIdentifierInfo info)
    {
        _guardianIdentifierInfo = info;
    }
    
    private void OnError(string error)
    {
        ShowLoading(false);
        Debugger.LogError(error);
        errorView.ShowErrorText(error);
    }

    public void OnClickSend()
    {
        CloseView();

        var verifiedStatusList = _guardianStatusList.Where(status => status.signature != null && status.verificationDoc != null);
        foreach (var status in verifiedStatusList)
        {
            var guardiansApproved = new GuardiansApproved
            {
                type = status.guardianItem.guardian.type,
                identifier = status.guardianItem.identifier ?? status.guardianItem.guardian.identifierHash ?? "",
                signature = status.signature,
                verificationDoc = status.verificationDoc,
                verifierId = status.guardianItem.verifier?.id ?? ""
            };
            _approvedGuardians.Add(guardiansApproved);
        }
        
        OpenSetPINView();
    }

    private void OpenSetPINView()
    {
        foreach (var status in _guardianStatusList.Where(status => IsMatchingGuardianIdentifier(_guardianIdentifierInfo, status.guardianItem)))
        {
            setPINViewController.VerifierItem = status.guardianItem.verifier;
            break;
        }
        setPINViewController.gameObject.SetActive(true);
        setPINViewController.GuardiansApprovedList = _approvedGuardians;
        setPINViewController.GuardianIdentifierInfo = _guardianIdentifierInfo;
        setPINViewController.Operation = SetPINViewController.OperationType.SIGN_IN;
        setPINViewController.SetPreviousView(gameObject);
    }

    private void CloseView()
    {
        gameObject.SetActive(false);
    }
    
    public void OnClickClose()
    {
        CloseView();
    }
    
    public void OnClickBack()
    {
        ResetView();
        CloseView();
        signInViewController.gameObject.SetActive(true);
    }
    
    private void ShowLoading(bool show, string text = "")
    {
        loadingView.DisplayLoading(show, text);
    }

    public void ResetView()
    {
        _guardianStatusList = new List<UserGuardianStatus>();
        _approvedGuardians = new List<GuardiansApproved>();
        
        ClearGuardianItems();
        
        _timeElapsed = 0.0f;
        _startTimer = false;
    }

    public void OnClickInfoDialog()
    {
        infoDialog.SetActive(!infoDialog.activeSelf);
    }
}
