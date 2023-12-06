using System;
using UnityEngine;

namespace Portkey.UI
{
    public class AccountCancellationView : MonoBehaviour
    {
        [Header("Necessasities")] 
        [SerializeField] private DID.PortkeySDK portkeySDK;
        
        [Header("Views")]
        [SerializeField] private PromptViewController promptView;
        
        private Action _onDeleteAccount;

        private void OnEnable()
        {
            promptView.Initialize("Warning", "Please note that once you cancel your account, it cannot be recovered. Do you still want to proceed with the account cancellation?", null, OnPromptClose);
        }
        
        public void Initialize(Action onDeleteAccount)
        {
            _onDeleteAccount = onDeleteAccount;
            OpenView();
        }
        
        private void OnPromptClose()
        {
            CloseView();
        }
        
        public void OnClickClose()
        {
            CloseView();
        }
        
        public void CloseView()
        {
            gameObject.SetActive(false);
        }

        private void OpenView()
        {
            gameObject.SetActive(true);
        }
    }
}