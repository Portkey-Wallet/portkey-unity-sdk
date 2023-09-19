using System.Collections.Generic;
using Newtonsoft.Json;
using Portkey.Core;
using Portkey.Utilities;

namespace Portkey.SocialProvider
{
    public abstract class GoogleLoginBase : ISocialLogin
    {
        protected const string AUTHORIZATION_ENDPOINT = "https://accounts.google.com/o/oauth2/v2/auth";
        protected const string TOKEN_ENDPOINT = "https://www.googleapis.com/oauth2/v4/token";
        private const string USERINFO_ENDPOINT = "https://www.googleapis.com/oauth2/v3/userinfo";
        protected const string ACCESS_SCOPE = "openid email profile";

        protected ISocialLogin.AuthCallback _successCallback;
        protected ErrorCallback _errorCallback;

        protected IHttp _request;
        
        protected string ClientId { get; set; }
        
        public GoogleLoginBase(IHttp request)
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
            
            var param = new FieldFormRequestData<Empty>()
            {
                Url = USERINFO_ENDPOINT,
                Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {accessToken}" }
                }
            };
            StaticCoroutine.StartCoroutine(_request.Get(param, (response) =>
            {
                var socialInfo = JsonConvert.DeserializeObject<SocialInfo>(response);

                var socialLoginInfo = new SocialLoginInfo
                {
                    isExpired = socialInfo == null,
                    access_token = accessToken,
                    accountType = AccountType.Google,
                    socialInfo = socialInfo
                };
                
                successCallback(socialLoginInfo);
            }, OnError(errorCallback)));
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