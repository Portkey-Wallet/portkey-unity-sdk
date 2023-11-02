using System.Collections;

namespace Portkey.Core
{
    public interface ICredentialProvider
    {
        AccountType AccountType { get; }
        IEnumerator Verify(ICredential credential, SuccessCallback<VerifiedCredential> successCallback, OperationTypeEnum operationType = OperationTypeEnum.register);
    }
}