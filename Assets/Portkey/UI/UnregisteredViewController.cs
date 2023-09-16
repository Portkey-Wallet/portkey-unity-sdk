using System;
using Portkey.Core;
using TMPro;
using UnityEngine;

namespace Portkey.UI
{
    public class UnregisteredViewController : MonoBehaviour
    {
        [SerializeField] private DID.DID did;
        [SerializeField] private SetPINViewController setPinViewController;
        [SerializeField] private SignInViewController signInViewController;
        [SerializeField] private LoadingViewController loadingView;
        [SerializeField] private ErrorViewController errorView;
        [SerializeField] private PromptViewController promptViewController;
        [SerializeField] private TextMeshProUGUI headerTextComponent;
        [SerializeField] private TextMeshProUGUI descriptionTextComponent;

        private GuardianIdentifierInfo _guardianIdentifierInfo;
        private Action _onConfirm;
        private Action _onClose;

        public void OnClickClose()
        {
            CloseView();
        }

        public void CloseView()
        {
            _onClose?.Invoke();
            gameObject.SetActive(false);
        }

        public void OnClickSignUp()
        {
            _onConfirm?.Invoke();
            CloseView();
            //ShowLoading(true, "Assigning a verifier on-chain...");
            //StartCoroutine(did.GetVerifierServers(_guardianIdentifierInfo.chainId, OnGetVerifierServers, OnError));
        }

        private void OnGetVerifierServers(VerifierItem[] verifierServerList)
        {
            ShowLoading(false);

            if (_guardianIdentifierInfo.accountType is AccountType.Apple or AccountType.Google)
            {
                setPinViewController.VerifierItem = verifierServerList[0];
                setPinViewController.gameObject.SetActive(true);
                setPinViewController.GuardianIdentifierInfo = _guardianIdentifierInfo;
                setPinViewController.SetPreviousView(signInViewController.gameObject);
            }
            else
            {
                var verifierServer = verifierServerList[0];
                var type = _guardianIdentifierInfo.accountType == AccountType.Email? "email address" : "phone number";
                var description = $"{verifierServer.name} will send a verification code to {_guardianIdentifierInfo.identifier} to verify your {type}.";
                promptViewController.ShowDescriptionText(description);
                promptViewController.VerifierItem = verifierServer;
                promptViewController.GuardianIdentifierInfo = _guardianIdentifierInfo;
            }

            CloseView();
        }

        public void Initialize(string headerText, string descriptionText, Action onConfirm, Action onClose = null)
        {
            headerTextComponent.gameObject.SetActive(!string.IsNullOrEmpty(headerText));
            headerTextComponent.text = headerText;
            descriptionTextComponent.gameObject.SetActive(!string.IsNullOrEmpty(descriptionText));
            descriptionTextComponent.text = descriptionText;
            _onConfirm = onConfirm;
            _onClose = onClose;
            
            gameObject.SetActive(true);
        }
        
        private void ShowLoading(bool show, string text = "")
        {
            loadingView.DisplayLoading(show, text);
        }
    }
}