using System;
using Portkey.Core;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class IOSPortkeyGoogleLoginCallback : MonoBehaviour, IPortkeySocialLoginCallback
    {
        public Action<string> OnSuccessCallback { get; set; }
        public Action<string> OnFailureCallback { get; set; }

        public void OnSuccess(string data)
        {
            Debugger.Log($"IOSPortkeySocialLoginOnSuccess {data}");
            
            OnSuccessCallback?.Invoke(data);
            Destroy(gameObject);
        }

        public void OnFailure(string error)
        {
            OnFailureCallback?.Invoke("Login Cancelled!");
            Destroy(gameObject);
        }
    }
}