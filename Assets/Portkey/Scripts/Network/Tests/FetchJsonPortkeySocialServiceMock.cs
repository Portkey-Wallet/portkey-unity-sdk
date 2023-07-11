using System.Collections;
using System.Collections.Generic;
using Portkey.Core;
using UnityEngine;

namespace Portkey.Network
{
    public class FetchJsonPortkeySocialServiceMock : IHttp
    {
        private int _registerStatusCallTimes = 0;
        private int _recoverStatusCallTimes = 0;
        
        public override IEnumerator Get(string url, string jsonData, IHttp.successCallback successCallback, IHttp.errorCallback errorCallback)
        {
            var subUrl = url.Replace("https://test.mock", "");
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
            }
        }

        public override IEnumerator Post(string url, string jsonData, IHttp.successCallback successCallback, IHttp.errorCallback errorCallback)
        {
            var subUrl = url.Replace("https://test.mock", "");
            switch (subUrl)
            {
                case "/api/app/account/register/request":
                    successCallback(JsonUtility.ToJson(new RegisterResult
                    {
                        sessionId = "sessionId_mock"
                    }));
                    yield break;
                case "/api/app/account/verifyCode":
                    successCallback(JsonUtility.ToJson(new VerifyVerificationCodeResult
                    {
                        verificationDoc = "verificationDoc_mock",
                        signature = "signature_mock"
                    }));
                    yield break;
            }
        }
    }
}