using System;
using System.Collections.Generic;
using System.Linq;
using Portkey.Core;
using TMPro;
using UnityEngine;

namespace Portkey.UI
{
    public class CountryCodeViewController : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject groupLabelPrefab;
        [SerializeField] private GameObject countryCodeSelectionButtonPrefab;
        
        [Header("UI Elements")]
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Transform scrollViewContent;
        
        public delegate void OnChangeCountryCode (string countryCode);
        public event OnChangeCountryCode ChangeCountryCodeEventHandler;
        
        private List<CountryCodeSelectionButtonComponent> _countryCodeSelectionButtonComponents = new List<CountryCodeSelectionButtonComponent>();
        private List<GameObject> _groupLabels = new List<GameObject>();
        public void OpenView(bool open)
        {
            gameObject.SetActive(open);
        }
        
        public void OnClickClose()
        {
            OpenView(false);
        }

        private void Start()
        {
            inputField.onValueChanged.AddListener(OnInputValueChanged);
        }
        
        private void OnInputValueChanged(string value)
        {
            _countryCodeSelectionButtonComponents.ForEach(comp =>
            {
                comp.gameObject.SetActive(comp.CountryName.ToLower().Contains(value.ToLower()));
            });
            
            var isGroupLabelActive = value == "";
            _groupLabels.ForEach(gObject =>
            {
                gObject.SetActive(isGroupLabelActive);
            });
        }

        private void OnEnable()
        {
            if (PhoneLoginViewController.PhoneCountryCodeResult != null && _countryCodeSelectionButtonComponents.Count == 0)
            {
                InitializeCountryCodeScrollView(PhoneLoginViewController.PhoneCountryCodeResult);
            }
            
            PhoneLoginViewController.OnGetCountryCodeEventHandler += OnGetCountryCode;
            
            inputField.text = "";
        }

        private void OnDisable()
        {
            PhoneLoginViewController.OnGetCountryCodeEventHandler -= OnGetCountryCode;
        }

        private void OnGetCountryCode(IPhoneCountryCodeResult result)
        {
            InitializeCountryCodeScrollView(result);
        }

        private void ClearCountryCodeScrollView()
        {
            _countryCodeSelectionButtonComponents.ForEach(comp =>
            {
                comp.CountryCodeClickedEventHandler -= OnCountryCodeClicked;
            });
            _countryCodeSelectionButtonComponents = new List<CountryCodeSelectionButtonComponent>();
            _groupLabels = new List<GameObject>();
            
            foreach (Transform child in scrollViewContent.transform)
            {
                Destroy(child.gameObject);
            }
        }
        
        private void InitializeCountryCodeScrollView(IPhoneCountryCodeResult result)
        {
            ClearCountryCodeScrollView();
            
            var firstLetter = ' ';
            
            var countryMap = result.data.ToDictionary(item => item.country, item => item);
            countryMap.ToList().ForEach(keyPair =>
            {
                var countryItem = keyPair.Value;
                if (firstLetter != countryItem.country[0])
                {
                    firstLetter = countryItem.country[0];
                    var groupLabel = InstantiateGroupLabel(firstLetter.ToString(), scrollViewContent);
                    
                    _groupLabels.Add(groupLabel);
                }
                
                var countryCodeSelectionButton = InstantiateCountryCodeSelectionButton(countryItem, scrollViewContent);
                countryCodeSelectionButton.Selected(result.locateData.code == countryItem.code);

                _countryCodeSelectionButtonComponents.Add(countryCodeSelectionButton);
            });
        }

        private void OnCountryCodeClicked(CountryCodeSelectionButtonComponent button)
        {
            _countryCodeSelectionButtonComponents.ForEach(comp =>
            {
                comp.Selected(false);
            });
            button.Selected(true);
            
            ChangeCountryCodeEventHandler?.Invoke(button.CountryCode);
            OpenView(false);
        }

        private CountryCodeSelectionButtonComponent InstantiateCountryCodeSelectionButton(ICountryItem countryItem, Transform parent)
        {
            var gObject = Instantiate(countryCodeSelectionButtonPrefab, parent);
            var countryCodeSelectionButton = gObject.GetComponent<CountryCodeSelectionButtonComponent>();
            countryCodeSelectionButton.Initialize(countryItem.country, countryItem.code);
            countryCodeSelectionButton.CountryCodeClickedEventHandler += OnCountryCodeClicked;
            
            return countryCodeSelectionButton;
        }

        private GameObject InstantiateGroupLabel(string label, Transform parent)
        {
            var groupLabel = Instantiate(groupLabelPrefab, parent);
            groupLabel.GetComponent<TextMeshProUGUI>().text = label;

            return groupLabel;
        }

        private void OnDestroy()
        {
            PhoneLoginViewController.OnGetCountryCodeEventHandler -= OnGetCountryCode;
        }
    }
}