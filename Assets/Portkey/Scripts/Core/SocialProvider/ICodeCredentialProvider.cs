using System.Collections;

namespace Portkey.Core
{
    public interface ICodeCredentialProvider
    {
        bool EnableCodeSendConfirmationFlow { get; }
        AccountType AccountType { get; }
        IEnumerator Verify(ICredential credential, SuccessCallback<VerifiedCredential> successCallback);
    }
}