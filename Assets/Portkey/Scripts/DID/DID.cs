using Portkey.Core;
using Portkey.SocialProvider;
using TMPro;
using UnityEngine;

namespace Portkey.DID
{
    public class DID : MonoBehaviour
    {
        [SerializeField] private IHttp _request;
        [SerializeField] private PortkeyConfig _config;
        [SerializeField] private TextMeshProUGUI _test;
        [SerializeField] private GraphQL.GraphQL _graphQL;
        
        private ISocialProvider _socialProvider;
        private IPortkeySocialService _portkeySocialService;

        public void Start()
        {
            _socialProvider = new SocialLoginProvider(_config, _request);
            _portkeySocialService = new PortkeySocialService(_config, _request, _graphQL);
            
#if UNITY_WEBGL

            if (Application.absoluteURL.Contains("access_token="))
            {
                SignIn();
            }

#endif
        }

        public void SignIn(int type)
        {
            var accountType = (AccountType) type;
            var socialLogin = _socialProvider.GetSocialLogin(accountType);
            socialLogin.Authenticate(AuthCallback, ErrorCallback);
        }

        private void AuthCallback(SocialLoginInfo info)
        {
            Debugger.Log($"User: {info.socialInfo.name}\nAEmail: {info.socialInfo.email}\nAccess Code: ${info.access_token}");
            _test.text =
                $"User: {info.socialInfo.name}\nAEmail: {info.socialInfo.email}\nAccess Code: ${info.access_token}";
            
            ValidateIdentifier(info.socialInfo.sub);
        }
        
        private void ErrorCallback(string error)
        {
            _test.text = error;
        }
        
        private void ValidateIdentifier(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new System.ArgumentException("Identifier cannot be null or empty", nameof(identifier));
            }

            var param = new GetRegisterInfoParams
            {
                loginGuardianIdentifier = identifier
            };
            StartCoroutine(_portkeySocialService.GetRegisterInfo(param, (result) =>
            {
                _test.text = result.originChainId;
                
                
            }, ErrorCallback));
        }
    }
}