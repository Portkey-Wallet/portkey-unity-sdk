using System;
using Newtonsoft.Json.Linq;
using Portkey.Core;
using Portkey.Utilities;

namespace Portkey.SocialProvider
{
    public abstract class TelegramLoginBase : ISocialLogin
    {
        protected ISocialLogin.AuthCallback _successCallback;
        protected ErrorCallback _errorCallback;

        protected IHttp _request;

        protected TelegramLoginBase(IHttp request)
        {
            _request = request;
        }
        
        public void Authenticate(ISocialLogin.AuthCallback successCallback, ErrorCallback errorCallback)
        {
            _successCallback = successCallback;
            _errorCallback = errorCallback;

            OnAuthenticate();
        }

        protected abstract void OnAuthenticate();

        public void RequestSocialInfo(string accessToken, ISocialLogin.AuthCallback successCallback, ErrorCallback errorCallback)
        {
            Debugger.Log("RequestSocialInfo");
            successCallback ??= _successCallback;
            errorCallback ??= _errorCallback;

            try
            {
                var socialLoginInfo = CreateSocialLoginInfo(accessToken);
                successCallback?.Invoke(socialLoginInfo);
            }
            catch (Exception e)
            {
                errorCallback?.Invoke(e.Message);
            }
        }

        private static SocialLoginInfo CreateSocialLoginInfo(string accessToken)
        {
            var jObject = new Jwt(accessToken);

            var exp = jObject.GetValue<long>("exp");
            var expiryDate = DateTime.UnixEpoch.AddSeconds(exp);
            var isExpired = DateTime.Now > expiryDate;
            Debugger.Log(jObject);

            var socialLoginInfo = new SocialLoginInfo
            {
                isExpired = isExpired,
                access_token = accessToken,
                accountType = AccountType.Telegram,
                socialInfo = new SocialInfo
                {
                    email = "*****",
                    sub = jObject.GetValue<string>("userId"),
                    // picture = jObject.GetValue<string>("protoUrl"),
                    // email_verified = jObject.GetValue<bool>("email_verified")
                }
            };
            return socialLoginInfo;
        }

        public void HandleError(string error)
        {
            _errorCallback?.Invoke(error);
        }
        
        protected static IHttp.ErrorCallback OnError(ErrorCallback errorCallback)
        {
            return (error) =>
            {
                errorCallback(error.message + error.details);
            };
        }
    }
    
}