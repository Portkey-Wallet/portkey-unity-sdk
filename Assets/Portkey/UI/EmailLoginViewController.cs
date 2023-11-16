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
        [SerializeField] protected VerifyCodeViewController verifyCodeViewController;
        [SerializeField] protected SetPINViewController setPinViewController;
        
        [Header("UI Elements")]
        [SerializeField] protected TMP_InputField inputField;
        [SerializeField] protected Button loginButton;
        [SerializeField] protected TextMeshProUGUI errorText;
        
        protected GameObject PreviousView { get; set; }
        protected DID.PortkeySDK PortkeySDK { get; set; }

        protected void OnEnable()
        {
            ResetView();
            PortkeySDK.AuthService.Message.OnVerifierServerSelectedEvent += OnVerifierServerSelected;
            PortkeySDK.AuthService.EmailCredentialProvider.EnableCodeSendConfirmationFlow = true;
        }
        
        public void Initialize(DID.PortkeySDK portkeySDK, GameObject previousView)
        {
            PortkeySDK = portkeySDK;
            PreviousView = previousView;

            OpenView();
        }
        
        public void OpenView()
        {
            gameObject.SetActive(true);
        }

        protected void ResetView()
        {
            inputField.text = "";
            errorText.text = "";
        }

        public void OnClickBack()
        {
            ResetView();
            CloseView();
            PreviousView.SetActive(true);
        }

        public virtual void OnClickLogin()
        {
            var value = inputField.text;
            
            if (!LoginHelper.IsValidEmail(value))
            {
                errorText.text = string.IsNullOrEmpty(value) ? "" : "Invalid email.";
                return;
            }

            errorText.text = "";
            
            StartLoading();

            var emailAddress = EmailAddress.Parse(inputField.text);
            StartCoroutine(PortkeySDK.AuthService.GetGuardians(emailAddress, guardians =>
            {
                CheckSignUpOrLogin(emailAddress, guardians);
            }));
        }
        
        protected void ShowLoading(bool show, string text = "")
        {
            PortkeySDK.AuthService.Message.Loading(show, text);
        }
        
        protected void StartLoading()
        {
            PortkeySDK.AuthService.Message.Loading(true, "Checking account on the chain...");
        }
        
        private void CheckSignUpOrLogin(EmailAddress emailAddress, List<Guardian> guardians)
        {
            ShowLoading(false);
            
            switch (guardians.Count)
            {
                case 0:
                    SignUpPrompt(() =>
                    {
                        ShowLoading(true, "Assigning a verifier on-chain...");
                        StartCoroutine(PortkeySDK.AuthService.EmailCredentialProvider.SendCode(emailAddress, result =>
                        {
                            ShowLoading(false);
                        }));
                    });
                    break;
                default:
                    guardianApprovalViewController.Initialize(guardians);
                    CloseView();
                    break;
            }
        }

        protected void OpenSetPINView(VerifiedCredential verifiedCredential)
        {
            verifyCodeViewController.CloseView();
            CloseView();
            setPinViewController.Initialize(verifiedCredential);
            setPinViewController.SetPreviousView(PreviousView);
        }

        protected void SignUpPrompt(Action onConfirm, Action onClose = null)
        {
            unregisteredView.Initialize("Continue with this account?", "This account has not been registered yet. Click \"Confirm\" to complete the registration.", onConfirm, onClose);
        }

        protected void OnVerifierServerSelected(string guardianId, AccountType accountType, Verifier verifier)
        {
            ShowLoading(false);
            
            var type = accountType == AccountType.Email? "email address" : "phone number";
            var description = $"{verifier.name} will send a verification code to {guardianId} to verify your {type}.";
            unregisteredView.Initialize("", description, () =>
            {
                ShowLoading(true, "Loading...");
                
                PortkeySDK.AuthService.Message.ConfirmSendCode();
                
                verifyCodeViewController.Initialize(guardianId, accountType, verifier, OpenSetPINView);
            }); 
        }
        
        public void OnClickClose()
        {
            CloseView();
        }
        
        protected void CloseView()
        {
            PortkeySDK.AuthService.Message.OnVerifierServerSelectedEvent -= OnVerifierServerSelected;
            gameObject.SetActive(false);
        }
    }
}