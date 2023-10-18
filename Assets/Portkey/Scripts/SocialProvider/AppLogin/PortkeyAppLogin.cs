using System.Collections;
using Newtonsoft.Json;
using Portkey.Core;
using Portkey.Utilities;
using UnityEngine;
using UnityEngine.Networking;

namespace Portkey.SocialProvider
{
    public class PortkeyAppLogin : IAppLogin
    {
        private readonly ISigningKeyGenerator _signingKeyGenerator;
        private readonly ILoginPoller _loginPoller;
        private readonly TransportConfig _config;
        
        private LoginPollerHandler _loginPollerHandler = null;
        
        public PortkeyAppLogin(TransportConfig config, ISigningKeyGenerator signingKeyGenerator, ILoginPoller loginPoller)
        {
            _config = config;
            _signingKeyGenerator = signingKeyGenerator;
            _loginPoller = loginPoller;
        }
        
        public IEnumerator Login(SuccessCallback<PortkeyAppLoginResult> successCallback, ErrorCallback errorCallback)
        {
            var signingKey = _signingKeyGenerator.Create();
            var guid = System.Guid.NewGuid().ToString().RemoveAllDash();

            var data = Data.GetDefaultData(signingKey, guid);
        
            var appData = new AppData
            {
                websiteName = Application.productName,
                websiteIcon = _config.IconURL
            };
        
            var dataParam = $"data={UnityWebRequest.EscapeURL(JsonConvert.SerializeObject(data))}";
            var extraDataParam = $"extraData={UnityWebRequest.EscapeURL(JsonConvert.SerializeObject(appData))}";

            var uri = $"portkey.sdk/login?{dataParam}&{extraDataParam}";

            _config.Send(uri);

            _loginPollerHandler = _loginPoller.Start(signingKey, result =>
            {
                _loginPollerHandler = null;
                successCallback(result);
            }, errorCallback);
            
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