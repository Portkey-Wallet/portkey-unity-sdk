using TMPro;
using UnityEngine;

namespace Portkey.UI
{
    public class ErrorViewController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI errorText;
        
        public void ShowErrorText(string error)
        {
            errorText.text = error;
        }
    }
}