using System.Text;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using Portkey.Core;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class IOSAppleLogin : AppleLoginBase
    {
        private IAppleAuthManager _appleAuthManager = null;
        private GameObject _updater = null;
        
        public IOSAppleLogin(PortkeyConfig config, IHttp request) : base(request)
        {
            
        }

        protected override void OnAuthenticate()
        {
            _appleAuthManager ??= new AppleAuthManager(new PayloadDeserializer());

            _updater ??= SetupUpdater(_appleAuthManager);
            _updater.SetActive(true);
            
            Authenticate();
        }
        
        private static GameObject SetupUpdater(IAppleAuthManager appleAuthManager)
        {
            var gameObject = new GameObject("IOSAppleLoginUpdater");
            var updaterComponent = gameObject.AddComponent<IOSAppleLoginUpdater>();
            updaterComponent.AppleAuthManager = appleAuthManager;

            return gameObject;
        }
        
        private void Authenticate()
        {
            _startLoadCallback?.Invoke(true);
            
            // Set the login arguments
            var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

            // Perform the login
            _appleAuthManager.LoginWithAppleId(
                loginArgs,
                credential =>
                {
                    var appleIDCredential = credential as IAppleIDCredential;
                    if (appleIDCredential != null)
                    {
                        var idToken = Encoding.UTF8.GetString(
                            appleIDCredential.IdentityToken,
                            0,
                            appleIDCredential.IdentityToken.Length);
                        Debugger.Log("Sign-in with Apple successfully done. IDToken: " + idToken);
                        
                        _updater.SetActive(false);
                        RequestSocialInfo(idToken, null, null);
                    }
                    else
                    {
                        Debugger.LogError("Sign-in with Apple error. Message: appleIDCredential is null");
                        OnError("Retrieving Apple Id Token failed.");
                    }
                },
                error =>
                {
                    Debugger.LogError("Sign-in with Apple error. Message: " + error);
                    OnError("Retrieving Apple Id Token failed.");
                }
            );
        }

        private void OnError(string error)
        {
            _updater.SetActive(false);
            _errorCallback?.Invoke(error);
        }
    }
}