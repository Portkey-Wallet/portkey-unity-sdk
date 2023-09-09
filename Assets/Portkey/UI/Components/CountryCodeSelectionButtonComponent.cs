using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Portkey.UI
{
    public class CountryCodeSelectionButtonComponent : MonoBehaviour
    {
        [SerializeField] private Color selectedTextColor;
        [SerializeField] private Color unselectedTextColor;
        [SerializeField] private TextMeshProUGUI countryNameText;
        [SerializeField] private TextMeshProUGUI countryCodeText;
        [SerializeField] private Button button;

        public delegate void OnCountryCodeClicked (CountryCodeSelectionButtonComponent button);
        public event OnCountryCodeClicked CountryCodeClickedEventHandler;

        public string CountryCode => countryCodeText.text;
        public string CountryName => countryNameText.text;

        private void Start()
        {
            button.onClick.AddListener(OnClicked);
        }

        private void SetTextColor(Color color)
        {
            countryNameText.color = color;
            countryCodeText.color = color;
        }

        public void Selected(bool selected)
        {
            var color = selected ? selectedTextColor : unselectedTextColor;
            SetTextColor(color);
        }
        
        public void Initialize(string countryName, string countryCode)
        {
            countryNameText.text = countryName;
            countryCodeText.text = $"+{countryCode}";
        }

        private void OnClicked()
        {
            CountryCodeClickedEventHandler?.Invoke(this);
        }
    }
}