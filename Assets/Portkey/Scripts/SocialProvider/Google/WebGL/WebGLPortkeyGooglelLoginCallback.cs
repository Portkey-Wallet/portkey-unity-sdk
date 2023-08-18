using System;
using Newtonsoft.Json;
using Portkey.Core;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class WebGLPortkeyGooglelLoginCallback : MonoBehaviour, IPortkeySocialLoginCallback
    {
        private class WebGLPortkeySocialLoginData
        {
            public string token;
            //Google or Apple
            public string provider;
        }
        
        private static AccountType GetAccountType(string provider) => provider switch
        {
            "Google" => AccountType.Google,
            "Apple" => AccountType.Apple,
            _ => throw new Exception("Not supported provider")
        };
        
        public ISocialLogin SocialLogin { get; set; }

        public void OnSuccess(string data)
        {
            Debugger.Log($"WebGLPortkeySocialLoginOnSuccess {data}");
            try
            {
                var loginData = JsonConvert.DeserializeObject<WebGLPortkeySocialLoginData>(data);
                SocialLogin.RequestSocialInfo(loginData.token);
            }
            catch (Exception e)
            {
                Debugger.LogException(e);
            }
            Destroy(gameObject);
        }

        public void OnFailure(string error)
        {
            SocialLogin.HandleError("Login Cancelled!");
            Destroy(gameObject);
        }
    }
}