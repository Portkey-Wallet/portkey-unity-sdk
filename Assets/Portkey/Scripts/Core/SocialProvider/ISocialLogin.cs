namespace Portkey.Core
{
    public interface ISocialLogin
    {
        delegate void AuthCallback(SocialLoginInfo info);
        void Authenticate(AuthCallback successCallback, ErrorCallback errorCallback);
        void RequestSocialInfo(string accessToken, AuthCallback successCallback = null, ErrorCallback errorCallback = null);
        void HandleError(string error);
    }
}