using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Portkey.UI
{
    public class CountryCodeButtonComponent : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI countryCodeText;
        [SerializeField] private Button button;
        [SerializeField] private CountryCodeViewController countryCodeViewController;

        public string CountryCode => countryCodeText.text;
        private void Start()
        {
            button.onClick.AddListener(OnClicked);
            countryCodeViewController.ChangeCountryCodeEventHandler += OnChangeCountryCode;
        }
        
        public void SetCountryCodeText(string countryCode)
        {
            OnChangeCountryCode(countryCode);
        }

        private void OnClicked()
        {
            countryCodeViewController.OpenView(true);
        }

        private void OnChangeCountryCode(string countryCode)
        {
            countryCodeText.text = countryCode;
        }

        private void OnDestroy()
        {
            countryCodeViewController.ChangeCountryCodeEventHandler -= OnChangeCountryCode;
        }
    }
}