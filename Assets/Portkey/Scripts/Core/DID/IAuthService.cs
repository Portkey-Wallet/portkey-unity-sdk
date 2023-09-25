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
        int GetRequiredApprovedGuardiansCount(int totalGuardians);
        IEnumerator GetGuardians(ICredential credential, SuccessCallback<List<Guardian>> successCallback);
        IEnumerator GetGuardians(PhoneNumber phoneNumber, SuccessCallback<List<Guardian>> successCallback);
        IEnumerator GetGuardians(EmailAddress emailAddress, SuccessCallback<List<Guardian>> successCallback);
        IEnumerator Verify(Guardian guardian, SuccessCallback<ApprovedGuardian> successCallback, ICredential credential = null);
        IEnumerator SignUp(PhoneNumber phoneNumber, SuccessCallback<DIDWalletInfo> successCallback);
        IEnumerator SignUp(EmailAddress emailAddress, SuccessCallback<DIDWalletInfo> successCallback);
        IEnumerator SignUp(ICredential credential, SuccessCallback<DIDWalletInfo> successCallback);
        IEnumerator SignUp(VerifiedCredential verifiedCredential, SuccessCallback<DIDWalletInfo> successCallback);
        IEnumerator Login(Guardian loginGuardian, List<ApprovedGuardian> approvedGuardians, SuccessCallback<DIDWalletInfo> successCallback);
        IEnumerator LoginWithPortkeyApp(SuccessCallback<DIDWalletInfo> successCallback);
        IEnumerator Logout(SuccessCallback<bool> successCallback);
    }
}