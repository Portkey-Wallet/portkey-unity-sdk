namespace Portkey.Core
{
    public interface ISocialLogin
    {
        delegate void AuthCallback(SocialLoginInfo info);
        void Authenticate(AuthCallback successCallback, ErrorCallback errorCallback);
    }
}