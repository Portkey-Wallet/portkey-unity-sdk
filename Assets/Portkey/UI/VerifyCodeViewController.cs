using TMPro;
using UnityEngine;

namespace Portkey.UI
{
    public class VerifyCodeViewController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI detailsText = null;
        [SerializeField] private GuardianItemComponent guardianItem = null;

        private void Start()
        {
            guardianItem.InitializeUI();
            guardianItem.SetEndOperation();
        }
    }
}