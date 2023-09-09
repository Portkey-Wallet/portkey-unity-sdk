using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Portkey.Core;
using Portkey.Utilities;
using UnityEngine;
using Empty = Portkey.Core.Empty;

namespace Portkey.DID
{
    /// <summary>
    /// Portkey Social Service class. Contains methods for making requests to the Portkey Social API.
    /// </summary>
    public class PortkeySocialService : IPortkeySocialService
    {
        private class Filter
        {
            public string filter;
        }
        
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
        
        private IEnumerator Post<T1, T2>(string url, T1 requestParams, SuccessCallback<T2> successCallback, IHttp.ErrorCallback errorCallback)
        {
            var jsonRequestData = new JsonRequestData
            {
                Url = GetFullApiUrl(url),
                JsonData = JsonConvert.SerializeObject(requestParams),
            };

            return _http.Post(jsonRequestData, JsonToObject<T2>(successCallback, errorCallback), errorCallback);
        }
        
        private IEnumerator HttpGet<T1, T2>(FieldFormRequestData<T1> requestData, SuccessCallback<T2> successCallback, IHttp.ErrorCallback errorCallback)
        {
            return _http.Get(requestData, JsonToObject<T2>(successCallback, errorCallback), errorCallback);
        }
        
        private IEnumerator Get<T1, T2>(string url, T1 requestParams, SuccessCallback<T2> successCallback, IHttp.ErrorCallback errorCallback)
        {
            var requestData = new FieldFormRequestData<T1>()
            {
                Url = GetFullApiUrl(url),
                FieldFormsObject = requestParams,
            };
            
            return HttpGet(requestData, successCallback, errorCallback);
        }
        
        private IEnumerator Get<T>(string url, SuccessCallback<T> successCallback, IHttp.ErrorCallback errorCallback)
        {
            var requestData = new FieldFormRequestData<Empty>
            {
                Url = GetFullApiUrl(url),
                FieldFormsObject = new Empty()
            };
            
            return HttpGet(requestData, successCallback, errorCallback);
        }
        
        private static IHttp.SuccessCallback JsonToObject<T>(SuccessCallback<T> successCallback, IHttp.ErrorCallback errorCallback)
        {
            return (response) =>
            {
                JToken json;
                try
                {
                    Debugger.Log($"Parsing: {response}");
                    json = JToken.Parse(response);
                }
                catch (Exception e)
                {
                    Debugger.LogException(e);
                    errorCallback(new IHttp.ErrorMessage { message = e.Message });
                    return;
                }

                switch (json)
                {
                    case JObject:
                    {
                        try
                        {
                            Debugger.Log($"Deserializing: {json.ToString()}");
                            //deserialize response
                            var deserializedObject = JsonConvert.DeserializeObject<T>(json.ToString());
                            //call success callback
                            successCallback(deserializedObject);
                        }
                        catch (Exception e)
                        {
                            Debugger.LogException(e);
                            errorCallback(new IHttp.ErrorMessage { message = e.Message });
                        }
                        break;
                    }
                    case JValue:
                        try
                        {
                            Debugger.Log($"Deserializing to Value...");
                            successCallback(json.Value<T>());
                        }
                        catch (Exception e)
                        {
                            Debugger.LogException(e);
                            errorCallback(new IHttp.ErrorMessage { message = e.Message });
                        }
                        break;
                    default:
                        errorCallback(new IHttp.ErrorMessage { message = "Unsupported type detected!" });
                        break;
                }
            };
        }

        private static IHttp.ErrorCallback OnError(ErrorCallback errorCallback)
        {
            return (error) =>
            {
                errorCallback(error.message);
            };
        }
        
        public IEnumerator GetVerificationCode(SendVerificationCodeRequestParams requestParams, SuccessCallback<SendVerificationCodeResult> successCallback, ErrorCallback errorCallback)
        {
            return Post("/api/app/account/sendVerificationRequest", requestParams.body, successCallback, OnError(errorCallback));
        }

        public IEnumerator VerifyVerificationCode(VerifyVerificationCodeParams requestParams, SuccessCallback<VerifyVerificationCodeResult> successCallback, ErrorCallback errorCallback)
        {
            return Post("/api/app/account/verifyCode", requestParams, successCallback, OnError(errorCallback));
        }

        public IEnumerator SendAppleUserExtraInfo(SendAppleUserExtraInfoParams requestParams, SuccessCallback<SendAppleUserExtraInfoResult> successCallback, ErrorCallback errorCallback)
        {
            return Post("/api/app/userExtraInfo/appleUserExtraInfo", requestParams, successCallback, OnError(errorCallback));
        }

        public IEnumerator VerifyGoogleToken(VerifyGoogleTokenParams requestParams, SuccessCallback<VerifyVerificationCodeResult> successCallback, ErrorCallback errorCallback)
        {
            return Post("/api/app/account/verifyGoogleToken", requestParams, successCallback, OnError(errorCallback));
        }

        public IEnumerator VerifyAppleToken(VerifyAppleTokenParams requestParams, SuccessCallback<VerifyVerificationCodeResult> successCallback, ErrorCallback errorCallback)
        {
            return Post("/api/app/account/verifyAppleToken", requestParams, successCallback, OnError(errorCallback));
        }

        internal class GetVerifierServerParams
        {
            public string chainId;
        }
        
