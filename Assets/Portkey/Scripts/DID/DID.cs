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

        public void Start()
        {
#if UNITY_WEBGL

            if (Application.absoluteURL.Contains("access_token="))
            {
                SignIn();
            }

#endif
        }

        public void GoogleSignIn()
        {
            ISocialLogin google = new GoogleLogin(_config, _request);
            google.Authenticate(AuthCallback, ErrorCallback);
        }

        private void AuthCallback(SocialLoginInfo info)
        {
            Debugger.Log($"User: {info.socialInfo.name}\nAEmail: {info.socialInfo.email}\nAccess Code: ${info.access_token}");
            _test.text =
                $"User: {info.socialInfo.name}\nAEmail: {info.socialInfo.email}\nAccess Code: ${info.access_token}";
        }
        
        private void ErrorCallback(string error)
        {
            _test.text = error;
        }
    }
}