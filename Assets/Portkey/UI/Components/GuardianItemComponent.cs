using System;
using System.Collections.Generic;
using System.Linq;
using Portkey.Core;
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
        
        [SerializeField] private VerifierIcon[] verifierIcons = null;
        [SerializeField] private GuardianIcon[] guardianIcons = null;
        [SerializeField] private TextMeshProUGUI account = null;
        [SerializeField] private TextMeshProUGUI detail = null;
        [SerializeField] private GameObject verifyButton = null;
        [SerializeField] private GameObject verifiedCheck = null;
        [SerializeField] private GameObject loginAccount = null;

        private UserGuardianStatus _userGuardianStatus = null;
        private GuardianIdentifierInfo _guardianIdentifierInfo = null;
        private Dictionary<string, GameObject> _verifierIconMap = null;
        private Dictionary<AccountType, GameObject> _guardianIconMap = null;
        private DID.DID _did = null;
        private LoadingViewController _loadingView = null;
        private ErrorViewController _errorView = null;
        
        public delegate void OnUserGuardianStatusChanged(UserGuardianStatus status);
        private OnUserGuardianStatusChanged _onUserGuardianStatusChanged = null;
        
        public LoadingViewController LoadingView
        {
            set => _loadingView = value;
        }
        
        public ErrorViewController ErrorView
        {
            set => _errorView = value;
        }

        public void SetDID(DID.DID did)
        {
            _did = did;
        }
        
        private void ShowLoading(bool show, string text = "")
        {
            _loadingView.DisplayLoading(show, text);
        }

        public void SetGuardianIdentifierInfo(GuardianIdentifierInfo info)
        {
            _guardianIdentifierInfo = info;
        }

        public void SetUserGuardianStatus(UserGuardianStatus status, OnUserGuardianStatusChanged onUserGuardianStatusChanged)
        {
            _userGuardianStatus = status;
            _onUserGuardianStatusChanged = onUserGuardianStatusChanged;
        }

        public void InitializeUI()
        {
            var guardianType = _userGuardianStatus.guardianItem.guardian.type;
            DisplayGuardianIcon(guardianType);

            var verifierType = _userGuardianStatus.guardianItem.verifier.name;
            DisplayVerifierIcon(verifierType);
            
            account.text = _userGuardianStatus.guardianItem.guardian.firstName;
            detail.text = _userGuardianStatus.guardianItem.guardian.thirdPartyEmail;
            loginAccount.SetActive(_userGuardianStatus.guardianItem.guardian.isLoginGuardian);
            
            DisplayVerificationStatus(_userGuardianStatus.status);
        }

        private void DisplayVerificationStatus(VerifierStatus status)
        {
            if (status == VerifierStatus.Verified)
            {
                verifyButton.SetActive(false);
                verifiedCheck.SetActive(true);
            }
            else
            {
                verifyButton.SetActive(true);
                verifiedCheck.SetActive(false);
            }
        }

        private void DisplayGuardianIcon(AccountType guardianType)
        {
            _guardianIconMap.ToList().ForEach(icon => icon.Value.SetActive(false));
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
            ShowLoading(true, "Loading...");
            
            var loginType = _userGuardianStatus.guardianItem.guardian.type;
            // let's see if we need this when we implement email and phone, very likely we don't need this
            if (loginType == AccountType.Google || loginType == AccountType.Apple)
            {
                if (_userGuardianStatus.guardianItem.verifier?.id == null)
                {
                    OnError("Verifier id does not exist!");
                    return;
                }

                var id = _userGuardianStatus.guardianItem.identifier ?? _userGuardianStatus.guardianItem.guardian.identifierHash;
                if (id == null)
                {
                    OnError("Identifier does not exist!");
                    return;
                }
                
                var socialVerifier = _did.GetSocialVerifier(loginType);

                var param = new VerifyAccessTokenParam
                {
                    verifierId = _userGuardianStatus.guardianItem.verifier?.id,
                    accessToken = _userGuardianStatus.guardianItem.accessToken,
                    chainId = _guardianIdentifierInfo.chainId
                };
                
                socialVerifier.AuthenticateIfAccessTokenExpired(param, OnVerified, OnError);
            }
        }
        
        private void OnVerified(string verifierId, string accessToken, VerifyVerificationCodeResult verificationResult)
        {
            _userGuardianStatus.status = VerifierStatus.Verified;
            _userGuardianStatus.verificationDoc = verificationResult.verificationDoc;
            _userGuardianStatus.signature = verificationResult.signature;
            _userGuardianStatus.guardianItem.accessToken = accessToken;
            DisplayVerificationStatus(_userGuardianStatus.status);
            
            _onUserGuardianStatusChanged?.Invoke(_userGuardianStatus);
            
            ShowLoading(false);
        }

        private void OnError(string error)
        {
            ShowLoading(false);
            Debugger.LogError(error);
            _errorView.ShowErrorText("Error: Network error occurred!");
        }
    }
}