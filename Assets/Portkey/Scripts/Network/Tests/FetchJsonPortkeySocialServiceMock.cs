using System.Collections;
using Portkey.Core;
using UnityEngine;

namespace Portkey.Network
{
    /// <summary>
    /// Class used to mock for PortkeySocialServiceTest
    /// </summary>
    public class FetchJsonPortkeySocialServiceMock : IHttp
    {
        private int _registerStatusCallTimes = 0;
        private int _recoverStatusCallTimes = 0;
        
        public override IEnumerator Get<T>(FieldFormRequestData<T> data, IHttp.SuccessCallback successCallback, ErrorCallback errorCallback)
        {
            var subUrl = data.Url.Replace("https://test.mock", "");
            switch (subUrl)
            {
                case "/api/app/search/accountregisterindex":
                    ++_registerStatusCallTimes;
                    switch (_registerStatusCallTimes)
                    {
                        case 1:
                            successCallback(JsonUtility.ToJson(new ArrayWrapper<RegisterStatusResult> { items = null } ));
                            break;
                        case 2:
                            RegisterStatusResult[] pendingResults = { new RegisterStatusResult
                            {
                                caAddress = "caAddress_mock",
                                caHash = "caHash_mock",
                                registerMessage = "registerMessage_mock",
                                registerStatus = "pending"
                            } };
                            successCallback(JsonUtility.ToJson(new ArrayWrapper<RegisterStatusResult> { items = pendingResults }));
                            break;
                        default:
                            RegisterStatusResult[] passResults = { new RegisterStatusResult
                            {
                                caAddress = "caAddress_mock",
                                caHash = "caHash_mock",
                                registerMessage = "registerMessage_mock",
                                registerStatus = "pass"
                            } };
                            successCallback(JsonUtility.ToJson(new ArrayWrapper<RegisterStatusResult> { items = passResults }));
                            break;
                    }
                    yield break;
                case "/api/app/search/accountrecoverindex":
                    ++_recoverStatusCallTimes;
                    switch (_recoverStatusCallTimes)
                    {
                        case 1:
                            successCallback(JsonUtility.ToJson(new ArrayWrapper<RecoverStatusResult> { items = null } ));
                            break;
                        case 2:
                            RecoverStatusResult[] pendingResults = { new RecoverStatusResult
                            {
                                caAddress = "caAddress_mock",
                                caHash = "caHash_mock",
                                recoveryMessage = "registerMessage_mock",
                                recoveryStatus = "pending"
                            } };
                            successCallback(JsonUtility.ToJson(new ArrayWrapper<RecoverStatusResult> { items = pendingResults }));
                            break;
                        default:
                            RecoverStatusResult[] passResults = { new RecoverStatusResult
                            {
                                caAddress = "caAddress_mock",
                                caHash = "caHash_mock",
                                recoveryMessage = "registerMessage_mock",
                                recoveryStatus = "pass"
                            } };
                            successCallback(JsonUtility.ToJson(new ArrayWrapper<RecoverStatusResult> { items = passResults }));
                            break;
                    }
                    yield break;
                case "/api/app/search/chainsinfoindex":
                    successCallback("{\"totalCount\":2,\"items\":[{\"chainId\":\"tDVV\",\"chainName\":\"tDVV\",\"endPoint\":\"https://aelf-node-sidechain-test1.aelf.io\",\"explorerUrl\":\"http://192.168.66.100:8000\",\"caContractAddress\":\"UYdd84gLMsVdHrgkr3ogqe1ukhKwen8oj32Ks4J1dg6KH9PYC\",\"defaultToken\":{\"name\":\"tDVV\",\"address\":\"7RzVGiuVWkvL4VfVHdZfQF2Tri3sgLe9U991bohHFfSRZXuGX\",\"imageUrl\":\"https://portkey-did.s3.ap-northeast-1.amazonaws.com/img/aelf_token_logo.png\",\"symbol\":\"ELF\",\"decimals\":\"8\"},\"lastModifyTime\":\"2023-07-05T10:53:27.8331876Z\",\"id\":\"tDVV\"},{\"chainId\":\"AELF\",\"chainName\":\"AELF\",\"endPoint\":\"http://192.168.66.61:8000\",\"explorerUrl\":\"http://192.168.66.61:8000\",\"caContractAddress\":\"2imqjpkCwnvYzfnr61Lp2XQVN2JU17LPkA9AZzmRZzV5LRRWmR\",\"defaultToken\":{\"name\":\"AELF\",\"address\":\"JRmBduh4nXWi1aXgdUsj5gJrzeZb2LxmrAbf7W99faZSvoAaE\",\"imageUrl\":\"https://portkey-did.s3.ap-northeast-1.amazonaws.com/img/aelf_token_logo.png\",\"symbol\":\"ELF\",\"decimals\":\"8\"},\"lastModifyTime\":\"2023-07-04T08:14:32.4893159Z\",\"id\":\"AELF\"}]}");
                    yield break;
                case "/api/app/search/caholderindex":
                    successCallback(JsonUtility.ToJson(new CAHolderInfo
                    {
                        userId = "userId_mock",
                        caAddress = "caAddress_mock",
                        caHash = "caHash_mock",
                        id = "id_mock",
                        nickName = "nickName_mock"
                    }));
                    yield break;
                case "/api/app/account/guardianIdentifiers":
                    successCallback(JsonUtility.ToJson(new IHolderInfo
                    {
                        caAddress = "caAddress_mock",
                        caHash = "caHash_mock"
                    }));
                    yield break;
                case "/api/app/account/registerInfo":
                    successCallback(JsonUtility.ToJson(new RegisterInfo
                    {
                        originChainId = "originChainId_mock"
                    }));
                    yield break;
                case "/api/app/phone/info":
                    var phoneInfo = @"{
                                    ""locateData"": {
                                            ""country"": ""Singapore"",
                                            ""code"": ""65"",
                                            ""iso"": ""SG""
                                    },
                                    ""data"": [
                                    {
                                        ""country"": ""China"",
                                        ""code"": ""86"",
                                        ""iso"": ""CN""
                                    },
                                    {
                                        ""country"": ""Denmark"",
                                        ""code"": ""45"",
                                        ""iso"": ""DK""
                                    },
                                    ]
                                }";
                    successCallback(phoneInfo);
                    yield break;
            }
        }

