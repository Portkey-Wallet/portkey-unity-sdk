using Portkey.Core;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class IOSPortkeyBiometricCallback : MonoBehaviour, IPortkeyBiometricCallback
    {
        private readonly string CANCELLED = "Cancelled";
        
        public IBiometric.SuccessCallback SuccessCallback { get; set; }
        public ErrorCallback ErrorCallback { get; set; }

        public void OnSuccess(string data)
        {
            Debugger.Log($"IOSPortkeyBiometricCallback Success: {data}");
            SuccessCallback?.Invoke(true);
            Destroy(gameObject);
        }

        public void OnFailure(string error)
        {
            if (error != CANCELLED)
            {
                Debugger.LogError($"IOSPortkeyBiometricCallback Error: {error}");
                ErrorCallback?.Invoke(error);
            }
            Destroy(gameObject);
        }
    }
}