using System.Runtime.InteropServices;
using Portkey.Core;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class IOSGoogleLogin : GoogleLoginBase
    {
        public IOSGoogleLogin(PortkeyConfig config, IHttp request) : base(request)
        {
            ClientId = config.GoogleIOSClientId;
        }

        protected override void OnAuthenticate()
        {
            SetupAuthenticationCallback();
            Authenticate();
        }
        
        private void SetupAuthenticationCallback()
        {
            var gameObject = new GameObject("IOSPortkeyGoogleLoginCallback");
            var callbackComponent = gameObject.AddComponent<IOSPortkeyGoogleLoginCallback>();
            callbackComponent.SocialLogin = this;
        }

#if UNITY_IOS
        [DllImport ("__Internal")]
        private static extern void SignIn ();
#endif
        
        private void Authenticate()
        {
            _startLoadCallback?.Invoke(true);
#if UNITY_IOS
            SignIn();
#endif
        }
    }
}