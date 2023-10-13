using System.Collections;
using Newtonsoft.Json;
using Portkey.Core;
using Portkey.Utilities;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class QRLogin : IQRLogin
    {
        private readonly ISigningKeyGenerator _signingKeyGenerator;
        private readonly ILoginPoller _loginPoller;
        private readonly IQRCodeGenerator _qrCodeGenerator;
        
        private LoginPollerHandler _loginPollerHandler = null;
        
        public QRLogin(ILoginPoller loginPoller, ISigningKeyGenerator signingKeyGenerator, IQRCodeGenerator qrCodeGenerator)
        {
            _loginPoller = loginPoller;
            _signingKeyGenerator = signingKeyGenerator;
            _qrCodeGenerator = qrCodeGenerator;
        }
        
        public IEnumerator Login(SuccessCallback<Texture2D> qrCodeCallback, SuccessCallback<PortkeyAppLoginResult> successCallback, ErrorCallback errorCallback)
        {
            var signingKey = _signingKeyGenerator.Create();
            
            Listen(signingKey, successCallback, errorCallback);
            
            var qrCodeAsTexture2D = CreateQRCode(signingKey);
            qrCodeCallback(qrCodeAsTexture2D);
            yield break;
        }

        private void Listen(ISigningKey signingKey, SuccessCallback<PortkeyAppLoginResult> successCallback,
            ErrorCallback errorCallback)
        {
            _loginPollerHandler = _loginPoller.Start(signingKey, result =>
            {
                _loginPollerHandler = null;
                successCallback(result);
            }, errorCallback, timeOut: ILoginPoller.INFINITE_TIMEOUT);
        }

        private Texture2D CreateQRCode(ISigningKey signingKey)
        {
            var guid = System.Guid.NewGuid().ToString().RemoveAllDash();

            var data = Data.GetDefaultData(signingKey, guid);

            var qrData = $"{JsonConvert.SerializeObject(data)}";
            var qrCodeAsTexture2D = _qrCodeGenerator.GenerateQRCode(qrData, 280, 280);
            return qrCodeAsTexture2D;
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