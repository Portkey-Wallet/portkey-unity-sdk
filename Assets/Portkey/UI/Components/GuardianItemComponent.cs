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
        
        [SerializeField]
        private VerifierIcon[] verifierIcons = null;
        [SerializeField]
        private GuardianIcon[] guardianIcons = null;
        [SerializeField]
        private TextMeshProUGUI account = null;
        [SerializeField] 
        private TextMeshProUGUI detail = null;
        [SerializeField] 
        private GameObject verifyButton = null;
        [SerializeField] 
        private GameObject verifiedCheck = null;
        
        private UserGuardianStatus _userGuardianStatus = null;
        private Dictionary<string, GameObject> _verifierIconMap = null;
        private Dictionary<AccountType, GameObject> _guardianIconMap = null;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetUserGuardianStatus(UserGuardianStatus status)
        {
            _userGuardianStatus = status;
            
            InitializeUI();
        }

        private void InitializeUI()
        {
            var guardianType = _userGuardianStatus.guardianItem.guardian.type;
            DisplayGuardianIcon(guardianType);

            var verifierType = _userGuardianStatus.guardianItem.verifier.name;
            DisplayVerifierIcon(verifierType);
            
            account.text = _userGuardianStatus.guardianItem.firstName;
            detail.text = _userGuardianStatus.guardianItem.thirdPartyEmail;
            
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
            
        }
    }
}