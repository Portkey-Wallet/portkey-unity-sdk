using QRCoder;
using System.Collections;
using Newtonsoft.Json;
using Portkey.Core;
using QRCoder.Unity;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class QRLogin : IQRLogin
    {
        private struct Data
        {
            public struct ExtraData
            {
                public DeviceInfoType deviceInfo;
                public string version;
            }

            public string type;
            public string address;
            public string netWorkType;
            public string chainType;
            public ExtraData extraData;
        }
        
        private readonly QRCodeGenerator _qrGenerator = new QRCodeGenerator();
        private readonly ISigningKeyGenerator _signingKeyGenerator;
        private readonly ILoginPoller _loginPoller;
        
        private LoginPollerHandler _loginPollerHandler = null;
        
        public QRLogin(ILoginPoller loginPoller, ISigningKeyGenerator signingKeyGenerator)
        {
            _loginPoller = loginPoller;
            _signingKeyGenerator = signingKeyGenerator;
        }
        
        public IEnumerator Login(string chainId, SuccessCallback<Texture2D> qrCodeCallback, SuccessCallback<PortkeyAppLoginResult> successCallback, ErrorCallback errorCallback)
        {
            var signingKey = _signingKeyGenerator.Create();

            var data = new Data
            {
                type = "login",
                address = signingKey.Address,
                netWorkType = "TESTNET",
                chainType = chainId.ToLower(),
                extraData = new Data.ExtraData
                {
                    deviceInfo = DeviceInfoType.GetDeviceInfo(),
                    version = "2.0.0"
                }
            };
        
            var qrData = $"{JsonConvert.SerializeObject(data)}";
            
            var qrCodeData = _qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new UnityQRCode(qrCodeData);
            var qrCodeAsTexture2D = qrCode.GetGraphic(20);

            _loginPollerHandler = _loginPoller.Start(chainId, signingKey, result =>
            {
                _loginPollerHandler = null;
                successCallback(result);
            }, errorCallback, ILoginPoller.INFINITE_TIMEOUT);
            
            qrCodeCallback(qrCodeAsTexture2D);
            yield break;
        }
        
        public void Cancel()
        {
            if (_loginPollerHandler == null)
            {
                return;
            }
            _loginPoller.Stop(_loginPollerHandler);
            _loginPollerHandler = null;
        }
    }
}