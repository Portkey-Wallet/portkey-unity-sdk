using Portkey.Core;
using Portkey.Core.Captcha;
using UnityEngine;

namespace Portkey.Captcha
{
    public class PortkeyGoogleRecaptchaCallback : MonoBehaviour, ICaptchaCallback
    {
        public event SuccessCallback<string> OnSuccess;
        public event ErrorCallback OnError;

        public void OnCaptchaSuccess(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                OnCaptchaError("Captcha token is null or empty!");
                return;
            }
            
            OnSuccess?.Invoke(token);
            
            Destroy(gameObject);
        }

        public void OnCaptchaError(string error)
        {
            OnError?.Invoke(string.IsNullOrEmpty(error) ? "Closed captcha verification!" : error);

            Destroy(gameObject);
        }
    }
}