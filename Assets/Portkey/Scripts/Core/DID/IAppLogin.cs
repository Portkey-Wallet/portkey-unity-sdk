using System.Collections;

namespace Portkey.Core
{
    public interface IAppLogin
    {
        IEnumerator Login(string chainId, SuccessCallback<PortkeyAppLoginResult> successCallback, ErrorCallback errorCallback);
    }
}