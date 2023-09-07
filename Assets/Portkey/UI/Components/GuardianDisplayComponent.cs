using System;
using System.Collections.Generic;
using System.Linq;
using Portkey.Core;
using TMPro;
using UnityEngine;

namespace Portkey.UI
{
    public class GuardianDisplayComponent : MonoBehaviour, ISerializationCallbackReceiver
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

        private GuardianIdentifierInfo _guardianIdentifierInfo = null;
        private Dictionary<string, GameObject> _verifierIconMap = null;
        private Dictionary<AccountType, GameObject> _guardianIconMap = null;

        private VerifierItem VerifierItem { get; set; }
        
        public void Initialize(GuardianIdentifierInfo info, VerifierItem verifierItem)
        {
            VerifierItem = verifierItem;
            _guardianIdentifierInfo = info;
            InitializeUI();
        }

        private void InitializeUI()
        {
            var guardianType = _guardianIdentifierInfo.accountType;
            DisplayGuardianIcon(guardianType);

            var verifierType = VerifierItem.name;
            DisplayVerifierIcon(verifierType);

            account.text = _guardianIdentifierInfo.identifier;
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
    }
}