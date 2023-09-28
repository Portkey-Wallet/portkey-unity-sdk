using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Portkey.BrowserWalletExtension;
using Portkey.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Portkey.UI
{
    public class SignInViewController : MonoBehaviour
    {
        [SerializeField] private DID.DID did;
        [SerializeField] private UnregisteredViewController unregisteredView;
        [FormerlySerializedAs("guardianApprovalView")] [SerializeField] private GuardiansApprovalViewController guardianApprovalViewController;
        [SerializeField] private ErrorViewController errorView;
        [SerializeField] private LoadingViewController loadingView;
        [SerializeField] private EmailLoginViewController emailLoginViewController;
        [SerializeField] private PhoneLoginViewController phoneLoginViewController;
        [SerializeField] private SetPINViewController setPinViewController;

        public void SignIn(int type)
        {
            var accountType = (AccountType)type;

            switch (accountType)
            {
                case AccountType.Apple:
                    did.AuthService.Message.Loading(true, "Loading...");
                    did.AuthService.AppleCredentialProvider.Get(AuthCallback);
                    break;
                case AccountType.Google:
                    did.AuthService.Message.Loading(true, "Loading...");
                    did.AuthService.GoogleCredentialProvider.Get(AuthCallback);
                    break;
                case AccountType.Email:
                    emailLoginViewController.Initialize(did, gameObject);
                    break;
                case AccountType.Phone:
                    phoneLoginViewController.Initialize(did, gameObject);
                    break;
                default:
                    throw new ArgumentException("Not expected account type!");
            }
        }
        
        private DIDWalletInfo _walletInfo;
        
        public void SignInWithExtension()
        {
            StartCoroutine(did.AuthService.LoginWithPortkeyExtension(OnConnect));
        }
        
        private void OnConnect(DIDWalletInfo walletInfo)
        {
            _walletInfo = walletInfo;
        }

        public void Signature()
        {
            //_walletInfo.wallet.SignTransaction("68656c6c6f20776f726c643939482801");
            //Debugger.Log($"Signature: {signature}");
        }

        private void AuthCallback(ICredential credential)
        {
            StartCoroutine(did.AuthService.GetGuardians(credential, guardians =>
            {
                CheckSignUpOrLogin(credential, guardians);
            }));
        }
        
        private void CheckSignUpOrLogin(ICredential credential, List<Guardian> guardians)
        {
            did.AuthService.Message.Loading(false);
            
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
                    guardianApprovalViewController.Initialize(guardians, credential);
                    CloseView();
                    break;
            }
        }
        
        private void SignUpPrompt(Action onConfirm, Action onClose = null)
        {
            unregisteredView.Initialize("Continue with this account?", "This account has not been registered yet. Click \"Confirm\" to complete the registration.", onConfirm, onClose);
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
