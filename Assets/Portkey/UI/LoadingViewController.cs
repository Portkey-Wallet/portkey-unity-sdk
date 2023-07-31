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
            this.text.text = text;
        }
    }
}