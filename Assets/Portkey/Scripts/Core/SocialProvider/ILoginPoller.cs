namespace Portkey.Core
{
    public class LoginPollerHandler
    {
        public int id;
    }
    
    public interface ILoginPoller
    {
        public const float INFINITE_TIMEOUT = -1.0f;
        private const float LOGIN_TIMEOUT = 40.0f;
        private const float WAIT_INTERVAL = 2.0f;
        
        LoginPollerHandler Start(ISigningKey signingKey, SuccessCallback<PortkeyAppLoginResult> successCallback,
            ErrorCallback errorCallback, float timeOut = LOGIN_TIMEOUT, float pollInterval = WAIT_INTERVAL);
        void Stop(LoginPollerHandler handler);
    }
}