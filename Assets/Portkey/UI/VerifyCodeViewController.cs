using System;
using Portkey.Core;
using TMPro;
using Unity.VisualScripting;
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
        
        private string _guardianId = null;
        private AccountType _accountType = AccountType.Email;
        private Action<ICredential> _verifyCode = null;
        private Action _sendCode = null;

        private void Start()
        {
            digitSequenceInput.OnComplete = InputVerificationCode;
            resendButton.OnClick += SendVerificationCode;
        }

        public void Initialize(string guardianId, AccountType accountType, Verifier verifier, SuccessCallback<VerifiedCredential> onSuccess)
        {
            Initialize(guardianId, accountType, verifier.name);
            
            _sendCode = () =>
            {
                switch(accountType)
                {
                    case AccountType.Email:
                        StartCoroutine(portkeySDK.AuthService.EmailCredentialProvider.SendCode(EmailAddress.Parse(guardianId), session =>
                        {
                            portkeySDK.AuthService.Message.Loading(false);
                        }));
                        break;
                    case AccountType.Phone:
                        StartCoroutine(portkeySDK.AuthService.PhoneCredentialProvider.SendCode(PhoneNumber.Parse(guardianId), session =>
                        {
                            portkeySDK.AuthService.Message.Loading(false);
                        }));
                        break;
                    case AccountType.Google:
                    case AccountType.Apple:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
            
            _verifyCode = (credential) =>
            {
                portkeySDK.AuthService.Message.Loading(true, "Loading...");
                switch(accountType)
                {
                    case AccountType.Email:
                        StartCoroutine(portkeySDK.AuthService.EmailCredentialProvider.Verify(credential, OnVerified));
                        break;
                    case AccountType.Phone:
                        StartCoroutine(portkeySDK.AuthService.PhoneCredentialProvider.Verify(credential, OnVerified));
                        break;
                    case AccountType.Google:
                    case AccountType.Apple:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
            
            return;

            void OnVerified(VerifiedCredential verifiedCredential)
            {
                portkeySDK.AuthService.Message.Loading(false);
                CloseView();
                onSuccess?.Invoke(verifiedCredential);
            }
        }
        
        private void Initialize(string guardianId, AccountType accountType, string verifierServerName)
        {
            _guardianId = guardianId;
            _accountType = accountType;
            digitSequenceInput.Clear();
            guardianDisplay.Initialize(guardianId, accountType, verifierServerName);
            detailsText.text = $"A 6-digit verification code has been sent to {guardianId}. Please enter the code within 10 minutes.";
            resendButton.Deactivate();

            portkeySDK.AuthService.Message.OnErrorEvent += OnError;
            
            OpenView();
        }

        private void OnError(string error)
        {
            //TODO: re-establish verification of code
        }
        
        public void Initialize(Guardian guardian, SuccessCallback<ApprovedGuardian> onSuccess)
        {
            Initialize(guardian.id, guardian.accountType, guardian.verifier.name);
            
            switch(guardian.accountType)
            {
                case AccountType.Email:
                    _sendCode = () =>
                    {
                        StartCoroutine(portkeySDK.AuthService.EmailCredentialProvider.SendCode(guardian, session =>
                        {
                            portkeySDK.AuthService.Message.Loading(false);
                        }));
                    };
                    break;
                case AccountType.Phone:
                    _sendCode = () =>
                    {
                        StartCoroutine(portkeySDK.AuthService.PhoneCredentialProvider.SendCode(guardian, session =>
                        {
                            portkeySDK.AuthService.Message.Loading(false);
                        }));
                    };
                    break;
                case AccountType.Google:
                case AccountType.Apple:
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _sendCode?.Invoke();
            
            _verifyCode = (credential) =>
            {
                StartCoroutine(portkeySDK.AuthService.Verify(guardian, approvedGuardian =>
                {
                    CloseView();
                    onSuccess?.Invoke(approvedGuardian);
                }, credential));
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
            
            ICredential credential = null;
            
            switch(_accountType)
            {
                case AccountType.Email:
                    credential= portkeySDK.AuthService.EmailCredentialProvider.Get(EmailAddress.Parse(_guardianId), code);
                    break;
                case AccountType.Phone:
                    credential= portkeySDK.AuthService.PhoneCredentialProvider.Get(PhoneNumber.Parse(_guardianId), code);
                    break;
                case AccountType.Google:
                case AccountType.Apple:
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _verifyCode?.Invoke(credential);
        }
        
        private void SendVerificationCode()
        {
            portkeySDK.AuthService.Message.Loading(true, "Loading...");
            
            _sendCode?.Invoke();
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