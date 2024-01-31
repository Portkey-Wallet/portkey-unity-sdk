using System;
using System.Text;
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
            portkeySDK.AuthService.Message.Loading(true, "Loading...");
            StartCoroutine(portkeySDK.ValidateAccountDeletion(_accountInfo, OnValidateAccountDeletion, OnError));
        }
        
        private void OnValidateAccountDeletion(AccountDeletionValidationResult result)
        {
            portkeySDK.AuthService.Message.Loading(false);
            
            var errorMessage = new StringBuilder();
            
            if (!result.validatedAssets)
            {
                errorMessage.AppendLine("Please transfer all of your assets out of your account, including Tokens and NFTs.\n");
            }
            if (!result.validatedGuardian)
            {
                errorMessage.AppendLine("Please ensure that other users have already disassociated the Guardian from your current Apple ID.\n");
            }
            if (!result.validatedDevice)
            {
                errorMessage.AppendLine("Please remove other login devices.\n");
            }
            
            if (errorMessage.Length > 0)
            {
                OnError(errorMessage.ToString());
                return;
            }
            
            promptView.Initialize("Warning", "Are you sure you want to delete your account? Please note that you won't be able to recover your account once it's deleted.", OnConfirmDeleteAccount, OnPromptClose);
        }

        private void OnConfirmDeleteAccount()
        {
            portkeySDK.AuthService.Message.Loading(true, "Loading...");
            
            StartCoroutine(portkeySDK.DeleteAccount(_accountInfo, result =>
            {
                portkeySDK.AuthService.Message.Loading(false);
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