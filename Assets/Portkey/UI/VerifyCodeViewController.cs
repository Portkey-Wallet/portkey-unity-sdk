using Portkey.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Portkey.UI
{
    public class VerifyCodeViewController : MonoBehaviour
    {
        [Header("Views")]
        [SerializeField] private ErrorViewController errorView = null;
        [SerializeField] private LoadingViewController loadingView;
        
        [Header("UI Elements")]
        [FormerlySerializedAs("did")] [SerializeField] private DID.PortkeySDK portkeySDK;
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

            portkeySDK.AuthService.Message.OnErrorEvent += OnError;
            
            OpenView();
        }

        private void OnError(string error)
        {
            //StartCoroutine(DID.AuthService.EmailCredentialProvider.Verify(credential, OpenSetPINView));
        }
        
        public void Initialize(Guardian guardian, SuccessCallback<ApprovedGuardian> onSuccess)
        {
            Initialize(guardian.id, guardian.accountType, guardian.verifier.name);
            StartCoroutine(portkeySDK.AuthService.Verify(guardian, onSuccess));
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
            
            portkeySDK.AuthService.Message.InputVerificationCode(code);
        }
        
        private void SendVerificationCode()
        {
            portkeySDK.AuthService.Message.Loading(true, "Loading...");

            portkeySDK.AuthService.Message.ResendVerificationCode();
            portkeySDK.AuthService.Message.OnResendVerificationCodeCompleteEvent += OnResendVerificationCodeComplete;
        }

        private void OnResendVerificationCodeComplete()
        {
            portkeySDK.AuthService.Message.OnResendVerificationCodeCompleteEvent -= OnResendVerificationCodeComplete;
            portkeySDK.AuthService.Message.Loading(false);
        }
        
        public void OnClickClose()
        {
            CloseView();
        }

        public void CloseView()
        {
            portkeySDK.AuthService.Message.OnErrorEvent -= OnError;
            portkeySDK.AuthService.Message.OnResendVerificationCodeCompleteEvent -= OnResendVerificationCodeComplete;
            portkeySDK.AuthService.Message.CancelCodeVerification();
            gameObject.SetActive(false);
        }
    }
}