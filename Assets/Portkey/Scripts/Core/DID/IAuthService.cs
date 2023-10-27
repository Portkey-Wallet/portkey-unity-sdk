using System.Collections;
using System.Collections.Generic;
using Portkey.SocialProvider;
using UnityEngine;

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
        IEnumerator SignUp(PhoneNumber phoneNumber, SuccessCallback<DIDAccountInfo> successCallback);
        IEnumerator SignUp(EmailAddress emailAddress, SuccessCallback<DIDAccountInfo> successCallback);
        IEnumerator SignUp(ICredential credential, SuccessCallback<DIDAccountInfo> successCallback);
        IEnumerator SignUp(VerifiedCredential verifiedCredential, SuccessCallback<DIDAccountInfo> successCallback);
        IEnumerator Login(Guardian loginGuardian, List<ApprovedGuardian> approvedGuardians, SuccessCallback<DIDAccountInfo> successCallback);
        IEnumerator LoginWithPortkeyApp(SuccessCallback<DIDAccountInfo> successCallback);
        IEnumerator LoginWithQRCode(SuccessCallback<Texture2D> qrCodeCallback, SuccessCallback<DIDAccountInfo> successCallback);
        IEnumerator LoginWithPortkeyExtension(SuccessCallback<DIDAccountInfo> successCallback);
        IEnumerator Logout();
    }
}