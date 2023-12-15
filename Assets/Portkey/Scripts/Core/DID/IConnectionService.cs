using System.Collections;

namespace Portkey.Core
{
    public interface IConnectionService
    {
        IEnumerator GetConnectToken(RequestTokenConfig config, SuccessCallback<ConnectToken> successCallback, ErrorCallback errorCallback);
        void Reset();
    }
}