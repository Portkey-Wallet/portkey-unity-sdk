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
        
        public void AuthenticateIfAccessTokenExpired(VerifyAccessTokenParam param, AuthCallback successCallback, ErrorCallback errorCallback)
        {
            if(param.accessToken == null)
            {
                _socialLogin.Authenticate((info) =>
                {
                    param.accessToken = info.access_token;
                    VerifyToken(param, successCallback, errorCallback);
                }, errorCallback);
                return;
            }
            
            // check if login access token is expired
            _socialLogin.RequestSocialInfo(param.accessToken, (socialLoginInfo) =>
            {
                if(socialLoginInfo.isExpired)
                {
                    //login expired, need to re-login
                    _socialLogin.Authenticate((info) =>
                    {
                        param.accessToken = info.access_token;
                        VerifyToken(param, successCallback, errorCallback);
                    }, errorCallback);
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