using Portkey.Core;
using Portkey.SocialProvider;
using Portkey.Utilities;
using UnityEngine;

namespace Portkey.DID
{
    public class AuthService : IAuthService
    {
        private IPortkeySocialService _portkeySocialService;
        private DIDWallet<WalletAccount> _did;
        
        public EmailLogin Email { get; private set; }
        public PhoneLogin Phone { get; private set; }
        
        public AuthService(IPortkeySocialService portkeySocialService, DIDWallet<WalletAccount> did)
        {
            _portkeySocialService = portkeySocialService;
            _did = did;

            Email = new EmailLogin(_portkeySocialService);
            Phone = new PhoneLogin(_portkeySocialService);
        }

        public void HasGuardian(string identifier, AccountType accountType, string token, SuccessCallback<GuardianIdentifierInfo> successCallback, ErrorCallback errorCallback)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new System.ArgumentException("Identifier cannot be null or empty", nameof(identifier));
            }

            var param = new GetRegisterInfoParams
            {
                loginGuardianIdentifier = identifier
            };
            StaticCoroutine.StartCoroutine(_portkeySocialService.GetRegisterInfo(param, (result) =>
            {
                var getHolderInfoParam = new GetHolderInfoParams
                {
                    guardianIdentifier = identifier,
                    chainId = result.originChainId
                };
                StaticCoroutine.StartCoroutine(_did.GetHolderInfo(getHolderInfoParam, (holderInfo) =>
                {
                    if (holderInfo == null || holderInfo.guardianList == null ||
                        holderInfo.guardianList.guardians == null)
                    {
                        CreateGuardianIdentifierInfo(identifier, accountType, token, false);
                        return;
                    }
                    
                    var isLoginGuardian = holderInfo.guardianList.guardians.Length > 0;
                    CreateGuardianIdentifierInfo(identifier, accountType, token, isLoginGuardian);
                }, (error) =>
                { 
                    CreateGuardianIdentifierInfo(identifier, accountType, token, false);
                }));
            }, (error) =>
            {
                if (error.Contains(IPortkeySocialService.UNREGISTERED_CODE))
                {
                    CreateGuardianIdentifierInfo(identifier, accountType, token, false);
                }
                else
                {
                    errorCallback(error);
                }
            }));
            
            void CreateGuardianIdentifierInfo(string identifier, AccountType accountType, string token, bool isLoginGuardian)
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
        
            void CheckChainInfo(GuardianIdentifierInfo guardianInfo)
            {
                var registerParam = new GetRegisterInfoParams
                {
                    loginGuardianIdentifier = guardianInfo.identifier.RemoveAllWhiteSpaces()
                };
                StaticCoroutine.StartCoroutine(_portkeySocialService.GetRegisterInfo(registerParam, info =>
                {
                    guardianInfo.chainId = info.originChainId;
                    //TODO: change chain UI if needed
                    successCallback(guardianInfo);
                }, (error) =>
                {
                    successCallback(guardianInfo);
                }));
            }
        }
    }
}