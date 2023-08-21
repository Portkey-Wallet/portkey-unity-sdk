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
#endif
        
        public void Authenticate(IBiometric.BiometricPromptInfo info, IBiometric.SuccessCallback onSuccess, ErrorCallback onError)
        {
            SetupAuthenticationCallback(onSuccess, onError);

            BiometricAuthenticate();
        }
        
        private static void SetupAuthenticationCallback(IBiometric.SuccessCallback onSuccess, ErrorCallback onError)
        {
            var gameObject = new GameObject("IOSPortkeyBiometricCallback");
            var callbackComponent = gameObject.AddComponent<IOSPortkeyBiometricCallback>();
            callbackComponent.SuccessCallback = onSuccess;
            callbackComponent.ErrorCallback = onError;
        }
    }
}