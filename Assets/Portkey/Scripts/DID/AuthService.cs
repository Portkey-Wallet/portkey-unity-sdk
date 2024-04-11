using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Portkey.Captcha;
using Portkey.Core;
using Portkey.SocialProvider;
using Portkey.Utilities;
using UnityEngine;
using DeviceInfoType = Portkey.Core.DeviceInfoType;

namespace Portkey.DID
{
    public partial class AuthService : IAuthService
    {
        private string DEFAULT_CHAIN_ID = "AELF";
        
        private readonly IPortkeySocialService _portkeySocialService;
        private readonly DIDAccount _did;
        private readonly ISocialVerifierProvider _socialVerifierProvider;
        private readonly IVerifierService _verifierService;
        private readonly PortkeyConfig _config;
        private readonly ICaptchaProvider _captchaProvider;

        public AppleCredentialProvider AppleCredentialProvider { get; private set; }
        public GoogleCredentialProvider GoogleCredentialProvider { get; private set; }
        public TelegramCredentialProvider TelegramCredentialProvider { get; private set; }
        public PhoneCredentialProvider PhoneCredentialProvider { get; private set; }
        public EmailCredentialProvider EmailCredentialProvider { get; private set; }
        public IAuthMessage Message { get; private set; }
        private EmailLogin Email { get; set; }
        private PhoneLogin Phone { get; set; }

        public AuthService(IPortkeySocialService portkeySocialService, DIDAccount did, ISocialProvider socialLoginProvider, ISocialVerifierProvider socialVerifierProvider, PortkeyConfig config, IVerifierService verifierService)
        {
            _portkeySocialService = portkeySocialService;
            _did = did;
            _socialVerifierProvider = socialVerifierProvider;
            _config = config;
            
            _verifierService = verifierService;
            Message = this;
            _captchaProvider = new CaptchaProvider(_config);
            Email = new EmailLogin(_portkeySocialService);
            Phone = new PhoneLogin(_portkeySocialService);
            AppleCredentialProvider = new AppleCredentialProvider(socialLoginProvider, Message, _verifierService, _socialVerifierProvider);
            GoogleCredentialProvider = new GoogleCredentialProvider(socialLoginProvider, Message, _verifierService, _socialVerifierProvider);
            TelegramCredentialProvider = new TelegramCredentialProvider(socialLoginProvider, Message, _verifierService,
                _socialVerifierProvider);
            PhoneCredentialProvider = new PhoneCredentialProvider(Phone, this, _verifierService, _captchaProvider.GetCaptcha());
            EmailCredentialProvider = new EmailCredentialProvider(Email, this, _verifierService, _captchaProvider.GetCaptcha());

            Message.ChainId = DEFAULT_CHAIN_ID;
            OnCancelLoginWithQRCodeEvent += _did.CancelLoginWithQRCode;
            OnCancelLoginWithPortkeyAppEvent += _did.CancelLoginWithPortkeyApp;
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
                            if (guardianId == guardian.id && guardian.isLoginGuardian)
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
                isLoginGuardian = guardianDto.isLoginGuardian,
                verifier = new Verifier(verifier.id, verifier.name),
                details = new SocialDetails
                {
                    thirdPartyEmail = guardianDto.thirdPartyEmail,
                    isPrivate = guardianDto.isPrivate,
                    firstName = guardianDto.firstName,
                    lastName = guardianDto.lastName
                }
            };
        }

        /// <summary> The GetRequiredApprovedGuardiansCount function returns the number of guardians that must be approved to recover the user's Portkey DID Account.        
        /// The function takes in an integer representing the total number of guardians, and returns an integer representing the minimum required approvals.</summary>
        ///
        /// <param name="int totalGuardians"> The total number of guardians binded to the account.
        /// </param>
        ///
        /// <returns> The number of approved guardians required to recover the user's Portkey DID Account.</returns>
        public int GetRequiredApprovedGuardiansCount(int totalGuardians)
        {
            if (totalGuardians <= _config.MinApprovals)
            {
                return totalGuardians;
            }
            return (int) (_config.MinApprovals * totalGuardians / (float)_config.Denominator + 1);
        }

        /// <summary> The GetGuardians function retrieves a list of guardians for the specified user.</summary>        
        ///
        /// <param name="ICredential credential"> /// the credential of the user to get guardians for.
        /// </param>
        /// <param name="SuccessCallback&lt;List&lt;Guardian&gt;&gt; successCallback"> /// this is the callback that will be called when the request has been completed. it returns a list of guardian objects.
        /// </param>
        ///
        /// <returns> A list of guardians.</returns>
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

