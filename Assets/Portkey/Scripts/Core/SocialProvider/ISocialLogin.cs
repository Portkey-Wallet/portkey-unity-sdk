namespace Portkey.Core
{
    public interface ISocialLogin
    {
        delegate void AuthCallback(SocialLoginInfo info);
        void Authenticate(AuthCallback successCallback, SuccessCallback<bool> startLoadCallback, ErrorCallback errorCallback);
        void RequestSocialInfo(string accessToken, AuthCallback successCallback, ErrorCallback errorCallback);
        void HandleError(string error);
    }
}