using System;
using Newtonsoft.Json;
using Portkey.Core;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class WebGLPortkeyLoginCallback : MonoBehaviour, IPortkeySocialLoginCallback
    {
        private class WebGLPortkeySocialLoginData
        {
            public string token;
            //Google or Apple or Telegram
            public string provider;
        }
        
        private static AccountType GetAccountType(string provider) => provider switch
        {
            "Google" => AccountType.Google,
            "Apple" => AccountType.Apple,
            "Telegram" => AccountType.Telegram,
            _ => throw new Exception("Not supported provider")
        };

        public Action<string> OnSuccessCallback { get; set; }
        public Action<string> OnFailureCallback { get; set; }

        public void OnSuccess(string data)
        {
            Debugger.Log($"WebGLPortkeySocialLoginOnSuccess {data}");
            try
            {
                var loginData = JsonConvert.DeserializeObject<WebGLPortkeySocialLoginData>(data);
                OnSuccessCallback?.Invoke(loginData.token);
            }
            catch (Exception e)
            {
                Debugger.LogException(e);
            }
            Destroy(gameObject);
        }

        public void OnFailure(string error)
        {
            OnFailureCallback?.Invoke("Login Cancelled!");
            Destroy(gameObject);
        }
    }
}