using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Portkey.UI
{
    public class DigitInputComponent : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI inputField = null;
        [SerializeField] private Image outline = null;

        public void SetText(string text)
        {
            inputField.text = text;
        }

        public void Select(bool selected)
        {
            outline.color = selected ? new Color(0.557f, 0.576f, 0.643f, 1.0f) : new Color(1.0f, 1.0f, 1.0f, 0.1f);
            inputField.text = selected ? "|" : inputField.text;
        }
    }
}