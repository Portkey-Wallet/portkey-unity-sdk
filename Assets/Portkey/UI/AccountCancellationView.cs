using System;
using Portkey.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Portkey.UI
{
    public class AccountCancellationView : MonoBehaviour
    {
        [Header("Necessasities")] 
        [SerializeField] private DID.PortkeySDK portkeySDK;
        
        [Header("Views")]
        [SerializeField] private PromptViewController promptView;
        
        private Action _onDeleteAccount;
        private DIDAccountInfo _accountInfo;
        
        public void Initialize(DIDAccountInfo accountInfo, Action onDeleteAccount)
        {
            _accountInfo = accountInfo;
            _onDeleteAccount = onDeleteAccount;
            OpenView();
            
            promptView.Initialize("Warning", "Please note that once you cancel your account, it cannot be recovered. Do you still want to proceed with the account cancellation?", null, OnPromptClose);
        }
        
        private void OnPromptClose()
        {
            CloseView();
        }
        
        public void OnClickClose()
        {
            CloseView();
        }
        
        public void OnClickConfirm()
        {
            StartCoroutine(portkeySDK.ValidateAccountDeletion(_accountInfo, OnValidateAccountDeletion, OnError));
        }
        
        private void OnValidateAccountDeletion(AccountDeletionValidationResult result)
        {
            if (!result.validatedAssets)
            {
                OnError("Please transfer all of your assets out of your account, including Tokens and NFTs.");
                return;
            }
            if (!result.validatedGuardian)
            {
                OnError("Please ensure that other users have already disassociated the Guardian from your current Apple ID.");
                return;
            }
            if (!result.validatedDevice)
            {
                OnError("Please remove other login devices.");
                return;
            }
            
            promptView.Initialize("Warning", "Are you sure you want to delete your account? Please note that you won't be able to recover your account once it's deleted.", OnConfirmDeleteAccount, OnPromptClose);
        }

        private void OnConfirmDeleteAccount()
        {
            StartCoroutine(portkeySDK.DeleteAccount(_accountInfo, result =>
            {
                if (result)
                {
                    CloseView();
                    _onDeleteAccount?.Invoke();
                }
                else
                {
                    OnError("Failed to delete account!");
                }
            }, OnError));
        }
        
        private void OnError(string error)
        {
            portkeySDK.AuthService.Message.Error(error);
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