        /// <summary> The Verify function is used to verify a guardian's identity.        
        /// &lt;para&gt;The function takes in a Guardian object and an optional ICredential object.&lt;/para&gt;
        /// &lt;para&gt;If the ICredential is not provided, the function will attempt to retrieve it from the user.&lt;/para&gt;
        /// &lt;list type=&quot;bullet&quot;&gt;
        ///     &lt;item&gt;&lt;description&gt;&lt;b&gt;(Step 1)&lt;/b&gt;: If an ICredential was provided, check if its accountType matches that of the Guardian&lt;/description&gt;&lt;/item&gt; 
        ///     &lt;item&gt;&lt;description&gt;&lt;b&gt;(Step 2)&lt;/b&gt;: Get token</summary>
        ///
        /// <param name="Guardian guardian"> ///     the guardian to be approved.
        /// </param>
        /// <param name="SuccessCallback&lt;ApprovedGuardian&gt; successCallback"> ///     the callback function that will be called when the guardian is approved.
        /// </param>
        /// <param name="ICredential credential"> ///     if the credential is not null, it will be used to verify the guardian.
        /// </param>
        ///
        /// <returns> A verifiedcredential.</returns>
        public IEnumerator Verify(Guardian guardian, SuccessCallback<ApprovedGuardian> successCallback, ICredential credential = null)
        {
            // 1. Create ICredential based on guardian's accountType
            // 2. Get the token verifier based on ICredential's accountType
            // 3. Verify the token
            // 4. If the token is valid, construct and return ApprovedGuardian

            if (credential != null)
            {
                VerifyGuardian(guardian, credential, successCallback);
                yield break;
            }

            switch (guardian.accountType)
            {
                case AccountType.Apple:
                    AppleCredentialProvider.Get(appleCredential =>
                    {
                        VerifyGuardian(guardian, appleCredential, successCallback);
                    });
                    break;
                case AccountType.Google:
                    GoogleCredentialProvider.Get(googleCredential =>
                    {
                        VerifyGuardian(guardian, googleCredential, successCallback);
                    });
                    break;
                case AccountType.Telegram:
                    TelegramCredentialProvider.Get(telegramCredential =>
                    {
                        VerifyGuardian(guardian, telegramCredential, successCallback);
                    });
                    break;
                case AccountType.Email:
                    yield return EmailCredentialProvider.Get(EmailAddress.Parse(guardian.id), emailCredential =>
                    {
                        VerifyGuardian(guardian, emailCredential, successCallback);
                    }, guardian.chainId, guardian.verifier.id, OperationTypeEnum.communityRecovery);
                    break;
                case AccountType.Phone:
                    yield return PhoneCredentialProvider.Get(PhoneNumber.Parse(guardian.id), phoneCredential =>
                    {
                        VerifyGuardian(guardian, phoneCredential, successCallback);
                    }, guardian.chainId, guardian.verifier.id, OperationTypeEnum.communityRecovery);
                    break;
                default:
                    OnError($"Unsupported account type: {guardian.accountType}");
                    break;
            }
        }

