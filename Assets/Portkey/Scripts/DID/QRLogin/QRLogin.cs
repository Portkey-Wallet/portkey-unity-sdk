//using QRCoder;
//using QRCoder.Unity;
using System.Collections;
using Portkey.Core;
using UnityEngine;

namespace Portkey.DID
{
    public class QRLogin : IQRLogin
    {
        public IEnumerator Login(string chainId, SuccessCallback<Texture2D> successCallback, ErrorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }
    }
}