using System.Collections;
using UnityEngine;

namespace Portkey.Core
{
    public interface IQRLogin
    {
        IEnumerator Login(string chainId, SuccessCallback<Texture2D> successCallback, ErrorCallback errorCallback);
    }
}