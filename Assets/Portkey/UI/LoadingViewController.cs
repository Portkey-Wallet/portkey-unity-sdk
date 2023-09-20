using Portkey.Core;
using TMPro;
using UnityEngine;

namespace Portkey.UI
{
    public class LoadingViewController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        
        public void DisplayLoading(bool display, string text = "")
        {
            gameObject.SetActive(display);
            if (text == null)
            {
               return;
            }
            this.text.text = text;
        }
    }
}