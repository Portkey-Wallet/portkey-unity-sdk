using Portkey.Core;

namespace Portkey.SocialProvider
{
    public abstract class SocialVerifierBase : ISocialVerifier
    {
        protected ISocialLogin _socialLogin;
        
        protected SocialVerifierBase(ISocialLogin socialLogin)
        {
            _socialLogin = socialLogin;
        }
        
        public void AuthenticateIfAccessTokenExpired(VerifyAccessTokenParam param, AuthCallback successCallback,
            SuccessCallback<bool> startLoadCallback, ErrorCallback errorCallback)
        {
            if(param.accessToken == null)
            {
                _socialLogin.Authenticate((info) =>
                {
                    param.accessToken = info.access_token;
                    VerifyToken(param, successCallback, errorCallback);
                }, startLoadCallback, errorCallback);
                return;
            }
            
            startLoadCallback(true);
            // check if login access token is expired
            _socialLogin.RequestSocialInfo(param.accessToken, (socialLoginInfo) =>
            {
                if(socialLoginInfo.isExpired)
                {
                    startLoadCallback(false);
                    //login expired, need to re-login
                    _socialLogin.Authenticate((info) =>
                    {
                        param.accessToken = info.access_token;
                        VerifyToken(param, successCallback, errorCallback);
                    }, startLoadCallback, errorCallback);
                }
                else
                {
                    VerifyToken(param, successCallback, errorCallback);
                }
            }, errorCallback);
        }

        protected abstract void VerifyToken(VerifyAccessTokenParam param, AuthCallback successCallback, ErrorCallback errorCallback);
    }
}