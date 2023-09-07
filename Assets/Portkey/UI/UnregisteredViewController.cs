using Portkey.Core;
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
        
        private GuardianIdentifierInfo _guardianIdentifierInfo;

        public void OnClickClose()
        {
            CloseView();
        }

        private void CloseView()
        {
            gameObject.SetActive(false);
        }

        public void OnClickSignUp()
        {
            ShowLoading(true, "Assigning a verifier on-chain...");
            StartCoroutine(did.GetVerifierServers(_guardianIdentifierInfo.chainId, OnGetVerifierServers, OnError));
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
        
        public void SetGuardianIdentifierInfo(GuardianIdentifierInfo info)
        {
            _guardianIdentifierInfo = info;
        }
        
        private void ShowLoading(bool show, string text = "")
        {
            loadingView.DisplayLoading(show, text);
        }

        private void OnError(string error)
        {
            ShowLoading(false);
            Debugger.LogError(error);
            errorView.ShowErrorText(error);
        }
    }
}