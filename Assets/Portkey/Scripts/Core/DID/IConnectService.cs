using System.Collections;

namespace Portkey.Core
{
    public interface IConnectService
    {
        IEnumerator GetConnectToken(RequestTokenConfig config, SuccessCallback<ConnectToken> successCallback, ErrorCallback errorCallback);
    }
}