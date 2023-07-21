using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Portkey.UI
{
    public class ErrorViewController : MonoBehaviour
    {
        [SerializeField] private GameObject pinView;
        
        private GuardianIdentifierInfo _info;
        
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
            pinView.SetActive(true);
            CloseView();
        }
        
        public void SetGuardianIdentifierInfo(GuardianIdentifierInfo info)
        {
            _info = info;
        }
    }
}