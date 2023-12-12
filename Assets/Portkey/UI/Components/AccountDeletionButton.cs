using Portkey.Core;
using UnityEngine;

namespace Portkey.UI
{
    public class AccountDeletionButton : MonoBehaviour
    {
        [SerializeField] private DID.PortkeySDK portkeySDK;
        
        public void Initialize(DIDAccountInfo accountInfo, ErrorCallback errorCallback)
        {
            gameObject.SetActive(false);
            StartCoroutine(portkeySDK.IsAccountDeletionPossible(accountInfo, display =>
            {
                gameObject.SetActive(display);
            }, errorCallback));
        }
    }
}