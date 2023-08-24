using System.Runtime.InteropServices;
using Portkey.Core;
using Portkey.SocialProvider;
using UnityEngine;

namespace Portkey.Biometric
{
    public class IOSBiometric : IBiometric
    {
        private class BiometricOutput
        {
            public bool isAuthenticated = false;
            public string message = null;
        }
        
#if UNITY_IOS
        [DllImport ("__Internal")]
        private static extern void BiometricAuthenticate ();
        private static extern void BiometricCanAuthenticate ();
#endif
        
        public void Authenticate(IBiometric.BiometricPromptInfo info, IBiometric.SuccessCallback onSuccess, ErrorCallback onError)
        {
            SetupAuthenticationCallback(onSuccess, onError);
#if UNITY_IOS
            BiometricAuthenticate();
#endif
        }

        public void CanAuthenticate(IBiometric.SuccessCallback onSuccess)
        {
            SetupCanAuthenticateCallback(onSuccess);
#if UNITY_IOS
            BiometricCanAuthenticate();
#endif
        }

        private static void SetupAuthenticationCallback(IBiometric.SuccessCallback onSuccess, ErrorCallback onError)
        {
            var gameObject = new GameObject("IOSPortkeyBiometricCallback");
            var callbackComponent = gameObject.AddComponent<IOSPortkeyBiometricCallback>();
            callbackComponent.SuccessCallback = onSuccess;
            callbackComponent.ErrorCallback = onError;
        }
        
        private static void SetupCanAuthenticateCallback(IBiometric.SuccessCallback onSuccess)
        {
            var gameObject = new GameObject("IOSPortkeyBiometricCallback");
            var callbackComponent = gameObject.AddComponent<IOSPortkeyBiometricCallback>();
            callbackComponent.CanAuthenticateCallback = onSuccess;
        }
    }
}