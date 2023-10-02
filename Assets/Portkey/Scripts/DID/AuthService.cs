using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Portkey.Core;
using Portkey.SocialProvider;
using Portkey.Utilities;
using UnityEngine;

namespace Portkey.DID
{
    public class AuthService : IAuthService
    {
        private string DEFAULT_CHAIN_ID = "AELF";
        
        private readonly IPortkeySocialService _portkeySocialService;
        private readonly DIDAccount _did;
        private readonly ISocialVerifierProvider _socialVerifierProvider;
        private readonly IVerifierService _verifierService;
        private readonly PortkeyConfig _config;

        public AppleCredentialProvider AppleCredentialProvider { get; private set; }
        public GoogleCredentialProvider GoogleCredentialProvider { get; private set; }
        public PhoneCredentialProvider PhoneCredentialProvider { get; private set; }
        public EmailCredentialProvider EmailCredentialProvider { get; private set; }
        public IAuthMessage Message { get; private set; }
        private EmailLogin Email { get; set; }
        private PhoneLogin Phone { get; set; }

        public AuthService(IPortkeySocialService portkeySocialService, DIDAccount did, ISocialProvider socialLoginProvider, ISocialVerifierProvider socialVerifierProvider, PortkeyConfig config)
        {
            _portkeySocialService = portkeySocialService;
            _did = did;
            _socialVerifierProvider = socialVerifierProvider;
            _config = config;
            
            _verifierService = new VerifierService(_did, _portkeySocialService);
            Message = new AuthMessage();
            Email = new EmailLogin(_portkeySocialService);
            Phone = new PhoneLogin(_portkeySocialService);
            AppleCredentialProvider = new AppleCredentialProvider(socialLoginProvider, Message);
            GoogleCredentialProvider = new GoogleCredentialProvider(socialLoginProvider, Message);
            PhoneCredentialProvider = new PhoneCredentialProvider(Phone, Message, _verifierService);
            EmailCredentialProvider = new EmailCredentialProvider(Email, Message, _verifierService);

            Message.ChainId = DEFAULT_CHAIN_ID;
            Message.OnCancelLoginWithQRCodeEvent += _did.CancelLoginWithQRCode;
        }

        private IEnumerator GetGuardians(string guardianId, SuccessCallback<List<Guardian>> successCallback, ErrorCallback errorCallback)
        {
            if (string.IsNullOrEmpty(guardianId))
            {
                throw new ArgumentException("Identifier cannot be null or empty", nameof(guardianId));
            }
            
            var param = new GetRegisterInfoParams
            {
                loginGuardianIdentifier = guardianId
            };
            yield return _portkeySocialService.GetRegisterInfo(param, (result) =>
            {
                var chainId = result.originChainId;
                
                var getHolderInfoParam = new GetHolderInfoParams
                {
                    guardianIdentifier = guardianId,
                    chainId = chainId
                };
                StaticCoroutine.StartCoroutine(_did.GetHolderInfo(getHolderInfoParam, (holderInfo) =>
                {
                    // If there is no guardian, the guardian list will be empty
                    if (holderInfo == null || holderInfo.guardianList == null ||
                        holderInfo.guardianList.guardians == null)
                    {
                        ReturnNoGuardian();
                        return;
                    }

                    StaticCoroutine.StartCoroutine(_verifierService.Initialize(chainId, b =>
                    {
                        var guardianDtos = (holderInfo.guardianList == null)
                            ? Array.Empty<GuardianDto>()
                            : holderInfo.guardianList.guardians;
                    
                        var guardians = new List<Guardian>();
                        foreach (var guardianDto in guardianDtos)
                        {
                            var verifier = _verifierService.GetVerifier(chainId, guardianDto.verifierId);
                            if (verifier == null)
                            {
                                errorCallback($"Verifier {guardianDto.verifierId} not found");
                                return;
                            }

                            var guardian = CreateGuardian(guardianId, guardianDto, chainId, verifier);
                            if (guardian.isLoginGuardian)
                            {
                                guardians.Insert(0, guardian);
                            }
                            else
                            {
                                guardians.Add(guardian);
                            }
                        }
                        
                        successCallback(guardians);
                    }, errorCallback));
                }, errorCallback));
            }, (error) =>
            {
                if (error.Contains(IPortkeySocialService.UNREGISTERED_CODE))
                {
                    ReturnNoGuardian();
                }
                else
                {
                    errorCallback(error);
                }
            });

            void ReturnNoGuardian()
            {
                successCallback(new List<Guardian>());
            }
        }

