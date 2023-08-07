using Portkey.Core;
using UnityEngine;

namespace Portkey.Biometric
{
    public class AndroidBiometric : IBiometric
    {
        private class BiometricJavaAuthCallback : AndroidJavaProxy
        {
            public BiometricJavaAuthCallback() : base("com.portkey.biometricunityplugin.BiometricUnityActivity$Callback")
            {
            }

            public virtual void onResult(bool authToken)
            {
                
            }

            public virtual void onError(AndroidJavaObject e)
            {
                var message = e.Call<string>("getMessage");
                
                Debugger.LogError(message);
            }
        }
        
        public void Authenticate(IBiometric.SuccessCallback onSuccess, ErrorCallback onError)
        {
            var callback = new BiometricJavaAuthCallback();
            
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            using var context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
            using var biometricPlugin = new AndroidJavaClass("com.portkey.biometricunityplugin.BiometricUnityActivity");
            biometricPlugin.SetStatic("title", "Biometric Authentication");
            biometricPlugin.SetStatic("negativeButtonText", "Cancel");
            biometricPlugin.SetStatic("description", "You may choose to autheticate with your biometric or cancel.");
            biometricPlugin.SetStatic("subtitle", "Biometric Authentication");
            biometricPlugin.CallStatic("SetCallback", callback);
            biometricPlugin.CallStatic("Call", currentActivity);
        }
    }
}