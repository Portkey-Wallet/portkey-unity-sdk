using System.Collections.Generic;
using Portkey.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Portkey.UI
{
    public class GuardiansApprovalViewController : MonoBehaviour
    {
        [FormerlySerializedAs("did")] [SerializeField] private DID.PortkeySDK portkeySDK;
        [SerializeField] private SetPINViewController setPINViewController;
        [SerializeField] private SignInViewController signInViewController;
        [SerializeField] private VerifyCodeViewController verifyCodeViewController;
        [SerializeField] private GameObject infoDialog;

        [Header("Guardian Item List")] [SerializeField]
        private GameObject guardianItemList;

        [SerializeField] private GameObject guardianItemPrefab;

        [Header("Guardian Info")] [SerializeField]
        private TextMeshProUGUI expireText;

        [SerializeField] private TextMeshProUGUI totalGuardiansText;
        [SerializeField] private TextMeshProUGUI totalVerifiedGuardiansText;
        [SerializeField] private ButtonComponent completeButtonGameObject;
        [SerializeField] private GameObject guardiansInfoLayout;
        [SerializeField] private GameObject guardiansInfoTextLayout;

        [Header("Progress Dial")] [SerializeField]
        private Image guardianProgressDial;

        [Range(0.0f, 1.0f)] [SerializeField] private float maxProgress = 0.875f;
        [SerializeField] private GameObject completeProgress;
        [SerializeField] private GameObject dialProgress;


        [Header("Expiry Time")] [SerializeField]
        private int expiryInMilliseconds = 360000;

        private List<GuardianItemComponent> _guardianItemComponents = new List<GuardianItemComponent>();
        private float _timeElapsed = 0.0f;
        private bool _startTimer = false;
        private bool _isUpdatedGuardianUIInfo = false;
        private ICredential _credential = null;
        private List<Guardian> _guardians = new List<Guardian>();
        private List<ApprovedGuardian> _approvedGuardians = new List<ApprovedGuardian>();

        private void Start()
        {
            _startTimer = true;
        }

        public void Initialize(List<Guardian> guardians, ICredential credential = null)
        {
            _guardians = guardians;
            _credential = credential;
            
            portkeySDK.AuthService.EmailCredentialProvider.EnableCodeSendConfirmationFlow = false;
            portkeySDK.AuthService.PhoneCredentialProvider.EnableCodeSendConfirmationFlow = false;
            
            expireText.text = "Expire after 1 hour.";
            totalGuardiansText.text = $"/{portkeySDK.AuthService.GetRequiredApprovedGuardiansCount(guardians.Count).ToString()}";
            completeButtonGameObject.gameObject.SetActive(true);
            infoDialog.SetActive(false);

            UpdateGuardianInfoUI();

            ClearGuardianItems();
            CreateGuardianItems(guardians);
            
            OpenView();
        }

        public void OpenView()
        {
            gameObject.SetActive(true);
        }

        private void LateUpdate()
        {
            if (!_isUpdatedGuardianUIInfo)
            {
                return;
            }

            _isUpdatedGuardianUIInfo = false;

            LayoutRebuilder.ForceRebuildLayoutImmediate(guardiansInfoTextLayout.transform as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(guardiansInfoLayout.transform as RectTransform);
        }

        private void UpdateGuardianInfoUI()
        {
            UpdateTotalApprovedGuardiansText();
            UpdateGuardianProgressDial();

            var isCompletedVerification = _approvedGuardians.Count >= portkeySDK.AuthService.GetRequiredApprovedGuardiansCount(_guardians.Count);
            SetSendButtonInteractable(isCompletedVerification);

            if (isCompletedVerification)
            {
                CompleteVerificationForGuardianItems();
            }

            _isUpdatedGuardianUIInfo = true;
        }

        private void Update()
        {
            if (!_startTimer)
            {
                return;
            }

            _timeElapsed += Time.deltaTime;
            if (_timeElapsed >= expiryInMilliseconds / 1000.0f)
            {
                UpdateExpiredUI();
                ResetTimer();
            }
        }

        private void UpdateExpiredUI()
        {
            expireText.text = "Expired";
            ExpireGuardianItems();
            completeButtonGameObject.gameObject.SetActive(false);
        }

        private void ExpireGuardianItems()
        {
            _guardianItemComponents.ForEach(item => item.SetExpired(true));
        }

        private void CompleteVerificationForGuardianItems()
        {
            _guardianItemComponents.ForEach(item => item.SetEndOperation());
        }

        private void ResetTimer()
        {
            _startTimer = false;
            _timeElapsed = 0.0f;
        }

        private void SetSendButtonInteractable(bool interactable)
        {
            completeButtonGameObject.SetDisabled(!interactable);
        }

        private void ClearGuardianItems()
        {
            foreach (Transform child in guardianItemList.transform)
            {
                Destroy(child.gameObject);
            }

            _guardianItemComponents = new List<GuardianItemComponent>();
        }

        private void CreateGuardianItems(List<Guardian> guardians)
        {
            foreach (var guardian in guardians)
            {
                var guardianItem = Instantiate(guardianItemPrefab, guardianItemList.transform).GetComponent<GuardianItemComponent>();
                if (guardianItem == null)
                {
                    continue;
                }

                ICredential cred = null;
                if (_credential.AccountType == guardian.accountType && _credential.SocialInfo.sub == guardian.id)
                {
                    cred = _credential;
                }
                
                guardianItem.Initialize(guardian, cred, portkeySDK, verifyCodeViewController, approvedGuardian =>
                {
                    _approvedGuardians.Add(approvedGuardian);
                    UpdateGuardianInfoUI();
                });

                _guardianItemComponents.Add(guardianItem);
            }
        }

        private void UpdateGuardianProgressDial()
        {
            var approvedCount = _approvedGuardians.Count;
            var requiredCount = portkeySDK.AuthService.GetRequiredApprovedGuardiansCount(_guardians.Count);

            if (approvedCount == requiredCount)
            {
                SetCompleteProgressDial(true);
            }
            else
            {
                SetCompleteProgressDial(false);

                var percentage = (float)approvedCount / requiredCount;
                percentage = Mathf.Clamp(percentage, 0.0f, maxProgress);
                guardianProgressDial.fillAmount = percentage;
            }
        }

        private void SetCompleteProgressDial(bool complete)
        {
            completeProgress.SetActive(complete);
            dialProgress.SetActive(!complete);
        }

        private void UpdateTotalApprovedGuardiansText()
        {
            totalVerifiedGuardiansText.text = _approvedGuardians.Count.ToString();
        }

        public void OnClickSend()
        {
            CloseView();
            OpenSetPINView();
        }

        private void OpenSetPINView()
        {
            setPINViewController.Initialize(_guardians[0], _approvedGuardians);
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

        public void ResetView()
        {
            _approvedGuardians = new List<ApprovedGuardian>();

            ClearGuardianItems();

            _timeElapsed = 0.0f;
            _startTimer = false;
        }

        public void OnClickInfoDialog()
        {
            infoDialog.SetActive(!infoDialog.activeSelf);
        }
    }
}