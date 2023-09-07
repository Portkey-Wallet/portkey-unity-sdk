using System;
using Portkey.Utilities;
using Portkey.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Portkey.UI
{
    public class SignInViewController : MonoBehaviour
    {
        private enum State
        {
            Login,
            Signup
        }
        
        [SerializeField] private DID.DID did;
        [SerializeField] private UnregisteredViewController unregisteredView;
        [FormerlySerializedAs("guardianApprovalView")] [SerializeField] private GuardiansApprovalViewController guardianApprovalViewController;
        [SerializeField] private ErrorViewController errorView;
        [SerializeField] private LoadingViewController loadingView;
        [SerializeField] private EmailLoginViewController emailLoginViewController;
        [SerializeField] private PhoneLoginViewController phoneLoginViewController;
        
        private State _state = State.Login;
        private IPortkeySocialService _portkeySocialService;
        
        public void Start()
        {
            _portkeySocialService = did.PortkeySocialService;

#if UNITY_WEBGL

            if (Application.absoluteURL.Contains("access_token="))
            {
                SignIn((int)AccountType.Google);
            }

#endif
        }
        
        public void SignIn(int type)
        {
            var accountType = (AccountType)type;

            switch (accountType)
            {
                case AccountType.Apple:
                case AccountType.Google:
                    var socialLogin = did.GetSocialLogin(accountType);
                    socialLogin.Authenticate(AuthCallback, OnStartLoading, OnError);
                    break;
                case AccountType.Email:
                    emailLoginViewController.DID = did;
                    emailLoginViewController.PreviousView = gameObject;
                    emailLoginViewController.gameObject.SetActive(true);
                    break;
                case AccountType.Phone:
                    //TODO: open up window to key in phone number or email then call ValidateIdentifier
                    phoneLoginViewController.PreviousView = gameObject;
                    phoneLoginViewController.gameObject.SetActive(true);
                    break;
                default:
                    throw new ArgumentException("Not expected account type!");
            }
        }

        private void OnStartLoading(bool show)
        {
            ShowLoading(show, "Checking account on the chain...");
        }

        private void ShowLoading(bool show, string text = "")
        {
            loadingView.DisplayLoading(show, text);
        }

        private void AuthCallback(SocialLoginInfo info)
        {
            Debugger.Log(
                $"User: {info.socialInfo.name}\nAEmail: {info.socialInfo.email}\nAccess Code: ${info.access_token}");
            
            did.AuthService.HasGuardian(info.socialInfo.sub, info.accountType, info.access_token, CheckSignUpOrLogin, OnError);
        }

        private void OnError(string error)
        {
            Debugger.LogError(error);
            ShowLoading(false);
            errorView.ShowErrorText(error);
        }

        private void CheckSignUpOrLogin(GuardianIdentifierInfo info)
        {
            ShowLoading(false);
            
            switch (info.isLoginGuardian)
            {
                case true when _state != State.Login:
                case false when _state != State.Signup:
                    unregisteredView.gameObject.SetActive(true);
                    unregisteredView.SetGuardianIdentifierInfo(info);
                    break;
                default:
                    //Change to Login View
                    PrepareGuardiansApprovalView(info);
                    break;
            }
        }

        private void PrepareGuardiansApprovalView(GuardianIdentifierInfo info)
        {
            guardianApprovalViewController.SetGuardianIdentifierInfo(info);
            guardianApprovalViewController.InitializeData(OpenGuardiansApprovalView, OnError);
        }
        
        private void OpenGuardiansApprovalView()
        {
            guardianApprovalViewController.gameObject.SetActive(true);
            CloseView();
        }

        public void OnClickClose()
        {
            CloseView();
        }
        
        private void CloseView()
        {
            gameObject.SetActive(false);
        }
    }
}
