using Portkey.Core;
using Portkey.Utilities;
using UnityEngine;

namespace Portkey.UI
{
    public class AccountDeletionButton : MonoBehaviour
    {
        [SerializeField] private DID.PortkeySDK portkeySDK;
        
        public void Initialize(DIDAccountInfo accountInfo, ErrorCallback errorCallback)
        {
            gameObject.SetActive(false);
#if UNITY_IOS
            StaticCoroutine.StartCoroutine(portkeySDK.IsAccountDeletionPossible(accountInfo, display =>
            {
                gameObject.SetActive(display);
            }, errorCallback));
#endif
        }
    }
}