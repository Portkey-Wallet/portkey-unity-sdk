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
        IAuthMessage Message { get; }
        EmailLogin Email { get; }
        PhoneLogin Phone { get; }
        IEnumerator GetGuardians(ICredential credential, SuccessCallback<List<GuardianNew>> successCallback);
        IEnumerator GetGuardians(PhoneNumber phoneNumber, SuccessCallback<List<GuardianNew>> successCallback);
        IEnumerator GetGuardians(EmailAddress emailAddress, SuccessCallback<List<GuardianNew>> successCallback);
        void Verify(GuardianNew guardian, SuccessCallback<ApprovedGuardian> successCallback, ICredential credential = null);
        IEnumerator SignUp(PhoneNumber phoneNumber, SuccessCallback<DIDWalletInfo> successCallback);
        IEnumerator SignUp(EmailAddress emailAddress, SuccessCallback<DIDWalletInfo> successCallback);
        IEnumerator SignUp(ICredential credential, SuccessCallback<DIDWalletInfo> successCallback);
        IEnumerator SignUp(VerifiedCredential verifiedCredential, SuccessCallback<DIDWalletInfo> successCallback);
        IEnumerator Login(GuardianNew loginGuardian, List<ApprovedGuardian> approvedGuardians, SuccessCallback<DIDWalletInfo> successCallback);
    }
}