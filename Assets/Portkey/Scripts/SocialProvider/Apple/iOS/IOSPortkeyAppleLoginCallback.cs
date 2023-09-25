using System;
using System.Collections;
using Portkey.Core;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class IOSPortkeyAppleLoginCallback : MonoBehaviour, IPortkeySocialLoginCallback
    {
        public ISocialLogin SocialLogin { get; set; }
        private bool _isSuccess;

        private void Start()
        {
            _isSuccess = false;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            Debugger.Log($"IOSPortkeyAppleLoginOnApplicationPause isPaused: {pauseStatus}");

            if(!pauseStatus)
            {
                StartCoroutine(WaitAndFail(2.5f));
            }
        }

        private IEnumerator WaitAndFail(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            if (!_isSuccess)
            {
                OnFailure("Login Cancelled!");
            }
        }

        public void OnSuccess(string data)
        {
            _isSuccess = true;
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