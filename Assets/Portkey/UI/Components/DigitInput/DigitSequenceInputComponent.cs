using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Portkey.UI
{
    public class DigitSequenceInputComponent : MonoBehaviour
    {
        [SerializeField] private List<DigitInputComponent> digitInputs = null;
        [SerializeField] private TMP_InputField inputField = null;

        private void Start()
        {
            inputField.characterLimit = digitInputs.Count;
            OnValueChanged("");
        }

        public void OnValueChanged(string value)
        {
            for (var i = 0; i < digitInputs.Count; i++)
            {
                digitInputs[i].SetText(i < value.Length ? value[i].ToString() : "");
                digitInputs[i].Select(i == value.Length);
            }
        }
    }
}