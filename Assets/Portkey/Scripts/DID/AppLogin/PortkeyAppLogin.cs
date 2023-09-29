using System.Collections;
using System.Linq;
using Newtonsoft.Json;
using Portkey.Core;
using Portkey.Utilities;
using UnityEngine;
using UnityEngine.Networking;

namespace Portkey.DID
{
    public class PortkeyAppLogin : IAppLogin
    {
        private float LOGIN_TIMEOUT = 40.0f;
        private float WAIT_INTERVAL = 2.0f;
        private struct Data
        {
            public struct ExtraData
            {
                public DeviceInfoType deviceInfo;
                public string version;
            }
        
            public string type;
            public string address;
            public string id;
            public string netWorkType;
            public string chainType;
            public ExtraData extraData;
        }
    
        private struct AppData
        {
            public string websiteName;
            public string websiteIcon;
        }
        
        private readonly ISigningKeyGenerator _signingKeyGenerator;
        private readonly IPortkeySocialService _portkeySocialService;
        private readonly TransportConfig _config;
        
        public PortkeyAppLogin(TransportConfig config, ISigningKeyGenerator signingKeyGenerator, IPortkeySocialService portkeySocialService)
        {
            _config = config;
            _signingKeyGenerator = signingKeyGenerator;
            _portkeySocialService = portkeySocialService;
        }
        
        public IEnumerator Login(string chainId, SuccessCallback<PortkeyAppLoginResult> successCallback, ErrorCallback errorCallback)
        {
            var privateKey = _signingKeyGenerator.Create();
            var guid = System.Guid.NewGuid().ToString();
            
            Debugger.LogError(privateKey.Address);

            var data = new Data
            {
                type = "login",
                address = privateKey.Address,
                id = guid,
                netWorkType = "TESTNET",
                chainType = chainId.ToLower(),
                extraData = new Data.ExtraData
                {
                    deviceInfo = DeviceInfoType.GetDeviceInfo(),
                    version = "2.0.0"
                }
            };
        
            var appData = new AppData
            {
                websiteName = Application.productName,
                websiteIcon = _config.IconURL
            };
        
            var dataParam = $"data={UnityWebRequest.EscapeURL(JsonConvert.SerializeObject(data))}";
            var extraDataParam = $"extraData={UnityWebRequest.EscapeURL(JsonConvert.SerializeObject(appData))}";

            var uri = $"portkey.sdk/login?{dataParam}&{extraDataParam}";

            _config.Send(uri);

            var timer = 0.0f;
            StaticCoroutine.StartCoroutine(WaitForResponse(timer, chainId, privateKey, successCallback, error =>
            {
                Debugger.LogError(error);
                errorCallback(error);
            }));
            
            yield break;
        }

        private IEnumerator WaitForResponse(float timer, string chainId, ISigningKey signingKey, SuccessCallback<PortkeyAppLoginResult> successCallback, ErrorCallback errorCallback)
        {
            yield return new WaitForSeconds(WAIT_INTERVAL);
            timer += WAIT_INTERVAL;

            var param = new GetCAHolderByManagerParams
            {
                manager = signingKey.Address,
                chainId = chainId
            };
            yield return _portkeySocialService.GetHolderInfoByManager(param, result =>
            {
                foreach (var caHolder in result.caHolders)
                {
                    if (caHolder.holderManagerInfo.managerInfos.All(manager => manager.address != signingKey.Address))
                    {
                        continue;
                    }
                    var loginResult = new PortkeyAppLoginResult
                    {
                        caHolder = caHolder, 
                        managementAccount = signingKey
                    };
                    successCallback(loginResult);
                    return;
                }

                if (timer > LOGIN_TIMEOUT)
                {
                    errorCallback("Login timeout");
                    return;
                }

                //if we did not find the manager, we poll again
                StaticCoroutine.StartCoroutine(WaitForResponse(timer, chainId, signingKey, successCallback, errorCallback));
            }, error =>
            {
                Debugger.LogError(error);
                
                if (timer > LOGIN_TIMEOUT)
                {
                    errorCallback("Login timeout");
                    return;
                }
                
                StaticCoroutine.StartCoroutine(WaitForResponse(timer, chainId, signingKey, successCallback, errorCallback));
            });
        }
    }
}