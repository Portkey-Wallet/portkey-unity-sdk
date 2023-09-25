using System;
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
            callbackComponent.OnSuccessCallback = OnSuccess;
            callbackComponent.OnFailureCallback = OnFailure;
        }

#if UNITY_IOS
        [DllImport ("__Internal")]
        private static extern void SignIn ();
#endif
        
        private void Authenticate()
        {
#if UNITY_IOS
            SignIn();
#endif
        }
        
        private void OnSuccess(string accessToken)
        {
            try
            {
                RequestSocialInfo(accessToken, null, null);
            }
            catch (Exception e)
            {
                Debugger.LogException(e);
                HandleError("Error in logging in. Please try again.");
            }
        }
        
        private void OnFailure(string error)
        {
            HandleError("Login Cancelled!");
        }
    }
}