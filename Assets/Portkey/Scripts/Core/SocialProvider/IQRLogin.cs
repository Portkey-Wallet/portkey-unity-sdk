using System.Collections;
using UnityEngine;

namespace Portkey.Core
{
    public interface IQRLogin
    {
        IEnumerator Login(string chainId, SuccessCallback<Texture2D> qrCodeCallback, SuccessCallback<PortkeyAppLoginResult> successCallback, ErrorCallback errorCallback);
        void Cancel();
    }
}