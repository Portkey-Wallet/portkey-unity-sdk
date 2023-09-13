using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Portkey.Core;
using Portkey.SocialProvider;
using Portkey.Utilities;

namespace Portkey.DID
{
    public class AuthService : IAuthService
    {
        private IPortkeySocialService _portkeySocialService;
        private DIDWallet<WalletAccount> _did;
        private ISocialProvider _socialLoginProvider;
        private ISocialVerifierProvider _socialVerifierProvider;
        private VerifierService _verifierService;
        private string _verificationCode = null;

        public AppleCredentialProvider AppleCredentialProvider { get; private set; }
        public GoogleCredentialProvider GoogleCredentialProvider { get; private set; }
        public PhoneCredentialProvider PhoneCredentialProvider { get; private set; }
        public EmailCredentialProvider EmailCredentialProvider { get; private set; }
        public EmailLogin Email { get; private set; }
        public PhoneLogin Phone { get; private set; }
        public ErrorCallback OnError { get; set; } = null;
        public Action OnSendVerificationCode { get; set; } = null;

        public AuthService(IPortkeySocialService portkeySocialService, DIDWallet<WalletAccount> did, ISocialProvider socialLoginProvider, ISocialVerifierProvider socialVerifierProvider)
        {
            _portkeySocialService = portkeySocialService;
            _did = did;
            _socialLoginProvider = socialLoginProvider;
            _socialVerifierProvider = socialVerifierProvider;
            
            _verifierService = new VerifierService(_did, _portkeySocialService);
            AppleCredentialProvider = new AppleCredentialProvider(_socialLoginProvider);
            GoogleCredentialProvider = new GoogleCredentialProvider(_socialLoginProvider);
            PhoneCredentialProvider = new PhoneCredentialProvider();
            EmailCredentialProvider = new EmailCredentialProvider();

            Email = new EmailLogin(_portkeySocialService);
            Phone = new PhoneLogin(_portkeySocialService);
            
            OnError = ErrorCallback;
        }

        private IEnumerator GetGuardians(string guardianId, SuccessCallback<List<GuardianNew>> successCallback, ErrorCallback errorCallback)
        {
            if (string.IsNullOrEmpty(guardianId))
            {
                throw new System.ArgumentException("Identifier cannot be null or empty", nameof(guardianId));
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
                            ? Array.Empty<Guardian>()
                            : holderInfo.guardianList.guardians;
                    
                        var guardians = new List<GuardianNew>();
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
                successCallback(new List<GuardianNew>());
            }
        }
        
        private void ErrorCallback(string error)
        {
            Debugger.LogError(error);
        }

