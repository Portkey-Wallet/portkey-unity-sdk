using System.Collections;
using System.Collections.Generic;
using Portkey.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Portkey.UI
{
    public class UnregisteredViewController : MonoBehaviour
    {
        [SerializeField] private DID.DID did;
        [SerializeField] private SetPINViewController setPinViewController;
        [SerializeField] private SignInViewController signInViewController;
        [SerializeField] private LoadingViewController loadingView;
        [SerializeField] private ErrorViewController errorView;
        
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
            
            setPinViewController.VerifierItem = verifierServerList[0];
            setPinViewController.gameObject.SetActive(true);
            setPinViewController.GuardianIdentifierInfo = _guardianIdentifierInfo;
            setPinViewController.SetPreviousView(signInViewController.gameObject);
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