using Portkey.Core;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class IOSPortkeyBiometricCallback : MonoBehaviour, IPortkeyBiometricCallback
    {
        private readonly string CANCELLED = "Cancelled";

        public IBiometric.SuccessCallback SuccessCallback { get; set; } = null;
        public IBiometric.SuccessCallback CanAuthenticateCallback { get; set; } = null;
        public ErrorCallback ErrorCallback { get; set; } = null;

        public void OnSuccess(string data)
        {
            Debugger.Log($"IOSPortkeyBiometricCallback Success: {data}");
            SuccessCallback?.Invoke(data != "Cannot Authenticate");
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

        public void CanAuthenticate(string data)
        {
            CanAuthenticateCallback?.Invoke(data == "true");
            Destroy(gameObject);
        }
    }
}