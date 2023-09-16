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
        
        [Header("UI Elements")]
        [SerializeField] private DID.DID did;
        [SerializeField] private TextMeshProUGUI detailsText = null;
        [SerializeField] private GuardianDisplayComponent guardianDisplay = null;
        [SerializeField] private DigitSequenceInputComponent digitSequenceInput = null;
        [SerializeField] private TimedButtonComponent resendButton = null;
        
        private Action<VerifiedCredential> _onComplete = null;
        private ICodeCredentialProvider _codeCredentialProvider = null;

        private void Start()
        {
            digitSequenceInput.OnComplete = InputVerificationCode;
            resendButton.OnClick += SendVerificationCode;
        }

        public void Initialize(string guardianId, AccountType accountType, string verifierServerName, Action<VerifiedCredential> onComplete)
        {
            _onComplete = onComplete;

            _codeCredentialProvider = GetCredentialProvider(accountType);

            guardianDisplay.Initialize(guardianId, accountType, verifierServerName);
            detailsText.text = $"A 6-digit verification code has been sent to {guardianId}. Please enter the code within 10 minutes.";
            resendButton.Deactivate();
            
            OpenView();
        }

        private ICodeCredentialProvider GetCredentialProvider(AccountType accountType)
        {
            return accountType switch
            {
                AccountType.Email => did.AuthService.EmailCredentialProvider,
                AccountType.Phone => did.AuthService.PhoneCredentialProvider,
                _ => throw new ArgumentException("Invalid account type: " + accountType)
            };
        }

        public void OpenView()
        {
            gameObject.SetActive(true);
        }

        private void InputVerificationCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                errorView.ShowErrorText("Please enter the verification code.");
                digitSequenceInput.Clear();
                return;
            }
            
            did.AuthService.Message.InputVerificationCode(code);
        }

        public void VerifyCode(ICredential credential)
        {
            ShowLoading(true, "Loading...");

            _codeCredentialProvider.Verify(credential, verifiedCredential =>
            {
                ShowLoading(false);
                _onComplete?.Invoke(verifiedCredential);
                CloseView();
            });
        }
        
        private void SendVerificationCode()
        {
            ShowLoading(true, "Loading...");

            did.AuthService.Message.ResendVerificationCode();
            did.AuthService.Message.OnResendVerificationCodeCompleteEvent += OnResendVerificationCodeComplete;
        }

        private void OnResendVerificationCodeComplete()
        {
            did.AuthService.Message.OnResendVerificationCodeCompleteEvent -= OnResendVerificationCodeComplete;
            ShowLoading(false);
        }

        private void ShowLoading(bool show, string text = "")
        {
            loadingView.DisplayLoading(show, text);
        }
        
        public void OnClickClose()
        {
            CloseView();
        }

        public void CloseView()
        {
            did.AuthService.Message.CancelCodeVerification();
            gameObject.SetActive(false);
        }
    }
}