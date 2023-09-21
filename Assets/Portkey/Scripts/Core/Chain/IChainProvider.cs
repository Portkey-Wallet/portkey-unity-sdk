using System.Collections;

namespace Portkey.Core
{
    public interface IChainProvider
    {
        IEnumerator GetChain(string chainId, SuccessCallback<IChain> successCallback, ErrorCallback errorCallback);
    }
}