        private void VerifyGuardian(Guardian guardian, ICredential credential, SuccessCallback<ApprovedGuardian> successCallback)
        {
            if (!IsCredentialMatchGuardian(credential, guardian))
            {
                OnError("Account does not match your guardian.");
                return;
            }

            var accountType = guardian.accountType;
            if (accountType.IsSocialAccountType())
            {
                SocialVerifyAndApproveGuardian(credential, guardian);
            }
            else if (accountType == AccountType.Email)
            {
                EmailVerifyAndApproveGuardian((EmailCredential)credential, guardian);
            }
            else if (accountType == AccountType.Phone)
            {
                PhoneVerifyAndApproveGuardian((PhoneCredential)credential, guardian);
            }
            else
            {
                OnError($"Unsupported account type: {guardian.accountType}.");
            }

            return;

            void EmailVerifyAndApproveGuardian(EmailCredential cred, Guardian guard)
            {
                if (!IsMatchingVerifier(cred, guard))
                {
                    OnError("Credential is verified differently from Guardian's definition.");
                    return;
                }
                VerifyEmailCredential(cred, OperationTypeEnum.communityRecovery, verifiedCredential =>
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
                VerifyPhoneCredential(cred, OperationTypeEnum.communityRecovery, verifiedCredential =>
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
        }

        private static bool IsMatchingVerifier(ICodeCredential cred, Guardian guard)
        {
            return cred.ChainId == guard.chainId && cred.VerifierId == guard.verifier.id;
        }

        public static bool IsCredentialMatchGuardian(ICredential cred, Guardian guard)
        {
            return cred != null && cred.SocialInfo.sub == guard.id && cred.AccountType == guard.accountType;
        }

        private void VerifyPhoneCredential(PhoneCredential credential, OperationTypeEnum operationType, SuccessCallback<VerifiedCredential> successCallback)
        {
            StaticCoroutine.StartCoroutine(PhoneCredentialProvider.Verify(credential, successCallback, operationType));
        }
        
        private void VerifyEmailCredential(EmailCredential credential, OperationTypeEnum operationType, SuccessCallback<VerifiedCredential> successCallback)
        {
            StaticCoroutine.StartCoroutine(EmailCredentialProvider.Verify(credential, successCallback, operationType));
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
                successCallback?.Invoke(new VerifiedCredential(credential, chainId, result.VerificationDoc, result.Signature));
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

        public IEnumerator SignUp(PhoneNumber phoneNumber, SuccessCallback<DIDAccountInfo> successCallback)
        {
            yield return PhoneCredentialProvider.Get(phoneNumber, phoneCredential =>
            {
                VerifyPhoneCredential(phoneCredential, OperationTypeEnum.register, verifiedCredential =>
                {
                    StaticCoroutine.StartCoroutine(SignUp(verifiedCredential, successCallback));
                });
            }, Message.ChainId);
        }

        public IEnumerator SignUp(EmailAddress emailAddress, SuccessCallback<DIDAccountInfo> successCallback)
        {
            yield return EmailCredentialProvider.Get(emailAddress, emailCredential =>
            {
                VerifyEmailCredential(emailCredential, OperationTypeEnum.register, verifiedCredential =>
                {
                    StaticCoroutine.StartCoroutine(SignUp(verifiedCredential, successCallback));
                });
            }, Message.ChainId);
        }
        
        /// <summary> The SignUp function is used to register a new DID on the blockchain.        
        /// &lt;para&gt;The SignUp function takes in a VerifiedCredential object, which contains information about the user's social media account and their verification document.&lt;/para&gt;
        /// &lt;para&gt;The SignUp function also takes in a SuccessCallback&lt;DIDAccountInfo&gt;, which is called when the registration process has completed successfully.&lt;/para&gt;</summary>
        ///
        /// <param name="VerifiedCredential verifiedCredential"> /// verifiedcredential is a class that contains the verified credential of a user.
        /// </param>
        /// <param name="SuccessCallback&lt;DIDAccountInfo&gt; successCallback"> /// a callback function that is called when the operation succeeds. 
        /// the parameter of this function is a DIDAccountInfo object containing information about the newly created wallet.
        /// </param>
        ///
        /// <returns> A DIDAccountInfo object.</returns>
        public IEnumerator SignUp(VerifiedCredential verifiedCredential, SuccessCallback<DIDAccountInfo> successCallback)
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

                var walletInfo = new DIDAccountInfo(verifiedCredential.ChainId, param.loginGuardianIdentifier, param.type, registerResult.Status,
                    registerResult.SessionId, AddManagerType.Register, _did.GetManagementSigningKey());
                successCallback(walletInfo);
            }, OnError);
        }

        /// <summary> The SignUp function is used to register a new DID on the blockchain.          
        /// The SignUp function takes in an ICredential object as its first parameter, which can be any ICredential.
        /// The second parameter of the SignUp function is a SuccessCallback&lt;DIDAccountInfo&gt; delegate that will be called when the sign up process has completed successfully.</summary>
        ///
        /// <param name="ICredential credential"> The credential to be verified.</param>
        /// <param name="SuccessCallback&lt;DIDAccountInfo&gt; successCallback"> /// the callback function that will be called when the sign up is successful. 
        /// </param>
        ///
        /// <returns> A DIDAccountInfo object.</returns>
        public IEnumerator SignUp(ICredential credential, SuccessCallback<DIDAccountInfo> successCallback)
        {
            var accountType = credential.AccountType;
            if (accountType.IsSocialAccountType())
            {
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
            }
            else switch (accountType)
            {
                case AccountType.Email:
                {
                    var emailCredential = (EmailCredential)credential;
                    VerifyEmailCredential(emailCredential, OperationTypeEnum.register, verifiedCredential =>
                    {
                        StaticCoroutine.StartCoroutine(SignUp(verifiedCredential, successCallback));
                    });
                    break;
                }
                case AccountType.Phone:
                {
                    var phoneCredential = (PhoneCredential)credential;
                    VerifyPhoneCredential(phoneCredential, OperationTypeEnum.register, verifiedCredential =>
                    {
                        StaticCoroutine.StartCoroutine(SignUp(verifiedCredential, successCallback));
                    });
                    break;
                }
                default:
                    throw new ArgumentException($"Credential holds invalid account type {credential.AccountType}!");
            }
        }

        /// <summary> The Login function is used to log in a user with a list of approved guardians.        
        /// &lt;para&gt;The Login function takes the following parameters:&lt;/para&gt;
        /// &lt;list type=&quot;bullet&quot;&gt;
        ///     &lt;item&gt;&lt;description&gt;&lt;paramref name=&quot;loginGuardian&quot;/&gt; - The Guardian object that contains the user's DID.&lt;/description&gt;&lt;/item&gt;
        ///     &lt;item&gt;&lt;description&gt;&lt;paramref name=&quot;approvedGuardians&quot;/&gt; - A list of approved Guardians for this login session.&lt;/description&gt;&lt;/item&gt; 
        /// &lt;/list&gt; 
        /// </summary>
        ///
        /// <param name="Guardian loginGuardian"> The guardian that is used to login</param>
        /// <param name="List&lt;ApprovedGuardian&gt; approvedGuardians"> /// list of approved guardians.
        /// </param>
        /// <param name="SuccessCallback&lt;DIDAccountInfo&gt; successCallback"> /// the callback function that will be called when the login is successful. 
        /// </param>
        ///
        /// <returns> The DIDAccountInfo object.</returns>
        public IEnumerator Login(Guardian loginGuardian, List<ApprovedGuardian> approvedGuardians, SuccessCallback<DIDAccountInfo> successCallback)
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
                
                var walletInfo = new DIDAccountInfo(param.chainId, loginGuardian.id, loginGuardian.accountType, result.Status, result.SessionId, AddManagerType.Recovery, _did.GetManagementSigningKey());
                successCallback(walletInfo);
            }, OnError));
        }

        /// <summary> The LoginWithPortkeyExtension function is used to login using Portkey Browser Extension.        
        /// &lt;para&gt;- The first parameter is a SuccessCallback function that returns the DIDAccountInfo object.&lt;/para&gt;
        /// </summary>
        ///
        /// <param name="SuccessCallback&lt;DIDAccountInfo&gt; successCallback"> /// the callback that is called when the login process succeeds.
        /// </param>
        ///
        /// <returns> A DIDAccountInfo object</returns>
        public IEnumerator LoginWithPortkeyExtension(SuccessCallback<DIDAccountInfo> successCallback)
        {
            yield return _did.LoginWithPortkeyExtension(successCallback, () =>
            {
                OnLogout(LogoutMessage.PortkeyExtensionLogout);
            }, OnError);
        }

        /// <summary> The LoginWithPortkeyApp function is used to login with the Portkey app on mobile devices.        
        /// &lt;para&gt;The function takes a SuccessCallback as an argument, which will be called if the login was successful.&lt;/para&gt;
        /// </summary>
        ///
        /// <param name="SuccessCallback&lt;DIDAccountInfo&gt; successCallback"> /// successcallback&lt;DIDAccountInfo&gt; successcallback is a callback function that returns the DIDAccountInfo object.
        /// </param>
        ///
        /// <returns> A DIDAccountInfo object.</returns>
        public IEnumerator LoginWithPortkeyApp(SuccessCallback<DIDAccountInfo> successCallback)
        {
            yield return _did.LoginWithPortkeyApp(PortkeyAppLoginSuccess(successCallback), OnError);
        }

        /// <summary> The LoginWithQRCode function is used to login with a QR code.        
        /// &lt;para&gt;The qrCodeCallback function will be called when the QR code is ready.&lt;/para&gt;
        /// &lt;para&gt;The successCallback function will be called when the user has successfully logged in.&lt;/para&gt;</summary>
        ///
        /// <param name="SuccessCallback&lt;Texture2D&gt; qrCodeCallback"> ///     &lt;para&gt;a callback to return the qr code image.&lt;/para&gt;
        /// </param>
        /// <param name="SuccessCallback&lt;DIDAccountInfo&gt; successCallback"> /// this is the callback that will be called when the login process is successful.
        /// </param>
        ///
        /// <returns> A texture2d object.</returns>
        public IEnumerator LoginWithQRCode(SuccessCallback<Texture2D> qrCodeCallback, SuccessCallback<DIDAccountInfo> successCallback)
        {
            yield return _did.LoginWithQRCode(qrCodeCallback, PortkeyAppLoginSuccess(successCallback), OnError);
        }
        
        private SuccessCallback<PortkeyAppLoginResult> PortkeyAppLoginSuccess(SuccessCallback<DIDAccountInfo> successCallback)
        {
            return result =>
            {
                Message.ChainId = result.caHolder.holderManagerInfo.originChainId;
                successCallback(new DIDAccountInfo(result.caHolder, result.managementAccount));
            };
        }

        /// <summary> The Logout function logs out the user from the DID service.</summary>        
        ///
        /// <returns> A bool value indicating if the log out operation was successful.</returns>
        public IEnumerator Logout()
        {
            var param = new EditManagerParams();
            yield return _did.Logout(param, success =>
            {
                if (!success)
                {
                    OnError("Logout failed!");
                    return;
                }
                OnLogout(LogoutMessage.Logout);
            }, OnError);
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