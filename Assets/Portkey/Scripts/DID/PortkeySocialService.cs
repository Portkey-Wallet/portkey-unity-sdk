using System;
using System.Collections;
using Portkey.Core;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Portkey.DID
{
    /// <summary>
    /// Portkey Social Service class. Contains methods for making requests to the Portkey Social API.
    /// </summary>
    public class PortkeySocialService : MonoBehaviour, IPortkeySocialService
    {
        [SerializeField]
        protected PortkeyConfig config;
        [SerializeField]
        protected IHttp _http;
        //[SerializeField]
        //private readonly GraphQL _graphQL;

        private IEnumerator Post<T1, T2>(string url, T1 requestParams, SuccessCallback<T2> successCallback, ErrorCallback errorCallback)
        {
            var jsonData = JsonConvert.SerializeObject(requestParams);
            var fullUrl = config.ApiBaseUrl + url;
            return _http.Post(fullUrl, jsonData, JsonToObject<T2>(successCallback, errorCallback),
                (error) =>
                {
                    errorCallback(error);
                });
        }
        
        private IEnumerator Get<T1, T2>(string url, T1 requestParams, SuccessCallback<T2> successCallback, ErrorCallback errorCallback)
        {
            var jsonData = JsonConvert.SerializeObject(requestParams);
            var fullUrl = config.ApiBaseUrl + url;
            return _http.Get(fullUrl, jsonData, JsonToObject<T2>(successCallback, errorCallback),
                (error) =>
                {
                    errorCallback(error);
                });
        }
        
        private IEnumerator Get<T>(string url, SuccessCallback<T> successCallback, ErrorCallback errorCallback)
        {
            var fullUrl = config.ApiBaseUrl + url;
            return _http.Get(fullUrl, "", JsonToObject<T>(successCallback, errorCallback),
                (error) =>
                {
                    errorCallback(error);
                });
        }
        
        private static IHttp.successCallback JsonToObject<T>(SuccessCallback<T> successCallback, ErrorCallback errorCallback)
        {
            return (response) =>
            {
                var json = JToken.Parse(response);

                if (json is JObject)
                {
                    //deserialize response
                    var deserializedObject = JsonConvert.DeserializeObject<T>(json.ToString());
                    //call success callback
                    successCallback(deserializedObject);
                }
                else
                {
                    //throw new Exception("Unknown type");
                    errorCallback("Unsupported type detected!");
                    return;
                }
            };
        }

        public IEnumerator GetVerificationCode(SendVerificationCodeRequestParams requestParams, SuccessCallback<SendVerificationCodeResult> successCallback, ErrorCallback errorCallback)
        {
            return Post("/api/app/account/sendVerificationRequest", requestParams, successCallback, errorCallback);
        }

        public IEnumerator VerifyVerificationCode(VerifyVerificationCodeParams requestParams, SuccessCallback<VerifyVerificationCodeResult> successCallback, ErrorCallback errorCallback)
        {
            return Post("/api/app/account/verifyCode", requestParams, successCallback, errorCallback);
        }

        public IEnumerator sendAppleUserExtraInfo(SendAppleUserExtraInfoParams requestParams, SuccessCallback<SendAppleUserExtraInfoResult> successCallback, ErrorCallback errorCallback)
        {
            return Post("/api/app/userExtraInfo/appleUserExtraInfo", requestParams, successCallback, errorCallback);
        }

        public IEnumerator verifyGoogleToken(VerifyGoogleTokenParams requestParams, SuccessCallback<VerifyVerificationCodeResult> successCallback, ErrorCallback errorCallback)
        {
            return Post("/api/app/account/verifyGoogleToken", requestParams, successCallback, errorCallback);
        }

        public IEnumerator verifyAppleToken(VerifyAppleTokenParams requestParams, SuccessCallback<VerifyVerificationCodeResult> successCallback, ErrorCallback errorCallback)
        {
            return Post("/api/app/account/verifyAppleToken", requestParams, successCallback, errorCallback);
        }

        public IEnumerator GetRegisterStatus(string id, QueryOptions queryOptions, SuccessCallback<RegisterStatusResult> successCallback, ErrorCallback errorCallback)
        {
            if(queryOptions.reCount > queryOptions.maxCount)
            {
                errorCallback("timeout");
                yield break;
            }
            
            yield return Get("/api/app/search/accountregisterindex", new { filter = $"_id:{id}" }, (ArrayWrapper<RegisterStatusResult> ret) =>
            {
                StartCoroutine(GetRegisterStatus(id, queryOptions, ret, (ArrayWrapper<RegisterStatusResult> response) =>
                {
                    if (response.items == null || response.items.Length == 0)
                    {
                        errorCallback("response.items is null");
                        return;
                    }
                    successCallback(response.items[0]);
                }, errorCallback));
            }, errorCallback);
        }
        
        private IEnumerator GetRegisterStatus(string id, QueryOptions queryOptions, SuccessCallback<ArrayWrapper<RegisterStatusResult>> successCallback, ErrorCallback errorCallback)
        {
            if(queryOptions.reCount > queryOptions.maxCount)
            {
                errorCallback("timeout");
                yield break;
            }
            
            yield return Get("/api/app/search/accountregisterindex", new { filter = $"_id:{id}" }, (ArrayWrapper<RegisterStatusResult> ret) =>
            {
                StartCoroutine(GetRegisterStatus(id, queryOptions, ret, successCallback, errorCallback));
            }, errorCallback);
        }

        private IEnumerator GetRegisterStatus(string id, QueryOptions queryOptions, ArrayWrapper<RegisterStatusResult> requestParam,
            SuccessCallback<ArrayWrapper<RegisterStatusResult>> successCallback, ErrorCallback errorCallback)
        {
            if(requestParam == null || requestParam.items == null || 
               requestParam.items.Length == 0 || requestParam.items[0].registerStatus == "pending")
            {
                yield return new WaitForSeconds(queryOptions.interval);
                ++queryOptions.reCount;
                yield return StartCoroutine(GetRegisterStatus(id, queryOptions, successCallback, errorCallback));
            }
            else
            {
                successCallback(requestParam);
            }
        }

        public IEnumerator GetRecoverStatus(string id, QueryOptions queryOptions, SuccessCallback<RecoverStatusResult> successCallback, ErrorCallback errorCallback)
        {
            if(queryOptions.reCount > queryOptions.maxCount)
            {
                errorCallback("timeout");
                yield break;
            }
            yield return Get("/api/app/search/accountrecoverindex", new { filter = $"_id:{id}" }, (ArrayWrapper<RecoverStatusResult> ret) =>
            {
                StartCoroutine(GetRecoverStatus(id, queryOptions, ret, (ArrayWrapper<RecoverStatusResult> response) =>
                {
                    if (response.items == null || response.items.Length == 0)
                    {
                        errorCallback("response.items is null");
                        return;
                    }
                    successCallback(response.items[0]);
                }, errorCallback));
            }, errorCallback);
        }
        
        private IEnumerator GetRecoverStatus(string id, QueryOptions queryOptions, SuccessCallback<ArrayWrapper<RecoverStatusResult>> successCallback, ErrorCallback errorCallback)
        {
            if(queryOptions.reCount > queryOptions.maxCount)
            {
                errorCallback("timeout");
                yield break;
            }
            
            yield return Get("/api/app/search/accountrecoverindex", new { filter = $"_id:{id}" }, (ArrayWrapper<RecoverStatusResult> ret) =>
            {
                StartCoroutine(GetRecoverStatus(id, queryOptions, ret, successCallback, errorCallback));
            }, errorCallback);
        }
        
        private IEnumerator GetRecoverStatus(string id, QueryOptions queryOptions, ArrayWrapper<RecoverStatusResult> requestParam,
            SuccessCallback<ArrayWrapper<RecoverStatusResult>> successCallback, ErrorCallback errorCallback)
        {
            if(requestParam == null || requestParam.items == null || 
               requestParam.items.Length == 0 || requestParam.items[0].recoveryStatus == "pending")
            {
                yield return new WaitForSeconds(queryOptions.interval);
                ++queryOptions.reCount;
                yield return StartCoroutine(GetRecoverStatus(id, queryOptions, successCallback, errorCallback));
            }
            else
            {
                successCallback(requestParam);
            }
        }

        public IEnumerator GetChainsInfo(SuccessCallback<ChainInfo[]> successCallback, ErrorCallback errorCallback)
        {
            return Get("/api/app/search/chainsinfoindex", successCallback, errorCallback);
        }

        public IEnumerator GetCAHolderInfo(string authorization, string caHash, SuccessCallback<CAHolderInfo> successCallback, ErrorCallback errorCallback)
        {
            return Get("/api/app/search/caholderindex", new { filter = $"caHash:{caHash}" }, successCallback, errorCallback);
        }

        public IEnumerator Register(RegisterParams requestParams, SuccessCallback<RegisterResult> successCallback, ErrorCallback errorCallback)
        {
            return Post("/api/app/account/register/request", requestParams, successCallback, errorCallback);
        }

        public IEnumerator Recovery(RecoveryParams requestParams, SuccessCallback<RecoveryResult> successCallback, ErrorCallback errorCallback)
        {
            return Post("/api/app/account/recovery/request", requestParams, successCallback, errorCallback);
        }

        public IEnumerator GetHolderInfo(GetHolderInfoParams requestParams, SuccessCallback<IHolderInfo> successCallback, ErrorCallback errorCallback)
        {
            return Get("/api/app/account/guardianIdentifiers", requestParams, successCallback, errorCallback);
        }

        //TODO: implement once graphQL is merged
        public IEnumerator GetHolderInfoByManager(GetCAHolderByManagerParams requestParams, SuccessCallback<GetCAHolderByManagerResult> successCallback, ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator GetRegisterInfo(GetRegisterInfoParams requestParams, SuccessCallback<RegisterInfo> successCallback, ErrorCallback errorCallback)
        {
            return Get("/api/app/account/registerInfo", requestParams, successCallback, errorCallback);
        }

        public IEnumerator CheckGoogleRecaptcha(CheckGoogleRecaptchaParams requestParams, SuccessCallback<bool> successCallback, ErrorCallback errorCallback)
        {
            return Post("/api/app/account/isGoogleRecaptchaOpen", requestParams, successCallback, errorCallback);
        }

        public IEnumerator GetPhoneCountryCode(SuccessCallback<ICountryItem[]> successCallback, ErrorCallback errorCallback)
        {
            return Get("/api/app/phone/info", successCallback, errorCallback);
        }

        public IEnumerator GetPhoneCountryCodeWithLocal(SuccessCallback<IPhoneCountryCodeResult> successCallback, ErrorCallback errorCallback)
        {
            return Get("/api/app/phone/info", successCallback, errorCallback);
        }
    }
}
