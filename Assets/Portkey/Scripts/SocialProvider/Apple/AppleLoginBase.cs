using System;
using Newtonsoft.Json.Linq;
using Portkey.Core;
using Portkey.Utilities;

namespace Portkey.SocialProvider
{
    public abstract class AppleLoginBase : ISocialLogin
    {
        protected ISocialLogin.AuthCallback _successCallback;
        protected ErrorCallback _errorCallback;

        protected IHttp _request;

        protected AppleLoginBase(IHttp request)
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

            var socialLoginInfo = new SocialLoginInfo
            {
                isExpired = isExpired,
                access_token = accessToken,
                accountType = AccountType.Apple,
                socialInfo = new SocialInfo
                {
                    email = jObject.GetValue<string>("email"),
                    sub = jObject.GetValue<string>("sub"),
                    email_verified = jObject.GetValue<bool>("email_verified")
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
    
    internal static class JObjectExtensions
    {
        internal static T GetValue<T>(this JObject jObject, string propertyName)
        {
            if (!jObject.TryGetValue(propertyName, out var propJToken))
            {
                throw new Exception($"Property {propertyName} not found in JObject");
            }

            if(propJToken is JValue prop)
            {
                return prop.Value<T>();
            }
            
            throw new System.Exception($"Property {propertyName} is not a Value");
        }
    }
}