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
        
        public override void OnClickLogin()
        {
            if (!LoginHelper.IsValidPhoneNumber(InputPhoneNumber))
            {
                errorText.text = string.IsNullOrEmpty(InputPhoneNumber) ? "" : "Invalid phone number.";
                return;
            }

            errorText.text = "";
            
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
                    SignUpPrompt(() =>
                    {
                        ShowLoading(true, "Assigning a verifier on-chain...");
                        StartCoroutine(PortkeySDK.AuthService.PhoneCredentialProvider.SendCode(phoneNumber, result =>
                        {
                            ShowLoading(false);
                            verifyCodeViewController.DeactivateTimedButton();
                        }));
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