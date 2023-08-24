using System.Collections;
using System.Collections.Concurrent;
using Portkey.Core;
using Portkey.Utilities;
using UnityEngine;

namespace Portkey.Biometric
{
    public class AndroidBiometric : IBiometric
    {
        private class BiometricOutput
        {
            public bool canAuthenticate = false;
            public bool isAuthenticated = false;
            public string message = null;
        }
        
        private class BiometricJavaAuthCallback : AndroidJavaProxy
        {
            private ConcurrentQueue<BiometricOutput> _biometricResults;
            public BiometricJavaAuthCallback(ConcurrentQueue<BiometricOutput> biometricResults) : base("com.portkey.biometricunityactivity.BiometricUnityActivity$Callback")
            {
                _biometricResults = biometricResults;
            }

            public virtual void onResult(bool result)
            {
                var output = new BiometricOutput
                {
                    canAuthenticate = true,
                    isAuthenticated = result,
                    message = null
                };
                _biometricResults.Enqueue(output);
            }

            public virtual void onError(AndroidJavaObject e)
            {
                var message = e.Call<string>("getMessage");
                
                var output = new BiometricOutput
                {
                    canAuthenticate = false,
                    isAuthenticated = false,
                    message = message
                };
                _biometricResults.Enqueue(output);
            }
            
            public virtual void canAuthenticate(bool enabled)
            {
                var output = new BiometricOutput
                {
                    canAuthenticate = enabled,
                    isAuthenticated = false,
                    message = "can authenticate"
                };
                _biometricResults.Enqueue(output);
            }
        }
        
        private ConcurrentQueue<BiometricOutput> _biometricResults = new ConcurrentQueue<BiometricOutput>();
        private IBiometric.SuccessCallback _onSuccess = null;
        private IBiometric.SuccessCallback _canAuthenticate = null;
        private ErrorCallback _onError = null;
        
        public void Authenticate(IBiometric.BiometricPromptInfo info, IBiometric.SuccessCallback onSuccess, ErrorCallback onError)
        {
            _onSuccess = onSuccess;
            _onError = onError;
            
            StaticCoroutine.StartCoroutine(HandleBiometricOutput());
            
            AuthenticateJava(info);
        }

        public void CanAuthenticate(IBiometric.SuccessCallback onSuccess)
        {
            _canAuthenticate = onSuccess;
            
            StaticCoroutine.StartCoroutine(HandleCanAuthenticate());

            CanAuthenticateJava();
        }
        
        private void CanAuthenticateJava()
        {
            var callback = new BiometricJavaAuthCallback(_biometricResults);

            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            using var context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
            using var biometricPlugin = new AndroidJavaClass("com.portkey.biometricunityactivity.BiometricUnityActivity");
            biometricPlugin.CallStatic("SetCallback", callback);
            biometricPlugin.CallStatic("CanAuthenticate", context);
        }

        private void AuthenticateJava(IBiometric.BiometricPromptInfo info)
        {
            var callback = new BiometricJavaAuthCallback(_biometricResults);

            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            using var context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
            using var biometricPlugin = new AndroidJavaClass("com.portkey.biometricunityactivity.BiometricUnityActivity");
            biometricPlugin.SetStatic("title", info.title);
            biometricPlugin.SetStatic("negativeButtonText", info.negativeButtonText);
            biometricPlugin.SetStatic("description", info.description);
            biometricPlugin.SetStatic("subtitle", info.subtitle);
            biometricPlugin.CallStatic("SetCallback", callback);
            biometricPlugin.CallStatic("Call", currentActivity);
        }
        
        private IEnumerator HandleBiometricOutput()
        {
            while (true)
            {
                if (_biometricResults.TryDequeue(out var output))
                {
                    if (output.message != null)
                    {
                        Debugger.LogError(output.message);
                        _onError?.Invoke("Error in biometric authentication!");
                    }
                    else
                    {
                        _onSuccess?.Invoke(output.isAuthenticated);
                    }
                    yield break;
                }
                yield return null;
            }
        }
        
        private IEnumerator HandleCanAuthenticate()
        {
            while (true)
            {
                if (_biometricResults.TryDequeue(out var output))
                {
                    _canAuthenticate?.Invoke(output.canAuthenticate);
                    yield break;
                }
                yield return null;
            }
        }
    }
}