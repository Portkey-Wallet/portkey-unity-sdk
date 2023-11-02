namespace Portkey.Core.Captcha
{
    public interface ICaptchaCallback
    { 
        public event SuccessCallback<string> OnSuccess;
        public event ErrorCallback OnError;
        void OnCaptchaSuccess(string token);
        void OnCaptchaError(string error);
    }
}