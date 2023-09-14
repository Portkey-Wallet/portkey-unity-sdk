using System.Collections;
using System.Collections.Generic;
using Portkey.SocialProvider;

namespace Portkey.Core
{
    public interface IAuthService
    {
        AppleCredentialProvider AppleCredentialProvider { get; }
        GoogleCredentialProvider GoogleCredentialProvider { get; }
        PhoneCredentialProvider PhoneCredentialProvider { get; }
        EmailCredentialProvider EmailCredentialProvider { get; }
        EmailLogin Email { get; }
        PhoneLogin Phone { get; }
        ErrorCallback OnError { get; set; }
        IEnumerator GetGuardians(ICredential credential, SuccessCallback<List<GuardianNew>> successCallback);
        IEnumerator GetGuardians(PhoneNumber phoneNumber, SuccessCallback<List<GuardianNew>> successCallback);
        IEnumerator GetGuardians(EmailAddress emailAddress, SuccessCallback<List<GuardianNew>> successCallback);
        void Verify(GuardianNew guardian, SuccessCallback<ApprovedGuardian> successCallback, ICredential credential = null);
        IEnumerator SignUp(string chainId, PhoneNumber phoneNumber, SuccessCallback<DIDWalletInfo> successCallback);
        IEnumerator SignUp(string chainId, EmailAddress emailAddress, SuccessCallback<DIDWalletInfo> successCallback);
        IEnumerator SignUp(string chainId, ICredential credential, SuccessCallback<DIDWalletInfo> successCallback);
        IEnumerator SignUp(string chainId, VerifiedCredential verifiedCredential, SuccessCallback<DIDWalletInfo> successCallback);
        IEnumerator Login(GuardianNew loginGuardian, List<ApprovedGuardian> approvedGuardians, SuccessCallback<DIDWalletInfo> successCallback);
        void SendVerificationCode(string verificationCode);
    }
}