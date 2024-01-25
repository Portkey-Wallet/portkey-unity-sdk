using System;
using System.Collections.Generic;
using System.Linq;
using Portkey.Core;
using Portkey.DID;
using TMPro;
using UnityEngine;

namespace Portkey.UI
{
    public class GuardianItemComponent : MonoBehaviour, ISerializationCallbackReceiver
    {
        [Serializable]
        public class VerifierIcon
        {
            public string name;
            public GameObject icon;
        }
        
        [Serializable]
        public class GuardianIcon
        {
            public AccountType type;
            public GameObject icon;
        }

        [SerializeField] private VerifyCodeViewController _verifyCodeViewController = null;
        [SerializeField] private VerifierIcon[] verifierIcons = null;
        [SerializeField] private GuardianIcon[] guardianIcons = null;
        [SerializeField] private TextMeshProUGUI account = null;
        [SerializeField] private TextMeshProUGUI detail = null;
        [SerializeField] private GameObject verifyButton = null;
        [SerializeField] private GameObject verifiedCheck = null;
        [SerializeField] private GameObject loginAccount = null;
        [SerializeField] private GameObject expiredText = null;

        private Dictionary<string, GameObject> _verifierIconMap = null;
        private Dictionary<AccountType, GameObject> _guardianIconMap = null;
        private DID.PortkeySDK _portkeySDK = null;
        private SuccessCallback<ApprovedGuardian> _onApproved = null;
        private Guardian _guardian = null;
        private bool _approved = false;
        private ICredential _credential = null;

        public void SetExpired(bool expired)
        {
            if (_approved)
            {
                return;
            }
            expiredText.SetActive(expired);
            if (!expired)
            {
                DisplayVerificationStatus(_approved);
            }
            else
            {
                verifiedCheck.SetActive(false);
                verifyButton.SetActive(false);
            }
        }
        
        public void SetEndOperation()
        {
            if (_approved)
            {
                return;
            }
            expiredText.SetActive(false);
            verifiedCheck.SetActive(false);
            verifyButton.SetActive(false);
        }

        public void Initialize(Guardian guardian, ICredential credential, DID.PortkeySDK portkeySDK, VerifyCodeViewController verifyCodeViewController, SuccessCallback<ApprovedGuardian> onApproved)
        {
            _portkeySDK = portkeySDK;
            _guardian = guardian;
            _verifyCodeViewController = verifyCodeViewController;
            _onApproved = onApproved;
            _approved = false;

            _credential = AuthService.IsCredentialMatchGuardian(credential, guardian) ? credential : null;
            
            DisplayGuardianIcon(guardian.accountType); ;
            DisplayVerifierIcon(guardian.verifier.name);
            
            var guardianText = GetGuardianText(guardian);
            account.text = guardianText.AccountText;
            detail.text = guardianText.DetailsText;
            detail.gameObject.SetActive(!guardianText.IsDisplayAccountTextOnly);

            loginAccount.SetActive(guardian.isLoginGuardian);
            
            DisplayVerificationStatus(_approved);
        }
        
        private static IGuardianText GetGuardianText(Guardian guardian) => guardian.accountType switch
        {
            AccountType.Apple  => new AppleGuardianText(guardian),
            AccountType.Google => new GoogleGuardianText(guardian),
            AccountType.Telegram => new TelegramGuardianText(guardian),
            AccountType.Phone  => new PhoneGuardianText(guardian),
            AccountType.Email  => new EmailGuardianText(guardian),
            _ => throw new ArgumentOutOfRangeException(nameof(guardian.accountType), $"Not expected AccountType value: {guardian.accountType}"),
        };

        private void DisplayVerificationStatus(bool verified)
        {
            verifyButton.SetActive(!verified);
            verifiedCheck.SetActive(verified);
        }

        private void DisplayGuardianIcon(AccountType guardianType)
        {
            Debugger.Log($"display icon");
            _guardianIconMap.ToList().ForEach(icon => icon.Value.SetActive(false));
            Debugger.Log($"icon list {_guardianIconMap.ToList()}");
            _guardianIconMap[guardianType]?.SetActive(true);
        }
        
        private void DisplayVerifierIcon(string verifierType)
        {
            _verifierIconMap.ToList().ForEach(verifier => verifier.Value.SetActive(false));
            _verifierIconMap[verifierType]?.SetActive(true);
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            _verifierIconMap = verifierIcons?.ToDictionary(verifierIcon => verifierIcon.name, verifierIcon => verifierIcon.icon);
            _guardianIconMap = guardianIcons?.ToDictionary(guardianIcon => guardianIcon.type, guardianIcon => guardianIcon.icon);
        }

        public void OnClickVerify()
        {
            if(_guardian.verifier.id == null)
            {
                _portkeySDK.AuthService.Message.Error("Verifier id does not exist!");
                return;
            }
            if(_guardian.id == null && _guardian.idHash == null)
            {
                _portkeySDK.AuthService.Message.Error("Guardian Identifier does not exist!");
                return;
            }

            if (_guardian.accountType is AccountType.Apple or AccountType.Google or AccountType.Telegram)
            {
                _portkeySDK.AuthService.Message.Loading(true, "Loading...");
                StartCoroutine(_portkeySDK.AuthService.Verify(_guardian, OnApproved, _credential));
            }
            else
            {
                _portkeySDK.AuthService.Message.Loading(true, "Loading...");
                _verifyCodeViewController.Initialize(_guardian, OnApproved);
            }
        }
        
        private void OnApproved(ApprovedGuardian approvedGuardian)
        {
            _portkeySDK.AuthService.Message.Loading(false);
            
            _approved = true;
            DisplayVerificationStatus(_approved);
            
            _onApproved?.Invoke(approvedGuardian);
        }
    }
}