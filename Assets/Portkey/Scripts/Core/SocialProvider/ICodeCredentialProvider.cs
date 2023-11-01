using System.Collections;

namespace Portkey.Core
{
    public interface ICodeCredentialProvider : ICredentialProvider
    {
        bool EnableCodeSendConfirmationFlow { get; }
        IEnumerator SendCode(string guardianId, SuccessCallback<string> successCallback, string chainId = null, string verifierId = null, OperationTypeEnum operationType = OperationTypeEnum.register);
    }
}