using System;
using System.Collections;
using System.Collections.Generic;
using Portkey.Core;
using Portkey.Utilities;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;

namespace Portkey.DID
{
    /// <summary>
    /// Portkey Social Service class. Contains methods for making requests to the Portkey Social API.
    /// </summary>
    public class PortkeySocialService : IPortkeySocialService
    {
        private PortkeyConfig _config;
        private IHttp _http;
        private GraphQL.GraphQL _graphQl;
        
        public PortkeySocialService(PortkeyConfig config, IHttp http, GraphQL.GraphQL graphQl)
        {
            _config = config;
            _http = http;
            _graphQl = graphQl;
        }

        private string GetFullApiUrl(string url)
        {
            return _config.ApiBaseUrl + url;
        }
        
        private IEnumerator Post<T1, T2>(string url, T1 requestParams, SuccessCallback<T2> successCallback, ErrorCallback errorCallback)
        {
            var jsonRequestData = new JsonRequestData
            {
                Url = GetFullApiUrl(url),
                JsonData = JsonConvert.SerializeObject(requestParams),
            };
            
            return _http.Post(jsonRequestData, JsonToObject<T2>(successCallback, errorCallback), errorCallback);
        }
        
        private IEnumerator Get<T>(JsonRequestData jsonRequestData, SuccessCallback<T> successCallback, ErrorCallback errorCallback)
        {
            return _http.Get(jsonRequestData, JsonToObject<T>(successCallback, errorCallback), errorCallback);
        }
        
        private IEnumerator Get<T1, T2>(string url, T1 requestParams, SuccessCallback<T2> successCallback, ErrorCallback errorCallback)
        {
            var jsonRequestData = new JsonRequestData
            {
                Url = GetFullApiUrl(url),
                JsonData = JsonConvert.SerializeObject(requestParams),
            };
            
            return Get(jsonRequestData, successCallback, errorCallback);
        }
        
        private IEnumerator Get<T>(string url, SuccessCallback<T> successCallback, ErrorCallback errorCallback)
        {
            var jsonRequestData = new JsonRequestData
            {
                Url = GetFullApiUrl(url),
            };
            
            return Get(jsonRequestData, successCallback, errorCallback);
        }
        
        private static IHttp.successCallback JsonToObject<T>(SuccessCallback<T> successCallback, ErrorCallback errorCallback)
        {
            return (response) =>
            {
                JToken json;
                try
                {
                    json = JToken.Parse(response);
                }
                catch (Exception e)
                {
                    errorCallback(e.Message);
                    return;
                }

                switch (json)
                {
                    case JObject:
                    {
                        try
                        {
                            //deserialize response
                            var deserializedObject = JsonConvert.DeserializeObject<T>(json.ToString());
                            //call success callback
                            successCallback(deserializedObject);
                        }
                        catch (Exception e)
                        {
                            errorCallback(e.Message);
                        }
                        break;
                    }
                    case JValue:
                        try
                        {
                            successCallback(json.Value<T>());
                        }
                        catch (Exception e)
                        {
                            errorCallback(e.Message);
                        }
                        break;
                    default:
                        errorCallback("Unsupported type detected!");
                        break;
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

        public IEnumerator SendAppleUserExtraInfo(SendAppleUserExtraInfoParams requestParams, SuccessCallback<SendAppleUserExtraInfoResult> successCallback, ErrorCallback errorCallback)
        {
            return Post("/api/app/userExtraInfo/appleUserExtraInfo", requestParams, successCallback, errorCallback);
        }

        public IEnumerator VerifyGoogleToken(VerifyGoogleTokenParams requestParams, SuccessCallback<VerifyVerificationCodeResult> successCallback, ErrorCallback errorCallback)
        {
            return Post("/api/app/account/verifyGoogleToken", requestParams, successCallback, errorCallback);
        }

        public IEnumerator VerifyAppleToken(VerifyAppleTokenParams requestParams, SuccessCallback<VerifyVerificationCodeResult> successCallback, ErrorCallback errorCallback)
        {
            return Post("/api/app/account/verifyAppleToken", requestParams, successCallback, errorCallback);
        }

        public IEnumerator GetRegisterStatus(string id, QueryOptions queryOptions, SuccessCallback<RegisterStatusResult> successCallback, ErrorCallback errorCallback)
        {
            var pollCount = 0;
            
            yield return Poll();

            IEnumerator Poll()
            {
                if(pollCount > queryOptions.maxCount)
                {
                    errorCallback("timeout");
                    yield break;
                }
            
                yield return Get("/api/app/search/accountregisterindex", new { filter = $"_id:{id}" }, (ArrayWrapper<RegisterStatusResult> ret) =>
                {
                    StaticCoroutine.StartCoroutine(RepollIfNeeded(ret));
                }, errorCallback);
            }
            
            IEnumerator RepollIfNeeded(ArrayWrapper<RegisterStatusResult> result)
            {
                if(result?.items == null || result.items.Length == 0 || result.items[0].registerStatus == "pending")
                {
                    yield return new WaitForSeconds(queryOptions.interval);
                    ++pollCount;
                    yield return Poll();
                }
                else
                {
                    successCallback(result.items[0]);
                }
            }
        }

        public IEnumerator GetRecoverStatus(string id, QueryOptions queryOptions, SuccessCallback<RecoverStatusResult> successCallback, ErrorCallback errorCallback)
        {
            var pollCount = 0;
            
            yield return Poll();
            
            IEnumerator Poll()
            {
                if(pollCount > queryOptions.maxCount)
                {
                    errorCallback("timeout");
                    yield break;
                }
            
                yield return Get("/api/app/search/accountrecoverindex", new { filter = $"_id:{id}" }, (ArrayWrapper<RecoverStatusResult> ret) =>
                {
                    StaticCoroutine.StartCoroutine(RepollIfNeeded(ret));
                }, errorCallback);
            }
            
            IEnumerator RepollIfNeeded(ArrayWrapper<RecoverStatusResult> result)
            {
                if(result?.items == null || result.items.Length == 0 || result.items[0].recoveryStatus == "pending")
                {
                    yield return new WaitForSeconds(queryOptions.interval);
                    ++pollCount;
                    yield return Poll();
                }
                else
                {
                    successCallback(result.items[0]);
                }
            }
        }

        public IEnumerator GetChainsInfo(SuccessCallback<ChainInfo[]> successCallback, ErrorCallback errorCallback)
        {
            return Get("/api/app/search/chainsinfoindex", successCallback, errorCallback);
        }

        public IEnumerator GetCAHolderInfo(string authorization, string caHash, SuccessCallback<CAHolderInfo> successCallback, ErrorCallback errorCallback)
        {
            var jsonRequestData = new JsonRequestData
            {
                Url = GetFullApiUrl("/api/app/search/caholderindex"),
                JsonData = JsonConvert.SerializeObject(new { filter = $"caHash:{caHash}" }),
                Headers = new Dictionary<string, string> { { "Authorization", authorization } }
            };
            
            return Get(jsonRequestData, successCallback, errorCallback);
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

        public IEnumerator GetHolderInfoByManager(GetCAHolderByManagerParams requestParams, SuccessCallback<GetCAHolderByManagerResult> successCallback, ErrorCallback errorCallback)
        {
            return _graphQl.GetHolderInfoByManager(requestParams.manager, requestParams.chainId, (ret) =>
            {
                var result = new GetCAHolderByManagerResult
                {
                    caHolders = ret
                };
                successCallback(result);
            }, errorCallback);
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
