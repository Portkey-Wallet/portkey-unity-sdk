using System.Collections;

namespace Portkey.Core
{
    public interface IAppLogin
    {
        IEnumerator Login(string chainId, SuccessCallback<DIDWalletInfo> successCallback, ErrorCallback errorCallback);
    }
}