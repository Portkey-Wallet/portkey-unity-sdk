using Portkey.SocialProvider;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Portkey.UI
{
    public class PhoneLoginViewController : MonoBehaviour
    {
        private readonly string TITLE = "Login with Phone";
        
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button loginButton;
        [SerializeField] private TextMeshProUGUI errorText;
        
        public GameObject PreviousView { get; set; }

        private void OnEnable()
        {
            ResetView();
        }

        private void ResetView()
        {
            title.text = TITLE;
            inputField.text = "";
            errorText.text = "";
            loginButton.interactable = false;
        }

        public void OnClickBack()
        {
            ResetView();
            CloseView();
            PreviousView.SetActive(true);
        }

        public void OnValueChanged()
        {
            var value = inputField.text;
            if (LoginHelper.IsValidPhoneNumber(value))
            {
                loginButton.interactable = true;
                errorText.text = "";
                return;
            }
            
            errorText.text = string.IsNullOrEmpty(value) ? "" : "Invalid phone number.";
            loginButton.interactable = false;
        }
        
        public void OnClickClose()
        {
            CloseView();
        }
        
        private void CloseView()
        {
            gameObject.SetActive(false);
        }
    }
}