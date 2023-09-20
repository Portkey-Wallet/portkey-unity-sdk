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

        private void Start()
        {
            digitSequenceInput.OnComplete = InputVerificationCode;
            resendButton.OnClick += SendVerificationCode;
        }

        public void Initialize(string guardianId, AccountType accountType, string verifierServerName)
        {
            digitSequenceInput.Clear();
            guardianDisplay.Initialize(guardianId, accountType, verifierServerName);
            detailsText.text = $"A 6-digit verification code has been sent to {guardianId}. Please enter the code within 10 minutes.";
            resendButton.Deactivate();

            did.AuthService.Message.OnErrorEvent += OnError;
            
            OpenView();
        }

        private void OnError(string error)
        {
            //StartCoroutine(DID.AuthService.EmailCredentialProvider.Verify(credential, OpenSetPINView));
        }
        
        public void Initialize(GuardianNew guardian, SuccessCallback<ApprovedGuardian> onSuccess)
        {
            Initialize(guardian.id, guardian.accountType, guardian.verifier.name);
            StartCoroutine(did.AuthService.Verify(guardian, onSuccess));
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
        
        private void SendVerificationCode()
        {
            did.AuthService.Message.Loading(true, "Loading...");

            did.AuthService.Message.ResendVerificationCode();
            did.AuthService.Message.OnResendVerificationCodeCompleteEvent += OnResendVerificationCodeComplete;
        }

        private void OnResendVerificationCodeComplete()
        {
            did.AuthService.Message.OnResendVerificationCodeCompleteEvent -= OnResendVerificationCodeComplete;
            did.AuthService.Message.Loading(false);
        }
        
        public void OnClickClose()
        {
            CloseView();
        }

        public void CloseView()
        {
            did.AuthService.Message.OnErrorEvent -= OnError;
            did.AuthService.Message.OnResendVerificationCodeCompleteEvent -= OnResendVerificationCodeComplete;
            did.AuthService.Message.CancelCodeVerification();
            gameObject.SetActive(false);
        }
    }
}