        public override IEnumerator Post(JsonRequestData data, IHttp.SuccessCallback successCallback, ErrorCallback errorCallback)
        {
            var subUrl = data.Url.Replace("https://test.mock", "");
            switch (subUrl)
            {
                case "/api/app/account/register/request":
                    successCallback(@"{
                    ""sessionId"": ""sessionId_mock""
                    }");
                    yield break;
                case "/api/app/account/recovery/request":
                    successCallback(@"{
                    ""sessionId"": ""sessionId_mock""
                    }");
                    yield break;
                case "/api/app/account/verifyCode":
                case "/api/app/account/verifyGoogleToken":
                case "/api/app/account/verifyAppleToken":
                    successCallback(JsonUtility.ToJson(new VerifyVerificationCodeResult
                    {
                        verificationDoc = "verificationDoc_mock",
                        signature = "signature_mock"
                    }));
                    yield break;
                case "/api/app/account/sendVerificationRequest":
                    successCallback(JsonUtility.ToJson(new SendVerificationCodeResult
                    {
                        verifierSessionId = "verifierSessionId_mock"
                    }));
                    yield break;
                case "/api/app/account/appleUserExtraInfo":
                    successCallback(JsonUtility.ToJson(new SendAppleUserExtraInfoResult
                    {
                        userId = "userId_mock"
                    }));
                    yield break;
                case "/api/app/account/isGoogleRecaptchaOpen":
                    successCallback("true");
                    yield break;
            }
        }

        public override IEnumerator PostFieldForm<T>(FieldFormRequestData<T> data, SuccessCallback successCallback, ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }
    }
}