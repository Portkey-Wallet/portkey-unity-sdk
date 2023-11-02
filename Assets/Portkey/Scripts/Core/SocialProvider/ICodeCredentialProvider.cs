using System.Collections;

namespace Portkey.Core
{
    public interface ICodeCredentialProvider : ICredentialProvider
    {
        bool EnableCodeSendConfirmationFlow { get; }
    }
}