        private static Guardian CreateGuardian(string guardianId, GuardianDto guardianDto, string chainId, VerifierItem verifier)
        {
            return new Guardian
            {
                accountType = guardianDto.type,
                id = guardianDto.guardianIdentifier,
                idHash = guardianDto.identifierHash,
                chainId = chainId,
                isLoginGuardian = guardianId == guardianDto.guardianIdentifier,
                verifier = new Verifier
                {
                    id = verifier.id,
                    name = verifier.name
                },
                details = new SocialDetails
                {
                    thirdPartyEmail = guardianDto.thirdPartyEmail,
                    isPrivate = guardianDto.isPrivate,
                    firstName = guardianDto.firstName,
                    lastName = guardianDto.lastName
                }
            };
        }

        public int GetRequiredApprovedGuardiansCount(int totalGuardians)
        {
            if (totalGuardians <= _config.MinApprovals)
            {
                return totalGuardians;
            }
            return (int) (_config.MinApprovals * totalGuardians / (float)_config.Denominator + 1);
        }

        public IEnumerator GetGuardians(ICredential credential, SuccessCallback<List<Guardian>> successCallback)
        {
            yield return GetGuardians(credential.SocialInfo.sub, successCallback, OnError);
        }

        public IEnumerator GetGuardians(PhoneNumber phoneNumber, SuccessCallback<List<Guardian>> successCallback)
        {
            yield return GetGuardians(phoneNumber.String, successCallback, OnError);
        }

        public IEnumerator GetGuardians(EmailAddress emailAddress, SuccessCallback<List<Guardian>> successCallback)
        {
            yield return GetGuardians(emailAddress.String, successCallback, OnError);
        }

        public IEnumerator Verify(Guardian guardian, SuccessCallback<ApprovedGuardian> successCallback, ICredential credential = null)
        {
            // 1. Create ICredential based on guardian's accountType
            // 2. Get the token verifier based on ICredential's accountType
            // 3. Verify the token
            // 4. If the token is valid, construct and return ApprovedGuardian

            if (credential != null)
            {
                if (!IsCredentialMatchGuardian(credential, guardian))
                {
                    OnError("Account does not match your guardian.");
                    yield break;
                }

                switch (guardian.accountType)
                {
                    case AccountType.Apple or AccountType.Google:
                        SocialVerifyAndApproveGuardian(credential, guardian);
                        break;
                    case AccountType.Email:
                        EmailVerifyAndApproveGuardian((EmailCredential)credential, guardian);
                        break;
                    case AccountType.Phone:
                        PhoneVerifyAndApproveGuardian((PhoneCredential)credential, guardian);
                        break;
                    default:
                        OnError($"Unsupported account type: {guardian.accountType}.");
                        break;
                }
                yield break;
            }
            
            if (guardian.accountType == AccountType.Apple)
            {
                AppleCredentialProvider.Get(appleCredential =>
                {
                    if (!IsCredentialMatchGuardian(appleCredential, guardian))
                    {
                        OnError("Account does not match your guardian.");
                        return;
                    }

                    SocialVerifyAndApproveGuardian(appleCredential, guardian);
                });
            }
            else if (guardian.accountType == AccountType.Google)
            {
                GoogleCredentialProvider.Get(googleCredential =>
                {
                    if (!IsCredentialMatchGuardian(googleCredential, guardian))
                    {
                        OnError("Account does not match your guardian.");
                        return;
                    }

                    SocialVerifyAndApproveGuardian(googleCredential, guardian);
                });
            }
            else if (guardian.accountType == AccountType.Email)
            {
                yield return EmailCredentialProvider.Get(EmailAddress.Parse(guardian.id), emailCredential =>
                {
                    if (!IsCredentialMatchGuardian(emailCredential, guardian))
                    {
                        OnError("Account does not match your guardian.");
                        return;
                    }
                    
                    EmailVerifyAndApproveGuardian(emailCredential, guardian);
                }, guardian.chainId, guardian.verifier.id, OperationTypeEnum.communityRecovery);
            }
            else if (guardian.accountType == AccountType.Phone)
            {
                yield return PhoneCredentialProvider.Get(PhoneNumber.Parse(guardian.id), phoneCredential =>
                {
                    if (!IsCredentialMatchGuardian(phoneCredential, guardian))
                    {
                        OnError("Account does not match your guardian.");
                        return;
                    }
                    
                    PhoneVerifyAndApproveGuardian(phoneCredential, guardian);
                }, guardian.chainId, guardian.verifier.id, OperationTypeEnum.communityRecovery);
            }
            else
            {
                OnError($"Unsupported account type: {guardian.accountType}");
            }

            void EmailVerifyAndApproveGuardian(EmailCredential cred, Guardian guard)
            {
                if (!IsMatchingVerifier(cred, guard))
                {
                    OnError("Credential is verified differently from Guardian's definition.");
                    return;
                }
                VerifyEmailCredential(cred, verifiedCredential =>
                {
                    ReturnApprovedGuardian(guard, verifiedCredential);
                });
            }
            
            void PhoneVerifyAndApproveGuardian(PhoneCredential cred, Guardian guard)
            {
                if (!IsMatchingVerifier(cred, guard))
                {
                    OnError("Credential is verified differently from Guardian's definition.");
                    return;
                }
                VerifyPhoneCredential(cred, verifiedCredential =>
                {
                    ReturnApprovedGuardian(guard, verifiedCredential);
                });
            }
            
            void SocialVerifyAndApproveGuardian(ICredential cred, Guardian guard)
            {
                VerifySocialCredential(cred, guard.chainId, guard.verifier.id, OperationTypeEnum.communityRecovery, verifiedCredential =>
                {
                    ReturnApprovedGuardian(guard, verifiedCredential);
                });
            }
            
            void ReturnApprovedGuardian(Guardian guard, VerifiedCredential verifiedCredential)
            {
                var approvedGuardian = CreateApprovedGuardian(guard, verifiedCredential);
                successCallback(approvedGuardian);
            }

            bool IsCredentialMatchGuardian(ICredential cred, Guardian guard)
            {
                return cred.SocialInfo.sub == guard.id && cred.AccountType == guard.accountType;
            }

            bool IsMatchingVerifier(ICodeCredential cred, Guardian guard)
            {
                return cred.ChainId == guard.chainId && cred.VerifierId == guard.verifier.id;
            }
        }

