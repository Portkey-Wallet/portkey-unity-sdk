using System;
using System.Collections;
using System.Net;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Portkey.Core;
using Portkey.DID;
using Portkey.Network;
using UnityEditor;

namespace Portkey.Test
{
    /// <summary>
    /// Portkey test for testing if implemented class can do request to api and return results.
    /// </summary>
    public class PortkeySocialServiceTest
    {
        public class PortkeySocialServiceMonoTest : PortkeySocialService, IMonoBehaviourTest
        {
            private const string PORTKEY_CONFIG_NAME = "PortkeyMockConfig";
            public bool IsTestFinished
            {
                get { return false; }
            }

            private void Start()
            {
                this.config = GetPortkeyConfig(PORTKEY_CONFIG_NAME);
                this._http = new FetchJsonPortkeySocialServiceMock();
            }
            
            protected static PortkeyConfig GetPortkeyConfig(string name)
            {
                var guids = AssetDatabase.FindAssets($"t:{nameof(PortkeyConfig)} {name}");
                if (guids.Length == 0)
                {
                    Assert.Fail($"No {nameof(PortkeyConfig)} found!");
                }
                else if (guids.Length > 0)
                {
                    Debug.LogWarning($"More than one {nameof(PortkeyConfig)} found, taking first one");
                }

                return (PortkeyConfig)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(PortkeyConfig));
            }
        }


        private MonoBehaviourTest<PortkeySocialServiceMonoTest> _portkeySocialService = new MonoBehaviourTest<PortkeySocialServiceMonoTest>();
        
        private void ErrorCallback(string param)
        {
            Debug.Log("errorCallback");
            Assert.Fail(param);
        }
        
        private void ExpectedErrorCallback(string param)
        {
            Debug.Log("errorCallback");
            Assert.Pass(param);
        }

        /// <summary>
        /// Test that Register is able to get and retrieve data.
        /// </summary>
        [UnityTest]
        public IEnumerator RegisterTest()
        {
            var registerParam = new RegisterParams
            {
                type = AccountType.Email,
                loginGuardianIdentifier = "loginGuardianIdentifier_mock",
                manager = "manager_mock",
                extraData = "extraData_mock",
                chainId = "AELF",
                verifierId = "verifierId_mock",
                verificationDoc = "verificationDoc_mock",
                signature = "signature_mock",
                context = new Context
                {
                    clientId = "clientId_mock",
                    requestId = "requestId_mock"
                }
            };
            return _portkeySocialService.component.Register(registerParam, (result) =>
            {
                Assert.AreNotEqual("", result.sessionId);
            }, ErrorCallback);
        }
        
        /// <summary>
        /// Test that GetRegisterStatus is able to get and retrieve data.
        /// </summary>
        [UnityTest]
        public IEnumerator GetRegisterStatusTest()
        {
            bool done = false;
            
            var options = new QueryOptions
            {
                interval = 1,
                reCount = 0,
                maxCount = 20
            };
            yield return _portkeySocialService.component.GetRegisterStatus("id_mock", options, (result) =>
            {
                done = true;
                Assert.AreEqual(true, (result.registerStatus == "pass" && result.registerMessage != ""));
            }, ErrorCallback);

            while (!done)
                yield return null;
        }
        
        /// <summary>
        /// Test failed parameter for GetRegisterStatus.
        /// </summary>
        [UnityTest]
        public IEnumerator GetRegisterStatusBadParameterTest()
        {
            bool done = false;
            
            var options = new QueryOptions
            {
                interval = 1,
                reCount = 1,
                maxCount = 0
            };
            yield return _portkeySocialService.component.GetRegisterStatus("id_mock", options, (result) =>
            {
                done = true;
                Assert.Fail("Should not be here.");
            }, ExpectedErrorCallback);

            while (!done)
                yield return null;
        }
        
        /// <summary>
        /// Test that GetRecoverStatusTest is able to get and retrieve data.
        /// </summary>
        [UnityTest]
        public IEnumerator GetRecoverStatusTest()
        {
            bool done = false;
            
            var options = new QueryOptions
            {
                interval = 1,
                reCount = 0,
                maxCount = 20
            };
            yield return _portkeySocialService.component.GetRecoverStatus("id_mock", options, (result) =>
            {
                done = true;
                Assert.AreEqual(true, (result.recoveryStatus == "pass" && result.recoveryMessage != ""));
            }, ErrorCallback);

            while (!done)
                yield return null;
        }
        
        /// <summary>
        /// Test failed parameter for GetRecoverStatus.
        /// </summary>
        [UnityTest]
        public IEnumerator GetRecoverStatusBadParameterTest()
        {
            bool done = false;
            
            var options = new QueryOptions
            {
                interval = 1,
                reCount = 1,
                maxCount = 0
            };
            yield return _portkeySocialService.component.GetRecoverStatus("id_mock", options, (result) =>
            {
                done = true;
                Assert.Fail("Should not be here.");
            }, ExpectedErrorCallback);

            while (!done)
                yield return null;
        }
        
        /// <summary>
        /// Test that VerifyVerificationCodeTest is able to get and retrieve data.
        /// </summary>
        [UnityTest]
        public IEnumerator VerifyVerificationCodeTest()
        {
            bool done = false;

            var requestParams = new VerifyVerificationCodeParams()
            {
                verificationCode = "verificationCode_mock",
                verifierId = "verifierId_mock",
                verifierSessionId = "verifierSessionId_mock",
                guardianIdentifier = "guardianIdentifier_mock",
                chainId = "AELF"
            };
            yield return _portkeySocialService.component.VerifyVerificationCode(requestParams, (result) =>
            {
                done = true;
                Assert.AreEqual(true, (result.signature == "signature_mock" && result.verificationDoc == "verificationDoc_mock"));
            }, ErrorCallback);

            while (!done)
                yield return null;
        }
    }
}
