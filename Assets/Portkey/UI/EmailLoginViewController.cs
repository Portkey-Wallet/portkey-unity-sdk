using System;
using System.Collections.Generic;
using Portkey.Core;
using Portkey.SocialProvider;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Portkey.UI
{
    public class EmailLoginViewController : MonoBehaviour
    {
        [Header("Views")]
        [SerializeField] protected GuardiansApprovalViewController guardianApprovalViewController;
        [SerializeField] protected UnregisteredViewController unregisteredView;
        [SerializeField] protected ErrorViewController errorView;
        [SerializeField] protected LoadingViewController loadingView;
        [SerializeField] protected VerifyCodeViewController verifyCodeViewController;
        [SerializeField] protected SetPINViewController setPinViewController;
        
        [Header("UI Elements")]
        [SerializeField] protected TMP_InputField inputField;
        [SerializeField] protected Button loginButton;
        [SerializeField] protected TextMeshProUGUI errorText;
        
        public GameObject PreviousView { get; set; }
        public DID.DID DID { get; set; }

        protected void OnEnable()
        {
            ResetView();
            DID.AuthService.Message.OnVerifierServerSelectedEvent += OnVerifierServerSelected;
        }

        protected void ResetView()
        {
            inputField.text = "";
            errorText.text = "";
            loginButton.interactable = false;
        }

        public void OnClickBack()
        {
            ResetView();
            CloseView();
            PreviousView.SetActive(true);
        }

        public virtual void OnClickLogin()
        {
            StartLoading();

            var emailAddress = EmailAddress.Parse(inputField.text);
            DID.AuthService.GetGuardians(emailAddress, guardians =>
            {
                CheckSignUpOrLogin(emailAddress, guardians);
            });
        }
        
        protected void ShowLoading(bool show, string text = "")
        {
            loadingView.DisplayLoading(show, text);
        }
        
        protected void StartLoading()
        {
            ShowLoading(true, "Checking account on the chain...");
        }
        
        protected void OnError(string error)
        {
            Debugger.LogError(error);
            ShowLoading(false);
            errorView.ShowErrorText(error);
        }
        
        private void CheckSignUpOrLogin(EmailAddress emailAddress, List<GuardianNew> guardians)
        {
            ShowLoading(false);
            
            switch (guardians.Count)
            {
                case 0:
                    SignUpPrompt(() =>
                    {
                        ShowLoading(true, "Loading...");
                        StartCoroutine(DID.AuthService.EmailCredentialProvider.Get(emailAddress, credential =>
                        {
                            verifyCodeViewController.VerifyCode(credential);
                        }));
                    });
                    break;
                default:
                    //Change to Login View
                    PrepareGuardiansApprovalView(info);
                    break;
            }
        }

        protected void SignUpPrompt(Action onConfirm, Action onClose = null)
        {
            unregisteredView.Initialize("Continue with this account?", "This account has not been registered yet. Click \"Confirm\" to complete the registration.", onConfirm, onClose);
        }

        protected void OnVerifierServerSelected(string guardianId, AccountType accountType, string verifierServerName)
        {
            ShowLoading(false);
            
            var type = accountType == AccountType.Email? "email address" : "phone number";
            var description = $"{verifierServerName} will send a verification code to {guardianId} to verify your {type}.";
            unregisteredView.Initialize("", description, () =>
            {
                DID.AuthService.Message.ConfirmSendCode();
                
                verifyCodeViewController.Initialize(guardianId, accountType, verifierServerName, verifiedCredential =>
                {
                    setPinViewController.Initialize(verifiedCredential);
                    setPinViewController.SetPreviousView(PreviousView);
                });
            }); 
        }
        
        private void PrepareGuardiansApprovalView(GuardianIdentifierInfo info)
        {
            guardianApprovalViewController.SetGuardianIdentifierInfo(info);
            guardianApprovalViewController.InitializeData(OpenGuardiansApprovalView, OnError);
        }
        
        private void OpenGuardiansApprovalView()
        {
            guardianApprovalViewController.gameObject.SetActive(true);
            CloseView();
        }

        public void OnValueChanged()
        {
            var value = inputField.text;
            if (LoginHelper.IsValidEmail(value))
            {
                loginButton.interactable = true;
                errorText.text = "";
                return;
            }
            
            errorText.text = string.IsNullOrEmpty(value) ? "" : "Invalid email.";
            loginButton.interactable = false;
        }
        
        public void OnClickClose()
        {
            CloseView();
        }
        
        private void CloseView()
        {
            DID.AuthService.Message.OnVerifierServerSelectedEvent -= OnVerifierServerSelected;
            gameObject.SetActive(false);
        }
    }
}