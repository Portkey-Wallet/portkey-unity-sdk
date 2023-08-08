using System.Collections;

namespace Portkey.Core
{
    public interface IContractProvider
    {
        public IEnumerator GetContract(string chainId, SuccessCallback<IContract> successCallback, ErrorCallback errorCallback);
    }
}