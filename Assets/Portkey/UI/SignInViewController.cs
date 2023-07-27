using Portkey.Utilities;
using Portkey.Core;
using TMPro;
using UnityEngine;

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
        [SerializeField] private GuardiansApprovalView guardianApprovalView;
        [SerializeField] private ErrorViewController errorView;
        
        private State _state = State.Login;
        private IPortkeySocialService _portkeySocialService;
        
        public void Start()
        {
            _portkeySocialService = did.PortkeySocialService;

#if UNITY_WEBGL

            if (Application.absoluteURL.Contains("access_token="))
            {
                SignIn();
            }

#endif
        }
        
        public void SignIn(int type)
        {
            var accountType = (AccountType)type;
            var socialLogin = did.GetSocialLogin(accountType);
            socialLogin.Authenticate(AuthCallback, OnError);
        }

        private void AuthCallback(SocialLoginInfo info)
        {
            Debugger.Log(
                $"User: {info.socialInfo.name}\nAEmail: {info.socialInfo.email}\nAccess Code: ${info.access_token}");

            ValidateIdentifier(info);
        }

        private void OnError(string error)
        {
            Debugger.LogError(error);
            errorView.ShowErrorText(error);
        }

        private void ValidateIdentifier(SocialLoginInfo info)
        {
            var identifier = info.socialInfo.sub;
            var accountType = info.accountType;
            var token = info.access_token;

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
                var getHolderInfoParam = new GetHolderInfoParams
                {
                    guardianIdentifier = identifier,
                    chainId = result.originChainId
                };
                StartCoroutine(did.GetHolderInfo(getHolderInfoParam, (holderInfo) =>
                {
                    if (holderInfo == null || holderInfo.guardianList == null ||
                        holderInfo.guardianList.guardians == null)
                    {
                        CheckChainInfo(identifier, accountType, token, false);
                        return;
                    }
                    
                    var isLoginGuardian = holderInfo.guardianList.guardians.Length > 0;
                    CheckChainInfo(identifier, accountType, token, isLoginGuardian);
                }, (error) =>
                { 
                    CheckChainInfo(identifier, accountType, token, false);
                }));
            }, (error) =>
            {
                if (error.Contains("3002"))
                {
                    CheckChainInfo(identifier, accountType, token, false);
                }
                else
                {
                    OnError(error);
                }
            }));
        }

        private void CheckChainInfo(string identifier, AccountType accountType, string token, bool isLoginGuardian)
        {
            var input = new GuardianIdentifierInfo
            {
                identifier = identifier,
                accountType = accountType,
                token = token,
                isLoginGuardian = isLoginGuardian,
                chainId = "AELF"
            };

            CheckChainInfo(input);
        }
        
        private void CheckChainInfo(GuardianIdentifierInfo guardianInfo)
        {
            var registerParam = new GetRegisterInfoParams
            {
                loginGuardianIdentifier = guardianInfo.identifier.RemoveAllWhiteSpaces()
            };
            StartCoroutine(_portkeySocialService.GetRegisterInfo(registerParam, info =>
            {
                guardianInfo.chainId = info.originChainId;
                //TODO: change chain UI if needed
                CheckSignUpOrLogin(guardianInfo);
            }, (error) =>
            {
                CheckSignUpOrLogin(guardianInfo);
            }));
        }

        private void CheckSignUpOrLogin(GuardianIdentifierInfo info)
        {
            switch (info.isLoginGuardian)
            {
                case true when _state != State.Login:
                case false when _state != State.Signup:
                    unregisteredView.gameObject.SetActive(true);
                    unregisteredView.SetGuardianIdentifierInfo(info);
                    break;
                default:
                    //Change to Login View
                    gameObject.SetActive(false);
                    OpenGuardiansApprovalView(info);
                    break;
            }
        }

        private void OpenGuardiansApprovalView(GuardianIdentifierInfo info)
        {
            guardianApprovalView.SetGuardianIdentifierInfo(info);
            guardianApprovalView.gameObject.SetActive(true);
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
