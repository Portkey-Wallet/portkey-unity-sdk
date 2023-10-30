using UnityEngine;
using UnityEngine.Serialization;

namespace Portkey.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private LoadingViewController _loadingView = null;
        [SerializeField] private ErrorViewController _errorView = null;
        [FormerlySerializedAs("_did")] [SerializeField] private DID.PortkeySDK portkeySDK;

        private void Start()
        {
            portkeySDK.AuthService.Message.OnErrorEvent += _errorView.ShowErrorText;
            portkeySDK.AuthService.Message.OnLoadingEvent += _loadingView.DisplayLoading;
            
#if PORTKEY_DEVELOPMENT
            Instantiate(Resources.Load("IngameDebugConsole/IngameDebugConsole"));
#endif
        }
        
        private void OnDestroy()
        {
            portkeySDK.AuthService.Message.OnErrorEvent -= _errorView.ShowErrorText;
            portkeySDK.AuthService.Message.OnLoadingEvent -= _loadingView.DisplayLoading;
        }
    }
}