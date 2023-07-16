using System.Collections;

namespace Portkey.Core
{
    public interface IContractProvider<T> where T : IContract
    {
        public IEnumerator GetContract(string chainId, SuccessCallback<IContract> successCallback, ErrorCallback errorCallback);
    }
}