        private void VerifyPhoneCredential(PhoneCredential credential, SuccessCallback<VerifiedCredential> successCallback)
        {
            StaticCoroutine.StartCoroutine(PhoneCredentialProvider.Verify(credential, successCallback));
        }
        
        private void VerifyEmailCredential(EmailCredential credential, SuccessCallback<VerifiedCredential> successCallback)
        {
            StaticCoroutine.StartCoroutine(EmailCredentialProvider.Verify(credential, successCallback));
        }

        private void VerifySocialCredential(ICredential credential, string chainId, string verifierId, OperationTypeEnum operationType, SuccessCallback<VerifiedCredential> successCallback)
        {
            var socialVerifier = _socialVerifierProvider.GetSocialVerifier(credential.AccountType);
            var param = new VerifyAccessTokenParam
            {
                verifierId = verifierId,
                accessToken = credential.SignInToken,
                chainId = chainId,
                operationType = (int)operationType
            };
            socialVerifier.AuthenticateIfAccessTokenExpired(param, (result, token) =>
            {
                successCallback?.Invoke(new VerifiedCredential(credential, chainId, result.verificationDoc, result.signature));
            }, OnError);
        }

        private static ApprovedGuardian CreateApprovedGuardian(Guardian guardian, VerifiedCredential verifiedCredential)
        {
            return new ApprovedGuardian
            {
                type = guardian.accountType,
                verifierId = guardian.verifier.id,
                identifier = guardian.id,
                verificationDoc = verifiedCredential.VerificationDoc.toString,
                signature = verifiedCredential.Signature
            };
        }

        public IEnumerator SignUp(PhoneNumber phoneNumber, SuccessCallback<DIDWalletInfo> successCallback)
        {
            yield return PhoneCredentialProvider.Get(phoneNumber, phoneCredential =>
            {
                VerifyPhoneCredential(phoneCredential, verifiedCredential =>
                {
                    StaticCoroutine.StartCoroutine(SignUp(verifiedCredential, successCallback));
                });
            }, Message.ChainId);
        }

        public IEnumerator SignUp(EmailAddress emailAddress, SuccessCallback<DIDWalletInfo> successCallback)
        {
            yield return EmailCredentialProvider.Get(emailAddress, emailCredential =>
            {
                VerifyEmailCredential(emailCredential, verifiedCredential =>
                {
                    StaticCoroutine.StartCoroutine(SignUp(verifiedCredential, successCallback));
                });
            }, Message.ChainId);
        }
        
        public IEnumerator SignUp(VerifiedCredential verifiedCredential, SuccessCallback<DIDWalletInfo> successCallback)
        {
            _did.Reset();
            
            var extraData = "";
            try
            {
                extraData = EncodeExtraData(DeviceInfoType.GetDeviceInfo());
            }
            catch (Exception e)
            {
                OnError(e.Message);
                yield break;
            }

            var param = new RegisterParams
            {
                type = verifiedCredential.AccountType,
                loginGuardianIdentifier = verifiedCredential.SocialInfo.sub.RemoveAllWhiteSpaces(),
                chainId = verifiedCredential.ChainId,
                verifierId = verifiedCredential.VerificationDoc.verifierId,
                extraData = extraData,
                verificationDoc = verifiedCredential.VerificationDoc.toString,
                signature = verifiedCredential.Signature
            };
            yield return _did.Register(param, registerResult =>
            {
                if (IsCAValid(registerResult.Status))
                {
                    OnError("Register failed! Missing caAddress or caHash.");
                    return;
                }

                var walletInfo = new DIDWalletInfo(verifiedCredential.ChainId, param.loginGuardianIdentifier, param.type, registerResult.Status,
                    registerResult.SessionId, AddManagerType.Register, _did.GetManagementSigningKey());
                successCallback(walletInfo);
            }, OnError);
        }

