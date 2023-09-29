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
        
        private readonly IAccountProvider<WalletAccount> _accountProvider;
        private readonly IPortkeySocialService _portkeySocialService;
        private readonly TransportConfig _config;
        
        public PortkeyAppLogin(TransportConfig config, IAccountProvider<WalletAccount> accountProvider, IPortkeySocialService portkeySocialService)
        {
            _config = config;
            _accountProvider = accountProvider;
            _portkeySocialService = portkeySocialService;
        }
        
        public IEnumerator Login(string chainId, SuccessCallback<DIDWalletInfo> successCallback, ErrorCallback errorCallback)
        {
            var privateKey = _accountProvider.CreateAccount();
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

        private IEnumerator WaitForResponse(float timer, string chainId, WalletAccount wallet, SuccessCallback<DIDWalletInfo> successCallback, ErrorCallback errorCallback)
        {
            yield return new WaitForSeconds(WAIT_INTERVAL);
            timer += WAIT_INTERVAL;

            var param = new GetCAHolderByManagerParams
            {
                manager = wallet.Address,
                chainId = chainId
            };
            yield return _portkeySocialService.GetHolderInfoByManager(param, result =>
            {
                foreach (var caHolder in result.caHolders)
                {
                    foreach (var didWalletInfo in from manager in caHolder.holderManagerInfo.managerInfos where manager.address == wallet.Address select CreateDIDWalletInfo(caHolder, wallet.Wallet))
                    {
                        successCallback(didWalletInfo);
                        return;
                    }
                }

                if (timer > LOGIN_TIMEOUT)
                {
                    errorCallback("Login timeout");
                    return;
                }

                //if we did not find the manager, we poll again
                StaticCoroutine.StartCoroutine(WaitForResponse(timer, chainId, wallet, successCallback, errorCallback));
            }, error =>
            {
                Debugger.LogError(error);
                
                if (timer > LOGIN_TIMEOUT)
                {
                    errorCallback("Login timeout");
                    return;
                }
                
                StaticCoroutine.StartCoroutine(WaitForResponse(timer, chainId, wallet, successCallback, errorCallback));
            });
        }
        
        private DIDWalletInfo CreateDIDWalletInfo(CaHolderWithGuardian caHolder, BlockchainWallet wallet)
        {
            var walletInfo = new DIDWalletInfo
            {
                caInfo = new CAInfo
                {
                    caAddress = caHolder.holderManagerInfo.caAddress,
                    caHash = caHolder.holderManagerInfo.caHash
                },
                chainId = caHolder.holderManagerInfo.originChainId,
                wallet = wallet,
                managerInfo = new ManagerInfoType
                {
                    managerUniqueId = caHolder.holderManagerInfo.id,
                    guardianIdentifier = caHolder.holderManagerInfo.id,
                    accountType = (AccountType)caHolder.loginGuardianInfo[0].loginGuardian.type,
                    type = AddManagerType.AddManager
                }
            };
            return walletInfo;
        }
    }
}