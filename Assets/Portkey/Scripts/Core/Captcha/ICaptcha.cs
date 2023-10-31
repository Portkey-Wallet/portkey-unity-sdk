namespace Portkey.Core
{
    public interface ICaptcha
    {
        void ExecuteCaptcha(SuccessCallback<string> successCallback, ErrorCallback errorCallback);
    }
}