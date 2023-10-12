using System.Collections;

namespace Portkey.Core
{
    public interface IAppLogin
    {
        IEnumerator Login(SuccessCallback<PortkeyAppLoginResult> successCallback, ErrorCallback errorCallback);
        void Cancel();
    }
}