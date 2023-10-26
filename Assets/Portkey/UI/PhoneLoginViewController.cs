using System.Collections.Generic;
using Portkey.Core;
using Portkey.SocialProvider;
using UnityEngine;

namespace Portkey.UI
{
    public class PhoneLoginViewController : EmailLoginViewController
    {
        [SerializeField] private CountryCodeButtonComponent countryCodeButtonComponent;
        public static IPhoneCountryCodeResult PhoneCountryCodeResult { get; set; } = null;
        public delegate void OnGetCountryCode (IPhoneCountryCodeResult result);
        public static event OnGetCountryCode OnGetCountryCodeEventHandler;
        private string InputPhoneNumber => countryCodeButtonComponent.CountryCode + inputField.text;
        
        private new void OnEnable()
        {
            base.OnEnable();
            
            PortkeySDK.AuthService.PhoneCredentialProvider.EnableCodeSendConfirmationFlow = true;
            StartCoroutine(PortkeySDK.GetPhoneCountryCodeWithLocal(result =>
            {
                PhoneCountryCodeResult = result;
                countryCodeButtonComponent.SetCountryCodeText($"+{result.locateData.code}");
            }, PortkeySDK.AuthService.Message.Error));
        }
        
        public new void OnValueChanged()
        {
            if (LoginHelper.IsValidPhoneNumber(InputPhoneNumber))
            {
                loginButton.interactable = true;
                errorText.text = "";
                return;
            }
            
            errorText.text = string.IsNullOrEmpty(InputPhoneNumber) ? "" : "Invalid phone number.";
            loginButton.interactable = false;
        }
        
        public override void OnClickLogin()
        {
            StartLoading();

            var phoneNumber = PhoneNumber.Parse(InputPhoneNumber);
            StartCoroutine(PortkeySDK.AuthService.GetGuardians(phoneNumber, guardians =>
            {
                CheckSignUpOrLogin(phoneNumber, guardians);
            }));
        }
        
        private void CheckSignUpOrLogin(PhoneNumber phoneNumber, List<Guardian> guardians)
        {
            ShowLoading(false);
            
            switch (guardians.Count)
            {
                case 0:
                    PortkeySDK.AuthService.Message.OnVerifierServerSelectedEvent += OnVerifierServerSelected;
                    SignUpPrompt(() =>
                    {
                        ShowLoading(true, "Loading...");
                        StartCoroutine(PortkeySDK.AuthService.PhoneCredentialProvider.Get(phoneNumber, credential =>
                        {
                            StartCoroutine(PortkeySDK.AuthService.PhoneCredentialProvider.Verify(credential, OpenSetPINView));
                        }));
                    }, () =>
                    {
                        PortkeySDK.AuthService.Message.OnVerifierServerSelectedEvent -= OnVerifierServerSelected;
                    });
                    break;
                default:
                    guardianApprovalViewController.Initialize(guardians);
                    CloseView();
                    break;
            }
        }
    }
}