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
        [SerializeField] private SetPINViewController setPinViewController = null;
        [SerializeField] private SignInViewController signInViewController = null;
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
                chainId = GuardianIdentifierInfo?.chainId,
                operationType = OperationTypeEnum.register
            };
            StartCoroutine(serviceLogin.SendCode(param, result => { OpenNextView(); }, OnError));
            
            OnClickClose();
        }

        private void OpenNextView()
        {
            ShowLoading(false);
            gameObject.SetActive(false);
            
            var arg = new VerifyCodeViewArg
            {
                accountType = GuardianIdentifierInfo.accountType,
                guardianIdentifier = GuardianIdentifierInfo.identifier,
                chainId = GuardianIdentifierInfo.chainId,
                operationType = OperationTypeEnum.register
            };
            verifyCodeViewController.Initialize(arg, VerifierItem, result =>
            {
                setPinViewController.VerifierItem = VerifierItem;
                setPinViewController.gameObject.SetActive(true);
                setPinViewController.GuardianIdentifierInfo = GuardianIdentifierInfo;
                setPinViewController.VerifyCodeResult = result;
                setPinViewController.Operation = SetPINViewController.OperationType.SIGN_UP;
                setPinViewController.SetPreviousView(signInViewController.gameObject);
            });
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