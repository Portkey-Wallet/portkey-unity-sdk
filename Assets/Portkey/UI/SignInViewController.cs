using System;
using System.Collections.Generic;
using Portkey.Utilities;
using Portkey.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Portkey.UI
{
    public class SignInViewController : MonoBehaviour
    {
        private enum State
        {
            Login,
            Signup
        }
        
        [SerializeField] private DID.DID did;
        [SerializeField] private UnregisteredViewController unregisteredView;
        [FormerlySerializedAs("guardianApprovalView")] [SerializeField] private GuardiansApprovalViewController guardianApprovalViewController;
        [SerializeField] private ErrorViewController errorView;
        [SerializeField] private LoadingViewController loadingView;
        [SerializeField] private EmailLoginViewController emailLoginViewController;
        [SerializeField] private PhoneLoginViewController phoneLoginViewController;
        [SerializeField] private SetPINViewController setPinViewController;
        
        private State _state = State.Login;
        
        public void Start()
        {
#if UNITY_WEBGL

            if (Application.absoluteURL.Contains("access_token="))
            {
                SignIn((int)AccountType.Google);
            }
#endif
        }
        
        public void SignIn(int type)
        {
            var accountType = (AccountType)type;

            switch (accountType)
            {
                case AccountType.Apple:
                    did.AuthService.AppleCredentialProvider.Get(AuthCallback);
                    break;
                case AccountType.Google:
                    did.AuthService.GoogleCredentialProvider.Get(AuthCallback);
                    break;
                case AccountType.Email:
                    emailLoginViewController.DID = did;
                    emailLoginViewController.PreviousView = gameObject;
                    emailLoginViewController.gameObject.SetActive(true);
                    break;
                case AccountType.Phone:
                    //TODO: open up window to key in phone number or email then call ValidateIdentifier
                    phoneLoginViewController.DID = did;
                    phoneLoginViewController.PreviousView = gameObject;
                    phoneLoginViewController.gameObject.SetActive(true);
                    break;
                default:
                    throw new ArgumentException("Not expected account type!");
            }
        }

        private void OnStartLoading(bool show)
        {
            ShowLoading(show, "Checking account on the chain...");
        }

        private void ShowLoading(bool show, string text = "")
        {
            loadingView.DisplayLoading(show, text);
        }

        private void AuthCallback(ICredential credential)
        {
            did.AuthService.GetGuardians(credential, guardians =>
            {
                CheckSignUpOrLogin(credential, guardians);
            });
        }

        private void OnError(string error)
        {
            Debugger.LogError(error);
            ShowLoading(false);
            errorView.ShowErrorText(error);
        }

        private void CheckSignUpOrLogin(ICredential credential, List<GuardianNew> guardians)
        {
            ShowLoading(false);
            
            switch (guardians.Count)
            {
                case 0:
                    SignUpPrompt(() =>
                    {
                        setPinViewController.Initialize(credential);
                        setPinViewController.SetPreviousView(gameObject);
                    });
                    break;
                default:
                    //Change to Login View
                    PrepareGuardiansApprovalView(info);
                    break;
            }
        }
        
        private void SignUpPrompt(Action onConfirm, Action onClose = null)
        {
            unregisteredView.Initialize("Continue with this account?", "This account has not been registered yet. Click \"Confirm\" to complete the registration.", onConfirm, onClose);
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

        public void OnClickClose()
        {
            CloseView();
        }
        
        private void CloseView()
        {
            gameObject.SetActive(false);
        }
    }
}
