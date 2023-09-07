using System;
using Portkey.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] private TimedButtonComponent resendButton = null;
        
        private GuardianIdentifierInfo _guardianIdentifierInfo = null;
        private VerifierItem _verifierItem = null;

        private void Start()
        {
            digitSequenceInput.OnComplete = VerifyCode;
            resendButton.OnClick += SendVerificationCode;
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
            
            ShowLoading(true, "Loading...");

            var serviceLogin = GetVerifyCodeLogin(_guardianIdentifierInfo.accountType);
            StartCoroutine(serviceLogin.VerifyCode(code, OpenNextView, OnError));
        }
        
        private void SendVerificationCode()
        {
            ShowLoading(true, "Loading...");
            
            var serviceLogin = GetVerifyCodeLogin(_guardianIdentifierInfo.accountType);

            var param = new SendCodeParams
            {
                guardianId = _guardianIdentifierInfo?.identifier,
                verifierId = _verifierItem?.id,
                chainId = _guardianIdentifierInfo?.chainId
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

        private void OpenNextView(VerifyCodeResult result)
        {
            ShowLoading(false);
            
            setPinViewController.VerifierItem = _verifierItem;
            setPinViewController.gameObject.SetActive(true);
            setPinViewController.GuardianIdentifierInfo = _guardianIdentifierInfo;
            setPinViewController.VerifyCodeResult = result;
            setPinViewController.Operation = SetPINViewController.OperationType.SIGN_UP;
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
        
        public void OnClickClose()
        {
            gameObject.SetActive(false);
        }
    }
}