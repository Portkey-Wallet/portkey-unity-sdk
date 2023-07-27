using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Portkey.UI
{
    public class UnregisteredViewController : MonoBehaviour
    {
        [SerializeField] private SetPINViewController setPinViewController;
        [SerializeField] private SignInViewController signInViewController;
        
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
            setPinViewController.gameObject.SetActive(true);
            setPinViewController.GuardianIdentifierInfo = _guardianIdentifierInfo;
            setPinViewController.SetPreviousView(signInViewController.gameObject);
            CloseView();
        }
        
        public void SetGuardianIdentifierInfo(GuardianIdentifierInfo info)
        {
            _guardianIdentifierInfo = info;
        }
    }
}