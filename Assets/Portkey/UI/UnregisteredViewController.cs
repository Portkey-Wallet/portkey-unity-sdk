using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Portkey.UI
{
    public class UnregisteredViewController : MonoBehaviour
    {
        [SerializeField] private SetPINViewController pinView;
        
        private GuardianIdentifierInfo _guardianIdentifierInfo;
        
        public void OnClickClose()
        {
            CloseView();
        }

        private void CloseView()
        {
            gameObject.SetActive(false);
        }

        public void OnClickSignUp()
        {
            pinView.gameObject.SetActive(true);
            pinView.GuardianIdentifierInfo = _guardianIdentifierInfo;
            CloseView();
        }
        
        public void SetGuardianIdentifierInfo(GuardianIdentifierInfo info)
        {
            _guardianIdentifierInfo = info;
        }
    }
}