namespace Portkey.Core.Captcha
{
    public interface ICaptcha
    {
        void ExecuteCaptcha(SuccessCallback<string> successCallback, ErrorCallback errorCallback);
    }
}