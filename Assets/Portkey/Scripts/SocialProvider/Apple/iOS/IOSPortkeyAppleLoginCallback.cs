using System;
using Portkey.Core;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class IOSPortkeyAppleLoginCallback : MonoBehaviour, IPortkeySocialLoginCallback
    {
        public ISocialLogin SocialLogin { get; set; }

        public void OnSuccess(string data)
        {
            Debugger.Log($"IOSPortkeyAppleLoginOnSuccess {data}");
            try
            {
                SocialLogin.RequestSocialInfo(data);
            }
            catch (Exception e)
            {
                Debugger.LogException(e);
            }
            Destroy(gameObject);
        }

        public void OnFailure(string error)
        {
            try
            {
                SocialLogin.HandleError("Login Cancelled!");
            }
            catch (Exception e)
            {
                Debugger.LogException(e);
            }
            Destroy(gameObject);
        }
    }
}