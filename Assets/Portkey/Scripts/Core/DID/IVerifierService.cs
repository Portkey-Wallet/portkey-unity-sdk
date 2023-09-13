using System.Collections;

namespace Portkey.Core
{
    public interface IVerifierService
    {
        bool IsInitialized(string chainId);
        VerifierItem GetVerifier(string chainId, string verifierId);
        IEnumerator Initialize(string chainId, SuccessCallback<bool> successCallback, ErrorCallback errorCallback);
    }
}