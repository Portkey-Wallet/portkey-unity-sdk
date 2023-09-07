using System;
using Portkey.Core;
using TMPro;
using UnityEngine;

namespace Portkey.UI
{
    public class PromptViewController : MonoBehaviour
    {
        [SerializeField] private DID.DID did;
        [SerializeField] private VerifyCodeViewController verifyCodeViewController;
        [SerializeField] private LoadingViewController loadingView;
        [SerializeField] private ErrorViewController errorView = null;
        [SerializeField] private TextMeshProUGUI descriptionText;
        
        public VerifierItem VerifierItem { get; set; }
        public GuardianIdentifierInfo GuardianIdentifierInfo { get; set; }
        
        public void ShowDescriptionText(string description)
        {
            descriptionText.text = description;
            gameObject.SetActive(true);
        }
        
        public void OnClickClose()
        {
            gameObject.SetActive(false);
        }
        
        public void OnClickConfirm()
        {
            SendVerificationCode();
        }
        
        private void SendVerificationCode()
        {
            ShowLoading(true, "Loading...");
            
            IVerifyCodeLogin serviceLogin = GuardianIdentifierInfo.accountType switch
            {
                AccountType.Email => did.AuthService.Email,
                AccountType.Phone => did.AuthService.Phone,
                _ => throw new ArgumentException($"Invalid account type: {GuardianIdentifierInfo.accountType}")
            };

            var param = new SendCodeParams
            {
                guardianId = GuardianIdentifierInfo?.identifier,
                verifierId = VerifierItem?.id,
                chainId = GuardianIdentifierInfo?.chainId
            };
            StartCoroutine(serviceLogin.SendCode(param, result => { OpenNextView(); }, OnError));
        }

        private void OpenNextView()
        {
            ShowLoading(false);
            gameObject.SetActive(false);
            verifyCodeViewController.Initialize(GuardianIdentifierInfo, VerifierItem);
            verifyCodeViewController.OpenView();
        }

        private void ShowLoading(bool show, string text = "")
        {
            loadingView.DisplayLoading(show, text);
        }

        private void OnError(string error)
        {
            ShowLoading(false);
            errorView.ShowErrorText(error);
            Debugger.LogError(error);
        }
    }
}