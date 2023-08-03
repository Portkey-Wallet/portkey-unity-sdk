using UnityEngine;

namespace Portkey.UI
{
    public class ButtonComponent : MonoBehaviour
    {
        [SerializeField] private GameObject disabledOverlay;
        
        public void SetDisabled(bool disabled)
        {
            disabledOverlay.SetActive(disabled);
        }
    }
}