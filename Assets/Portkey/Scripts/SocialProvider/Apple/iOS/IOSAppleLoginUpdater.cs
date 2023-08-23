using AppleAuth;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class IOSAppleLoginUpdater : MonoBehaviour
    {
        public IAppleAuthManager AppleAuthManager { private get; set; }
        private void Update()
        {
            AppleAuthManager?.Update();
        }
    }
}