        public IEnumerator GetVerifierServer(string chainId, SuccessCallback<VerifierServerResult> successCallback, ErrorCallback errorCallback)
        {
            var requestParam = new GetVerifierServerParams
            {
                chainId = chainId
            };
            return Post("/api/app/account/getVerifierServer", requestParam, successCallback, OnError(errorCallback));
        }

        public IEnumerator GetRegisterStatus(string sessionId, QueryOptions queryOptions, SuccessCallback<RegisterStatusResult> successCallback, ErrorCallback errorCallback)
        {
            var pollCount = 0;
            
            yield return Poll();

            IEnumerator Poll()
            {
                if(pollCount > queryOptions.maxCount)
                {
                    errorCallback("Network Error!");
                    yield break;
                }
            
                yield return Get("/api/app/search/accountregisterindex", new Filter { filter = $"_id:{sessionId}" }, (ArrayWrapper<RegisterStatusResult> ret) =>
                {
                    StaticCoroutine.StartCoroutine(RepollIfNeeded(ret));
                }, OnError(errorCallback));
            }
            
            IEnumerator RepollIfNeeded(ArrayWrapper<RegisterStatusResult> result)
            {
                if(result?.items == null || result.items.Length == 0 || result.items[0].registerStatus == "pending")
                {
                    yield return new WaitForSeconds(queryOptions.interval/1000.0f);
                    ++pollCount;
                    yield return Poll();
                }
                else
                {
                    successCallback(result.items[0]);
                }
            }
        }

        public IEnumerator GetRecoverStatus(string sessionId, QueryOptions queryOptions, SuccessCallback<RecoverStatusResult> successCallback, ErrorCallback errorCallback)
        {
            var pollCount = 0;
            
            yield return Poll();
            
            IEnumerator Poll()
            {
                if(pollCount > queryOptions.maxCount)
                {
                    errorCallback("Network Error!");
                    yield break;
                }
            
                yield return Get("/api/app/search/accountrecoverindex", new Filter { filter = $"_id:{sessionId}" }, (ArrayWrapper<RecoverStatusResult> ret) =>
                {
                    Debugger.Log($"Recover status returned..");
                    StaticCoroutine.StartCoroutine(RepollIfNeeded(ret));
                }, OnError(errorCallback));
            }
            
            IEnumerator RepollIfNeeded(ArrayWrapper<RecoverStatusResult> result)
            {
                if(result?.items == null || result.items.Length == 0 || result.items[0].recoveryStatus == "pending")
                {
                    Debugger.Log($"We got nothing, repoll...");
                    yield return new WaitForSeconds(queryOptions.interval/1000.0f);
                    ++pollCount;
                    yield return Poll();
                }
                else
                {
                    Debugger.Log($"Success callback..");
                    successCallback(result.items[0]);
                }
            }
        }

        public IEnumerator GetChainsInfo(SuccessCallback<ArrayWrapper<ChainInfo>> successCallback, ErrorCallback errorCallback)
        {
            return Get("/api/app/search/chainsinfoindex", successCallback, OnError(errorCallback));
        }

        public IEnumerator GetCAHolderInfo(string authorization, string caHash, SuccessCallback<CAHolderInfo> successCallback, ErrorCallback errorCallback)
        {
            var requestData = new FieldFormRequestData<Filter>()
            {
                Url = GetFullApiUrl("/api/app/search/caholderindex"),
                FieldFormsObject = new Filter { filter = $"caHash:{caHash}" },
                Headers = new Dictionary<string, string> { { "Authorization", authorization } }
            };
            
            return HttpGet(requestData, successCallback, OnError(errorCallback));
        }

        public IEnumerator Register(RegisterParams requestParams, SuccessCallback<SessionIdResult> successCallback, ErrorCallback errorCallback)
        {
            return Post("/api/app/account/register/request", requestParams, successCallback, OnError(errorCallback));
        }

        public IEnumerator Recovery(RecoveryParams requestParams, SuccessCallback<SessionIdResult> successCallback, ErrorCallback errorCallback)
        {
            return Post("/api/app/account/recovery/request", requestParams, successCallback, OnError(errorCallback));
        }

        public IEnumerator GetHolderInfo(GetHolderInfoParams requestParams, SuccessCallback<IHolderInfo> successCallback, ErrorCallback errorCallback)
        {
            return Get("/api/app/account/guardianIdentifiers", requestParams, successCallback, OnError(errorCallback));
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
            return Get("/api/app/account/registerInfo", requestParams, successCallback, (error) =>
            {
                if (error.details.Contains(IPortkeySocialService.UNREGISTERED_CODE))
                {
                    errorCallback(IPortkeySocialService.UNREGISTERED_CODE);
                    return;
                }
                errorCallback(error.message);
            });
        }

        public IEnumerator CheckGoogleRecaptcha(CheckGoogleRecaptchaParams requestParams, SuccessCallback<bool> successCallback, ErrorCallback errorCallback)
        {
            return Post("/api/app/account/isGoogleRecaptchaOpen", requestParams, successCallback, OnError(errorCallback));
        }

        public IEnumerator GetPhoneCountryCode(SuccessCallback<ICountryItem[]> successCallback, ErrorCallback errorCallback)
        {
            return Get("/api/app/phone/info", successCallback, OnError(errorCallback));
        }

        public IEnumerator GetPhoneCountryCodeWithLocal(SuccessCallback<IPhoneCountryCodeResult> successCallback, ErrorCallback errorCallback)
        {
            return Get("/api/app/phone/info", successCallback, OnError(errorCallback));
        }
    }
}
