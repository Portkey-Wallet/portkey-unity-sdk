using System;
using System.Collections.Generic;
using Portkey.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Portkey.UI
{
    public class SignInViewController : MonoBehaviour
    {
        [FormerlySerializedAs("did")] [SerializeField] private DID.PortkeySDK portkeySDK;
        [FormerlySerializedAs("unregisteredView")] [SerializeField] private PromptViewController promptView;
        [FormerlySerializedAs("guardianApprovalView")] [SerializeField] private GuardiansApprovalViewController guardianApprovalViewController;
        [SerializeField] private ErrorViewController errorView;
        [SerializeField] private LoadingViewController loadingView;
        [SerializeField] private EmailLoginViewController emailLoginViewController;
        [SerializeField] private PhoneLoginViewController phoneLoginViewController;
        [SerializeField] private SetPINViewController setPinViewController;
        [SerializeField] private QRCodeViewController qrCodeViewController;
        [SerializeField] private CancelLoadingViewController cancelLoadingViewController;

        public void SignIn(int type)
        {
            var accountType = (AccountType)type;

            switch (accountType)
            {
                case AccountType.Apple:
                    StartLoginLoading();
                    portkeySDK.AuthService.AppleCredentialProvider.Get(AuthCallback);
                    break;
                case AccountType.Google:
                    StartLoginLoading();
                    portkeySDK.AuthService.GoogleCredentialProvider.Get(AuthCallback);
                    break;
                case AccountType.Email:
                    emailLoginViewController.Initialize(portkeySDK, gameObject);
                    break;
                case AccountType.Phone:
                    phoneLoginViewController.Initialize(portkeySDK, gameObject);
                    break;
                default:
                    throw new ArgumentException("Not expected account type!");
            }
        }
        
        private void StartLoginLoading()
        {
            portkeySDK.AuthService.Message.Loading(true, "Loading...");
#if UNITY_EDITOR || UNITY_STANDALONE
            cancelLoadingViewController.Initialize(portkeySDK, gameObject);
#endif
        }
        
#if UNITY_WEBGL && !UNITY_EDITOR
        public void SignInWithExtension()
        {
            StartCoroutine(portkeySDK.AuthService.LoginWithPortkeyExtension(LoggedIn));
        }
#endif

        public void SignInWithApp()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            SignInWithExtension();
#elif (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            portkeySDK.AuthService.Message.Loading(true, "Loading...");
            cancelLoadingViewController.Initialize(portkeySDK, gameObject, () =>
            {
                portkeySDK.AuthService.Message.CancelLoginWithPortkeyApp();
            });
            StartCoroutine(portkeySDK.AuthService.LoginWithPortkeyApp(accountInfo =>
            {
                cancelLoadingViewController.CloseView();
                LoggedIn(accountInfo);
            }));
#endif
        }
        
        public void SignInWithQRCode()
        {
            qrCodeViewController.Initialize(LoggedIn);
        }

        private void LoggedIn(DIDAccountInfo accountInfo)
        {
            portkeySDK.AuthService.Message.Loading(false);
            setPinViewController.Initialize(accountInfo);
            setPinViewController.SetPreviousView(gameObject);
            CloseView();
        }

        private void AuthCallback(ICredential credential)
        {
            StartCoroutine(portkeySDK.AuthService.GetGuardians(credential, guardians =>
            {
                CheckSignUpOrLogin(credential, guardians);
            }));
#if UNITY_EDITOR || UNITY_STANDALONE
            cancelLoadingViewController.CloseView();
#endif
        }
        
        private void CheckSignUpOrLogin(ICredential credential, List<Guardian> guardians)
        {
            portkeySDK.AuthService.Message.Loading(false);
            
            switch (guardians.Count)
            {
                case 0:
                    SignUpPrompt(() =>
                    {
                        switch(credential.AccountType)
                        {
                            case AccountType.Apple:
                                StartCoroutine(portkeySDK.AuthService.AppleCredentialProvider.Verify(credential, OpenSetPinView));
                                break;
                            case AccountType.Google:
                                StartCoroutine(portkeySDK.AuthService.GoogleCredentialProvider.Verify(credential, OpenSetPinView));
                                break;
                            default: throw new ArgumentException("Not expected account type!");
                        };
                    });
                    break;
                default:
                    guardianApprovalViewController.Initialize(guardians, credential);
                    CloseView();
                    break;
            }
        }

        private void OpenSetPinView(VerifiedCredential verifiedCredential)
        {
            portkeySDK.AuthService.Message.Loading(false);
            setPinViewController.Initialize(verifiedCredential);
            setPinViewController.SetPreviousView(gameObject);
        }
        
        private void SignUpPrompt(Action onConfirm, Action onClose = null)
        {
            promptView.Initialize("Continue with this account?", "This account has not been registered yet. Click \"Confirm\" to complete the registration.", onConfirm, onClose);
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
