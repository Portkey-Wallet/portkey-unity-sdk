using System.Collections;
using Portkey.Core;
using Portkey.Network;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace Portkey.DID
{
    //create a class inheriting from ServiceBase<FetchJsonHttp> and implementing IPortkeySocialService
    //this class will be used to make requests to the Portkey Social API
    public class PortkeySocialService : ServiceBase<IHttp>, IPortkeySocialService
    {
        private PortkeyConfig _config;
        public PortkeySocialService(IHttp http, PortkeyConfig config) : base(http)
        {
            this._config = config;
        }

        public IEnumerator GetVerificationCode(SendVerificationCodeRequestParams requestParams, IVerificationService.SuccessCallback<SendVerificationCodeResult> successCallback,
            IVerificationService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator VerifyVerificationCode(VerifyVerificationCodeParams requestParams, IVerificationService.SuccessCallback<VerifyVerificationCodeResult> successCallback,
            IVerificationService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator sendAppleUserExtraInfo(SendAppleUserExtraInfoParams requestParams, IVerificationService.SuccessCallback<SendAppleUserExtraInfoResult> successCallback,
            IVerificationService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator verifyGoogleToken(VerifyGoogleTokenParams requestParams, IVerificationService.SuccessCallback<VerifyVerificationCodeResult> successCallback,
            IVerificationService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator verifyAppleToken(VerifyAppleTokenParams requestParams, IVerificationService.SuccessCallback<VerifyVerificationCodeResult> successCallback,
            IVerificationService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator GetRegisterStatus(string id, QueryOptions queryOptions, ISearchService.SuccessCallback<RegisterStatusResult> successCallback,
            ISearchService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator GetRecoverStatus(string id, QueryOptions queryOptions, ISearchService.SuccessCallback<RecoverStatusResult> successCallback,
            ISearchService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator GetChainsInfo(ISearchService.SuccessCallback<ChainInfo[]> successCallback, ISearchService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator GetCAHolderInfo(string authorization, string caHash, ISearchService.SuccessCallback<CAHolderInfo> successCallback,
            ISearchService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator Register(RegisterParams requestParams, IPortkeySocialService.SuccessCallback<RegisterResult> successCallback, IPortkeySocialService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator Recovery(RecoveryParams requestParams, IPortkeySocialService.SuccessCallback<RecoveryResult> successCallback, IPortkeySocialService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator GetHolderInfo(GetHolderInfoParams requestParams, IPortkeySocialService.SuccessCallback<IHolderInfo> successCallback,
            IPortkeySocialService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator GetHolderInfoByManager(GetCAHolderByManagerParams requestParams, IPortkeySocialService.SuccessCallback<GetCAHolderByManagerResult> successCallback,
            IPortkeySocialService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator GetRegisterInfo(GetRegisterInfoParams requestParams, IPortkeySocialService.SuccessCallback<RegisterInfo> successCallback,
            IPortkeySocialService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator CheckGoogleRecaptcha(CheckGoogleRecaptchaParams requestParams, IPortkeySocialService.SuccessCallback<bool> successCallback,
            IPortkeySocialService.ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
            /*
            string url = _config.apiBaseUrl + "/api/app/account/isGoogleRecaptchaOpen";
            return this.Http.Post(url, JsonToObject<bool>(successCallback, errorCallback),
                (error) =>
                {
                    errorCallback(error);
                });*/
        }

        public IEnumerator GetPhoneCountryCode(IPortkeySocialService.SuccessCallback<ICountryItem[]> successCallback, IPortkeySocialService.ErrorCallback errorCallback)
        {
            string url = _config.apiBaseUrl + "/api/app/phone/info";
            return this.Http.Get(url, JsonToObject<ICountryItem[]>(successCallback, errorCallback),
                (error) =>
                {
                    errorCallback(error);
                });
        }

        public IEnumerator GetPhoneCountryCodeWithLocal(IPortkeySocialService.SuccessCallback<IPhoneCountryCodeResult> successCallback, IPortkeySocialService.ErrorCallback errorCallback)
        {
            string url = _config.apiBaseUrl + "/api/app/phone/info";
            return this.Http.Get(url, JsonToObject<IPhoneCountryCodeResult>(successCallback, errorCallback),
                (error) =>
                {
                    errorCallback(error);
                });
        }
        
        private static IHttp.successCallback JsonToObject<T>(IPortkeySocialService.SuccessCallback<T> successCallback, IPortkeySocialService.ErrorCallback errorCallback)
        {
            return (response) =>
            {
                var json = JObject.Parse(response);
                string data = null;
                if (json.TryGetValue("errors", out var errorMessage))
                {
                    //process data and wrapper
                    var error = errorMessage.ToString();
                    errorCallback(error);
                    return;
                }
                                    
                if (json.TryGetValue("data", out var value))
                {
                    //process data and wrapper
                    data = value.ToString();
                }
                else
                {
                    errorCallback("No data in response. Incorrect response format.");
                    return;
                }
                //deserialize response
                var deserializedObject = JsonConvert.DeserializeObject<T>(data);
                //call success callback
                successCallback(deserializedObject);
            };
        }
    }
}
