using System;
using Portkey.Core;
using TMPro;
using UnityEngine;

namespace Portkey.UI
{
    public class PromptViewController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI headerTextComponent;
        [SerializeField] private TextMeshProUGUI descriptionTextComponent;

        private GuardianIdentifierInfo _guardianIdentifierInfo;
        private Action _onConfirm;
        private Action _onClose;

        public void OnClickClose()
        {
            CloseView();
        }

        public void CloseView()
        {
            _onClose?.Invoke();
            gameObject.SetActive(false);
        }

        public void OnClickConfirm()
        {
            _onConfirm?.Invoke();
            gameObject.SetActive(false);
        }

        public void Initialize(string headerText, string descriptionText, Action onConfirm, Action onClose = null)
        {
            headerTextComponent.gameObject.SetActive(!string.IsNullOrEmpty(headerText));
            headerTextComponent.text = headerText;
            descriptionTextComponent.gameObject.SetActive(!string.IsNullOrEmpty(descriptionText));
            descriptionTextComponent.text = descriptionText;
            _onConfirm = onConfirm;
            _onClose = onClose;
            
            gameObject.SetActive(true);
        }
    }
}