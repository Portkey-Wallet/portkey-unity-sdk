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
        public string PhoneNumber => countryCodeButtonComponent.CountryCode + inputField.text;
        
        private new void OnEnable()
        {
            base.OnEnable();
            StartCoroutine(DID.PortkeySocialService.GetPhoneCountryCodeWithLocal(result =>
            {
                PhoneCountryCodeResult = result;
                countryCodeButtonComponent.SetCountryCodeText($"+{result.locateData.code}");
            }, OnError));
        }
        
        public new void OnValueChanged()
        {
            if (LoginHelper.IsValidPhoneNumber(PhoneNumber))
            {
                loginButton.interactable = true;
                errorText.text = "";
                return;
            }
            
            errorText.text = string.IsNullOrEmpty(PhoneNumber) ? "" : "Invalid phone number.";
            loginButton.interactable = false;
        }
        
        public new void OnClickLogin()
        {
            StartLoading();
            DID.AuthService.HasGuardian(PhoneNumber, AccountType.Phone, "", CheckSignUpOrLogin, OnError);
        }
    }
}