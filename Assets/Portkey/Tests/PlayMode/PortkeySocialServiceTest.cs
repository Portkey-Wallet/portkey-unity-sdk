using System.Collections;
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
        private const string PORTKEY_CONFIG_NAME = "PortkeyTestConfig";
        private IPortkeySocialService _portkeySocialService = new PortkeySocialService(GetPortkeyConfig(PORTKEY_CONFIG_NAME), new FetchJsonPortkeySocialServiceMock(), null);
            
        private static PortkeyConfig GetPortkeyConfig(string name)
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
            
            return _portkeySocialService.Register(registerParam, (result) =>
            {
                Assert.AreEqual("sessionId_mock", result.sessionId);
            }, ErrorCallback);
        }
        
        /// <summary>
        /// Test that GetRegisterStatus is able to get and retrieve data.
        /// </summary>
        [UnityTest]
        public IEnumerator GetRegisterStatusTest()
        {
            var done = false;
            
            var options = new QueryOptions
            {
                interval = 1,
                maxCount = 20
            };
            
            yield return _portkeySocialService.GetRegisterStatus("id_mock", options, (result) =>
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
            var done = false;
            
            var options = new QueryOptions
            {
                interval = 1,
                maxCount = -1
            };
            
            yield return _portkeySocialService.GetRegisterStatus("id_mock", options, (result) =>
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
            var done = false;
            
            var options = new QueryOptions
            {
                interval = 1,
                maxCount = 20
            };
            yield return _portkeySocialService.GetRecoverStatus("id_mock", options, (result) =>
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
            var done = false;
            
            var options = new QueryOptions
            {
                interval = 1,
                maxCount = -1
            };
            yield return _portkeySocialService.GetRecoverStatus("id_mock", options, (result) =>
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
            var done = false;

            var requestParams = new VerifyVerificationCodeParams()
            {
                verificationCode = "verificationCode_mock",
                verifierId = "verifierId_mock",
                verifierSessionId = "verifierSessionId_mock",
                guardianIdentifier = "guardianIdentifier_mock",
                chainId = "AELF"
            };
            yield return _portkeySocialService.VerifyVerificationCode(requestParams, (result) =>
            {
                done = true;
                Assert.AreEqual(true, (result.signature == "signature_mock" && result.verificationDoc == "verificationDoc_mock"));
            }, ErrorCallback);

            while (!done)
                yield return null;
        }
        
        [UnityTest]
        public IEnumerator RecoveryTest()
        {
            var done = false;

            var requestParams = new RecoveryParams()
            {
                chainId = "AELF",
                loginGuardianIdentifier = "loginGuardianIdentifier_mock",
            };
            yield return _portkeySocialService.Recovery(requestParams, (result) =>
            {
                done = true;
                Assert.AreEqual("sessionId_mock", result.sessionId);
            }, ErrorCallback);

            while (!done)
                yield return null;
        }
        
        [UnityTest]
        public IEnumerator CheckGoogleRecaptchaTest()
        {
            var done = false;

            var requestParams = new CheckGoogleRecaptchaParams()
            {
                operationType = RecaptchaType.register
            };
            yield return _portkeySocialService.CheckGoogleRecaptcha(requestParams, (result) =>
            {
                done = true;
                Assert.AreEqual(true, result);
            }, ErrorCallback);

            while (!done)
                yield return null;
        }
        
        [UnityTest]
        public IEnumerator GetRegisterInfoTest()
        {
            var done = false;

            var requestParams = new GetRegisterInfoParams()
            {
                caHash = "caHash_mock",
                loginGuardianIdentifier = "loginGuardianIdentifier_mock"
            };
            yield return _portkeySocialService.GetRegisterInfo(requestParams, (result) =>
            {
                done = true;
                Assert.AreEqual("originChainId_mock", result.originChainId);
            }, ErrorCallback);

            while (!done)
                yield return null;
        }
        
        [UnityTest]
        public IEnumerator GetPhoneCountryCodeWithLocalTest()
        {
            var done = false;

            yield return _portkeySocialService.GetPhoneCountryCodeWithLocal((result) =>
            {
                done = true;
                Assert.AreEqual("65", result.locateData.code);
            }, ErrorCallback);

            while (!done)
                yield return null;
        }
        
        [UnityTest]
        public IEnumerator GetHolderInfoTest()
        {
            var done = false;

            var requestParams = new GetHolderInfoParams()
            {
                caHash = "caHash_mock"
            };
            yield return _portkeySocialService.GetHolderInfo(requestParams, (result) =>
            {
                done = true;
                Assert.AreEqual("caAddress_mock", result.caAddress);
            }, ErrorCallback);

            while (!done)
                yield return null;
        }
        
        [UnityTest]
        public IEnumerator VerifyAppleTokenTest()
        {
            var done = false;

            var requestParams = new VerifyAppleTokenParams()
            {
                chainId = "AELF"
            };
            yield return _portkeySocialService.VerifyAppleToken(requestParams, (result) =>
            {
                done = true;
                Assert.AreEqual("verificationDoc_mock", result.verificationDoc);
            }, ErrorCallback);

            while (!done)
                yield return null;
        }
        
        [UnityTest]
        public IEnumerator VerifyGoogleTokenTest()
        {
            var done = false;

            var requestParams = new VerifyGoogleTokenParams()
            {
                chainId = "AELF"
            };
            yield return _portkeySocialService.VerifyGoogleToken(requestParams, (result) =>
            {
                done = true;
                Assert.AreEqual("verificationDoc_mock", result.verificationDoc);
            }, ErrorCallback);

            while (!done)
                yield return null;
        }
        
        
        [UnityTest]
        public IEnumerator GetCAHolderInfoTest()
        {
            var done = false;

            yield return _portkeySocialService.GetCAHolderInfo("authorization_mock","caHash_mock", (result) =>
            {
                done = true;
                Assert.AreEqual("userId_mock", result.userId);
            }, ErrorCallback);

            while (!done)
                yield return null;
        }
        
        [UnityTest]
        public IEnumerator GetChainsInfoTest()
        {
            var done = false;

            yield return _portkeySocialService.GetChainsInfo( (result) =>
            {
                done = true;
                Assert.NotZero(result.items.Length);
            }, ErrorCallback);

            while (!done)
                yield return null;
        }
    }
}
