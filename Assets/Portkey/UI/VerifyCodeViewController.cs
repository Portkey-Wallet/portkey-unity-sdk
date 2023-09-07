using System;
using Portkey.Core;
using TMPro;
using UnityEngine;

namespace Portkey.UI
{
    public class VerifyCodeViewController : MonoBehaviour
    {
        [Header("Views")]
        [SerializeField] private ErrorViewController errorView = null;
        [SerializeField] private LoadingViewController loadingView;
        [SerializeField] private SetPINViewController setPinViewController = null;
        [SerializeField] private SignInViewController signInViewController = null;
        
        [Header("UI Elements")]
        [SerializeField] private DID.DID did;
        [SerializeField] private TextMeshProUGUI detailsText = null;
        [SerializeField] private GuardianDisplayComponent guardianDisplay = null;
        [SerializeField] private DigitSequenceInputComponent digitSequenceInput = null;

        private GuardianIdentifierInfo _guardianIdentifierInfo = null;
        private VerifierItem _verifierItem = null;
        
        private void Start()
        {
            digitSequenceInput.OnComplete = VerifyCode;
        }
        
        public void Initialize(GuardianIdentifierInfo info, VerifierItem verifierItem)
        {
            _guardianIdentifierInfo = info;
            _verifierItem = verifierItem;
            
            guardianDisplay.Initialize(info, verifierItem);
            detailsText.text = $"A 6-digit verification code has been sent to {info.identifier}. Please enter the code within 10 minutes.";
        }

        public void OpenView()
        {
            gameObject.SetActive(true);
        }

        private void VerifyCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                errorView.ShowErrorText("Please enter the verification code.");
                digitSequenceInput.Clear();
                return;
            }
            
            IVerifyCodeLogin serviceLogin = _guardianIdentifierInfo.accountType switch
            {
                AccountType.Email => did.AuthService.Email,
                AccountType.Phone => did.AuthService.Phone,
                _ => throw new ArgumentException($"Invalid account type: {_guardianIdentifierInfo.accountType}")
            };
            
            StartCoroutine(serviceLogin.VerifyCode(code, result => { OpenNextView(); }, OnError));
        }

        private void OpenNextView()
        {
            setPinViewController.VerifierItem = _verifierItem;
            setPinViewController.gameObject.SetActive(true);
            setPinViewController.GuardianIdentifierInfo = _guardianIdentifierInfo;
            setPinViewController.SetPreviousView(signInViewController.gameObject);
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