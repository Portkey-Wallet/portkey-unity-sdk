using System.Collections.Generic;
using Newtonsoft.Json;
using Portkey.Core;
using Portkey.Utilities;

namespace Portkey.SocialProvider
{
    public abstract class GoogleLoginBase : ISocialLogin
    {
        private const string USERINFO_ENDPOINT = "https://www.googleapis.com/oauth2/v3/userinfo";
        
        protected ISocialLogin.AuthCallback _successCallback;
        protected ErrorCallback _errorCallback;
        protected SuccessCallback<bool> _startLoadCallback;

        protected IHttp _request;
        
        public void Authenticate(ISocialLogin.AuthCallback successCallback, SuccessCallback<bool> startLoadCallback, ErrorCallback errorCallback)
        {
            _successCallback = successCallback;
            _errorCallback = errorCallback;
            _startLoadCallback = startLoadCallback;

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
        
        private static IHttp.ErrorCallback OnError(ErrorCallback errorCallback)
        {
            return (error) =>
            {
                errorCallback(error.message + error.details);
            };
        }
    }
}