        private static GuardianNew CreateGuardian(string guardianId, Guardian guardianDto, string chainId, VerifierItem verifier)
        {
            return new GuardianNew
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
                }
            };
        }

        public IEnumerator GetGuardians(ICredential credential, SuccessCallback<List<GuardianNew>> successCallback)
        {
            yield return GetGuardians(credential.SocialInfo.sub, successCallback, OnError);
        }

        public IEnumerator GetGuardians(PhoneNumber phoneNumber, SuccessCallback<List<GuardianNew>> successCallback)
        {
            yield return GetGuardians(phoneNumber.ToString, successCallback, OnError);
        }

        public IEnumerator GetGuardians(EmailAddress emailAddress, SuccessCallback<List<GuardianNew>> successCallback)
        {
            yield return GetGuardians(emailAddress.ToString, successCallback, OnError);
        }

        public void Verify(GuardianNew guardian, SuccessCallback<ApprovedGuardian> successCallback, ICredential credential = null)
        {
            // 1. Create ICredential based on guardian's accountType
            // 2. Get the token verifier based on ICredential's accountType
            // 3. Verify the token
            // 4. If the token is valid, construct and return ApprovedGuardian

            if (credential != null)
            {
                if (credential.SocialInfo.sub != guardian.id)
                {
                    OnError?.Invoke("Account does not match your guardian.");
                }
                
                VerifySocialCredential(credential, guardian, successCallback);
                return;
            }
            
            if (guardian.accountType == AccountType.Apple)
            {
                AppleCredentialProvider.Get(appleCredential =>
                {
                    if (appleCredential.SocialInfo.sub != guardian.id)
                    {
                        OnError?.Invoke("Account does not match your guardian.");
                    }

                    VerifySocialCredential(appleCredential, guardian, successCallback);
                }, OnError);
            }
            else if (guardian.accountType == AccountType.Google)
            {
                GoogleCredentialProvider.Get(googleCredential =>
                {
                    if (googleCredential.SocialInfo.sub != guardian.id)
                    {
                        OnError?.Invoke("Account does not match your guardian.");
                    }

                    VerifySocialCredential(googleCredential, guardian, successCallback);
                }, OnError);
            }
            else if (guardian.accountType == AccountType.Email)
            {
                var param = new SendCodeParams
                {
                    guardianId = guardian.id,
                    verifierId = guardian.verifier.id,
                    chainId = guardian.chainId,
                    operationType = OperationTypeEnum.communityRecovery
                };
                SendCodeForEmailAndWaitForInput(param, emailCredential =>
                {
                    VerifyEmailCredential(emailCredential, guardian, successCallback);
                });
            }
            else if (guardian.accountType == AccountType.Phone)
            {
                var param = new SendCodeParams
                {
                    guardianId = guardian.id,
                    verifierId = guardian.verifier.id,
                    chainId = guardian.chainId,
                    operationType = OperationTypeEnum.communityRecovery
                };
                SendCodeForPhoneAndWaitForInput(param, phoneCredential =>
                {
                    VerifyPhoneCredential(phoneCredential, guardian, successCallback);
                });
            }
            else
            {
                OnError?.Invoke($"Unsupported account type: {guardian.accountType}");
            }
        }

        private void SendCodeForEmailAndWaitForInput(SendCodeParams param, SuccessCallback<EmailCredential> successCallback)
        {
            StaticCoroutine.StartCoroutine(Email.SendCode(param, ret =>
            {
                OnSendVerificationCode?.Invoke();

                StaticCoroutine.StartCoroutine(WaitForInputCode((code) =>
                {
                    var emailCredential = EmailCredentialProvider.Get(EmailAddress.Parse(param.guardianId), code);
                    successCallback(emailCredential);
                }));
            }, OnError));
        }

        private void SendCodeForPhoneAndWaitForInput(SendCodeParams param, SuccessCallback<PhoneCredential> successCallback)
        {
            StaticCoroutine.StartCoroutine(Phone.SendCode(param, ret =>
            {
                OnSendVerificationCode?.Invoke();

                StaticCoroutine.StartCoroutine(WaitForInputCode((code) =>
                {
                    var phoneCredential = PhoneCredentialProvider.Get(PhoneNumber.Parse(param.guardianId), code);
                    successCallback(phoneCredential);
                }));
            }, OnError));
        }

        private IEnumerator WaitForInputCode(Action<string> onComplete)
        {
            while (_verificationCode == null)
            {
                yield return null;
            }
            
            onComplete?.Invoke(_verificationCode);
            _verificationCode = null;
        }

        private void VerifyPhoneCredential(ICredential credential, GuardianNew guardian, SuccessCallback<ApprovedGuardian> successCallback)
        {
            StaticCoroutine.StartCoroutine(Phone.VerifyCode(credential.SignInToken, result =>
            {
                var approvedGuardian = CreateApprovedGuardian(guardian, result);
                successCallback(approvedGuardian);
            }, OnError));
        }
        
        private void VerifyEmailCredential(ICredential credential, GuardianNew guardian, SuccessCallback<ApprovedGuardian> successCallback)
        {
            StaticCoroutine.StartCoroutine(Email.VerifyCode(credential.SignInToken, result =>
            {
                var approvedGuardian = CreateApprovedGuardian(guardian, result);
                successCallback(approvedGuardian);
            }, OnError));
        }

        private void VerifySocialCredential(ICredential credential, GuardianNew guardian, SuccessCallback<ApprovedGuardian> successCallback)
        {
            var socialVerifier = _socialVerifierProvider.GetSocialVerifier(credential.AccountType);
            var param = new VerifyAccessTokenParam
            {
                verifierId = guardian.verifier.id,
                accessToken = credential.SignInToken,
                chainId = guardian.chainId
            };
            socialVerifier.AuthenticateIfAccessTokenExpired(param, (result, token) =>
            {
                var approvedGuardian = CreateApprovedGuardian(guardian, result);
                successCallback(approvedGuardian);
            }, null, OnError);
        }

        private static ApprovedGuardian CreateApprovedGuardian(GuardianNew guardian, VerifyCodeResult result)
        {
            return new ApprovedGuardian
            {
                type = guardian.accountType,
                verifierId = guardian.verifier.id,
                identifier = guardian.id,
                verificationDoc = result.verificationDoc.toString,
                signature = result.signature
            };
        }

        public IEnumerator Signup(string chainId, PhoneNumber phoneNumber, SuccessCallback<DIDWalletInfo> successCallback)
        {
            yield return Signup(chainId, phoneNumber.ToString, AccountType.Phone, successCallback);
        }

        public IEnumerator Signup(string chainId, EmailAddress emailAddress, SuccessCallback<DIDWalletInfo> successCallback)
        {
            var param = new SendCodeParams
            {
                guardianId = emailAddress.ToString,
                verifierId = guardian.verifier.id,
                chainId = chainId,
                operationType = OperationTypeEnum.register
            };
            SendCodeForEmailAndWaitForInput(param, emailCredential =>
            {
                StaticCoroutine.StartCoroutine(Email.VerifyCode(emailCredential.SignInToken, result =>
                {
                    
                }, OnError));
            });
        }

        public IEnumerator Signup(string chainId, ICredential credential, SuccessCallback<DIDWalletInfo> successCallback)
        {
            yield return Signup(chainId, credential.SocialInfo.sub, credential.AccountType, successCallback);
        }

        public IEnumerator Login(GuardianNew loginGuardian, List<ApprovedGuardian> approvedGuardians, SuccessCallback<DIDWalletInfo> successCallback)
        {
            var extraData = "";
            try
            {
                extraData = EncodeExtraData(DeviceInfoType.GetDeviceInfo());
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
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
                    OnError?.Invoke("Login failed! Missing caAddress or caHash.");
                    return;
                }
                
                var walletInfo = CreateDIDWalletInfo(loginGuardian, result.Status, result.SessionId, AddManagerType.Recovery);
                successCallback(walletInfo);
            }, OnError));
        }
        
        private DIDWalletInfo CreateDIDWalletInfo(GuardianNew guardian, CAInfo caInfo, string sessionId, AddManagerType type)
        {
            var walletInfo = new DIDWalletInfo
            {
                caInfo = caInfo,
                chainId = guardian.chainId,
                wallet = _did.GetWallet(),
                managerInfo = new ManagerInfoType
                {
                    managerUniqueId = sessionId,
                    guardianIdentifier = guardian.id,
                    accountType = guardian.accountType,
                    type = type
                }
            };
            return walletInfo;
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

        public void SendVerificationCode(string verificationCode)
        {
            _verificationCode = verificationCode;
        }
    }
}