        public IEnumerator SignUp(ICredential credential, SuccessCallback<DIDWalletInfo> successCallback)
        {
            switch (credential.AccountType)
            {
                case AccountType.Email:
                    var emailCredential = (EmailCredential)credential;
                    VerifyEmailCredential(emailCredential, verifiedCredential =>
                    {
                        StaticCoroutine.StartCoroutine(SignUp(verifiedCredential, successCallback));
                    });
                    break;
                case AccountType.Phone:
                    var phoneCredential = (PhoneCredential)credential;
                    VerifyPhoneCredential(phoneCredential, verifiedCredential =>
                    {
                        StaticCoroutine.StartCoroutine(SignUp(verifiedCredential, successCallback));
                    });
                    break;
                case AccountType.Apple or AccountType.Google:
                    var chainId = Message.ChainId;
                    Message.Loading(true, "Assigning a verifier on-chain...");
                    yield return _verifierService.GetVerifierServer(chainId, verifierServer =>
                    {
                        VerifySocialCredential(credential, chainId, verifierServer.id, OperationTypeEnum.register, verifiedCredential =>
                        {
                            Message.Loading(true, "Creating address on the chain...");
                            StaticCoroutine.StartCoroutine(SignUp(verifiedCredential, successCallback));
                        });
                    }, OnError);
                    break;
                default:
                    throw new ArgumentException($"Credential holds invalid account type {credential.AccountType}!");
            }
        }

        public IEnumerator Login(Guardian loginGuardian, List<ApprovedGuardian> approvedGuardians, SuccessCallback<DIDWalletInfo> successCallback)
        {
            _did.Reset();
            
            var extraData = "";
            try
            {
                extraData = EncodeExtraData(DeviceInfoType.GetDeviceInfo());
            }
            catch (Exception e)
            {
                OnError(e.Message);
                yield break;
            }
            
            var param = new AccountLoginParams
            {
                loginGuardianIdentifier = loginGuardian.id.RemoveAllWhiteSpaces(),
                guardiansApprovedList = approvedGuardians.ToArray(),
                chainId = loginGuardian.chainId,
                extraData = extraData
            };
            StaticCoroutine.StartCoroutine(_did.Login(param, result =>
            {
                if(IsCAValid(result.Status))
                {
                    OnError("Login failed! Missing caAddress or caHash.");
                    return;
                }
                
                var walletInfo = new DIDWalletInfo(loginGuardian.chainId, loginGuardian.id, loginGuardian.accountType, result.Status, result.SessionId, AddManagerType.Recovery, _did.GetManagementSigningKey());
                successCallback(walletInfo);
            }, OnError));
        }

        public IEnumerator LoginWithPortkeyApp(SuccessCallback<DIDWalletInfo> successCallback)
        {
            yield return _did.LoginWithPortkeyApp(Message.ChainId, PortkeyAppLoginSuccess(successCallback), OnError);
        }

        public IEnumerator LoginWithQRCode(SuccessCallback<Texture2D> qrCodeCallback, SuccessCallback<DIDWalletInfo> successCallback)
        {
            yield return _did.LoginWithQRCode(Message.ChainId, qrCodeCallback, PortkeyAppLoginSuccess(successCallback), OnError);
        }
        
        private static SuccessCallback<PortkeyAppLoginResult> PortkeyAppLoginSuccess(SuccessCallback<DIDWalletInfo> successCallback)
        {
            return result =>
            {
                successCallback(new DIDWalletInfo(result.caHolder, result.managementAccount));
            };
        }

        public IEnumerator Logout(SuccessCallback<bool> successCallback)
        {
            var param = new EditManagerParams
            {
                chainId = Message.ChainId
            };
            yield return _did.Logout(param, successCallback, OnError);
        }
        
        private static bool IsCAValid(CAInfo caInfo)
        {
            return caInfo.caAddress == null || caInfo.caHash == null;
        }
        
        private static string EncodeExtraData(DeviceInfoType deviceInfo)
        {
            var extraData = new ExtraData
            {
                transactionTime = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds().ToString(),
                deviceInfo = JsonConvert.SerializeObject(deviceInfo)
            };
            return JsonConvert.SerializeObject(extraData);
        }

        private void OnError(string error)
        {
            Message.Error(error);
        }
    }
}