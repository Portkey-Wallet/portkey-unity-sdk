using System.Collections;
using Portkey.Core;

namespace Portkey.Contract
{
    public class AElfContractBasic : IContract
    {
        public IEnumerator CallViewMethod(string methodName, object[] paramOptions, IHttp.successCallback successCallback,
            IHttp.errorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator CallSendMethod(string methodName, object[] paramOptions, SendOptions sendOptions,
            IHttp.successCallback successCallback, IHttp.errorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator EncodeTx(string methodName, object[] paramOptions, IHttp.successCallback successCallback,
            IHttp.errorCallback errorCallback)
        {
            throw new System.NotImplementedException();
        }
    }
}