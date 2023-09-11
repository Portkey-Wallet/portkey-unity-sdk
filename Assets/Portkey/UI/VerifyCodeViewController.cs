using System;
using Portkey.Core;
using TMPro;
using UnityEngine;

namespace Portkey.UI
{
    public class VerifyCodeViewArg
    {
        public VerifierItem verifierItem;
        public string chainId;
        public string guardianIdentifier;
        public AccountType accountType;
        public OperationTypeEnum operationType;
    }
    
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
        [SerializeField] private TimedButtonComponent resendButton = null;
        
        private VerifierItem _verifierItem = null;
        private VerifyCodeViewArg _verifyCodeViewArg = null;
        private Action<VerifyCodeResult> _onComplete = null;

        private void Start()
        {
            digitSequenceInput.OnComplete = VerifyCode;
            resendButton.OnClick += SendVerificationCode;
        }

        public void Initialize(VerifyCodeViewArg arg, VerifierItem verifierItem, Action<VerifyCodeResult> onComplete)
        {
            _verifyCodeViewArg = arg;
            _verifierItem = verifierItem;
            _onComplete = onComplete;
            
            guardianDisplay.Initialize(_verifyCodeViewArg.guardianIdentifier, _verifyCodeViewArg.accountType, verifierItem);
            detailsText.text = $"A 6-digit verification code has been sent to {_verifyCodeViewArg.guardianIdentifier}. Please enter the code within 10 minutes.";
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
            
            ShowLoading(true, "Loading...");

            var serviceLogin = GetVerifyCodeLogin(_verifyCodeViewArg.accountType);
            StartCoroutine(serviceLogin.VerifyCode(code, result =>
            {
                ShowLoading(false);
                _onComplete?.Invoke(result);
                CloseView();
            }, OnError));
        }
        
        private void SendVerificationCode()
        {
            ShowLoading(true, "Loading...");

            if (_verifyCodeViewArg == null)
            {
                throw new ArgumentException("VerifyCodeViewArg is null");
            }

            var serviceLogin = GetVerifyCodeLogin(_verifyCodeViewArg.accountType);

            var param = new SendCodeParams
            {
                guardianId = _verifyCodeViewArg.guardianIdentifier,
                verifierId = _verifierItem?.id,
                chainId = _verifyCodeViewArg.chainId,
                operationType = _verifyCodeViewArg.operationType
            };
            StartCoroutine(serviceLogin.SendCode(param, result => { ShowLoading(false); }, error =>
            {
                resendButton.Activate();
                OnError(error);
            }));
        }

        private IVerifyCodeLogin GetVerifyCodeLogin(AccountType accountType)
        {
            return accountType switch
            {
                AccountType.Email => did.AuthService.Email,
                AccountType.Phone => did.AuthService.Phone,
                _ => throw new ArgumentException($"Invalid account type: {accountType}")
            };
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
        
        public void OnClickClose()
        {
            CloseView();
        }

        public void CloseView()
        {
            gameObject.SetActive(false);
        }
    }
}