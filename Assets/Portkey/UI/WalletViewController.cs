using System;
using AElf.Client.Extensions;
using Portkey.Contracts.CA;
using Portkey.Core;
using TMPro;
using UnityEngine;

namespace Portkey.UI
{
    public class WalletViewController : MonoBehaviour
    {
        [SerializeField] private DID.DID did;
        
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI addressText;
        [SerializeField] private GameObject confirmSignOutDialog;
        
        [Header("View")]
        [SerializeField] private SignInViewController signInViewController;
        [SerializeField] private ErrorViewController errorView;
        [SerializeField] private LoadingViewController loadingView;
        [SerializeField] private SetPINViewController setPinViewController;
        [SerializeField] private GuardiansApprovalViewController guardiansApprovalViewController;

        private DIDWalletInfo walletInfo = null;
        
        public DIDWalletInfo WalletInfo
        {
            set => walletInfo = value;
        }

        private void Start()
        {
            addressText.text = walletInfo.caInfo.caAddress;
        }

        private void OnEnable()
        {
            confirmSignOutDialog.SetActive(false);
        }

        public void OnClickSignOut()
        {
            var param = new EditManagerParams
            {
                chainId = walletInfo.chainId
            };

            ShowLoading(true, "Signing out of Portkey...");
            
            StartCoroutine(did.Logout(param, OnSuccessLogout, OnError));
        }
        
        private void OnSuccessLogout(bool success)
        {
            ShowLoading(false);
            
            if (!success)
            {
                OnError("Log out failed.");
                return;
            }

            ResetViews();
            OpenSignInView();
        }

        private void ResetViews()
        {
            setPinViewController.ResetToEnterPINState();
            guardiansApprovalViewController.ResetView();
        }

        private void OpenSignInView()
        {
            signInViewController.gameObject.SetActive(true);
            CloseView();
        }
        
        private void OnError(string error)
        {
            Debug.LogError(error);
            errorView.ShowErrorText(error);
            
            ShowLoading(false);
        }

        private void CloseView()
        {
            gameObject.SetActive(false);
        }
        
        private void ShowLoading(bool show, string text = "")
        {
            loadingView.DisplayLoading(show, text);
        }
    }
}